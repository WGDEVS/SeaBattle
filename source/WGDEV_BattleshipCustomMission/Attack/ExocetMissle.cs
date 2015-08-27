/*
Class Description:
This class used to initalize a standard Exocet Missle from the classic game of Sea battle.
It works by filling in information to the attack class constructor

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Attack
{
    class ExocetMissle:Attack
    {
        public ExocetMissle()
            : base(2, new int[][][] { new int[][] { new int[] { -1, -1 }, new int[] { 1, -1 }, new int[] { -1, 1 }, new int[] { 1, 1 } }, 
            new int[][]{  new int[]{ 0, -1 },  new int[]{ -1, 0 },  new int[]{ 1, 0 },  new int[]{ 0, 1 } } }, new bool[] { true, true }, AttackType.Direct, "Exocet Missles")
        {
        }
    }
}
