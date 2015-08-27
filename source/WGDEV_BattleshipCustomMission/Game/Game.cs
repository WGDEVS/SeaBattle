/*
Class Description:
This class is used as a base to streamline all the different types 
of games available. It contains variables and functions that are 
required for all game types.

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Game
{
    class Game
    {
        public bool Bonus;//Determines if the bonus option is enabled for the game
        public bool Salvo;//Determines if the salvo option is enabled for the game
        public bool Advanced;//Determines if the advanced option is enabled for the game

        public Map Player1;//The map of the player that does a turn sequence first
        public Map Player2;//The map of the player that does a turn sequence second

        public List<Ship.Ship> ShipList;//The list of ships to be placed on the maps during setup
        public List<Plane.Plane> PlaneList;//The list of planes to be placed on the maps during setup

        /// <summary>Initializes a member of the game class. Used to define key properties of the game.</summary>
        /// <param name="Bonus">Determines if the Bonus option is enabled.</param>
        /// <param name="Advanced">Determines if the advanced option is enabled.</param>
        /// <param name="MapWidth">Determines the width of both maps.</param>
        /// <param name="MapHeight">Determines the height of both maps.</param> 
        /// <param name="Ships">The ships that all players are assigned at the begining of the game.</param>
        /// <param name="Planes">The planes that all players are assigned at the begining of the game.</param> 
        public Game(bool Bonus, bool Salvo, bool Advanced, int MapWidth, int MapHeight, List<Ship.Ship> Ships, List<Plane.Plane> Planes) {
             Player1 = new Map(MapWidth, MapHeight);
             Player2 = new Map(MapWidth, MapHeight);

             ShipList = Ships;
             PlaneList = Planes;

             this.Bonus = Bonus;
             this.Salvo = Salvo;
             this.Advanced = Advanced;
        }

        /// <summary>Does not do anything and is meant to be overritten by the class that inherits from this class.
        /// How all games work is that the players set up their maps and take turns performing turn sequnces until all the ships on one map is destroyed
        /// </summary>
        public virtual void RunGame() { }

        /// <summary>
        /// Prints the description of a key to the screen as a line. Automatically indicates if the key can be used.
        /// </summary>
        /// <param name="Message">The text that describes the function of the key</param>
        /// <param name="CanUse">A boolean representing if the key can currently be used</param>
        public static void DisplayControl(string Message, bool CanUse) {
            if (!CanUse)
                Console.ForegroundColor = Program.DeselectedTextColor;
            Console.WriteLine(Message);
            Console.ForegroundColor = Program.DefaultTextColor;
        }

        /// <summary>
        /// Prints the description of a key to the screen as a character. Automatically indicates if the key can be used.
        /// </summary>
        /// <param name="Message">The string representation of the key</param>
        /// <param name="CanUse">A boolean representing if the key can currently be used</param>
        public static void DisplayControlKey(string Message, bool CanUse)
        {
            if (!CanUse)
                Console.ForegroundColor = Program.DeselectedTextColor;
            Console.Write(Message);
            Console.ForegroundColor = Program.DefaultTextColor;
        }

        /// <summary>
        /// Clears the screen and shows a message until the escape key is pressed.
        /// </summary>
        /// <param name="Message">The string representation of the message, will need to indicate that the ESC key is required to dismiss the message</param>
        public static void PauseGame(string Message) {
            Console.Clear();
            Console.Write(Message);
            do
            {
            } while (Console.ReadKey().Key != ConsoleKey.Escape);
        }
    }
}
