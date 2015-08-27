/*
Class Description:
This class is used for keeping track of any item with coordinates 
on a map. So far this includes and is limited to the ships and planes 
used by the players. The items so far, which are the ships and planes, 
have their own classes and inherit from this class.

Made by WGDEV, some rights reserved, see licence.txt for more info
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission
{
    public class Item
    {
        public int Number;//The index of this item in the initial list in the program, used for gameplay over the lan
        public int[] Location;//The location of this item on a map
        public List<Attack.Attack> Attacks; //The attacks this item can perform

        /// <summary>Creates a new item at the specified location.</summary>
        /// <param name="Location">The specified location.</param>
        public Item(int[] Location ) {
            this.Location = Location;
        }

        /// <summary>Checks if another item is in the same spot as this item</summary>
        /// <param name="Other">The other item.</param>
        /// <returns>A bool represening if the item is in the same spot</returns>
        public virtual bool TileConflict(Item Other) {
            if (Location[0] == Other.Location[0] && Location[1] == Other.Location[1])
                return true;
            else
                return false;
        }

        /// <summary>Checks if another location is in the same spot as this item</summary>
        /// <param name="OtherLoc">The other location.</param>
        /// <returns>A bool represening if the location is in the same spot</returns>
        public bool TileConflict(int[] OtherLoc)
        {
            if (Location[0] == OtherLoc[0] && Location[1] == OtherLoc[1])
                return true;
            else
                return false;
        }

        /// <summary>Checks if the item is within the bounds of a map</summary>
        /// <param name="InpMap">The specified map.</param>
        /// <returns>A bool represening if the item is within the map</returns>
        public virtual bool WithinArea(Map InpMap)
        {
            if (Location[0] >= 0 && Location[0] < InpMap.Width &&
                Location[1] >= 0 && Location[1] < InpMap.Height)
                return true;
            return false;
        }
    }
}
