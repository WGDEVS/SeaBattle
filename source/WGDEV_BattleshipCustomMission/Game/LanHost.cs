/*
Class Description:
This class is used for providing a LAN game where this computer is used as the host.
The host has an ip address and waits for the client to connect. The host does the first turn sequence.
It also provides functions for LAN games.
Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace WGDEV_BattleshipCustomMission.Game
{
    class LanHost : Game

    {
        private const int bytestotellsize = 4;//The amount of bytes reserved to give the length of a string of LAN data
        private const int buffersize = 1000;//The maximum length of a string of LAN data in byte form, must be less than 256*bytestotellsize
        public const string ActionDelimiter = "/";//A delimiter used for LAN data
        public const string FieldDelimiter = " ";// Another delimiter used for LAN data

        /// <summary>Initializes a member of the Lan Host class. Used to define key properties of the game.</summary>
        /// <param name="Bonus">Determines if the Bonus option is enabled.</param>
        /// <param name="Advanced">Determines if the advanced option is enabled.</param>
        /// <param name="MapWidth">Determines the width of both maps.</param>
        /// <param name="MapHeight">Determines the height of both maps.</param> 
        /// <param name="Ships">The ships that all players are assigned at the begining of the game.</param>
        /// <param name="Planes">The planes that all players are assigned at the begining of the game.</param> 
        public LanHost(bool Bonus, bool Salvo, bool Advanced, int MapWidth, int MapHeight, List<Ship.Ship> Ships, List<Plane.Plane> Planes):base(
         Bonus,  Salvo,  Advanced,  MapWidth,  MapHeight,  Ships, Planes){
        }

        /// <summary>Runs a LAN game, here this computer is the host.</summary>
        public override void RunGame()
        {
            string buf = "";
            Console.Clear();
            IPAddress[] hostIPs = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress hostIP = null;
            foreach (IPAddress i in hostIPs)
                if ((i.AddressFamily == AddressFamily.InterNetworkV6) == Program.IPv6Hosting && !i.ToString().Substring(0, 3).Equals("127"))
                {
                    hostIP = i;
                    break;
                }
            if (hostIP == null)
            {
                Console.Write("Cannot get ip address, make sure your IPv" + (Program.IPv6Hosting ? "6" : "4") + " network is connected.\nPress ESC to return to main menu");
                while (Console.ReadKey().Key != ConsoleKey.Escape)
                { }
                return;
            }
            Console.Clear();
            Console.WriteLine("Waiting for a connection, your ip is: " + hostIP.ToString());
            TcpListener host = new TcpListener(hostIP,Program.Port);
            host.Start();
            TcpClient client = host.AcceptTcpClient();

            if (!SendData(Salvo.ToString() + FieldDelimiter + Bonus.ToString() + FieldDelimiter + Advanced.ToString(), client)) return;
            SetUpMap temp = new SetUpMap(Player1, Advanced);
            if (!SendData(temp.ManualSetUp("Host", ShipList, PlaneList),client)) { host.Stop(); return; }

            Console.Clear();
            Console.WriteLine("Waiting for Client to finish setting up map.");

            temp = new SetUpMap(Player2, Advanced);
            if ((buf = GetData(client)).Equals("")) { host.Stop(); return; }
            temp.TextSetUp(buf, ShipList, PlaneList);

            Turn t;
            do
            {
                if (Advanced)
                    t = new AdvancedTurn(Bonus, Salvo, Player1, Player2);
                else
                    t = new Turn(Bonus, Salvo, Player1, Player2);
                PauseGame("It's time for the host to play, press ESC to continue.");
                buf = t.DoManualTurnSequence();
                if (!SendData(buf, client)) { host.Stop(); return; }
                if (Player2.hasLost())
                {
                    PauseGame("The host has won the game, press ESC to return to main menu.");
                    client.Close();
                    host.Stop();
                    return;
                }
                Console.Clear();
                Console.WriteLine("It's time for the client to play, please wait.");
                if ((buf = GetData(client)).Equals("")) { host.Stop(); return; }
                if (Advanced)
                    t = new AdvancedTurn(Bonus, Salvo, Player2, Player1);
                else
                    t = new Turn(Bonus, Salvo, Player2, Player1);

                t.DoTextTurnSequence(buf);
                if (Player1.hasLost())
                {
                    PauseGame("The host has lost the game, press ESC to return to main menu.");
                    client.Close();
                    host.Stop();
                    return;
                }
            } while (true);
        }

        /// <summary>
        /// Attempts to gets data from the other computer in a LAN game
        /// </summary>
        /// <param name="Client">The specified other computer</param>
        /// <returns>The string representing the data recieved, or a blank string if the other computer disconnected</returns>
        public static string GetData(TcpClient Client) {
            NetworkStream tStream = Client.GetStream();
            if (tStream.CanRead && Client.Connected)
            {
                Byte[] recievedBytes = new byte[buffersize];
                try
                {
                    tStream.Read(recievedBytes, 0, buffersize);
                }
                catch (Exception ex)
                {
                    Game.PauseGame("Opponent has disconnected, press ESC to return to main menu.");
                    Client.Close();
                    return "";
                }
                int culSize = 0;
                for (int i = 0; i < bytestotellsize; i++)
                    culSize += recievedBytes[i];
                Byte[] inp = new byte[culSize];
                Array.ConstrainedCopy(recievedBytes, bytestotellsize, inp, 0, culSize);
                return System.Text.Encoding.ASCII.GetString(inp);
            }
            Game.PauseGame("Opponent has disconnected, press ESC to return to main menu.");
            Client.Close();
            return "";
        }

        /// <summary>
        /// Attempts to send data to the other computer in a LAN game
        /// </summary>
        /// <param name="SentString">The specified data in string form</param>
        /// <param name="Client">The specified other computer</param>
        /// <returns>A boolean representing if the data was sucessfully sent</returns>
        public static bool SendData(string SentString,TcpClient Client) {
            NetworkStream tStream = Client.GetStream();
            if (tStream.CanWrite && Client.Connected)
            {
                byte[] outStream = new byte[buffersize];
                byte[] buf = System.Text.Encoding.ASCII.GetBytes(SentString);
                int sizeLeft = buf.Length;
                for (int i = 0; i < bytestotellsize; i++)
                    if (sizeLeft <= 0)
                        break;
                    else if (sizeLeft % Byte.MaxValue == 0)
                    {
                        outStream[i] = Byte.MaxValue;
                        sizeLeft -= Byte.MaxValue;
                    }
                    else
                    {
                        outStream[i] = (byte)(sizeLeft % Byte.MaxValue);
                        sizeLeft -= sizeLeft % Byte.MaxValue;
                    }


                for (int i = bytestotellsize; i < buf.Length + bytestotellsize; i++)
                    outStream[i] = buf[i - bytestotellsize];
                try
                {
                    tStream.Write(outStream, 0, buffersize);
                }
                catch (Exception ex)
                {
                    Game.PauseGame("Opponent has disconnected, press ESC to return to main menu.");
                    Client.Close();

                    return false;
                }
                tStream.Flush();
                return true;
            }
            else
            {
                Game.PauseGame("Opponent has disconnected, press ESC to return to main menu.");
                Client.Close();

                return false;
            }
        }
    }
}
