/*
Class Description:
This class used to initalize a standard Battleship from the classic game of Sea battle.
It works by filling in information to the ship class constructor

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Ship
{
    class Battleship : Ship
    {
                public Battleship()
                    : base(4, (int[])Program.DefaultLocation.Clone(), Program.DefaultDirection,"Battleship","B")
        {

            Attacks = new List<Attack.Attack>();
            Attacks.Add(new Attack.TomahawkMissle());
        }
    }
}
