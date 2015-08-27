/*
Class Description:
This is the main class for the program.
The class contains very important constants and variables that affect all the classes.

This program simulates a game of Sea Battle, it offers optional twists by offering gameplay options.
Currently there are three options that change how the game works: the bonus option, the salvo option and the advanced option.
How each one will affect gameplay will be explained later.

The program contains a main menu, from there the player can start a game or go into the options menu and turn the various options on and off.

There are may different types of games, each one is explained in detail in their classes, however there are common traits to all the game types
The game first starts with setup, the players place ships and, if the advanced option is enabled, planes.
Then the games move on to turn sequences.

The players take turns doing a turn sequence until all the ships of one of the players are destroyed, in which case, the player with the destroyed ships loses.

In a turn sequence the player currently doing the turn sequence is refered to as the player and the other player is refered to as the opponent.
The player starts off the turn seqence doing one turn, extra turns are given based on the salvo and bonus options. The salvo option gives the player 1 extra turn for every ship above 1 not destroyed. The bonus mission gives the player 1 extra turn if the player has hit a part of one of the opponent's ships in the previous turn.

What happens during a turn depends on if the player has the advanced opion enabled.
The term coordinate and point are used interchangably, they refer to an integer array with two elements that represent coordinates on a standard sea battle nmap
If the advanced option is disabled, the player can only target a coordinate on the opponent's map and destroy any items or part of items that are there
If the advanced option is enabled, the player can preform many actions, all of which are listed in the header for the class AdvancedTurn


Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission
{
    class Program
    {
        public const bool IPv6Hosting = false;//Determines if ipv6 will be used for LAN games

        public static bool Advanced = false;//Determines if the advanced option is enabled, it allows the use of attacks and planes
        public static bool Bonus = false;//Determines if the bonus option is enabled, it allows an extra turn if an enemy ship is damaged
        public static bool Salvo = false;//Determines if the salvo option is enabled, it allows an guarenteed amount of turns depending on the amount of friendly ships 

        public const int Port = 49111;//The port used for LAN games
        public static int[] DefaultLocation = { 0, 0 };//The place where all important locations begin
        public const Ship.Ship.Direction DefaultDirection = Ship.Ship.Direction.East; //The Directions all ships face at the start

        //The planes that everyone starts with
        public static List<Plane.Plane> PlaneList = new List<Plane.Plane> { new Plane.ReconPlane(ConsoleColor.DarkGreen), new Plane.ReconPlane(ConsoleColor.Blue), new Plane.DecoyPlane(ConsoleColor.DarkGray) };//The planes 

        //The ships that everyone starts with
        public static List<Ship.Ship> Shiplist = new List<Ship.Ship> { new Ship.AircraftCarrier(), new Ship.Battleship(), new Ship.Destroyer(), new Ship.PatrolBoat(), new Ship.Submarine() };

        public const ConsoleColor DefaultTextColor = ConsoleColor.Gray; //The default front color of the console
        public const ConsoleColor SelectedTextColor = ConsoleColor.Yellow; //If someting in a menu is selected, it will be printed in this color
        public const ConsoleColor DefaultBackColor = ConsoleColor.Black;//The default back color of the console
        public const ConsoleColor DeselectedTextColor = ConsoleColor.DarkGray;//During gameplay, if a key can't be used, it will be printed with this color

        public const ConsoleColor TargetIncCol = ConsoleColor.Red;//During gameplay, it highlights a location when it can be changed; 
        public const ConsoleColor TargetExCol = ConsoleColor.Yellow;//During gameplay, it highlights a location when it can be changed, this variant is used over the other when the location is being used to aim an attack and the attack will not hit the location;
        public const ConsoleColor SplashCol = ConsoleColor.DarkCyan;//During gameplay it indicates the places that will be affected. If an attack is being aimed, it will indicate the places that the attacks hit. If a ship/plane is selected, it will indicate its existance


        private static string[] SelectedTextDelimiter = { "[", "]" };//If someting in a Menu is selected, it will be surrounded by the delimiters

        //The selections for the Main Menu
        private static string[] Options = {
                                   "Single player",
                                   "2 player",
                                   "2 player lan host",
                                   "2 player lan client (will use host game settings)",
                                   "Game Settings"
                               };

        //The selections for the Settings Menu
        private static string[] Settings = {
            "Bonus Option",
            "Salvo Option",
            "Advanced Option",
        };
        //Stroes the state of the options in the settings menu
        private static bool[] SettingsEnabled = new bool[Settings.Length];

        //Stores the current selection in the menus
        private static int selected = 0;

        /// <summary>This is the Main Menu. It allow the user to start a game of Sea Battle or go into the Settings Menu.</summary>
        static void Main(string[] args)
        {
            //Initialize the program
            Console.CursorVisible = false;
            Console.WindowHeight = (int)(Console.LargestWindowHeight*0.95);

            do
            {
                Console.Clear();
                PrintMainMenu();
                ConsoleKey inp = Console.ReadKey().Key;
                if (inp == ConsoleKey.UpArrow) {
                    if (selected != 0)
                        selected--;
                }
                else if (inp == ConsoleKey.DownArrow) {
                    if (selected != Options.Length - 1)
                        selected++;
                }
                else if (inp == ConsoleKey.Enter && selected == Options.Length -1)
                {
                    SettingsMenu();
                }
                else if (inp == ConsoleKey.Enter) {
                    RunGame();
                }
            } while (true);
        }

        /// <summary>This is the Settings Menu. It allow the user to change settings for the game.</summary>
        private static void SettingsMenu() {
            selected = 0;
            do
            {
                Console.Clear();
                PrintSettingsMenu();
                ConsoleKey inp = Console.ReadKey().Key;
                if (inp == ConsoleKey.UpArrow)
                {
                    if (selected != 0)
                        selected--;
                }
                else if (inp == ConsoleKey.DownArrow)
                {
                    if (selected != Settings.Length - 1)
                        selected++;
                }
                else if (inp == ConsoleKey.Enter)
                {
                    SettingsEnabled[selected] = true;
                }
                else if (inp == ConsoleKey.C)
                {
                    SettingsEnabled[selected] = false;
                }
                else if (inp == ConsoleKey.Escape)
                {
                    selected = 0;
                    return;
                }

            } while (true);
        }

        /// <summary>Starts a game of Sea Battle, the type will depend on what the user has selected in the main menu.</summary>
        private static void RunGame()
        {
            Game.Game curGame;
            switch (selected) {
                case 0:
                    curGame = new Game._1Player(SettingsEnabled[0], SettingsEnabled[1], SettingsEnabled[2], 10, 10, Shiplist, PlaneList);
                    break;
                case 1:
                    curGame = new Game._2Player(SettingsEnabled[0], SettingsEnabled[1], SettingsEnabled[2], 10, 10, Shiplist, PlaneList);
                    break;
                case 2:
                    curGame = new Game.LanHost(SettingsEnabled[0], SettingsEnabled[1], SettingsEnabled[2], 10, 10, Shiplist, PlaneList);
                    break;
                case 3:
                    curGame = new Game.LanClient(SettingsEnabled[0], SettingsEnabled[1], SettingsEnabled[2], 10, 10, Shiplist, PlaneList);
                    break;
                default:
                    return;
            }

            curGame.RunGame();
        }

        /// <summary>Adjusts the selected index of a list based on a key pressed. The selected index will change by 1 depending on the key pressed. The selected index will wrap around the list.</summary>
        /// <param name="IndexSelected">The original index that has been selected.</param>
        ///<param name="InpKey">The key that has been pressed.</param>
        ///<param name="ListSelectedMax">The size of the list.</param>
        public static void AdjustListSelected(ref int IndexSelected, ConsoleKey InpKey, int ListSelectedMax)
        {
            switch (InpKey)
            {
                case ConsoleKey.R:
                case ConsoleKey.F:
                case ConsoleKey.V:
                case ConsoleKey.UpArrow:
                    WrapAdd(ref IndexSelected, -1, ListSelectedMax);
                    break;
                case ConsoleKey.T:
                case ConsoleKey.G:
                case ConsoleKey.B:
                case ConsoleKey.DownArrow:
                    WrapAdd(ref IndexSelected, 1, ListSelectedMax);
                    break;
            }
        }

        /// <summary>Adds a value to a number and mods it so that it is below a maximum value.</summary>
        /// <param name="Number">The original value of the number.</param>
        ///<param name="Increment">The value added to the number.</param>
        ///<param name="Maximum">The Maximum value that the number must be below.</param>
        public static void WrapAdd(ref int Number, int Increment, int Maximum) {
            if (Maximum == 0)
            {Number = 0;
                return;}
            Number += Increment;
            if (Number < 0) Number += Maximum;
            Number %= Maximum;
        }

        /// <summary>Prints the text for the settings menu.</summary>
        private static void PrintSettingsMenu() {
            Console.Write("Settings Menu\nUse up/down keys to make a selection\nEnter to enable setting\nC to disable setting\nESC to return to main menu\n");

            for (int i = 0; i < Settings.Length; i++)
            {
                if (i == selected)
                    Console.ForegroundColor = SelectedTextColor;
                else if (i % 2 == 0)
                    Console.ForegroundColor = DefaultTextColor;
                else
                    Console.ResetColor();
                Console.CursorLeft = (Console.WindowWidth / 2 - Settings[i].Length / 2 - (i == selected ? (SelectedTextDelimiter[0].Length + SelectedTextDelimiter[1].Length) / 2 : 0)) - (SettingsEnabled[i] ? 1 : 0);
                Console.Write((i == selected ? SelectedTextDelimiter[0] : "") + (SettingsEnabled[i] ? "*" : "") + Settings[i] + (i == selected ? SelectedTextDelimiter[1] : "") + "\n");
            }

            Console.ForegroundColor = DefaultTextColor;
            Console.CursorLeft = 0;
        }

        /// <summary>Prints the text for the main menu.</summary>
        private static void PrintMainMenu() {

            Console.WriteLine("\t" + @"  ______                ______                    _        ");
            Console.WriteLine("\t" + @" / _____)              (____  \         _     _  | |       ");
            Console.WriteLine("\t" + @"( (____  _____ _____    ____)  )_____ _| |_ _| |_| | _____ ");
            Console.WriteLine("\t" + @" \____ \| ___ (____ |  |  __  ((____ (_   _|_   _) || ___ |");
            Console.WriteLine("\t" + @" _____) ) ____/ ___ |  | |__)  ) ___ | | |_  | |_| || ____|");
            Console.WriteLine("\t" + @"(______/|_____)_____|  |______/\_____|  \__)  \__)\_)_____)");

            Console.Write("\nUse up/down keys to make a selection, Enter to select\n\n");
      

            for (int i = 0; i < Options.Length; i++) {
                if (i == selected)
                    Console.ForegroundColor = SelectedTextColor;
                else if (i % 2 == 0)
                    Console.ForegroundColor = DefaultTextColor;
                else
                    Console.ResetColor();
                Console.CursorLeft = (Console.WindowWidth / 2 - Options[i].Length / 2 - (i == selected ? (SelectedTextDelimiter[0].Length + SelectedTextDelimiter[1].Length)/2 : 0));
                Console.Write((i == selected ? SelectedTextDelimiter[0] : "") + Options[i] + (i == selected ? SelectedTextDelimiter[1] : "") + "\n");
            }

            Console.ForegroundColor = DefaultTextColor;
            Console.CursorLeft = 0;
            Console.Write("\nMade by WGDEV, some rights reserved, see licence.txt for more info.");
            Console.WriteLine();
        }
    }
}
