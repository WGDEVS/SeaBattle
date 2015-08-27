/*
Class Description:
A plane in the game is placed on a ship owned by the player or over the opponent's map.
This class is used for keeping track of a plane. It is responsable 
for describing all aspects of a plane and for providing all the 
procedures that directly affect the plane
plane have an location, which is a point on a two dimensional grid that represents the ships location.
If a plane is hit by AA while over the other player's map or is hit by a shot while on a ship owned by 
this player, the plane is destroyed and removed from the game

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Plane
{
    public class Plane:Item
    {
        public string PlaneName = "Plane";//The string representation of the plane
        public string Tile = "*";//The one character representation of the plane (will be displayed on the map if nothing else is there)
        public ConsoleColor PlaneColor;//The color representation of the plane, unique to each type of plane

        /// <summary>Initializes a member of the plane class. Use generic plane string and character indentifier</summary>
        /// <param name="PlaneColor">The color of the plane shown on the map.</param>
        /// <param name="Location">The position of the plane</param>
        public Plane(ConsoleColor PlaneColor,int[] Location)
            : this(PlaneColor,Location,"UFO")
        {
        }

        /// <summary>Initializes a member of the plane class.</summary>
        /// <param name="PlaneColor">The color of the plane shown on the map.</param>
        /// <param name="Location">The position of the plane</param>
        /// <param name="PlaneName">The string identifier of the plane</param>
        public Plane(ConsoleColor PlaneColor,int[] Location, string PlaneName,string Tile)
         : base(Location)
        {
            this.PlaneColor = PlaneColor;
            this.PlaneName = PlaneName;
            this.Tile = Tile;
        }

        /// <summary>Initializes a member of the plane class. Use a generic plane character indentifier</summary>
        /// <param name="PlaneColor">The color of the plane shown on the map.</param>
        /// <param name="Location">The position of the plane</param>
        /// <param name="PlaneName">The string identifier of the plane</param>
        public Plane(ConsoleColor PlaneColor,int[] Location, string PlaneName)
 : this(PlaneColor,Location,PlaneName,"*")
        {
        }


        /// <summary>Checks if the plane is within the bounds of a map</summary>
        /// <param name="InpMap">The specified map.</param>
        /// <returns>A boolean representing if the plane is within the bounds of the map</returns>
        public override bool WithinArea(Map InpMap)
        {
                    return (this.Location[0] >= 0 && this.Location[0] < InpMap.Width &&
                        this.Location[1] >= 0 && this.Location[1] < InpMap.Height);
        }


        /// <summary>Makes a copy of this ship.</summary>
        /// <returns>The copy of this plane (in object form).</returns>
        public object Clone()
        {
            Plane t = (Plane)this.MemberwiseClone();
            t.Location = new int[] { t.Location[0], t.Location[1] };
            if (Attacks != null) {
                t.Attacks = new List<Attack.Attack>();
                foreach (Attack.Attack i in Attacks)
                {
                    t.Attacks.Add((Attack.Attack)i.Clone());

                }
            }
            return t;
        }
    }
}
