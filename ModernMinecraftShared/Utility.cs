using System;
using System.Diagnostics;
using System.IO;
using Mono.Unix;

namespace ModernMinecraftShared
{
    public class Utility
    {
        public static void DeleteFile(string url)
        {
            if (!File.Exists(url)) return;
            var unixFileInfo = new UnixFileInfo(url)
            {
                // set file permission to 644
                FileAccessPermissions = FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite | FileAccessPermissions.GroupRead | FileAccessPermissions.OtherRead
            };
            File.Delete(url);
        }

        public static void MoveFile(string from, string to)
        {
            if (!File.Exists(from)) return;
            var unixFileInfo = new UnixFileInfo(from);
            // set file permission to 644
            unixFileInfo.FileAccessPermissions = FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite | FileAccessPermissions.GroupRead | FileAccessPermissions.OtherRead;
            new FileInfo(from).MoveTo(to);
        }

        public static int GetScaledSize(int size)
        {
            return Convert.ToInt32(size * Pango.Scale.PangoScale);
        }

        public static string GetLastLine(string file)
        {
            string st = string.Empty;
            if (File.Exists(file))
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        while (!sr.EndOfStream)
                        {
                            st = sr.ReadLine();
                        }
                    }
                }
            }
            return st;
        }

        public static bool CheckProcessSuccess(string filename, string argument)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = filename,
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = argument,
                RedirectStandardOutput = true
            };
            try
            {
                Process process = Process.Start(processStartInfo);
                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
