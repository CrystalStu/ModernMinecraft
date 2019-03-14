// #define ENABLE_TONGYI_AUTHENTICATION

using System;
using System.IO;
using System.Threading;
using Gtk;
using KMCCC.Authentication;
using KMCCC.Launcher;
using ModernMinecraftShared;

namespace ModernLauncher
{
    public static class Launch
    {
        const string ServerTongyiStr = "28f8f58a8a7f11e88feb525400b59b6a";
        public static string VersionName;
        const string ServerIP = "";
        const ushort ServerPort = 25565;

        public static void ChangeOption(string rootPath, int lang)
        {
            if (!File.Exists(rootPath + "/.minecraft/versions/" + VersionName + "/options.txt.tmpl")) return;
            if (!File.Exists(rootPath + "/.minecraft/versions/" + VersionName + "/options.txt"))
            {
                Utility.CopyFile(rootPath + "/.minecraft/versions/" + VersionName + "/options.txt.tmpl", rootPath + "/.minecraft/versions/" + VersionName + "/options.txt");
            }
            string langStr;
            switch (lang)
            {
                default:
                    langStr = "en_us";
                    break;
                case 1:
                    langStr = "ja_jp";
                    break;
                case 2:
                    langStr = "zh_cn";
                    break;
            }
            string[] properties = File.ReadAllLines(rootPath + "/.minecraft/versions/" + VersionName + "/options.txt");
            for (int t = 0; t < properties.Length; t++)
            {
                if (properties[t].Split(':')[0] == "gamma")
                {
                    properties[t] = properties[t].Replace("1", "0").Replace("2", "0").Replace("3", "0").Replace("4", "0").Replace("5", "0").Replace("6", "0").Replace("7", "0").Replace("8", "0").Replace("9", "0");
                    continue;
                }
                if (properties[t].Split(':')[0] == "lang")
                {
                    properties[t] = "lang:" + langStr;
                    continue;
                }
            }
            File.WriteAllLines(rootPath + "/.minecraft/versions/" + VersionName + "/options.txt", properties);
        }

        public static void VLW(Window window, string username, string password, string memory, string path, bool fullScreen)
        {
            LauncherCore core = LauncherCore.Create();
            var option = new LaunchOptions
            {
                Version = core.GetVersion(VersionName),
                MaxMemory = Convert.ToInt32(memory),
                Mode = LaunchMode.MCLauncher,
                Size = new WindowSize
                {
                    Height = (ushort?)(Gdk.Screen.Default.Height / 2),
                    Width = (ushort?)(Gdk.Screen.Default.Width / 2)
                }
            };
            if (fullScreen) option.Size.FullScreen = true;
#if ENABLE_TONGYI_AUTHENTICATION
            option.AgentPath = "nide8auth.jar=" + ServerTongyiStr; // Chinese Third-party Authentication System: Minecraft 统一通行证
            option.Authenticator = new YggdrasilLogin(username, password, true, null, "https://auth2.nide8.com:233/" + ServerTongyiStr + "/authserver");
#else
            option.Authenticator = new OfflineAuthenticator(username);
#endif
            if (ServerIP.Length != 0) option.Server = new ServerInfo { Address = ServerIP, Port = ServerPort };
            var result = core.Launch(option);
            if (!result.Success)
            {
                MessageDialog messageDialog;
                switch (result.ErrorType)
                {
                    case ErrorType.NoJAVA:
                        messageDialog = new MessageDialog(window, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Java Error, try to reinstall the client.")
                        {
                            Title = "Java Error"
                        };
                        break;
                    case ErrorType.AuthenticationFailed:
                        messageDialog = new MessageDialog(window, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Please check your username and password.")
                        {
                            Title = "Profile Error"
                        };
                        break;
                    case ErrorType.UncompressingFailed:
                        messageDialog = new MessageDialog(window, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Decompressing Failed!\n\nCheck your client or reinstall.")
                        {
                            Title = "Decompression Failure"
                        };
                        break;
                    default:
                        messageDialog = new MessageDialog(window, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, result.ErrorMessage + "\n" + (result.Exception == null ? string.Empty : result.Exception.StackTrace))
                        {
                            Title = "Unknown Error"
                        };
                        break;
                }
                messageDialog.Run();
            }
            Thread.Sleep(50);
        }
    }
}
