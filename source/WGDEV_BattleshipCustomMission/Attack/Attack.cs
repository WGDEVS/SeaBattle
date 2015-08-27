/*
Class Description:
This class is used for describing an attack, it's use is defined in the map class.
An attack in the game is attached to an item.
It can be used if the item is not destroyed. For planes it can be used only if the item is over the enemy map.
The attack can be used to attack on the opponent's map or it can be used defensively to destroy planes over the players's map.
The main part of the attack is a list of a collection of coordinates that are checked for items. When the attack is used, one list will be chosen as sepecified by the player. The coordinates in the specified list will be check for (parts of) items.


Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Attack
{
    public class Attack
    {
        public enum AttackType {Direct = 0,WeakScan = 1, StrongScan = 2};//The possible things that will happen if an item is found, direct destroys the ship part, weak scan marks all the coordinates in the selected list with ? and strong scan marks the ship part with !.

        public int Amount = 0;//This is the number of times this attack can be used, set it to -1 for unlimited uses
        public int[][][] SplashDamage;//This is a list of a list of coordinates that are checked for ships, they are offsets for the center which is implied to have a coordinate of {0,0} and each coordinate is added to the coordinates that the player is aiming at
        public bool[] CenterIncluded;//This is a list that determines if the center coordinate {0,0} is included in the attack
        public AttackType MyAttack;//This is the thing that will happen if one of the opponent's item is found by this attack
        public string AttackName;//The string representation of the attack

        /// <summary>
        /// Initializes a member of the attack class, uses a generic string identifier.
        /// </summary>
        /// <param name="Amount">the number of times this attack can be used, set it to -1 for unlimited uses</param>
        /// <param name="SplashDamage">A list of a list of coordinates that are checked for ships, they are offsets for the center which is implied to have a coordinate of {0,0} and each coordinate is added to the coordinates that the player is aiming at</param>
        /// <param name="CenterIncluded">A list that determines if the center coordinate {0,0} is included in the attack</param>
        /// <param name="MyAttack">/This is the thing that will happen if one of the opponent's item is found by this attack</param>
        public Attack(int Amount, int[][][] SplashDamage, bool[] CenterIncluded, AttackType MyAttack)
            :this(Amount,SplashDamage,CenterIncluded,MyAttack,"Generic Missle")
        {
        }

        /// <summary>
        /// Initializes a member of the attack class.
        /// </summary>
        /// <param name="Amount">the number of times this attack can be used, set it to -1 for unlimited uses</param>
        /// <param name="SplashDamage">A list of a list of coordinates that are checked for ships, they are offsets for the center which is implied to have a coordinate of {0,0} and each coordinate is added to the coordinates that the player is aiming at</param>
        /// <param name="CenterIncluded">A list that determines if the center coordinate {0,0} is included in the attack</param>
        /// <param name="MyAttack">/This is the thing that will happen if one of the opponent's items is found by this attack</param>
        /// <param name="AttackName">The string representation of the attack</param>
        public Attack(int Amount, int[][][] SplashDamage, bool[] CenterIncluded, AttackType MyAttack, string AttackName)
        {
            this.Amount = Amount;
            this.SplashDamage = SplashDamage;
            this.CenterIncluded = CenterIncluded;
            this.MyAttack = MyAttack;
            this.AttackName = AttackName;
        }

        /// <summary>Makes a copy of this attack.</summary>
        /// <returns>The copy of this attack (in object form).</returns>
        public object Clone()
        {
            Attack t = (Attack)this.MemberwiseClone();
            //Don't need but in case
            /*t.SplashDamage = new int[SplashDamage.Length][][];
            for (int i = 0; i < t.SplashDamage.Length; i++) { 
                t.SplashDamage[i] = new int[SplashDamage[i].Length][];
                for (int j = 0; j < t.SplashDamage[i].Length; j++) {
                    t.SplashDamage[i][j] = new int[] { SplashDamage[i][j][0], SplashDamage[i][j][1] };
                }
            } */
            return t;
        }

        /// <summary>
        /// Gets a detailed string representation of the attack
        /// </summary>
        /// <rerturns>A detailed string representation of the attack</rerturns>
        public string AttackString {
            get {
                return AttackName + "(" + (MyAttack == AttackType.Direct ? "X" : MyAttack == AttackType.WeakScan ? "?" : "!") + ")" + (Amount > 0 ? " x" + Amount.ToString() : "");
            }
        }
    }
}
