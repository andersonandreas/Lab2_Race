using System.Diagnostics;

namespace lab2_Race
{
    public class Car
    {

        public string Name { get; }
        public double MaxSpeed { get; set; }
        public bool EventActive { get; private set; }
        public double CurrentPosition { get; private set; }
        public TimeSpan FinishTime { get; private set; }

        private static readonly object _lock = new();
        private static bool _winner;
        private int _eventDuration;
        private readonly int _raceTotalMeter;
        private DateTime _timeSinceMethod;

        private readonly Random _random;
        private readonly Stopwatch _stopwatch;


        public Car(string name)
        {
            Name = name;
            MaxSpeed = 120;
            CurrentPosition = 0;
            _timeSinceMethod = DateTime.Now;
            _raceTotalMeter = 5_000;
            _random = new();
            _stopwatch = new();

        }

        public async Task StartRaceAsync()
        {
            try
            {
                _stopwatch.Start();

                while (CurrentPosition <= _raceTotalMeter)
                {
                    await Task.Delay(1000);

                    double distance = (MaxSpeed / 3.6) * 1;
                    CurrentPosition += distance;
                    CheckForEvents();

                    if (_eventDuration > 0)
                    {
                        EventActive = true;
                        await Task.Delay(_eventDuration * 1000);
                        _eventDuration = 0;
                        EventActive = false;
                    }
                }

                if (!_winner)
                {
                    SetWinner();
                }


                _stopwatch.Stop();
                FinishTime = _stopwatch.Elapsed;


            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
        }

        private void SetWinner()
        {
            lock (_lock)
            {
                if (!_winner)
                {
                    _winner = true;
                    Console.Out.WriteLine($"{Name} wins the race!");
                    Console.WriteLine(
                        "Now waiting for all the other cars to enter goal and then party..");
                }
            }
        }

        private void CheckForEvents()
        {
            if (DateTime.Now - _timeSinceMethod >= TimeSpan.FromSeconds(30) || CurrentPosition < 50)
            {
                _eventDuration = RandomEventDuration();
                _timeSinceMethod = DateTime.Now;
            }

            if (_eventDuration > 0)
            {
                EventActive = true;
                Console.WriteLine($"\n{Name} encountered an event: {_eventDuration} seconds");
            }
        }



        private int RandomEventDuration()
        {
            const double NoGasProbability = 0.02;
            const double FlatTireProbability = 0.04;
            const double BirdPoopProbability = 0.1;
            const double EngineFailureProbability = 0.2;


            var eventHappened = Odds();
            if (eventHappened <= NoGasProbability)
            {
                Console.WriteLine($"{Name}: No gas left... Haha loser!");
                return 30;
            }

            if (eventHappened <= FlatTireProbability)
            {
                Console.WriteLine($"{Name}: Flat tire");
                return 20;
            }

            if (eventHappened <= BirdPoopProbability)
            {
                Console.WriteLine($"{Name}: Bird poop on the windshield");
                return 10;
            }

            if (eventHappened <= EngineFailureProbability)
            {
                MaxSpeed--;
                Console.WriteLine($"{Name}: Engine failure... new max speed {MaxSpeed}");
                return 0;
            }

            return 0;
        }

        private double Odds() => _random.NextDouble();
    }

}

