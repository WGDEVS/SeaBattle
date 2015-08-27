/*
Class Description:
This class is used for setting up the maps before the game. 
The class presents functions that are called once to set up the map for the player. 

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Game
{
    class SetUpMap
    {
        private int MapIndexSelected = 0;//The index of the selected item on the map
        private int QIndexSelected = 0;//The index of the selected item in the queue
        private Map Map;//The map that is currently being set up
        private bool Advanced;//Is the advanced option enabled for this map?

        /// <summary>Initializes a member of the SetUpMap class. Used to define the map to set up and if the advanced option is enabled for the player.</summary>
        /// <param name="InpMap">The players map.</param>
        /// <param name="Advanced">Determines if the advanced option is enabled.</param>
        public SetUpMap(Map InpMap, bool Advanced) {
            Map = InpMap;
            this.Advanced = Advanced;
        }

        /// <summary>Randomly sets up the map. Used for the AI</summary>
        /// <param name="ShipList">The list of ships to add to the map.</param>
        /// <param name="PlaneList">The list of planes to add to the map.</param>
        public void RandomSetUpMap(List<Ship.Ship> ShipList, List<Plane.Plane> PlaneList)
        {
            RandomAddShips(ShipList);
            if (Advanced)
                RandomAddPlanes(PlaneList);
        }

        /// <summary>Randomly adds ships to the map. Used for the AI</summary>
        /// <param name="InQ">The list of ships to add to the map.</param>
        public void RandomAddShips(List<Ship.Ship> InQ) {
            Random rng = new Random();
            foreach (Ship.Ship j in InQ) {
                Ship.Ship i = (Ship.Ship)j.Clone();

                do { 
                    switch (rng.Next(0, 2))
                    {
                        case 0:
                            i.LocationDirection = Ship.Ship.Direction.East;
                            break;
                        case 1:
                            i.LocationDirection = Ship.Ship.Direction.South;
                            break;
                    }


                    i.Location[0] = rng.Next(0, i.LocationDirection == Ship.Ship.Direction.East ? Map.Width-i.BodyList().Length + 1 : Map.Width);
                    i.Location[1] = rng.Next(0, i.LocationDirection == Ship.Ship.Direction.South ? Map.Height - i.BodyList().Length + 1 : Map.Height);
                } while (!Map.CanPlace(i));
                Map.AddShip(i);
            }
        }

        /// <summary>Randomly adds planes to the map. Used for the AI</summary>
        /// <param name="InQ">The list of planes to add to the map.</param>
        public void RandomAddPlanes(List<Plane.Plane> InQ)
        {
            Random rng = new Random();

            int InQIndex = 0;

            foreach (Ship.Ship i in Map.Ships) {
                if (i.CanCarryPlanes)
                {
                    for (int j = 0; InQIndex < InQ.Count && j < i.BodyList().Length; j++)
                    {
                        Plane.Plane temp = (Plane.Plane)InQ[InQIndex++].Clone();
                        temp.Location[0] = i.BodyList()[j][0];
                        temp.Location[1] = i.BodyList()[j][1];
                        Map.AddPlane(temp);
                    }
                    if (InQIndex >= InQ.Count)
                        return;
                }
            }
        }

        /// <summary>Sets up the map based on a string, used for LAN games</summary>
        /// <param name="Inp">The specified string.</param>
        /// <param name="ShipList">The list of ships to add to the map.</param>
        /// <param name="PlaneList">The list of planes to add to the map.</param>
        public void TextSetUp(string Inp, List<Ship.Ship> ShipList, List<Plane.Plane> PlaneList)
        {
            string[] inps = Inp.Split(LanHost.ActionDelimiter.ToCharArray());
            TextAddShips(inps[0], ShipList);
            if (Advanced)
                TextAddPlanes(inps[1], PlaneList);
        }

        /// <summary>Adds planes to the map based on a string, used for LAN games</summary>
        /// <param name="InText">The specified string.</param>
        /// <param name="InQ">The list of planes to add to the map.</param>
        public void TextAddPlanes(string InText, List<Plane.Plane> InQ) {
            string[] inp = InText.Split(LanHost.FieldDelimiter.ToCharArray());
            for (int i = 0; i < inp.Length;)
            {
                Plane.Plane t = (Plane.Plane)InQ[int.Parse(inp[i++])].Clone();
                t.Location[0] = int.Parse(inp[i++]);
                t.Location[1] = int.Parse(inp[i++]);
                Map.AddPlane(t);
            }
        }

        /// <summary>Adds ships to the map based on a string, used for LAN games</summary>
        /// <param name="InText">The specified string.</param>
        /// <param name="InQ">The list of ships to add to the map.</param>
        public void TextAddShips(string InText, List<Ship.Ship> InQ)
        {
            string[] inp = InText.Split(LanHost.FieldDelimiter.ToCharArray());
            for (int i = 0; i < inp.Length;)
            {
                Ship.Ship t = (Ship.Ship)InQ[int.Parse(inp[i++])].Clone();
                t.LocationDirection = (Ship.Ship.Direction)int.Parse(inp[i++]);
                t.Location[0] = int.Parse(inp[i++]);
                t.Location[1] = int.Parse(inp[i++]);
                Map.AddShip(t);
            }
        }

        /// <summary>Sets up the map based on user input</summary>
        /// <param name="PlayerName">The string representation of the player.</param>
        /// <param name="ShipList">The list of ships to add to the map.</param>
        /// <param name="PlaneList">The list of planes to add to the map.</param>
        /// <returns>A string representation of the set up pattern that has been performed, used for LAN games</returns>
        public string ManualSetUp(string PlayerName, List<Ship.Ship> ShipList, List<Plane.Plane> PlaneList)
        {
            string outp = "";
            Game.PauseGame("It's time for  " + PlayerName + " to place ships, press ESC to continue.");
            outp += ManualAddShips(ShipList);
            if (Advanced)
            {
                Game.PauseGame("It's time for " + PlayerName + " to place planes, press ESC to continue.");
                outp += LanHost.ActionDelimiter + ManualAddPlanes(PlaneList);
            }
            return outp;
        }

        /// <summary>Adds planes to the map based on user input</summary>
        /// <param name="InpPlanes">The list of planes to add to the map.</param>
        /// <returns>A string representation of the set up pattern of the planes, used for LAN games</returns>
        public string ManualAddPlanes(List<Plane.Plane> InpPlanes)
        {
            Map.SettingUpPlanes = true;
            List<Plane.Plane> q = new List<Plane.Plane>();

            int num = 0;
            foreach (Plane.Plane i in InpPlanes)
            {
                Plane.Plane t = (Plane.Plane)i.Clone();
                t.Number = num++;
                q.Add(t);
            }

            bool movingExisting = false;
            MapIndexSelected = 0;
            QIndexSelected = 0;

            do
            {
                if (!movingExisting)
                {
                    Map.SelectedPlane = q[QIndexSelected];
                }
                else
                {
                    Map.SelectedPlane = Map.FPlanes[MapIndexSelected];
                }
                Console.Clear();


                Map.PrintMap(true, Map.Display.Plane, -1);
                if (Map.SelectedPlane != null)
                {
                    Console.WriteLine("Plane selected: " + Map.SelectedPlane.PlaneName);
                };

                string t = "";
                if (Map.SelectedPlane.Attacks != null)
                {
                    foreach (Attack.Attack i in Map.SelectedPlane.Attacks)
                        t += ", " + i.AttackString;
                    t = t.Substring(1, t.Length - 1);
                }
                    Console.WriteLine("Attacks:" + t);

                if (movingExisting)
                    Console.WriteLine("The plane selected has been placed on the map, use \"c\" to pick it up.");
                else
                    Game.DisplayControl("The plane selected has not been placed on the map, use SPACE to place it.",Map.CanPlaceFPlane(Map.SelectedPlane));

                Console.WriteLine();
                Console.WriteLine("Use arrow keys to change the plane's location");
                Game.DisplayControl("Use R/T to select planes that have not been placed.",q.Count>0);
                Game.DisplayControl("Use F/G to select planes that have been placed.",Map.FPlanes.Count >0);
                Game.DisplayControl("Use ENTER to finish placing planes when all planes have been placed.", q.Count <= 0);

                ConsoleKey k;
                switch (k= Console.ReadKey().Key)
                {
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.DownArrow:
                        if (!movingExisting)
                            AttemptMove(Map.SelectedPlane, k);
                        break;
                    case ConsoleKey.R:
                    case ConsoleKey.T:
                        if (q.Count > 0)
                        {
                            if (!movingExisting)
                            {
                                Program.AdjustListSelected(ref QIndexSelected, k, q.Count);
                            }
                            else
                                movingExisting = false;

                        }
                        break;
                    case ConsoleKey.Spacebar:
                        if (!movingExisting && Map.CanPlaceFPlane(Map.SelectedPlane))
                        {
                            Map.AddPlane(Map.SelectedPlane);
                            q.Remove(Map.SelectedPlane);
                            if (q.Count == 0)
                                movingExisting = true;
                            Program.WrapAdd(ref QIndexSelected, 0, q.Count);
                        }
                        break;
                    case ConsoleKey.F:
                    case ConsoleKey.G:
                        if (Map.FPlanes.Count > 0)
                        {
                            if (movingExisting)
                            {
                                Program.AdjustListSelected(ref MapIndexSelected, k, Map.FPlanes.Count);
                            }
                            else
                                movingExisting = true;

                        }
                        break;
                    case ConsoleKey.C:
                        if (movingExisting)
                        {
                            movingExisting = false;
                            q.Insert(QIndexSelected, Map.SelectedPlane);
                            Map.FPlanes.Remove(Map.SelectedPlane);
                            Program.WrapAdd(ref MapIndexSelected, 0, Map.FPlanes.Count);
                        }
                        break;
                    case ConsoleKey.Enter:
                        if (q.Count == 0)
                        {
                            string outp = "";
                            foreach (Plane.Plane i in Map.FPlanes)
                                outp += i.Number + LanHost.FieldDelimiter + i.Location[0] + LanHost.FieldDelimiter + i.Location[1] + LanHost.FieldDelimiter;
                            return outp.Substring(0, outp.Length - LanHost.FieldDelimiter.Length);
                        }
                        break;


                }

            } while (true);
        }

        /// <summary>Adds ships to the map based on user input</summary>
        /// <param name="InpShips">The list of ships to add to the map.</param>
        /// <returns>A string representation of the set up pattern of the ships, used for LAN games</returns>
        public string ManualAddShips(List<Ship.Ship> InpShips)
        {

            List<Ship.Ship> q = new List<Ship.Ship>();
            int num = 0;
            foreach (Ship.Ship i in InpShips)
            {
                Ship.Ship t = (Ship.Ship)i.Clone();
                t.Number = num++;
                q.Add(t);
            }


            bool movingExisting = false;

            do
            {
                if (!movingExisting)
                {
                    Map.Selected = q[QIndexSelected];
                }
                else
                {
                    Map.Selected = Map.Ships[MapIndexSelected];
                }
                Console.Clear();

                Map.PrintMap(true, Map.Display.Ship, -1);

                Console.WriteLine("Ship selected: " + Map.Selected.ShipName);

                if (Advanced)
                {
                    string t = "";
                    if (Map.Selected.Attacks != null)
                    {
                        foreach (Attack.Attack i in Map.Selected.Attacks)
                                t += ", " + i.AttackString;
                        t = t.Substring(1, t.Length - 1);
                    }

                    Console.WriteLine("Anti-ship attacks:" + t);
                    t = "";
                    if (Map.Selected.AA != null)
                    {
                        foreach (Attack.Attack i in Map.Selected.AA)
                        {
                            if (i.Amount > 0)
                                t += ", " + i.AttackString;
                            else if (i.Amount != 0)
                                t += ", " + i.AttackName;
                        }
                        t = t.Substring(1, t.Length - 1);
                    }
                    Console.WriteLine("Anti-air attacks:" + t);
                    Console.WriteLine("Aircraft Storage: " + (!Map.Selected.CanCarryPlanes ? "un" : "") + "availabe");
                }
                Console.WriteLine();
                if (movingExisting)
                    Console.WriteLine("The ship selected has been placed on the map, use \"c\" to pick it up.");
                else
                {
                    Game.DisplayControl("The ship selected has not been placed on the map, use SPACE to place it.", Map.CanPlace(Map.Selected));
                }
                Game.DisplayControl("Use wasd to change the ship's direction",!movingExisting);
                Game.DisplayControl("Use arrow keys to change the ship's location",!movingExisting);
                Game.DisplayControl("Use R/T to select ships that have not been placed.", q.Count > 0);
                Game.DisplayControl("Use F/G to select ships that have been placed.", Map.Ships.Count > 0);
                Game.DisplayControl("Use ENTER to finish placing ships when all ships have been placed.", q.Count <= 0);

                Ship.Ship.Direction lastDir = Map.Selected.LocationDirection;
                ConsoleKey k;
                switch (k = Console.ReadKey().Key)
                {
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.DownArrow:
                        if (!movingExisting)
                            AttemptMove(Map.Selected, k);
                        break;

                    case ConsoleKey.D:
                    case ConsoleKey.W:
                    case ConsoleKey.A:
                    case ConsoleKey.S:
                        if (!movingExisting)
                            AttemptChangeDirection(Map.Selected, k);
                        break;

                    case ConsoleKey.R:
                    case ConsoleKey.T:
                        if (q.Count > 0)
                        {
                            if (!movingExisting)
                            {
                                Program.AdjustListSelected(ref QIndexSelected, k, q.Count);
                            }
                            else
                                movingExisting = false;

                        }
                        break;
                    case ConsoleKey.Spacebar:
                        if (!movingExisting && Map.CanPlace(Map.Selected))
                        {
                            Map.AddShip(Map.Selected);
                            q.Remove(Map.Selected);
                            if (q.Count == 0)
                                movingExisting = true;
                            Program.WrapAdd(ref QIndexSelected, 0, q.Count);
                        }
                        break;
                    case ConsoleKey.F:
                    case ConsoleKey.G:
                        if (Map.Ships.Count > 0)
                        {
                            if (movingExisting)
                            {
                                Program.AdjustListSelected(ref MapIndexSelected, k, Map.Ships.Count);
                            }
                            else
                                movingExisting = true;

                        }
                        break;
                    case ConsoleKey.C:
                        if (movingExisting)
                        {
                            movingExisting = false;
                            q.Insert(QIndexSelected, Map.Selected);
                            Map.Ships.Remove(Map.Selected);
                            Program.WrapAdd(ref MapIndexSelected, 0, Map.Ships.Count);
                        }
                        break;
                    case ConsoleKey.Enter:
                        if (q.Count == 0)
                        {
                            string outp = "";
                            foreach (Ship.Ship i in Map.Ships)
                                outp += i.Number + LanHost.FieldDelimiter + ((int)i.LocationDirection).ToString() + LanHost.FieldDelimiter + i.Location[0] + LanHost.FieldDelimiter + i.Location[1] + LanHost.FieldDelimiter;
                            return outp.Substring(0, outp.Length - LanHost.FieldDelimiter.Length);
                        }
                        break;

                }

            } while (true);
        }

        private void AttemptMove(Item InpItem, ConsoleKey InpKey)
        {
            switch (InpKey)
            {
                case ConsoleKey.RightArrow:
                    AttemptMove(InpItem, new int[] { 1, 0 });
                    break;
                case ConsoleKey.UpArrow:
                    AttemptMove(InpItem, new int[] { 0, -1 });
                    break;
                case ConsoleKey.LeftArrow:
                    AttemptMove(InpItem, new int[] { -1, 0 });
                    break;
                case ConsoleKey.DownArrow:
                    AttemptMove(InpItem, new int[] { 0, 1 });
                    break;
            }
        }

        private void AttemptMove(Item InpItem, int[] Adjustment) { 
            InpItem.Location[0] += Adjustment[0];
            InpItem.Location[1] += Adjustment[1];
            if (!InpItem.WithinArea(Map))
            {
                InpItem.Location[0] -= Adjustment[0];
                InpItem.Location[1] -= Adjustment[1];
            }
        }


        private void AttemptChangeDirection(Ship.Ship InpShip, ConsoleKey InpKey) {
            switch (InpKey)
            {
                case ConsoleKey.D:
                    AttemptChangeDirection(InpShip, Ship.Ship.Direction.East);
                    break;
                case ConsoleKey.W:
                    AttemptChangeDirection(InpShip, Ship.Ship.Direction.North);
                    break;
                case ConsoleKey.A:
                    AttemptChangeDirection(InpShip, Ship.Ship.Direction.West);
                    break;
                case ConsoleKey.S:
                    AttemptChangeDirection(InpShip, Ship.Ship.Direction.South);
                    break;
            }

        }
        private void AttemptChangeDirection(Ship.Ship InpShip, Ship.Ship.Direction NewDirection) {
            Ship.Ship.Direction OldDirection = InpShip.LocationDirection;
            InpShip.LocationDirection = NewDirection;
            if (InpShip.WithinArea(Map))
                return;
            InpShip.LocationDirection = OldDirection;
        }
    }
}
