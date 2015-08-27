/*
Class Description:
This class used to initalize a stinger missle.
It attacks in a diamond shape that excludes the center.
It works by filling in information to the attack class constructor

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Attack
{
    class Stinger : Attack
    {
        public Stinger()
            : base(2, new int[][][] { new int[][] {
                new int[] { 0, -2 },
                new int[] { -1, -1 }, new int[] { 0, -1 }, new int[] { 1, -1 },
               new int[] { -2, 0 }, new int[] { -1, 0 }, new int[] { 1, 0 },new int[] { 2, 0 },
                new int[] { -1, 1 }, new int[] { 0, 1 }, new int[] { 1, 1 },
                new int[] { 0, 2 }

            }}, new bool[] { false }, AttackType.Direct, "Stinger Missles")
        {
        }
    }
}
