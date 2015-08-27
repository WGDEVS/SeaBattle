/*
Class Description:
This class used to initalize a modified Patrol boat based on the one from the classic game of Sea battle.
It is modified to include AA capabilities.
It works by filling in information to the ship class constructor

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Ship
{
    class PatrolBoat : Ship
    {
                        public PatrolBoat()
            : base(2, (int[])Program.DefaultLocation.Clone(), Program.DefaultDirection, "Patrol Boat","P")
        {
            AA = new List<Attack.Attack>();
            AA.Add(new Attack.Stinger());
        }
    }
}
