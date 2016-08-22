namespace AutofacTests
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;

    using Autofac.Features.Indexed;

    using AutofacTests.Ping;

    public class StatusManager
    {
        private readonly IIndex<Checker, IPingService> chekers;

        private readonly ConcurrentDictionary<Checker, bool> statuses;

        private readonly Thread statusThread;

        public BlockingCollection<CheckerEventArgs> EventQueue { get; } = new BlockingCollection<CheckerEventArgs>(); 
        public StatusManager(IIndex<Checker, IPingService> chekers)
        {
            this.chekers = chekers;
            this.statuses = new ConcurrentDictionary<Checker, bool>(Enum
                .GetValues(typeof(Checker))
                .Cast<Checker>()
                .ToDictionary(k => k, v => false));
            this.statusThread = new Thread(this.Run) { Name = "Status checker thread", IsBackground = true };
            
        }

        private void Run()
        {
            while (true)
            {
                Thread.Sleep(3456);
                lock (this.statuses)
                {
                    foreach (var checker in this.statuses.Keys)
                    {
                        var newStatus = this.chekers[checker].IsOnline();
                        if (this.statuses[checker] == newStatus)
                        {
                            continue;
                        }
                        this.EventQueue.Add(new CheckerEventArgs(checker, newStatus));
                        this.statuses[checker] = newStatus;
                    }
                }
            }
        }

        public void Start()
        {
            this.statusThread.Start();
        }

        public bool GetCurrentStatus(Checker checker)
        {
            return this.statuses[checker];
        }



        public void Stop()
        {
            this.statusThread.Abort();
        }
    }

    public class CheckerEventArgs   
    {
        public CheckerEventArgs(Checker checker, bool status)
        {
            this.Checker = checker;
            this.Status = status;
        }

        public Checker Checker { get; }

        public bool Status { get; }
    }
}