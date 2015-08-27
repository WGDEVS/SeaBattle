/*
Class Description:
This class used to initalize a standard Aircraft Carrier from the classic game of Sea battle.
It works by filling in information to the ship class constructor

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Ship
{
    class AircraftCarrier : Ship
    {
        public AircraftCarrier()
            : base(5, new int[]{Program.DefaultLocation[0],Program.DefaultLocation[1]}, Program.DefaultDirection,"Aircraft Carrier","A")
        {
            Attacks = new List<Attack.Attack>();
            Attacks.Add(new Attack.ExocetMissle());
            CanCarryPlanes = true;
        }
    }
}
