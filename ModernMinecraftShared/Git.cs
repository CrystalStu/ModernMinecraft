using System;
using System.Diagnostics;

namespace ModernMinecraftShared
{
    public delegate void RepositoryEventHandler(RepositoryEventArgs e);

    public class RepositoryEventArgs : EventArgs
    {
        private string status;
        private string cmdLine;
        private StatusType type;

        public enum StatusType
        {
            Normal = 0,
            Error = 1
        }

        public RepositoryEventArgs(string Status, string CmdLine, StatusType Type)
        {
            status = Status;
            cmdLine = CmdLine;
            type = Type;
        }

        public string GetStatus()
        {
            return status;
        }

        public string GetCmdLine()
        {
            return cmdLine;
        }

        public StatusType GetStatusType()
        {
            return type;
        }
    }

    public class Git
    {
        public class Repository
        {
            public static event RepositoryEventHandler OnDataArrived;
            protected string path;

            public Repository(string Path)
            {
                path = Path;
            }

            public void Reset()
            {
                Execute("reset --hard");
            }

            public void Clean()
            {
                Execute("clean -df");
            }

            public void Fetch()
            {
                Execute("fetch origin master -f --progress");
            }

            public void Pull()
            {
                Execute("pull origin master");
            }

            public void Execute( string arguments)
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    UseShellExecute = false,
                    WorkingDirectory = path,
                    Arguments = arguments,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                Process process = Process.Start(processStartInfo);
                process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    if(e.Data != null) OnDataArrived?.Invoke(new RepositoryEventArgs(e.Data, arguments, RepositoryEventArgs.StatusType.Normal));
                };
                process.BeginOutputReadLine();
                process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    if (e.Data != null) OnDataArrived?.Invoke(new RepositoryEventArgs(e.Data, arguments, RepositoryEventArgs.StatusType.Error));
                };
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }
    }
}
