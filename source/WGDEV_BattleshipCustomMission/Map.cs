/*
Class Description:
This class is used for keeping track of a map. The class presents all the variables neccessary to 
keep track of everything in a standard Sea Battle map. It includes functions for checking to see 
if it is legal to do an action within the game, code for preforming some actions in the game and 
is responsible for providing a visual representation of the Map for both players.


Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission
{
    public class Map
    {
        public bool SettingUpPlanes = false;//Indicates if the map is currently used to set up planes, used in the draw function
        public enum Display { Nothing = 0, Center = 1, Attack = 2, Ship = 3, Plane = 4, AttackingPlane = 5};//Possible things to highlight
        public enum Tiles { Unknown = 0, Possible = 1, Detected = 2, Miss = 3, Hit = 4 };//Possible things the opponent knows about the map

        public Ship.Ship Selected;//The ship to be highlighted during a draw function
        public Attack.Attack SelectedAttack;//The attack to be shown during a draw function
        public Plane.Plane SelectedPlane;//The plane to be highlighted during a draw function
        
        public int[] FTargetLocation = (int[])Program.DefaultLocation.Clone();//Where the player is targenting
        public int[] ETargetLocation = (int[])Program.DefaultLocation.Clone();//Where the opponent is targeting

        public List<Ship.Ship> Ships;//The list of ships that the player has, if it is empty during gameplay, the player loses
        public List<Plane.Plane> EPlanes = new List<Plane.Plane>();//The opponent's planes over the map, can be destroyed by AA
        public List<Plane.Plane> FPlanes = new List<Plane.Plane>();//The player's planes on the player's ships in the map, becomes destroyed with the part of the ship it is on
        public int Width;//The width of the map
        public int Height;//The height of the map
        public List<string> DestroyedList = new List<string>();//A list of string representations of items destroyed during a turn on the map

        public Tiles[,] info;//A list of information that the opponent has about the map, some of it is shown to the player

        /// <summary>Creates a Map with the dimensions specified</summary>
        /// <param name="Width">The width of the map.</param>
         /// <param name="Height">The Height of the map.</param>
        public Map(int Width,int Height) {
            this.Height = Height;
            this.Width = Width;
            info = new Tiles[Width, Height];
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    info[i, j] = Tiles.Unknown;
            Ships = new List<Ship.Ship>();
            EPlanes = new List<Plane.Plane>();
        }

        /// <summary>Checks if the opponent can deploy a plane at a specified spot</summary>
        /// <param name="NewLoc">The spot specified.</param>
        /// <returns>A bool indicating if the opponent can deploy the plane</returns>
        public bool CanPlaceEPlane(int[] NewLoc) {
            foreach (Plane.Plane j in EPlanes)
            {
                if (j.TileConflict(NewLoc))
                    return false;
            }
            return true;
        }

        /// <summary>Checks if the player can place a plane during setup</summary>
        /// <param name="NewPlane">The plane to be place.</param>
        /// <returns>A bool indicating if the plane can be placed</returns>
        public bool CanPlaceFPlane(Plane.Plane NewPlane)
        {
            foreach (Ship.Ship i in Ships)
            {
                if (i.CanHoldPlaneAt(NewPlane.Location))
                {
                    foreach (Plane.Plane j in FPlanes) { 
                        if (NewPlane.TileConflict(j))
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }


        /// <summary>Checks if the player can land a plane at a spot during gameplay</summary>
        /// <param name="NewLoc">The spot specified by the player.</param>
        /// <returns>A bool indicating if the plane can be landed</returns>
        public bool CanPlaceFPlane(int[] NewLoc)
        {
            foreach (Ship.Ship i in Ships)
            {
                if (i.CanHoldPlaneAt(NewLoc))
                {
                    foreach (Plane.Plane j in FPlanes)
                    {
                        if (j.TileConflict(NewLoc))
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>Places a plane during setup</summary>
        /// <param name="NewPlane">The plane to be place.</param>
        /// <returns>A bool indicating if the plane was successfully placed</returns>
        public bool AddPlane(Plane.Plane NewPlane)
        {
            if (CanPlaceFPlane(NewPlane))
            {
                FPlanes.Add(NewPlane);
                return true;
            }
            return false;
        }

        public bool AddShip(Ship.Ship NewShip)
        {
            if  (CanPlace(NewShip)){
            Ships.Add(NewShip);
                return true;
            }
            return false;
        }
        public bool CanPlace(Ship.Ship tShip) {
            foreach (Ship.Ship i in Ships)
            {
                if (i == tShip)
                    continue;
                foreach (int[] j in tShip.BodyList())
                    if (i.CheckHit(j, false))
                        return false;
            }

            return true;
        }

        /// <summary>Prints a representation of the map</summary>
        /// <param name="Detail">Indicates if the map is being printed for the player (opponent if false)</param>
        /// <param name="Mode">The feature of the map highlighted</param>
        /// <param name="SelectedIndex">If the map is being used to highlight an attack, this specified which configuration of the attack is shown</param>
        public void PrintMap(bool Detail, Display Mode, int SelectedIndex) {

            //Declare some temporary variables to buffer the data before it is printed
            string[,] shownTileStr = new string[Width, Height];
            ConsoleColor[,] shownTileBack = new ConsoleColor[Width, Height];
            ConsoleColor[,] shownTileFront = new ConsoleColor[Width, Height];

            //Fill  out the temporary variables
            if (Detail)
            {
                //Show the ships for the player
                bool inShips = false;
                foreach (Ship.Ship i in Ships)
                {
                    if (i == Selected)
                    {
                        inShips = true;
                    }
                    foreach (int[] j in i.BodyList())
                    {
                        shownTileStr[j[0], j[1]] = i.ShipTile;
                    }
                }

                //Show the planes on the ships for the player
                foreach (Plane.Plane j in FPlanes)
                {
                    shownTileFront[j.Location[0], j.Location[1]] = j.PlaneColor;
                    if (j == SelectedPlane && Mode == Display.Plane && SettingUpPlanes)
                        shownTileBack[j.Location[0], j.Location[1]] = Program.SplashCol;
                }

                //Highlight the specified ship for the player
                if (Selected != null && Mode == Display.Ship)
                {
                    int[][] tempy = Selected.BodyList();

                    for (int i = 0; i < tempy.Length; i++)
                    {
                        if (i == 0 && !inShips && SelectedIndex < 0)
                            shownTileBack[tempy[i][0], tempy[i][1]] = Program.TargetIncCol;
                        else
                            shownTileBack[tempy[i][0], tempy[i][1]] = Program.SplashCol;
                        if (!inShips)
                            shownTileStr[tempy[i][0], tempy[i][1]] = Selected.ShipTile;
                    }
                    if (SelectedIndex >= 0 && shownTileBack[tempy[SelectedIndex][0], tempy[SelectedIndex][1]] == new ConsoleColor())
                        shownTileBack[tempy[SelectedIndex][0], tempy[SelectedIndex][1]] = Program.TargetIncCol;
                }
            }
            else {
                //Show the planes over the map for the opponent
                foreach (Plane.Plane j in EPlanes)
                {
                    shownTileFront[j.Location[0], j.Location[1]] = j.PlaneColor;
                    if (shownTileStr[j.Location[0], j.Location[1]] == null)
                        shownTileStr[j.Location[0], j.Location[1]] = "*";
                }
            }

            //Highlight and show the specified attack
            if (Mode == Display.Center)
                shownTileBack[Detail ? FTargetLocation[0] : ETargetLocation[0], Detail ? FTargetLocation[1] : ETargetLocation[1]] = Program.TargetIncCol;

            if ((Mode == Display.Attack || (Mode == Display.AttackingPlane && SelectedPlane != null)) && SelectedAttack != null)
            {
                int x = (Mode == Display.Attack ? (Detail ? FTargetLocation[0] : ETargetLocation[0]) : SelectedPlane.Location[0]);
                int y = (Mode == Display.Attack ? (Detail ? FTargetLocation[1] : ETargetLocation[1]) : SelectedPlane.Location[1]);
                foreach (int[] i in SelectedAttack.SplashDamage[SelectedIndex])
                {
                    if (i[0] + x >= 0 && i[0] + x < Width &&
                        i[1] + y >= 0 && i[1] + y < Height
                        )
                        shownTileBack[i[0] + x, i[1] + y] = Program.SplashCol;
                }
                shownTileBack[x, y] = SelectedAttack.CenterIncluded[SelectedIndex] ? Program.TargetIncCol : Program.TargetExCol;
            }
            else if (SelectedAttack == null && Mode == Display.AttackingPlane && SelectedPlane != null)
            {
                shownTileBack[SelectedPlane.Location[0], SelectedPlane.Location[1]] = Program.TargetExCol;
            }

            //Copy information about the tiles to the buffer
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    switch (info[i, j])
                    {
                        case Tiles.Hit:
                            shownTileStr[i, j] = "X";
                            break;
                        case Tiles.Detected:
                            if (!Detail)
                            shownTileStr[i, j] = "!";
                            break;
                        case Tiles.Possible:
                            if (!Detail)
                            shownTileStr[i, j] = "?";
                            break;
                        case Tiles.Miss:
                            shownTileStr[i, j] = "°";
                            break;
                    }
                }

            //Highligh the specified plane
            if (Mode == Display.Plane && SelectedPlane != null && shownTileBack[SelectedPlane.Location[0], SelectedPlane.Location[1]] != Program.SplashCol)
            {
                shownTileBack[SelectedPlane.Location[0], SelectedPlane.Location[1]] = Program.TargetIncCol;
                if (shownTileStr[SelectedPlane.Location[0], SelectedPlane.Location[1]] == null)
                    shownTileStr[SelectedPlane.Location[0], SelectedPlane.Location[1]] = "*";
                shownTileFront[SelectedPlane.Location[0], SelectedPlane.Location[1]] = SelectedPlane.PlaneColor;
            }
            
            //Make sure that the plane can be seen (it is shown with forecolor and will not be seen if there is no text)
            if (!Detail)
                foreach (Plane.Plane i in EPlanes)
                {
                shownTileFront[i.Location[0], i.Location[1]] = i.PlaneColor;
                if (shownTileStr[i.Location[0], i.Location[1]] == "") {
                        shownTileStr[i.Location[0], i.Location[1]] = i.Tile;
                }
            }

            //Print the buffer to the screen
            string buf = "";
            Console.CursorLeft = Console.WindowWidth / 2 - (Width * 2 + 1) / 2;
            for (int j = 0; j < Width * 2 + 1; j++)
                if (j % 2 != 0)
                    buf+=((char)(j/2 + 65));
                else
                    buf += (" ");
            Console.WriteLine(buf);

            buf = "";
            Console.CursorLeft = Console.WindowWidth/2 - (Width * 2 + 1) / 2;
            for (int j = 0; j < Width * 2 + 1; j++)
                buf += ("-");
            Console.WriteLine(buf);

            buf = "";
            for (int i = 0; i < Height; i++)
            {
                Console.CursorLeft = Console.WindowWidth / 2 - (Width * 2 + 1) / 2 - (int)Math.Log(i+1,10) -1;
                Console.Write(i+1);
                for (int j = 0; j < Width * 2 + 1; j++)
                {
                    if (j % 2 == 0)
                        Console.Write("|");
                    else {
                        if (shownTileFront[(j-1) / 2, i] != Program.DefaultBackColor)
                        Console.ForegroundColor = shownTileFront[(j-1) / 2, i];

                        Console.BackgroundColor = shownTileBack[(j-1) / 2, i];

                        if (shownTileStr[(j - 1) / 2, i] != null && shownTileStr[(j - 1) / 2, i] != "")
                            Console.Write(shownTileStr[(j - 1) / 2, i]);
                        else
                            Console.Write(" ");

                        Console.ForegroundColor = Program.DefaultTextColor;
                        Console.BackgroundColor = Program.DefaultBackColor;
                    }
                }
                Console.WriteLine();
                Console.CursorLeft = Console.WindowWidth / 2 - (Width * 2 + 1) / 2;
                buf = "";
                for (int j = 0; j < Width * 2 + 1; j++)
                    buf += ("-");
                Console.WriteLine(buf);
            }

        }

        /// <summary>Fires at the map during a game with the advanced option</summary>
        /// <param name="Plane">Indicates if the attack is coming from a plane</param>
        /// <param name="Detail">Indicates if the player is using an AA attack (the opponent is firing at the ships if false)</param>
        /// <param name="AttackList">If an attack is being used, this points to the list it is from</param>
        /// <param name="SplashIndex">Indicates which configuration of the attack is being used (if there is an attack being used)</param>
        /// <returns>A bool indicating if the plane was successfully placed</returns>
        public bool AdvancedFire(bool Plane, bool Detail, List<Attack.Attack> AttackList, int SplashIndex)
        {
            bool outp = false;
            if (SelectedAttack == null)
            {
                if (Detail)
                {
                    AttemptAA();
                    return false;
                }
                else
                 return   AttemptFire();
                           }

            Map.Tiles WeakScanResults = Tiles.Unknown;
            if (SelectedAttack.MyAttack == Attack.Attack.AttackType.WeakScan)
                WeakScanResults = GetWeakScanTile(ETargetLocation, SplashIndex);

            int[] coords;
            foreach (int[] i in SelectedAttack.SplashDamage[SplashIndex])
            {
                 coords = new int[] {i[0] + (Plane ? SelectedPlane.Location : Detail ? FTargetLocation : ETargetLocation)[0],
                    i[1] + (Plane ? SelectedPlane.Location : Detail ? FTargetLocation : ETargetLocation)[1]};
                if (coords[0] < 0 || coords[0] >= Width
                    || coords[1] < 0 || coords[1] >= Height)
                    continue;
                switch (SelectedAttack.MyAttack)
                {
                    case Attack.Attack.AttackType.Direct:
                        if (!Detail)
                           outp = AttemptFire(coords) ? true : outp;
                        else
                            AttemptAA(coords);
                        break;
                    case Attack.Attack.AttackType.WeakScan:
                        if (info[coords[0], coords[1]] == Tiles.Unknown)
                            info[coords[0], coords[1]] = WeakScanResults;
                        break;
                    case Attack.Attack.AttackType.StrongScan:
                        if ((int)info[coords[0], coords[1]] < (int)Tiles.Detected)
                            if (CheckSpot(coords))
                                info[coords[0], coords[1]] = Tiles.Detected;
                            else
                                info[coords[0], coords[1]] = Tiles.Miss;
                        break;

                }
            }

            if (SelectedAttack.CenterIncluded[SplashIndex])
            {
                coords = (Plane ? SelectedPlane.Location : Detail ? FTargetLocation : ETargetLocation);
                switch (SelectedAttack.MyAttack)
                {
                    case Attack.Attack.AttackType.Direct:
                        if (!Detail)
                            outp = AttemptFire(coords) ? true : outp;
                        else
                            AttemptAA(coords);
                        break;
                    case Attack.Attack.AttackType.WeakScan:
                        if (info[coords[0], coords[1]] == Tiles.Unknown)
                            info[coords[0], coords[1]] = WeakScanResults;
                        break;
                    case Attack.Attack.AttackType.StrongScan:
                        if ((int)info[coords[0], coords[1]] < (int)Tiles.Detected)
                            if (CheckSpot(coords))
                                info[coords[0], coords[1]] = Tiles.Detected;
                            else
                                info[coords[0], coords[1]] = Tiles.Miss;
                        break;

                }
            }
            if (SelectedAttack.Amount > 0)
                if (--SelectedAttack.Amount <= 0)
                {
                    AttackList.Remove(SelectedAttack);
                }
            return outp;
        }

        /// <summary>Uses AA on an indicated location, destroying any of the opponent's planes there</summary>
        /// <param name="TargetLocation">The indicated location</param>
        public void AttemptAA(int[] TargetLocation)
        {
            for (int i = 0; i < EPlanes.Count; i++)
                if (EPlanes[i].Location[0] == TargetLocation[0] && EPlanes[i].Location[1] == TargetLocation[1])
                {
                    DestroyedList.Add(EPlanes[i].PlaneName);
                    EPlanes.RemoveAt(i--);
                    break;
                }
        }

        /// <summary>Scans the tiles highlighted by the selected attack.</summary>
        /// <param name="TargetLocation">The center location of the selected attack.</param>
        /// <returns>A tiles variable representing what is eventualy shown to the opponent about the tiles</returns>
        public Tiles GetWeakScanTile(int[] TargetLocation , int Index)
        {
            foreach (int[] i in SelectedAttack.SplashDamage[Index])
                if (CheckSpot(new int[] { TargetLocation[0] + i[0], TargetLocation[1] + i[1] }))
                    return Tiles.Possible;
            if (SelectedAttack.CenterIncluded[Index])
                if (CheckSpot(TargetLocation))
                    return Tiles.Possible;

            return Tiles.Miss;
        }

        /// <summary>Scans a specified location on the map for ships.</summary>
        /// <param name="TargetLocation">The specified location.</param>
        /// <returns>A boolean representing if there is a ship</returns>
        public bool CheckSpot(int[] TargetLocation)
        {
            foreach (Ship.Ship i in Ships)
                if (i.CheckHit(TargetLocation, false))
                    return true;
            return false;
        }

        /// <summary>Fires at a location on the map, destroying ship parts and landed planes owned by the player.</summary>
        /// <param name="TargetLocation">The specified location.</param>
        /// <returns>A boolean representing if the shot hit</returns>
        public bool AttemptFire(int[] TargetLocation)
        {
            bool outp = false;
            for (int i = 0; i < FPlanes.Count; i++)
                if (FPlanes[i].Location[0] == TargetLocation[0] && FPlanes[i].Location[1] == TargetLocation[1])
                {
                    DestroyedList.Add(FPlanes[i].PlaneName);
                    FPlanes.RemoveAt(i--);
                    break;
                }

            for (int i = 0; i < Ships.Count; i++)
                if (Ships[i].CheckHit(TargetLocation, true))
                {
                    outp = true;
                    info[TargetLocation[0], TargetLocation[1]] = Tiles.Hit;
                    if (Ships[i].Destroyed)
                    {
                        DestroyedList.Add(Ships[i].ShipName);
                        Ships.RemoveAt(i--);
                    }
                }
            if (info[TargetLocation[0], TargetLocation[1]] != Tiles.Hit)
            {
                info[TargetLocation[0], TargetLocation[1]] = Tiles.Miss;
            }
            return outp;
        }
        /// <summary>Fires AA at the location that the player is aiming at, destroying planes owned by the opponent.</summary>
        /// <returns>A boolean representing if the shot hit</returns>
        public void AttemptAA() {
            AttemptAA(FTargetLocation);
        }
        /// <summary>Fires at the location that the opponent is aiming at, destroying ship parts and landed planes owned by the player.</summary>
        /// <returns>A boolean representing if the shot hit</returns>
        public bool AttemptFire() {
            return AttemptFire(ETargetLocation);
        }
        /// <summary>Checks if the player still has ships left.</summary>
        /// <returns>A boolean representing if there are still ships left</returns>
        public bool hasLost() {
            if (Ships.Count > 0)
                    return false;
            return true;
        }
    }
}
