using System.Diagnostics;

namespace lab2_Race
{
    public class Car
    {
        public string Name { get; set; }
        public double MaxSpeed { get; set; } = 120;
        public bool EventActive { get; set; } = false;

        private double _raceTotalMeter;
        public double StartRacePosition;
        private DateTime _timeSinceMethod;
        private Random _random;
        private int _eventDuration;

        public Car(string name)
        {
            Name = name;
            _raceTotalMeter = 500;
            StartRacePosition = 0;
            _timeSinceMethod = DateTime.Now;
            _random = new Random();
        }

        public async Task StartRace(CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //hardcoded for now
            while (StartRacePosition <= _raceTotalMeter)
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(1000, cancellationToken);

                double distance = (MaxSpeed / 3.6) * 1;
                StartRacePosition += distance;

                if (DateTime.Now - _timeSinceMethod >= TimeSpan.FromSeconds(30) || StartRacePosition < 50)
                {
                    _eventDuration = RandomThings();
                    _timeSinceMethod = DateTime.Now;
                }

                if (_eventDuration > 0)
                {
                    EventActive = true;
                    Console.WriteLine($"Sleeping for {_eventDuration}");
                    await Task.Delay(_eventDuration * 1000, cancellationToken);
                    Console.WriteLine($"waking up {Name}, after {_eventDuration} sleep");
                    _eventDuration = default;
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
            Console.WriteLine($"{Name}: Entering event");

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

        private double Odds() => _random.NextDouble();
    }
}
