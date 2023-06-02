using BattleShipLibrary;
using BattleShipLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipLite
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();

            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                Console.WriteLine($"Active Player: {activePlayer.UsersName}\n");

                DisplayShotGrid(activePlayer);

                RecordPlayerShot(activePlayer, opponent);

                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);

                if (doesGameContinue == true)
                {
                    (activePlayer, opponent) = (opponent, activePlayer);
                }
                else
                {
                    winner = activePlayer;
                }

                Console.Clear();

            } while (winner == null);

            IdentifyWinner(winner);

            Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratulations to {winner.UsersName} for winning!");
            Console.WriteLine($"{winner.UsersName} took {GameLogic.GetShotCount(winner)} shots.");
            Console.WriteLine();
            DisplayShotGrid(winner);
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            string row = "";
            int column = 0;

            do
            {
                string shot = AskForShot();
                try
                {
                    (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                    isValidShot = GameLogic.ValidateShot(activePlayer, row, column);
                }
                catch (Exception ex)
                {

                    isValidShot=false;
                }
                

                if (!isValidShot)
                {
                    Console.WriteLine("Invalid shot location.  Please try again.");
                }
            } while (!isValidShot);

            bool isAHit = GameLogic.IdentifyShotResult(opponent, row, column);

            Console.WriteLine($"{(isAHit ? "BOOM! HIT!!" : "UH OH, MISS") }");

            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);

            Console.Write("\n ([Enter] to continue ..) ");
            Console.ReadLine();

        }

        private static string AskForShot()
        {
            Console.Write("Please enter your shot selection: ");
            string output = Console.ReadLine();

            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;

            Console.WriteLine($"     1   2   3   4   5\n");

            Console.Write($" {currentRow}  ");

            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                    Console.Write($" {currentRow}  ");
                }

                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" -  ");
                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X  ");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O  ");
                }
                else
                {
                    Console.Write(" ?  ");
                }
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to Battleship Lite");
            Console.WriteLine("created by Tim Corey");
            Console.WriteLine();
        }

        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel output = new PlayerInfoModel();

            Console.WriteLine($"Player information for {playerTitle}");

            output.UsersName = AskForUsersName();
            GameLogic.InitialiseGrid(output);
            PlaceShips(output);
            Console.Clear();

            return output;
        }

        private static string AskForUsersName()
        {
            Console.Write("What is your name? ");
            string output = Console.ReadLine();

            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            Console.Clear();

            do
            {
                Console.WriteLine($"Active PLayer: {model.UsersName}");
                Console.WriteLine("\nPlace Your Ships\n");

                DisplayPlayerShipsGrid(model);
                Console.Write($"Where do you want to place ship number {model.ShipLocations.Count + 1}: ");
                string location = Console.ReadLine();

                bool isValidLocation = false;

                try
                {
                    isValidLocation = GameLogic.PlaceShip(model, location);

    }
                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                };

                if (!isValidLocation)
                {
                    Console.WriteLine("That was not a valid location.  Please try again.");
                }

                Console.Clear();
            } while (model.ShipLocations.Count < 5 );

            Console.WriteLine($"Active PLayer: {model.UsersName}");
            Console.WriteLine("Here are your ship placements\n");
            DisplayPlayerShipsGrid(model);
            Console.Write("\n([Enter] to continue ...)");
            Console.ReadLine();
        }

        private static void DisplayPlayerShipsGrid(PlayerInfoModel model)
        {
            string currentRow = model.ShotGrid[0].SpotLetter;

            Console.WriteLine($"     1   2   3   4   5\n");

            Console.Write($" {currentRow}  ");

            foreach (var gridSpot in model.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                    Console.Write($" {currentRow}  ");
                }

                bool availableLocation = GameLogic.ValidateShipLocation(model, gridSpot.SpotLetter, gridSpot.SpotNumber);

                if (!availableLocation)
                {
                    Console.Write($" S  ");
                }
                else 
                {
                    Console.Write(" -  ");
                }
            }

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
