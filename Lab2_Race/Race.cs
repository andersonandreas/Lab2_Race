using lab2_Race;

namespace Lab2_Race
{
    public class Race
    {

        private List<Car> FinnishOrder = new List<Car>();


        public int RaceTotalMeter = 1000;


        public async Task StartRace(List<Car> raceCars)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var allCarsInRace = new List<Task>();

            foreach (var car in raceCars)
            {

                allCarsInRace.Add(Task.Run(() => car.StartRace(cancellationTokenSource.Token)));
            }


            await Task.WhenAll(allCarsInRace);

















        }




    }
}
