/*
Class Description:
This class used to initalize a standard Recon Plane from the classic game of Sea battle.
It works by filling in information to the plane class constructor

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Plane
{
    class ReconPlane : Plane
    {
        public ReconPlane(ConsoleColor Col)
            : base(Col,new int[] { 0, 0 },"Recon Plane")
        {
            Attacks = new List<Attack.Attack>();
            Attacks.Add(new Attack.ReconScan());
        }
    }
}
