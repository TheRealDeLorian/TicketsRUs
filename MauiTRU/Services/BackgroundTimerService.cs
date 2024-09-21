using MauiTRU.Database;

namespace MauiTRU.Services
{
    public class BackgroundTimerService
    {
        private readonly LocalTRUDatabase _db;
        private readonly ConnectivityForPhone _connectivity;
        private int _timeperiod = Constants.DefaultRefreshRate;
        public bool isRunning;
        public int GetTimePeriod { get => _timeperiod; }
        
        public BackgroundTimerService(LocalTRUDatabase database, ConnectivityForPhone cfp)
        {
            _db = database;
            _connectivity = cfp;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Starting timer");
            
            await DoWork();
            isRunning = true;

            using PeriodicTimer timer = new(TimeSpan.FromSeconds(_timeperiod));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await DoWork();
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Timer is stopping");
            }
        }

        private async Task DoWork()
        {
            Console.WriteLine("Synchronizing databases...");

            try
            {
                if (_db is not null && _connectivity.GetNetworkAccess() == NetworkAccess.Internet )
                {
                    await _db.UpdateLocalDbFromMainDb();
                    await _db.UpdateMainDbFromLocalDb();
                }
            }
            catch
            {
                Console.WriteLine("Couldn't sync database");
            }
        }

        public async Task RestartTimer()
        {
            await ExecuteAsync(new CancellationToken(true));
            await ExecuteAsync(new CancellationToken());
        }

        public async Task ChangeTimePeriod(int periodinseconds)
        {
            _timeperiod = periodinseconds;
            await RestartTimer();
        }
    }
}
