/*
Class Description:
A ship in the game is placed on a map. The player that owns the map owns the ship.
This class is used for keeping track of a ship. It is responsable 
for describing all aspects of a ship and for providing all the 
procedures that directly affect the ship
Ships have an "anchor point", which is a point on a two dimensional grid that represents the ships location.
Ships also have a "direction" which is which direction the ship extends towards from the "anchor point"
Ships also have an array of booleans, the index of which represents points on the map where parts of the ship are located. 
The booleans themselves represent if the ship part that that point on the map has been hit.
Ships carry a list of attacks and AA, the player is allowed to use them if the ship has not been destroyed.

Made by WGDEV, some rights reserved, see licence.txt for more info
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Ship
{
    public class Ship : Item
    {
        public enum Direction { North = -1, South = 1, East = 2, West = -2};//Defines the directions the ship can face
        public Direction LocationDirection = new Direction();//Which direction is the ship facing
        public string ShipTile;//A one character character used to identifiy the ship on a printout of the map
        public string ShipName = "";//The name of the ship

        public List<Attack.Attack> AA;//The list of AA attacks that the ship can perform
        public bool CanCarryPlanes = false;//A boolean representing if the ship can hold planes

        private int length = 0;//The length of the ship
        private int health;//A count of how many parts of the ship have not been hit 
        private bool[] hit;//Represents the parts of the ship, will be true if hit
        public bool Destroyed { get{ //Determines if the ship is destroyed and should be removed from the map
            if (health <= 0)
                return true;
            return false;
        } 
        }
        public int[][] BodyList()//Returns a list of coordinates that represent the locations of all the parts of the ship
        { 
            int[][] outp = new int[length][];
            for (int i = 0; i < length; i++)
            {
                outp[i] = new int[2];
                outp[i][0] = this.Location[0] + i * (Math.Abs((int)this.LocationDirection) < 2 ? 0 : 1) * ((int)this.LocationDirection / 2);
                outp[i][1] = this.Location[1] + i * (Math.Abs((int)this.LocationDirection) < 2 ? 1 : 0) * ((int)this.LocationDirection);
            }
            return outp;
        }

        /// <summary>Initializes a member of the ship class. Uses generic ship character and string identifiers</summary>
        /// <param name="Length">The length of the ship on the map.</param>
        /// <param name="Location">The position of the anchor point of the ship on the map.</param>
        /// <param name="LocationDirection">The direction that the ship extends to from the anchor point on the map.</param>
        public Ship(int Length, int[] Location, Direction LocationDirection)
            : this(Length, Location, LocationDirection,"Ghost Ship")
        { }

        /// <summary>Initializes a member of the ship class. Uses a generic ship character identifier</summary>
        /// <param name="Length">The length of the ship on the map.</param>
        /// <param name="Location">The position of the anchor point of the ship on the map.</param>
        /// <param name="LocationDirection">The direction that the ship extends to from the anchor point on the map.</param>
        /// <param name="ShipName">A string identifier of the ship</param>
        public Ship(int Length, int[] Location, Direction LocationDirection, string ShipName)
            : this(Length, Location, LocationDirection, ShipName, "#")
        { }

        /// <summary>Initializes a member of the ship class.</summary>
        /// <param name="Length">The length of the ship on the map.</param>
        /// <param name="Location">The position of the anchor point of the ship on the map.</param>
        /// <param name="LocationDirection">The direction that the ship extends to from the anchor point on the map.</param>
        /// <param name="ShipName">A string identifier of the ship</param>
        /// <param name="ShipTile">A character identifier of the ship</param>
        public Ship(int Length,int[] Location,Direction LocationDirection,string ShipName, string ShipTile):base(Location){
            this.ShipTile = ShipTile;
            this.ShipName = ShipName;

            length = Length;
            this.LocationDirection = LocationDirection;
            hit = new Boolean[Length];
            health = Length;
        }

        /// <summary>Checks if the ship is currently within the bounds of a map.</summary>
        /// <param name="InpMap">The specified map.</param>
        /// <returns>A boolean representing if the ship is within the bounds of a map.</returns>
        public override bool WithinArea(Map InpMap)
        {
            switch (this.LocationDirection) 
            { 
                case Direction.East:
                    return (this.Location[0] >= 0 && this.Location[0] + length - 1 < InpMap.Width && this.Location[1] >= 0 && this.Location[1] < InpMap.Height);
                case Direction.West:
                    return (this.Location[0] - length + 1 >= 0 && this.Location[0] < InpMap.Width && this.Location[1] >= 0 && this.Location[1] < InpMap.Height);
                case Direction.South:
                    return (this.Location[1] >= 0 && this.Location[1] + length - 1 < InpMap.Height && this.Location[0] >= 0 && this.Location[0] < InpMap.Width);
                case Direction.North:
                    return (this.Location[1] - length + 1 >= 0 && this.Location[1] < InpMap.Height && this.Location[0] >= 0 && this.Location[0] < InpMap.Width);
                default:
                    return false;
            }
        }

        /// <summary>Finds the index of the boolean in the array "Destroyed" that a coordinate maps to.</summary>
        /// <param name="Cords">The specified coordinates.</param>
        /// <returns>The index that the coordinates map to, or -1 if it does not map to any coordinates.</returns>
        public int GetLocIndex(int[] Cords) {
            int align;
            int across;
            if (Math.Abs((int)this.LocationDirection) < 2) //Ship going north/south
            {
                align = 1;
                across = 0;
                
            }
            else//Ship going east or west
            {
                align = 0;
                across = 1;
            }

            if (Cords[across] != this.Location[across])
                return -1;
            if ((int)this.LocationDirection < 0 && (Cords[align] > this.Location[align] || Cords[align] <= this.Location[align] - length))
                return -1;
            if ((int)this.LocationDirection > 0 && (Cords[align] < this.Location[align] || Cords[align] >= this.Location[align] + length))
                return -1;

            if ((int)this.LocationDirection < 0)
                return this.Location[align] - Cords[align];
            else
                return Cords[align] - this.Location[align];
        }

        /// <summary>Checks if this ship can provide a platform for a plane landing.</summary>
        /// <param name="PlaneLoc">The coordinates that the plane wants to land on.</param>
        /// <returns>A boolean representing if the plane can land on the ship at the specified coordinates.</returns>
        public bool CanHoldPlaneAt(int[] PlaneLoc) {
            if (!CanCarryPlanes)
                return false;
            int t = 0;
            t = GetLocIndex(PlaneLoc);
            if (t != -1)
                if (!hit[t])
                    return true;
            return false;
        }

        /// <summary>Checks if this ship can be hit by a shot fired at a location.</summary>
        /// <param name="Shot">The specified location that the shot is being aimed at.</param>
        /// <param name="DestroyOnCheck">Destroys the part of the ship that the shot is being aimed at if true.</param>
        /// <returns>A boolean representing if the ship can be hit by the shot.</returns>
        public bool CheckHit(int[] Shot, bool DestroyOnCheck) {
            int t = GetLocIndex(Shot);
            if (t == -1)
                return false;
            if (hit[t])
                return false;
                
                    if (DestroyOnCheck)
                    {
                        hit[t] = true;
                        health--;
                    }
                    return true;

         
        }

        /// <summary>Checks if this ship is overlapping with another ship.</summary>
        /// <param name="Other">The other ship.</param>
        /// <returns>A boolean representing if the ships are overlapping.</returns>
        public bool TileConflict(Ship Other)
        {
            foreach( int[] i in BodyList()){
                if (Other.CheckHit(i, false))
                    return true;
                }
            return false;
        }

        /// <summary>Makes a copy of this ship.</summary>
        /// <returns>The copy of this ship (in object form).</returns>
        public object Clone()
        {
            Ship t = (Ship)this.MemberwiseClone();
            t.Location = new int[] { Location[0], Location[1] };
            t.hit = new bool[hit.Length];

            if (Attacks != null)
            {
                t.Attacks = new List<Attack.Attack>();
                foreach (Attack.Attack i in Attacks)
                {
                    t.Attacks.Add((Attack.Attack)i.Clone());

                }
            }

            return t;
        }
        //object ICloneable.Clone() //Shallow!!!
        //{
        //    return this.MemberwiseClone();
        //    throw new NotImplementedException();
        //}
    }
}
