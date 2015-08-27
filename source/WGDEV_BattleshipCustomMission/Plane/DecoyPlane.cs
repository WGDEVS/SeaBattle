/*
Class Description:
This class used to initalize a Decoy planes.
Decoy planes are planes with no attacks, they are there to confuse unobservant players.
It works by filling in information to the plane class constructor

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Plane
{
    class DecoyPlane : Plane
    {
        public DecoyPlane(ConsoleColor Col)
            : base(Col,new int[] { 0, 0 },"Decoy Plane")
        {
        }
    }
}
