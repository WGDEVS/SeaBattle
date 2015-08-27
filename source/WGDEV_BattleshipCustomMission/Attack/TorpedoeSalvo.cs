/*
Class Description:
This class used to initalize a torpedo salvo.
It works by filling in information to the attack class constructor
It hits everything in a five tile long line

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Attack
{
    class TorpedoeSalvo : Attack
    {
        public TorpedoeSalvo()
            : base(2, new int[][][] { 
            new int[][] { new int[] { 0,-4 }, new int[] { 0, -3 }, new int[] { 0, -2 }, new int[] { 0, -1 } },
            new int[][] { new int[] { 4, 0 }, new int[] { 3, 0 }, new int[] { 2, 0 }, new int[] { 1, 0 } }, 
            new int[][] { new int[] { 0,4 }, new int[] { 0, 3 }, new int[] { 0, 2 }, new int[] { 0, 1 } },
            new int[][] { new int[] { -4, 0 }, new int[] { -3, 0 }, new int[] { -2, 0 }, new int[] { -1, 0 } }
            }, new bool[] { true, true, true, true }, AttackType.Direct, "Torpedo Salvos")
        {
        }
    }
}
