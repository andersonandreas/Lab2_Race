using System.Diagnostics;

namespace lab2_Race
{
    internal class Program
    {
        static async Task Main(string[] args)
        {


            var vehicle1 = new Vehicle("Volvo");
            var vehicle2 = new Vehicle("Ford");

            var cancellationTokenSource = new CancellationTokenSource();

            var task1 = Task.Run(() => vehicle1.StartRace(cancellationTokenSource.Token));
            var task2 = Task.Run(() => vehicle2.StartRace(cancellationTokenSource.Token));

            Console.WriteLine("Race started");

            while (vehicle1.StartRacePosition < 2_000 && vehicle2.StartRacePosition < 2_000)
            {
                await Task.Run(() =>
                {
                    string read = Console.ReadLine();


                    if (read == "status")
                    {
                        Console.WriteLine(
                            $"{vehicle1.Name} {Math.Round(vehicle1.StartRacePosition, 2)}m, " +
                            $"speed {vehicle1.MaxSpeed:F0}km/h, active: {!vehicle1.EventActive}");

                        Console.WriteLine
                        ($"{vehicle2.Name} {Math.Round(vehicle2.StartRacePosition, 2)}m,  " +
                        $"speed {vehicle2.MaxSpeed:F0}km/h, active: {!vehicle2.EventActive}");
                    }





                });
            }

            // i need to fix better logic when the first car is in the goal




            cancellationTokenSource.Cancel();

            await Task.WhenAny(task1, task2);
            //await Task.WhenAll(task1, task2);
        }
    }

    public class Vehicle
    {
        public string Name { get; set; }
        public double MaxSpeed { get; set; } = 120;

        private double _raceTotalMeter;
        public double StartRacePosition;
        private DateTime _timeSinceMethod;

        public bool EventActive { get; set; } = false;

        private int _sleep;

        public Vehicle(string name)
        {
            Name = name;
            _raceTotalMeter = 10_000;
            StartRacePosition = 0;
            _timeSinceMethod = DateTime.Now;
        }

        public async Task StartRace(CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //hardcoded for now
            while (StartRacePosition < 2_000)
            {
                await Task.Delay(1000, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }


                double distance = (MaxSpeed / 3.6) * 1;
                StartRacePosition += distance;

                if (DateTime.Now - _timeSinceMethod >= TimeSpan.FromSeconds(10))
                {
                    _sleep = RandomThings();
                    _timeSinceMethod = DateTime.Now;
                }

                if (_sleep > 0)
                {
                    EventActive = true;
                    Console.WriteLine($"Sleeping for {_sleep}");
                    await Task.Delay(_sleep * 1000, cancellationToken);
                    Console.WriteLine($"waking up {Name}, after {_sleep} sleep");
                    _sleep = default;
                    EventActive = false;
                }
            }

            stopWatch.Stop();
            TimeSpan raceTime = stopWatch.Elapsed;

            Console.WriteLine($"{Name} has won the race!");
            Console.WriteLine($"{raceTime.Minutes}m, {raceTime.Seconds}s, {raceTime.Milliseconds}ms");
        }

        private int RandomThings()
        {

            var eventHappened = Odds();
            if (eventHappened <= 0.02)
            {
                Console.WriteLine($"{Name}: No gas left... Loser!");
                return 30;
            }

            if (eventHappened <= 0.04)
            {
                Console.WriteLine($"{Name}: Flat tire");
                return 20;
            }

            if (eventHappened <= 0.1)
            {
                Console.WriteLine($"{Name}: Bird pop on the windshield");
                return 10;
            }

            if (eventHappened <= 0.2)
            {
                MaxSpeed--;
                Console.WriteLine($"{Name}: Engine failure");

                Console.WriteLine($"{Name} new max speed {MaxSpeed} after problem with engine");
                return 0;
            }

            return 0;
        }

        private double Odds() => new Random().NextDouble();
    }
}
