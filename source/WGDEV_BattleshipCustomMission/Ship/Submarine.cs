/*
Class Description:
This class used to initalize a modified Submarine based on the one from the classic game of Sea battle.
It is modified to fire its torpedos in a slavo, covering a line of five coordinates on the other player's map
It works by filling in information to the ship class constructor

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Ship
{
    class Submarine : Ship
    {
                public Submarine()
            : base(3, (int[])Program.DefaultLocation.Clone(), Program.DefaultDirection,"Submarine","S")
        {
            Attacks = new List<Attack.Attack>();
            Attacks.Add(new Attack.SonarScan());
            Attacks.Add(new Attack.TorpedoeSalvo());
        }
    }
}
