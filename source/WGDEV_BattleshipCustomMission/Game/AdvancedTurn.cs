/*
Class Description:
This class is used for preforming an advanced turn sequence during the game. 
The class presents functions that are called once to preform a turn sequence. 
In an advanced turn sequence, the player is given 5 possible things to do.
Option 1: Fire at the opponent's map, aim at a spot on the opponent's map and either fire and destroy all of the opponent's items there or use an attack that targets surrounding spots and what happens depends on the type of attack
Option 2: Use planes over the opponent's map by using an attack from the plane aimed at the plane's location.
Option 3: Deploy planes over the opponent's map
option 4: Recall planes back to the player's ships
Option 5: Fire at the player's map , aim at a spot on the player's map and either fire and destroy all the opponent's plane there or use an attack that targets surrounding spots and destroys all planes targeted

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Game
{
    class AdvancedTurn:Turn
    {
        private bool UsingSelectedAttack;//Determines if an attack from a ship is being used for the turn (requires option 1)
        private bool UsingSelectedAA;//Determines if AA from a ship is being used for the turn (requires option 5)

        private int SelectedOption;//The option that the player has currently selected

        private int[][] SelectedIndex;//A collection of indices that represent the element in the relevant lists that the player has selected to use

        /// <summary>Initializes a member of the advanced turn class. Used to define maps used and how extra turns are awarded for the turn sequence.</summary>
        /// <param name="Bonus">Determines if a turn is awarded for destroying a part of the opponent's ship.</param>
        /// <param name="Salvo">Determines if a turn is awarded for each of the players ships above one.</param>
        /// <param name="FriendlyMap">The player's map.</param
        /// <param name="EnemyMap">The opponent's map</param> 
        public AdvancedTurn(bool Bonus, bool Salvo, Map FriendlyMap, Map EnemyMap) :
            base(Bonus, Salvo, FriendlyMap, EnemyMap)
        {
            SelectedOption = 0;
            SetSelectedIndex();
            UsingSelectedAA = false;
        }

        /// <summary>Resets all the selected indices to zero so that if the list the index is from shrinks, there is no error</summary>
        private void SetSelectedIndex() {
            SelectedIndex = new int[5][];
            SelectedIndex[0] = new int[3];
            SelectedIndex[1] = new int[3];
            SelectedIndex[2] = new int[1];
            SelectedIndex[3] = new int[1];
            SelectedIndex[4] = new int[3];
            UsingSelectedAttack = false;
        }

        /// <summary>Preforms a turn based on a string. Used for LAN games</summary>
        /// <param name="Text">The specified string.</param>
        public override void DoTextTurn(string Text)
        {
            string[] inArray = Text.Split(IndexDelimiter.ToCharArray());
            EnemyMap.ETargetLocation[0] = int.Parse(inArray[0]);
            EnemyMap.ETargetLocation[1] = int.Parse(inArray[1]);
            FriendlyMap.FTargetLocation[0] = int.Parse(inArray[2]);
            FriendlyMap.FTargetLocation[1] = int.Parse(inArray[3]);
            SelectedOption = int.Parse(inArray[4]);
            UsingSelectedAttack = bool.Parse(inArray[5]);
            UsingSelectedAA = bool.Parse(inArray[6]);
            for (int i = 0; i < SelectedIndex[SelectedOption].Length; i++)
                SelectedIndex[SelectedOption][i] = int.Parse(inArray[7 + i]);
            UpdateMapData();
            switch (SelectedOption)
            {
                case 0:
                    EnemyMap.AdvancedFire(false, false, FriendlyMap.Selected == null ? null : FriendlyMap.Selected.Attacks, SelectedIndex[SelectedOption][2]);
                    break;
                case 1:
                    EnemyMap.AdvancedFire(true, false, EnemyMap.SelectedPlane.Attacks, SelectedIndex[SelectedOption][2]);
                    break;
                case 2:
                    FriendlyMap.SelectedPlane.Location = new int[] { EnemyMap.ETargetLocation[0], EnemyMap.ETargetLocation[1] };
                    EnemyMap.EPlanes.Add(FriendlyMap.SelectedPlane);
                    FriendlyMap.FPlanes.Remove(FriendlyMap.SelectedPlane);
                    break;
                case 3:
                    EnemyMap.SelectedPlane.Location = new int[] { FriendlyMap.FTargetLocation[0], FriendlyMap.FTargetLocation[1] };
                    EnemyMap.EPlanes.Remove(EnemyMap.SelectedPlane);
                    FriendlyMap.FPlanes.Add(EnemyMap.SelectedPlane);
                    break;
                case 4:
                    FriendlyMap.AdvancedFire(false, true, FriendlyMap.Selected == null ? null : FriendlyMap.Selected.Attacks, SelectedIndex[SelectedOption][2]);
                    break;

            }
        }


        /// <summary>Preforms a turn based on user input</summary>
        /// <returns>A bool representing if the player scored a hit during the turn</returns>
        public override bool DoManualTurn() {
            SelectedOption = 0;
            SetSelectedIndex();
            UpdateMapData();
            do
            {
                Console.Clear();
                Console.Write("Options available: ");
                Game.DisplayControlKey("1 ", true);
                Game.DisplayControlKey("2 ", EnemyMap.EPlanes.Count > 0);
                Game.DisplayControlKey("3 ", FriendlyMap.FPlanes.Count > 0);
                Game.DisplayControlKey("4 ", EnemyMap.EPlanes.Count > 0);
                Game.DisplayControlKey("5 ", FriendlyMap.EPlanes.Count > 0 || EnemyMap.FPlanes.Count > 0);
                Console.WriteLine();
                Console.Write("Selected option (Use # to change): " + (SelectedOption + 1).ToString() + ".");

                Map AdjustingMap = null;

                switch (SelectedOption)
                {
                    case 0:
                        Console.WriteLine("attack ships");
                        EnemyMap.PrintMap(false, EnemyMap.SelectedAttack != null ? Map.Display.Attack : Map.Display.Center, SelectedIndex[SelectedOption][2]);
                        FriendlyMap.PrintMap(true, UsingSelectedAttack ? Map.Display.Ship : Map.Display.Nothing, -1);

                        Console.WriteLine("\nShip: " + (UsingSelectedAttack ? FriendlyMap.Selected.ShipName : "none"));

                            Console.WriteLine("Attack: " + (EnemyMap.SelectedAttack == null ? "default" : EnemyMap.SelectedAttack.AttackString));
                            if (EnemyMap.SelectedAttack != null)
                                Console.WriteLine("Firing pattern: " + (1 + SelectedIndex[0][2]).ToString() + "/" + EnemyMap.SelectedAttack.SplashDamage.Length);


                        Console.WriteLine();
                        Console.WriteLine("Use R/T to browse between ships, F/G for attacks and V/B for attack layout.");
                        if (UsingSelectedAttack)
                        Console.WriteLine("Use C deselect ship and use default attack");
                        Console.WriteLine("Use arrow keys to move selection, use enter to fire.");

                        AdjustingMap = EnemyMap;
                        break;
                    case 1:
                        Console.WriteLine("use aircraft");
                        EnemyMap.PrintMap(false, Map.Display.AttackingPlane, SelectedIndex[SelectedOption][2]);
                        FriendlyMap.PrintMap(true, Map.Display.Nothing, -1);

                           Console.WriteLine("Attack: " + (EnemyMap.SelectedAttack == null ? "nothing!" : (EnemyMap.SelectedAttack.AttackName + (EnemyMap.SelectedAttack.Amount > 0 ? " x"+ EnemyMap.SelectedAttack.Amount.ToString() : ""))));
                           if (EnemyMap.SelectedAttack != null)
                                Console.WriteLine("Firing pattern: " + (1 + SelectedIndex[0][2]).ToString() + "/" + EnemyMap.SelectedAttack.SplashDamage.Length);


                        Console.WriteLine();
                        Console.WriteLine("Use R/T to browse between planes");
                        Game.DisplayControl("Use enter to fire.",EnemyMap.SelectedAttack != null);

                        AdjustingMap = null;
                        break;
                    case 2:
                        Console.WriteLine("deploy aircraft");
                        EnemyMap.PrintMap(false, Map.Display.Center, -1);
                        FriendlyMap.PrintMap(true, Map.Display.Plane, -1);
                        AdjustingMap = EnemyMap;

                        Console.WriteLine();
                        Console.WriteLine("Use R/T to browse between planes.");
                        Console.WriteLine("Use arrow keys to move selection.");
                        Game.DisplayControl("Use enter to deploy.", EnemyMap.CanPlaceEPlane(EnemyMap.ETargetLocation));
                        break;
                    case  3:
                        Console.Write("recall aircraft");
                        EnemyMap.PrintMap(false, Map.Display.Plane, -1);
                        FriendlyMap.PrintMap(true, Map.Display.Center, -1);

                        Console.WriteLine();
                        Console.WriteLine("Use R/T to browse between planes.");
                        Console.WriteLine("Use arrow keys to move selection.");
                        Game.DisplayControl("Use enter to recall.", FriendlyMap.CanPlaceFPlane(FriendlyMap.FTargetLocation));

                        AdjustingMap = FriendlyMap;
                        break;
                    case 4:
                        Console.WriteLine("AA");
                        AdjustingMap = FriendlyMap;
                        FriendlyMap.PrintMap(true, FriendlyMap.SelectedAttack != null ? Map.Display.Attack : Map.Display.Center, SelectedIndex[SelectedOption][2]);
                        FriendlyMap.PrintMap(true, Map.Display.Ship, -1);
                       Console.WriteLine("Ship: " + (UsingSelectedAA ? FriendlyMap.Selected.ShipName : "none"));

                            Console.WriteLine("\nAA: " + (FriendlyMap.SelectedAttack == null ? "default" : FriendlyMap.SelectedAttack.AttackString));
                            if (FriendlyMap.SelectedAttack != null)
                                Console.WriteLine("Firing pattern: " + (1 + SelectedIndex[4][2]).ToString() + "/" + FriendlyMap.SelectedAttack.SplashDamage.Length);
                        Console.WriteLine();
                        Console.WriteLine("Use R/T to browse between ships, F/G for attacks and V/B for attack layout.");
                        if (UsingSelectedAA)
                        Console.WriteLine("Use C deselect ship and use default attack");

                        Console.WriteLine("Use arrow keys to move selection, use enter to fire.");
                        break;
                }

                System.ConsoleKey k;
                switch (k = Console.ReadKey().Key)
                { 
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.RightArrow:
                        AttemptMoveTarget(k, AdjustingMap);
                        break;

                    case ConsoleKey.D1:
                    case ConsoleKey.D2:
                    case ConsoleKey.D3:
                    case ConsoleKey.D4:
                    case ConsoleKey.D5:
                        AttemptChangeOption(k);
                        break;

                    case ConsoleKey.R:
                    case ConsoleKey.T:
                        if (SelectedOption == 0 && UsingSelectedAttack)
                            AttemptChangeSelectedIndex(0, k);
                        else if (SelectedOption == 0)
                        {
                            UsingSelectedAttack = true;
                            UpdateMapData();
                        }
                        else if (SelectedOption == 4 && UsingSelectedAA)
                            AttemptChangeSelectedIndex(0, k);
                        else if (SelectedOption == 4)
                        {
                            UsingSelectedAA = true;
                            UpdateMapData();
                        }
                        else
                            AttemptChangeSelectedIndex(0, k);
                     break;
                    case ConsoleKey.F:
                    case ConsoleKey.G:
                     AttemptChangeSelectedIndex(1, k);
                     break;
                    case ConsoleKey.V:
                    case ConsoleKey.B:
                     AttemptChangeSelectedIndex(2, k);
                     break;
                    case ConsoleKey.C:
                     if (SelectedOption == 0 && UsingSelectedAttack)
                     {
                         UsingSelectedAttack = false;
                         UpdateMapData();
                     }
                     else if (SelectedOption == 4 && UsingSelectedAA)
                     {
                         UsingSelectedAA = false;
                         UpdateMapData();
                     }
                     break;
                    case ConsoleKey.Enter:
                        bool t = false;
                        switch (SelectedOption) {
                            case 0:
                                t= EnemyMap.AdvancedFire(false, false, FriendlyMap.Selected == null ? null : FriendlyMap.Selected.Attacks, SelectedIndex[SelectedOption][2]);
                                break;
                            case 1:
                                if (EnemyMap.SelectedAttack == null)
                                    continue;
                               t = EnemyMap.AdvancedFire(true, false, EnemyMap.SelectedPlane.Attacks, SelectedIndex[SelectedOption][2]);
                                break;
                            case 2:
                                if (!EnemyMap.CanPlaceEPlane(EnemyMap.ETargetLocation))
                                    continue;
                                FriendlyMap.SelectedPlane.Location = new int[] { EnemyMap.ETargetLocation[0], EnemyMap.ETargetLocation[1] };
                                EnemyMap.EPlanes.Add(FriendlyMap.SelectedPlane);
                                FriendlyMap.FPlanes.Remove(FriendlyMap.SelectedPlane);
                                break;
                            case 3:
                                if (!FriendlyMap.CanPlaceFPlane(FriendlyMap.FTargetLocation))
                                    continue;
                                EnemyMap.SelectedPlane.Location = new int[] { FriendlyMap.FTargetLocation[0], FriendlyMap.FTargetLocation[1] };
                                EnemyMap.EPlanes.Remove(EnemyMap.SelectedPlane);
                                FriendlyMap.FPlanes.Add(EnemyMap.SelectedPlane);
                                break;
                            case 4:
                                FriendlyMap.AdvancedFire(false, true, FriendlyMap.Selected == null ? null : FriendlyMap.Selected.Attacks, SelectedIndex[SelectedOption][2]);
                                break;

                        }

                        string outp = "The results of your turn.";
                        if (EnemyMap.DestroyedList.Count > 0)
                        {
                            outp += "\nThe following were destroyed on the enemy map: ";
                            foreach (string i in EnemyMap.DestroyedList)
                                outp += i + ", ";
                            outp = outp.Substring(0, outp.Length - 2);
                            EnemyMap.DestroyedList = new List<string>();
                        }
                        if (FriendlyMap.DestroyedList.Count > 0)
                        {
                            outp += "\nThe following were destroyed over your map: ";
                            foreach (string i in FriendlyMap.DestroyedList)
                                outp += i + ", ";
                            outp = outp.Substring(0, outp.Length - 2);
                            FriendlyMap.DestroyedList = new List<string>();
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
                           + SelectedOption + IndexDelimiter + UsingSelectedAttack + IndexDelimiter + UsingSelectedAA;
                        for (int i = 0; i < SelectedIndex[SelectedOption].Length; i++)
                            TurnText += IndexDelimiter + SelectedIndex[SelectedOption][i].ToString();
                        TurnText += TurnDelimiter;
                         
                        return t;

                }

            } while (true);
        }

        /// <summary>Attempts to adjust the location that the player is targeting based on a key that the player has pressed</summary>
        /// <param name="InpMap">The map that the player is targeting</param>
        /// <param name="k">The key that the player has pressed</param>
        private void AttemptMoveTarget(ConsoleKey k, Map InpMap) {
            if (InpMap == null)
                return;

            int[] adjustment;

            switch (k) {
                case ConsoleKey.UpArrow:
                    adjustment = new int[] { 0, -1 };
                    break;
                case ConsoleKey.DownArrow:
                    adjustment = new int[] { 0, 1 };
                    break;
                case ConsoleKey.RightArrow:
                    adjustment = new int[] { 1, 0 };
                    break;
                case ConsoleKey.LeftArrow:
                    adjustment = new int[] { -1, 0 };
                    break;
                default:
                    return;
            }
            int[] baseLocation =  InpMap == FriendlyMap ? InpMap.FTargetLocation : InpMap.ETargetLocation;
            Item tempItem = new Item(new int[] { baseLocation[0] + adjustment[0], baseLocation[1] + adjustment[1] });
            if (tempItem.WithinArea(InpMap))
            {
                baseLocation[0] += adjustment[0];
                baseLocation[1] += adjustment[1];
            }

        }

        /// <summary>
        /// Changes the selected index of the item, attack or, attack pattern that the player has selected based on user input 
        /// </summary>
        /// <param name="SelectedIndexIndex">The index of the index that the player wants to change</param>
        /// <param name="k">The key that the player has pressed</param>
        private void AttemptChangeSelectedIndex(int SelectedIndexIndex, ConsoleKey k) {
            if (GetMaxSelectedIndex(SelectedIndexIndex) >0)
            {
                for (int i = SelectedIndexIndex+ 1; i < SelectedIndex[SelectedOption].Length; i++)
                    SelectedIndex[SelectedOption][i] = 0;
                Program.AdjustListSelected(ref SelectedIndex[SelectedOption][SelectedIndexIndex], k, GetMaxSelectedIndex(SelectedIndexIndex));
                UpdateMapData();
            }
        }

        /// <summary>
        /// Changes the selected option based on user input 
        /// </summary>
        /// <param name="k">The key that the player has pressed</param>
        private void AttemptChangeOption (ConsoleKey k){
            switch (k) { 
                case ConsoleKey.D1:
                    SelectedOption = 0;
                    break;
                case ConsoleKey.D2:
                    if (EnemyMap.EPlanes.Count > 0)
                        SelectedOption = 1;
                    else
                        return;
                    break;
                case ConsoleKey.D3:
                    if (FriendlyMap.FPlanes.Count > 0)
                        SelectedOption = 2;
                    else
                        return;
                    break;
                case ConsoleKey.D4:
                    if (EnemyMap.EPlanes.Count > 0)
                        SelectedOption = 3;
                    else
                        return;
                    break;
                case ConsoleKey.D5:
                    if (FriendlyMap.EPlanes.Count > 0 || EnemyMap.FPlanes.Count > 0)
                    SelectedOption = 4;
                    break;
            }
            UpdateMapData();
        }

        /// <summary>
        /// Gets the maximum index of the item, attack or, attack pattern list that the player has selected
        /// </summary>
        /// <param name="Index">The index of the index to return</param>
        private int GetMaxSelectedIndex(int Index) {
            if (Index == 0)
            {
                switch (SelectedOption) { 
                    case 0:
                    case 4:
                        return FriendlyMap.Ships.Count;
                    case 1:
                        return EnemyMap.EPlanes.Count;
                    case 2:
                        return FriendlyMap.FPlanes.Count;
                    case 3:
                        return EnemyMap.EPlanes.Count;
                }
            } else if (Index == 1)
                switch (SelectedOption)
                {
                    case 0:
                        if (FriendlyMap.Selected != null)
                            if (FriendlyMap.Selected.Attacks != null)
                        return FriendlyMap.Selected.Attacks.Count;
                        break;
                    case 1:
                           if (EnemyMap.SelectedPlane != null)
                               if (EnemyMap.SelectedPlane.Attacks != null)
                                   return EnemyMap.SelectedPlane.Attacks.Count;
                        break;
                    case 4:
                        if (FriendlyMap.Selected != null)
                            if (FriendlyMap.Selected.AA != null)
                        return FriendlyMap.Selected.AA.Count;
                        break;
                }
            else if (Index == 2)
                switch (SelectedOption)

                { 
                    case 0:
                    case 1:
                        if (EnemyMap.SelectedAttack != null)
                        return EnemyMap.SelectedAttack.SplashDamage.Length;
                        break;
                    case 4:
                        if (FriendlyMap.SelectedAttack != null)
                        return FriendlyMap.SelectedAttack.SplashDamage.Length;
                        break;
                }
            return -1;
        }

        /// <summary>
        /// Updates the data shown on the maps based on the selected indices
        /// </summary>
        private void UpdateMapData() {
            switch (SelectedOption)
            {
                case 0:
                    if (!UsingSelectedAttack)
                    {
                        EnemyMap.SelectedAttack = null;
                        FriendlyMap.Selected = null;
                    }
                    else {
                        FriendlyMap.Selected = FriendlyMap.Ships[SelectedIndex[SelectedOption][0]];
                        if (FriendlyMap.Selected.Attacks != null && FriendlyMap.Selected.Attacks.Count > 0)
                            EnemyMap.SelectedAttack = FriendlyMap.Selected.Attacks[SelectedIndex[SelectedOption][1]];
                        else
                            EnemyMap.SelectedAttack = null;
                    }
                    break;
                case 1:
                        EnemyMap.SelectedPlane = EnemyMap.EPlanes[SelectedIndex[SelectedOption][0]];
                        if (FriendlyMap.SelectedPlane.Attacks != null && FriendlyMap.SelectedPlane.Attacks.Count > 0)
                            EnemyMap.SelectedAttack = FriendlyMap.SelectedPlane.Attacks[SelectedIndex[SelectedOption][1]];
                        else
                            EnemyMap.SelectedAttack = null;
                        break;
                case 2:
                        FriendlyMap.SelectedPlane = FriendlyMap.FPlanes[SelectedIndex[SelectedOption][0]];
                        break;
                case 3:
                        EnemyMap.SelectedPlane = EnemyMap.EPlanes[SelectedIndex[SelectedOption][0]];
                        break;
                case 4:
                        if (!UsingSelectedAA)
                        {
                            FriendlyMap.SelectedAttack = null;
                            FriendlyMap.Selected = null;
                        }
                        else
                        {
                            FriendlyMap.Selected = FriendlyMap.Ships[SelectedIndex[SelectedOption][0]];
                            if (FriendlyMap.Selected.AA != null)
                                FriendlyMap.SelectedAttack = FriendlyMap.Selected.AA[SelectedIndex[SelectedOption][1]];
                            else
                                FriendlyMap.SelectedAttack = null;
                        }
                        break;

            }
        }
    }
}
