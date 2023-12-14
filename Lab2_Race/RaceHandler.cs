using lab2_Race;

namespace Lab2_Race
{
    internal class RaceHandler
    {
        private readonly List<Car> _cars;

        public RaceHandler(List<Car> cars)
        {
            _cars = cars;
        }

        public async Task StartRaceAsync(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("Race started");

                var raceTasks = _cars.Select(car => car.StartRaceAsync()).ToList();
                await UserStatusInputDuringRace(raceTasks, cancellationToken);
                await Task.WhenAll(raceTasks);

                PrintRaceResults(_cars);

                await Console.Out.WriteLineAsync("Press any key to close.");
                Console.ReadKey();
            }
            catch (AggregateException e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }

            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
        }




        private async Task UserStatusInputDuringRace(
            List<Task> raceTasks, CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                while (!raceTasks.All(t => t.IsCompleted))
                {
                    if (Console.KeyAvailable)
                    {
                        string? input = Console.ReadLine();
                        if (input?.ToLower() == "status" && !string.IsNullOrWhiteSpace(input))
                        {
                            PrintRaceStatus(_cars);
                        }


                    }

                    await Task.Delay(100);
                }
            }, cancellationToken);
        }

        private static void PrintRaceStatus(List<Car> cars)
        {
            Console.WriteLine();
            foreach (var car in cars)
            {
                Console.WriteLine(
                    $"{car.Name} {Math.Round(car.CurrentPosition, 2)}m, " +
                    $"speed {car.MaxSpeed:F0}km/h, active: {!car.EventActive}");
            }
        }

        private static void PrintRaceResults(List<Car> cars)
        {
            var carGoalTime = cars.OrderBy(car => car.FinishTime);

            int position = 1;
            Console.WriteLine("\n------------------Race places--------------------------");
            foreach (var car in carGoalTime)
            {

                Console.WriteLine(
                    $"{car.Name}: {position}th place, on the time {car.FinishTime.Minutes}m," +
                    $" {car.FinishTime.Seconds}s, {car.FinishTime.Milliseconds}ms");

                position++;
            }
        }
    }
}

