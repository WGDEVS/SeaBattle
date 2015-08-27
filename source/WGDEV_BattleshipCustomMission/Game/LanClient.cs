/*
Class Description:
This class is used for providing a LAN game where this computer is used as the client.
The host attemps to connect to a specified ip address. The client also does the second turn sequence.

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace WGDEV_BattleshipCustomMission.Game
{
    class LanClient : Game
    {
        /// <summary>Initializes a member of the Lan Client class. Used to define key properties of the game.</summary>
        /// <param name="Bonus">Determines if the Bonus option is enabled.</param>
        /// <param name="Advanced">Determines if the advanced option is enabled.</param>
        /// <param name="MapWidth">Determines the width of both maps.</param>
        /// <param name="MapHeight">Determines the height of both maps.</param> 
        /// <param name="Ships">The ships that all players are assigned at the begining of the game.</param>
        /// <param name="Planes">The planes that all players are assigned at the begining of the game.</param> 
        public LanClient(bool Bonus, bool Salvo, bool Advanced, int MapWidth, int MapHeight, List<Ship.Ship> Ships, List<Plane.Plane> Planes):base(
         Bonus,  Salvo,  Advanced,  MapWidth,  MapHeight,  Ships, Planes){
        }

        /// <summary>Runs a LAN game, here this computer is the client.</summary>
        public override void RunGame()
        {
            string buf = "";
            Console.CursorVisible = true;
            IPAddress hostIP = null;
            do {
                Console.Clear();
                Console.Write("Enter the ip of the host:");
                try
                {
                    hostIP = IPAddress.Parse(Console.ReadLine());
                    if (hostIP != null) break;
                }
                catch (Exception ex) {
                    continue;
                }
            }while(true) ;
            Console.CursorVisible = false;

            TcpClient client = new TcpClient(hostIP.ToString(),Program.Port);

            if ((buf = LanHost.GetData(client)).Equals("")) return;

            string[] inps = buf.Split(LanHost.FieldDelimiter.ToCharArray());

            bool clientSalvo = bool.Parse(inps[0]);
            bool clientBonus = bool.Parse(inps[1]);
            bool clientAdvanced = bool.Parse(inps[2]);

            SetUpMap temp = new SetUpMap(Player2, clientAdvanced);
            if (!LanHost.SendData(temp.ManualSetUp("Client", ShipList, PlaneList), client)) return;

            Console.Clear();
            Console.WriteLine("Waiting for Host to finish setting up map.");

            if ((buf = LanHost.GetData(client)).Equals("")) return;
            temp = new SetUpMap(Player1, clientAdvanced);
            temp.TextSetUp(buf, ShipList, PlaneList);

            Turn t;
            do
            {
                Console.Clear();
                Console.WriteLine("It's time for the host to play, please wait.");
                if ((buf = LanHost.GetData(client)).Equals("")) return;
                if (clientAdvanced)
                    t = new AdvancedTurn(clientBonus, clientSalvo, Player1, Player2);
                else
                    t = new Turn(clientBonus, clientSalvo, Player1, Player2);

                t.DoTextTurnSequence(buf);
                if (Player2.hasLost())
                {
                    PauseGame("The client has lost the game, press ESC to return to main menu.");
                    client.Close();
                    return;
                }

                if (clientAdvanced)
                    t = new AdvancedTurn(clientBonus, clientSalvo, Player2, Player1);
                else
                    t = new Turn(clientBonus, clientSalvo, Player2, Player1);
                PauseGame("It's time for the client to play, press ESC to continue.");
                if (!LanHost.SendData(t.DoManualTurnSequence(), client)) return;
                if (Player1.hasLost())
                {
                    PauseGame("The client has won the game, press ESC to return to main menu.");
                    client.Close();
                    return;
                }
            } while (true);


        }
     }
}
