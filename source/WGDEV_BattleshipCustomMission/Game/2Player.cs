/*
Class Description:
This class is used for providing a 2 player game on the same computer.

Made by WGDEV, some rights reserved, see licence.txt for more info
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WGDEV_BattleshipCustomMission.Game
{
    class _2Player:Game
    {
        /// <summary>Initializes a member of the 2 Player class. Used to define key properties of the game.</summary>
        /// <param name="Bonus">Determines if the Bonus option is enabled.</param>
        /// <param name="Advanced">Determines if the advanced option is enabled.</param>
        /// <param name="MapWidth">Determines the width of both maps.</param>
        /// <param name="MapHeight">Determines the height of both maps.</param> 
        /// <param name="Ships">The ships that all players are assigned at the begining of the game.</param>
        /// <param name="Planes">The planes that all players are assigned at the begining of the game.</param> 
        public _2Player(bool Bonus, bool Salvo, bool Advanced, int MapWidth, int MapHeight, List<Ship.Ship> Ships, List<Plane.Plane> Planes):base(
         Bonus,  Salvo,  Advanced,  MapWidth,  MapHeight,  Ships, Planes){
        }

        /// <summary>Runs a 2 player game.</summary>
        public override void RunGame()
        {
            SetUpMap temp = new SetUpMap(Player1, Advanced);
            temp.ManualSetUp("Player 1", ShipList, PlaneList);

            temp = new SetUpMap(Player2, Advanced);
            temp.ManualSetUp("Player 2", ShipList, PlaneList);
            Turn t;
            do
            {
                if (Advanced)
                    t = new AdvancedTurn(Bonus, Salvo, Player1, Player2);
                else
                    t = new Turn(Bonus, Salvo, Player1, Player2);
                PauseGame("It's time for player 1 to play, press ESC to continue.");
                t.DoManualTurnSequence();
                if (Player2.hasLost())
                {
                    PauseGame("Player 1 has won the game, press ESC to return to main menu.");
                    return;
                }

                if (Advanced)
                    t = new AdvancedTurn(Bonus, Salvo, Player2, Player1);
                else
                    t = new Turn(Bonus, Salvo, Player2, Player1);
                PauseGame("It's time for player 2 to play, press ESC to continue.");
                t.DoManualTurnSequence();
                if (Player1.hasLost())
                {
                    PauseGame("Player 2 has won the game, press ESC to return to main menu.");
                    return;
                }
            } while (true);
            //base.RunGame();
        }

    }
}
