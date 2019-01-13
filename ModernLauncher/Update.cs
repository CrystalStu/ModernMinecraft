using System;
using System.Threading.Tasks;
using static ModernMinecraftShared.Git;
using ModernMinecraftShared;

namespace ModernLauncher
{
    public delegate void UpdateEventHandler(UpdateEventArgs e);

    public class UpdateEventArgs : EventArgs
    {
        private bool InteriorSet;
        private int InteriorArg; // 0: Big Status, 1: Normal Status
        private string InteriorCont;

        public UpdateEventArgs(bool Set, int Arg, string Cont)
        {
            InteriorSet = Set;
            InteriorArg = Arg;
            InteriorCont = Cont;
        }

        public bool GetSet()
        {
            return InteriorSet;
        }

        public int GetArg()
        {
            return InteriorArg;
        }

        public string GetCont()
        {
            return InteriorCont;
        }
    }

    public class Update
    {
        public static event UpdateEventHandler OnUpdate;

        protected const string SupportMail = "support@mail.cstu.gq";

        protected static string LogMessage;

        protected static void SetBigStatus(string status)
        {
            OnUpdate?.Invoke(new UpdateEventArgs(true, 0, status));
        }

        protected static void SetStatus(string status)
        {
            OnUpdate?.Invoke(new UpdateEventArgs(true, 1, status));
        }

        public static void Subete()
        {
            SetBigStatus("Preparing...");
            Repository.OnDataArrived += (RepositoryEventArgs e) =>
            {
                string Base = string.Empty;
                switch(e.GetStatusType())
                {
                    case RepositoryEventArgs.StatusType.Normal:
                        Base = "Normal: ";
                        break;
                    case RepositoryEventArgs.StatusType.Error:
                        if (e.GetCmdLine().Contains("reset")) SetBigStatus("Error while resetting...");
                        if (e.GetCmdLine().Contains("clean")) SetBigStatus("Error while cleaning...");
                        if (e.GetCmdLine().Contains("fetch")) SetBigStatus("Processing fetching...");
                        if (e.GetCmdLine().Contains("pull")) SetBigStatus("Processing pulling...");
                        Base = "Advanced: ";
                        break;
                }
                SetStatus(Base + e.GetStatus());
            };
            Repository repository = new Repository(Environment.CurrentDirectory);
            SetBigStatus("Resetting...");
            Task.Run(() => repository.Reset()).Wait();
            SetBigStatus("Cleaning...");
            Task.Run(() => repository.Clean()).Wait();
            SetBigStatus("Fetching...");
            Task.Run(() => repository.Fetch()).Wait();
            SetBigStatus("Pulling...");
            Task.Run(() => repository.Pull()).Wait();
            SetBigStatus("Done.");
        }

        // Deprecated since NGit not working
        // Git repository = Git.Open(Environment.CurrentDirectory);
        // repository.Reset().SetMode(ResetCommand.ResetType.HARD);
        // repository.Clean();
        // repository.Fetch().SetRemote("origin").SetRefSpecs(new RefSpec("+refs/heads/*:refs/remotes/origin/*")).SetProgressMonitor(new ThreadSafeProgressMonitor()).Call();

        /* Deprecated since libgit2sharp not working
        public static void Subete()
        {
            SetBigStatus("Preparing...");
            Repository repository = new Repository(Environment.CurrentDirectory);
            Remote remote = repository.Network.Remotes["origin"];
            IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
            FetchOptions fetchOptions = new FetchOptions
            {
                OnTransferProgress = SetFetchTransferProgress
            };
            PullOptions pullOptions = new PullOptions();
            pullOptions.FetchOptions.OnTransferProgress = SetPullFetchTransferProgress;
            pullOptions.MergeOptions.OnCheckoutProgress = SetPullCheckoutProgress;
            SetBigStatus("Resetting...");
            Task.Run(() =>
            {
                repository.Reset(ResetMode.Hard);
            });
            SetBigStatus("Cleaning...");
            Task.Run(() =>
            {
                repository.RemoveUntrackedFiles();
            }).Wait();
            SetBigStatus("Fetching...");
            Task.Run(() =>
            {
                Commands.Fetch(repository, remote.Name, refSpecs, fetchOptions, LogMessage);
            }).Wait();
            Console.WriteLine("[Launcher Updater] Fetching completed: " + LogMessage);
            SetBigStatus("Pulling...");
            Task.Run(() =>
            {
                Commands.Pull(repository, new Signature(new Identity("ModernMinecraft User", SupportMail), DateTimeOffset.UtcNow), pullOptions);
            }).Wait();
            SetBigStatus("Done.");
        }

        static bool SetFetchTransferProgress(TransferProgress transferProgress)
        {
            SetBigStatus("Fetching...");
            SetStatus(transferProgress.ReceivedBytes + " downloaded.\t" + transferProgress.ReceivedObjects + "/" + transferProgress.TotalObjects + " objects completed.\t" + Convert.ToDecimal(transferProgress.ReceivedObjects / transferProgress.TotalObjects).ToString() + "% completed.");
            return true;
        }

        static bool SetPullFetchTransferProgress(TransferProgress transferProgress)
        {
            SetBigStatus("Pulling and fetching...");
            SetStatus("Downloaded: " + transferProgress.ReceivedBytes + "\t Completed: " + transferProgress.ReceivedObjects + "/" + transferProgress.TotalObjects + "\t Percentage:" + Convert.ToDecimal(transferProgress.ReceivedObjects / transferProgress.TotalObjects).ToString() + "%");
            return true;
        }

        static void SetPullCheckoutProgress(string path, int completedSteps, int totalSteps)
        {
            SetBigStatus("Pulling and checking out...");
            SetStatus("Path: " + path + "\nCompleted: " + completedSteps + "/" + totalSteps + "\tPercentage: " + Convert.ToDecimal(completedSteps / totalSteps).ToString() + "%");
        }
        */
    }
}
