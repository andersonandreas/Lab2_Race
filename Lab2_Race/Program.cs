using Lab2_Race;

namespace lab2_Race
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {

            var cars = new List<Car>
            {
                new("Volvo"),
                new("Saab"),
                new("Volkswagen"),
                new("Tesla"),
            };

            try
            {
                var raceManager = new RaceHandler(cars);
                var cts = new CancellationTokenSource();
                await raceManager.StartRaceAsync(cts.Token);
            }
            catch (Exception e)
            {

                Console.WriteLine(
                    $"The application need to close to an error occurred: {e.Message}");
            }
        }





    }


}


