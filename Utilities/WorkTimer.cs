namespace Utilities
{
    public  class WorkTimer
    {
        public bool IsTimedOut { get; private set; }
        private readonly System.Timers.Timer timedOutTimer;

        public WorkTimer()
        {
            timedOutTimer = new System.Timers.Timer();
            IsTimedOut = false;
            timedOutTimer.Elapsed += OnTimedEvent;
            timedOutTimer.AutoReset = false;

        }
        public void InitTimer(uint time = 10000)
        {
            timedOutTimer.Interval=time;
            IsTimedOut = false;
            timedOutTimer.Enabled = true;

        }
        public void EndTimer()
        {
            timedOutTimer.Stop();
            timedOutTimer.Dispose();
        }
        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e) => IsTimedOut = true;

    }
}
