using System;
using System.IO;
using System.Threading;
using Gtk;
using KMCCC.Authentication;
using KMCCC.Launcher;

namespace ModernLauncher
{
    public static class Launch
    {
        public static void ChangeOption(string rootPath, int lang)
        {
            if (!File.Exists(rootPath + "/.minecraft/versions/1.13/options.txt.tmpl")) return;
            if (!File.Exists(rootPath + "/.minecraft/versions/1.13/options.txt"))
            {
                Utility.MoveFile(rootPath + "/.minecraft/versions/1.13/options.txt.tmpl", rootPath + "/.minecraft/versions/1.13/options.txt");
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
            string[] properties = File.ReadAllLines(rootPath + "/.minecraft/versions/1.13/options.txt");
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
            File.WriteAllLines(rootPath + "/.minecraft/versions/1.13/options.txt", properties);
        }

        public static void VLW(Window window, string username, string password, string memory, string path, bool fullScreen)
        {
            string serverStr = "28f8f58a8a7f11e88feb525400b59b6a";
            // var versions = Program.Core.GetVersions().ToArray();
            var core = LauncherCore.Create(new LauncherCoreCreationOption(
                javaPath: "java"
            ));
            var ver = core.GetVersion("1.13");
            var option = new LaunchOptions
            {
                Version = ver,
                MaxMemory = Convert.ToInt32(memory),
                AgentPath = "nide8auth.jar=" + serverStr,
                // Authenticator = new OfflineAuthenticator(username),
                Authenticator = new YggdrasilLogin(username, password, true, null, "https://auth2.nide8.com:233/" + serverStr + "/authserver"), // 伪正版启动，最后一个为是否twitch登录
                Mode = LaunchMode.MCLauncher, //启动模式
                // Server = new ServerInfo { Address = "ali.cge.hm", Port = 30033 },
                /*
                Size = new WindowSize
                {
                    Height = 720,
                    Width = 1280,
                }
                */
            };
            if (fullScreen) option.Size.FullScreen = true;
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
