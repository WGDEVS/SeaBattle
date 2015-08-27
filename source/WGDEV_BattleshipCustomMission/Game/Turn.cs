/*
Class Description:
This class is used for preforming a normal turn sequence during the game. 
The class presents functions that are called once to preform a turn sequence. 
In a normal turn sequence, the player is not allowed to use any attacks 
(including AA) other than the basic attack of firing on the enemy map.

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Game
{
    class Turn
    {
        public Map FriendlyMap;//This is the player's map
        public Map EnemyMap;//This is the opponent's map
        public bool Bonus;//Boolean to check if this turn sequence is under the bonus option
        public bool Salvo;//Boolean to check if this turn sequence is under the salvo option
        protected string TurnText;//The string representation of this turn, used in LAN games
        protected const string TurnDelimiter = "/";//A delimiter for the string representation
        protected const string IndexDelimiter = " ";//A different delimiter for the string representation

        /// <summary>Initializes a member of the turn class. Used to define maps used and how extra turns are awarded for the turn sequence.</summary>
        /// <param name="Bonus">Determines if a turn is awarded for destroying a part of the opponent's ship.</param>
        /// <param name="Salvo">Determines if a turn is awarded for each of the players ships above one.</param>
        /// <param name="FriendlyMap">The player's map.</param
        /// <param name="EnemyMap">The opponent's map</param> 
        public Turn(bool Bonus,bool Salvo,Map FriendlyMap, Map EnemyMap)
        {
            this.FriendlyMap = FriendlyMap;
            this.EnemyMap = EnemyMap;
            this.Bonus = Bonus;
            this.Salvo = Salvo;
        }

        /// <summary>Preforms a turn sequence based on a string. Used for LAN games</summary>
        /// <param name="Text">The specified turn string.</param>
        public void DoTextTurnSequence(string Text) {
            string[] mainArray = Text.Split(TurnDelimiter.ToCharArray());
            foreach (string i in mainArray)
                if (i != "")
                    DoTextTurn(i);
        }

        /// <summary>Preforms a turn sequence based on a string. Used for LAN games</summary>
        /// <param name="Text">The specified turn string.</param>
        public virtual void DoTextTurn(string Text)
        {
            string[] inArray = Text.Split(IndexDelimiter.ToCharArray());
            EnemyMap.ETargetLocation[0] = int.Parse(inArray[0]);
            EnemyMap.ETargetLocation[1] = int.Parse(inArray[1]);
            EnemyMap.AttemptFire();
        }

        /// <summary>Preforms a turn sequence based on user input</summary>
        /// <param name="Text">The string representation of the turn sequence. Used for lan games</param>
        public string DoManualTurnSequence() {
            TurnText = "";
            bool t = false;
            int TurnCount = 0;
            do
            {
                string outp = "";

                outp += "This is your "+(TurnCount+1).ToString()+" turn in this sequence.\n";
                outp += "Salvo Option is " + (Salvo ? "en" : "dis") + "abled.";
                if (Salvo)
                    outp += " You have " + (FriendlyMap.Ships.Count - TurnCount > 0 ? FriendlyMap.Ships.Count - TurnCount : 0).ToString() + " salvos left.";
                outp += "\nBonus Option is " + (Bonus ? "en" : "dis") + "abled.\nPress ESC to continue.";
                Game.PauseGame(outp);
                t = DoManualTurn();
            } while ((++TurnCount < (Salvo ? FriendlyMap.Ships.Count : 1)) || (t&& Bonus));
            return TurnText;
        }

        /// <summary>Preforms a turn based on user input</summary>
        /// <returns>A bool representing if the player scored a hit on this turn</returns>
        public virtual bool DoManualTurn() {
            do
            {
                Console.Clear();
                EnemyMap.PrintMap(false, Map.Display.Center, -1);
                FriendlyMap.PrintMap(true, Map.Display.Nothing, -1);

                Console.WriteLine("Use the arrow keys to make a selection.");
                Console.WriteLine("Use Enter to fire.");

                ConsoleKey k;
                switch (k = Console.ReadKey().Key) { 
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.LeftArrow:
                        AttemptAdjustTarget(EnemyMap, false, k);
                        break;
                    case ConsoleKey.Enter:
                        {
                            bool t = EnemyMap.AttemptFire();

                            string outp = "The results of your turn.";
                            if (EnemyMap.DestroyedList.Count > 0)
                            {
                                outp += "\nThe following were destroyed on the enemy map: ";
                                foreach (string i in EnemyMap.DestroyedList)
                                    outp += i + ", ";
                                outp = outp.Substring(0, outp.Length - 2);
                                EnemyMap.DestroyedList = new List<string>();
                            }
                            outp += "\nPress SPACE to continue.";
                            Console.Clear();
                            EnemyMap.PrintMap(false, Map.Display.Nothing, -1);
                            FriendlyMap.PrintMap(true, Map.Display.Nothing, -1);
                            Console.Write(outp);
                            do
                            {
                            } while (Console.ReadKey().Key != ConsoleKey.Spacebar);

                            TurnText += EnemyMap.ETargetLocation[0].ToString() + IndexDelimiter + EnemyMap.ETargetLocation[1].ToString() + IndexDelimiter
                             + FriendlyMap.FTargetLocation[0].ToString() + IndexDelimiter + FriendlyMap.FTargetLocation[1].ToString() + IndexDelimiter
                            + "0" + IndexDelimiter + "false" + IndexDelimiter + "false";
                            for (int i = 0; i < 3; i++)
                                TurnText += IndexDelimiter + "0";
                            TurnText += TurnDelimiter;

                            return t;
                        }
                }
            } while (true);
        }

        /// <summary>Attempts to adjust the location that the player is targeting based on key pressed</summary>
        /// <param name="InpMap">The map that the player is targeting</param>
        /// <param name="Detail">Determines if the player owns the specified map</param>
        /// <param name="InpKey">The key that the player has pressed</param>
        private void AttemptAdjustTarget(Map InpMap, bool Detail, ConsoleKey InpKey)
        {
            switch (InpKey)
            {
                case ConsoleKey.RightArrow:
                    AttemptAdjustTarget(InpMap,Detail, new int[] { 1, 0 });
                    break;
                case ConsoleKey.UpArrow:
                    AttemptAdjustTarget(InpMap, Detail, new int[] { 0, -1 });
                    break;
                case ConsoleKey.LeftArrow:
                    AttemptAdjustTarget(InpMap, Detail, new int[] { -1, 0 });
                    break;
                case ConsoleKey.DownArrow:
                    AttemptAdjustTarget(InpMap, Detail, new int[] { 0, 1 });
                    break;
            }
        }

        /// <summary>Attempts to adjust the location that the player is targeting based on a coordinate that is added to to the location</summary>
        /// <param name="InpMap">The map that the player is targeting</param>
        /// <param name="Detail">Determines if the player owns the specified map</param>
        /// <param name="Adjustment">The specified coordinate</param>
        private void AttemptAdjustTarget(Map InpMap, bool Detail, int[] Adjustment)
        {
            int[] tTargetLocation = (Detail ? InpMap.FTargetLocation : InpMap.ETargetLocation);
            tTargetLocation[0] += Adjustment[0];
            tTargetLocation[1] += Adjustment[1];
            if (tTargetLocation[0] >= InpMap.Width || tTargetLocation[0] < 0
                || tTargetLocation[1] >= InpMap.Height || tTargetLocation[1] < 0)
            {
                tTargetLocation[0] -= Adjustment[0];
                tTargetLocation[1] -= Adjustment[1];
            }
        }
    }
}
