namespace lab2_Race
{
    internal class Program
    {
        static async Task Main(string[] args)
        {


            var car1 = new Car("Volvo");
            var car2 = new Car("Ford");

            var cancellationTokenSource = new CancellationTokenSource();

            var task1 = Task.Run(() => car1.StartRace(cancellationTokenSource.Token));
            var task2 = Task.Run(() => car2.StartRace(cancellationTokenSource.Token));

            Console.WriteLine("Race started");

            while (car1.StartRacePosition < 500 && car2.StartRacePosition < 500)
            {

                // just for now, my laptop fans loud of cpu usage....
                await Task.Delay(100);

                await Task.Run(() =>
                {


                    string read = Console.ReadLine();


                    if (read == "status")
                    {
                        Console.WriteLine(
                            $"{car1.Name} {Math.Round(car1.StartRacePosition, 2)}m, " +
                            $"speed {car1.MaxSpeed:F0}km/h, active: {!car1.EventActive}");

                        Console.WriteLine
                        ($"{car2.Name} {Math.Round(car2.StartRacePosition, 2)}m,  " +
                        $"speed {car2.MaxSpeed:F0}km/h, active: {!car2.EventActive}");
                    }





                });
            }

            // i need to fix better logic when the first car is in the goal




            cancellationTokenSource.Cancel();

            await Task.WhenAny(task1, task2);
            //await Task.WhenAll(task1, task2);
        }
    }
}
