using System;
using Gtk;

namespace ModernLauncher
{
    class MainClass
    {
        public static MainWindow win;
        public static void Main(string[] args)
        {
            if (System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 < 1152)
            {
                Console.WriteLine("[Launcher] Cannot start: Insufficient total memory.");
                Environment.Exit(-1);
            };
            Application.Init();
            win = new MainWindow
            {
                Icon = Gdk.Pixbuf.LoadFromResource("ModernLauncher.Resources.Icon")
            };
            win.SetSizeRequest(1024, 576);
            Gdk.Pixbuf.LoadFromResource("ModernLauncher.Resources.Background").RenderPixmapAndMask(out Gdk.Pixmap pixmapBackground, out Gdk.Pixmap pixmapMask, 255);
            win.Style.SetBgPixmap(StateType.Normal, pixmapBackground);
            win.Show();
            Application.Run();
        }
    }
}
