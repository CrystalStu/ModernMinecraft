using System;
using System.Diagnostics;
using Xwt;

namespace ModernLauncher
{
    class MainClass
    {
        public static MainWindow win;

        [STAThread]
        public static void Main(string[] args)
        {
            CheckDotNet();
            // Application.Initialize(ToolkitType.Gtk);
            Application.Initialize("Xwt.GtkBackend.GtkEngine, Xwt.Gtk, Version=1.0.0.0");
            // Application.Init();
            win = new MainWindow
            {
                Icon = Gdk.Pixbuf.LoadFromResource("ModernLauncher.Resources.Icon")
            };
            win.SetSizeRequest(1024, 576);
            Gdk.Pixbuf.LoadFromResource("ModernLauncher.Resources.Background").RenderPixmapAndMask(out Gdk.Pixmap pixmapBackground, out Gdk.Pixmap pixmapMask, 255);
            win.Style.SetBgPixmap(Gtk.StateType.Normal, pixmapBackground);
            win.Show();
            Application.Run();
        }

        private static void CheckDotNet()
        {
            if (!ModernMinecraftShared.Utility.GetIsWindows()) return;
            if (Type.GetType("Mono.Runtime") != null) return;
            Process monoInstance = new Process();
            monoInstance.StartInfo.FileName = "mono.exe";
            monoInstance.StartInfo.Arguments = "--debug LauncherU.exe";
            monoInstance.StartInfo.UseShellExecute = false;
            monoInstance.StartInfo.CreateNoWindow = true;
            monoInstance.Start();
            Environment.Exit(0);
        }
    }
}
