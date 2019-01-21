using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Gdk;

namespace ModernLauncher
{
    public partial class AboutDialog : Gtk.Dialog
    {
        public AboutDialog()
        {
            Build();
            ApplyLanguage();
            Icon = Pixbuf.LoadFromResource("ModernLauncher.Resources.Icon");
            ModifyBg(Gtk.StateType.Normal, new Color(200, 200, 200));
            Pixbuf.LoadFromResource("ModernLauncher.Resources.Icon").RenderPixmapAndMask(out Pixmap pixmapLogo, out Pixmap pixmapMask, 255);
            imageLogo.SetFromPixmap(pixmapLogo, pixmapMask);
            labelVersion.Text = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        }

        protected void ApplyLanguage()
        {
            if (Thread.CurrentThread.CurrentUICulture.Name.Contains("ja"))
            {
                Title = "について";
                labelTitle.Text = "<b>Modern ランチャー マインクラフト</b>";
                labelTitle.UseMarkup = true;
                labelVersion.Text = "読み込み中...";
                labelDescription.Text = "<b>Modern マインクラフト</b>プロジェクトの一部、軽いと便利なマインクラフトランチャーです。";
                labelDescription.UseMarkup = true;
                labelWebsite.Text = "<a href=\"https://github.com/CrystalStu/ModernMinecraft\">ウェブサイト</a>";
                labelWebsite.UseMarkup = true;
                labelLicense.Text = "<i>GNU General Public License v3.0</i>で配布しています。";
                labelLicense.UseMarkup = true;
            }
            if (Thread.CurrentThread.CurrentUICulture.Name.Contains("zh"))
            {
                Title = "关于";
                labelTitle.Text = "<b>Modern 启动器 Minecraft</b>";
                labelTitle.UseMarkup = true;
                labelVersion.Text = "加载中...";
                labelDescription.Text = "这个程序是<b>Modern Minecraft</b>项目的一部分，帮助人们更轻便地启动Minecraft。";
                labelDescription.UseMarkup = true;
                labelWebsite.Text = "<a href=\"https://github.com/CrystalStu/ModernMinecraft\">网站</a>";
                labelWebsite.UseMarkup = true;
                labelLicense.Text = "使用<i>GNU General Public License v3.0</i>进行分发。";
                labelLicense.UseMarkup = true;
            }
        }

        protected void OnButtonOkClicked(object sender, EventArgs e)
        {
            Destroy();
        }
    }
}
