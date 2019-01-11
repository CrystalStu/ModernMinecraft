using System;
using Gtk;

namespace ModernLauncher
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 < 1152) Environment.Exit(-1);
            Application.Init();
            MainWindow win = new MainWindow
            {
                Icon = Gdk.Pixbuf.LoadFromResource("ModernLauncher.Resources.Icon")
            };
            win.SetSizeRequest(1200, 630);
            Gdk.Pixbuf.LoadFromResource("ModernLauncher.Resources.Background").RenderPixmapAndMask(out Gdk.Pixmap pixmapBackground, out Gdk.Pixmap pixmapMask, 255);
            win.Style.SetBgPixmap(StateType.Normal, pixmapBackground);
            win.Show();
            Application.Run();
        }
    }
}
