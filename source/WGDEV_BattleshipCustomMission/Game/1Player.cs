/*
Class Description:
This class is used for providing a 1 player game against a cheating AI.
The ai works by targeting a ship and after a random ammount of turns, it will hit a random part of that ship.
The ai fires randomly at the player's map at other turns.
The ai picks another ship if the current one is destroyed and continues until the game ends.


Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Game
{
    class _1Player : Game
    {
        private int[] HitCoolExRange = { 4, 6 };//The range of amount of turns that the ai will wait before hitting a ship for sure
        private int CurCool = 0;//The current amount of turns that the ai will wait before hitting a ship for sure
        Random RNG = new Random();//The random number generator
        private List<int> PossibleList = new List<int>();//List of coordinates on the player's map to be hit
        private Ship.Ship SelectedShip;//The ship that is targeted
        private List<int> AttackingList = new List<int>();//List of coordinates in the targeted ship to be hit

        /// <summary>Initializes a member of the 1 Player class. Used to define key properties of the game.</summary>
        /// <param name="Bonus">Determines if the Bonus option is enabled.</param>
        /// <param name="Advanced">Determines if the advanced option is enabled.</param>
        /// <param name="MapWidth">Determines the width of both maps.</param>
        /// <param name="MapHeight">Determines the height of both maps.</param> 
        /// <param name="Ships">The ships that all players are assigned at the begining of the game.</param>
        /// <param name="Planes">The planes that all players are assigned at the begining of the game.</param> 
        public _1Player(bool Bonus, bool Salvo, bool Advanced, int MapWidth, int MapHeight, List<Ship.Ship> Ships, List<Plane.Plane> Planes) : base(
         Bonus, Salvo, Advanced, MapWidth, MapHeight, Ships, Planes)
        {

        }

        /// <summary>Runs a 1 player game.</summary>
        public override void RunGame()
        {


            SetUpMap temp = new SetUpMap(Player1, Advanced);
            temp.ManualSetUp("Player 1", ShipList, PlaneList);

            temp = new SetUpMap(Player2, Advanced);
            temp.RandomSetUpMap(ShipList, PlaneList);

            CurCool = RNG.Next(HitCoolExRange[0], HitCoolExRange[1]);
            for (int i = 0; i < Player1.Height * Player1.Width; i++)
                PossibleList.Add(i);
            SelectedShip = Player1.Ships[RNG.Next(0, Player1.Ships.Count())];
            foreach (int[] i in SelectedShip.BodyList())
                AttackingList.Add(i[0] + i[1] * Player1.Width);

            Turn t;
            do
            {
                if (Advanced)
                    t = new AdvancedTurn(Bonus, Salvo, Player1, Player2);
                else
                    t = new Turn(Bonus, Salvo, Player1, Player2);
                PauseGame("It's time for player 1 to play, press ESC to continue.");
                t.DoManualTurnSequence();
                if (Player2.hasLost())
                {
                    PauseGame("Player 1 has won the game, press ESC to return to main menu.");
                    return;
                }

                RandomTurnSequence();
                if (Player1.hasLost())
                {
                    PauseGame("Player 1 has lost the game, press ESC to return to main menu.");
                    return;
                }
            } while (true);
        }

        /// <summary>Preforms a random turn sequence for the AI</summary>
        public void RandomTurnSequence()
        {
            bool t = false;
            int TurnCount = 0;
            do
            {
                t = RandomTurn();
            } while ((++TurnCount < (Salvo ? Player2.Ships.Count : 1)) || (t && Bonus));
        }

        /// <summary>Preforms a random turn sequence for the AI</summary>
         /// <returns>A bool representing if the AI scored a hit on this turn</returns>
        public bool RandomTurn()
        {
            bool outp = false;

            int x = 0;
            int y = 0;

            CurCool--;
            do
            {
                int t = 0;
                if (CurCool <= 0 && AttackingList.Count > 0)
                {
                    t = RNG.Next(0, AttackingList.Count());
                    x = AttackingList[t];
                    AttackingList.RemoveAt(t);
                }
                else if (PossibleList.Count > 0)
                {
                    t = RNG.Next(0, PossibleList.Count());
                    x = PossibleList[t];
                    PossibleList.RemoveAt(t);
                }
                else
                    x = 0;
                y = (int)(x / Player1.Width);
                x %= Player1.Width;
            }
            while (Player1.info[x, y] != Map.Tiles.Unknown);
            if (outp = Player1.AttemptFire(new int[] { x, y })) {
                if (SelectedShip.Destroyed && Player1.Ships.Count > 0) {
                    SelectedShip = Player1.Ships[RNG.Next(0, Player1.Ships.Count())];
                    foreach (int[] i in SelectedShip.BodyList())
                        AttackingList.Add(i[0] + i[1] * Player1.Width);
                }

            }
            if (CurCool <= 0)
                CurCool = RNG.Next(HitCoolExRange[0], HitCoolExRange[1]);
            return outp;
            
        }
    }
}
