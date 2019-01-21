#undef REMOTE_ENABLE
#undef REMOTE_UPDATE
#undef REMOTE_LAUNCHER_UPDATE
#undef REMOTE_NOTICE

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using AutoUpdaterDotNET;
using Gtk;
using KMCCC.Launcher;
using ModernLauncher;
using ModernMinecraftShared;

public partial class MainWindow : Window
{
    protected const string RemoteUrl = "https://vl.cstu.gq/support/launcher";
    protected const string Website = "https://vl.cstu.gq";
    protected const string Register = "https://login2.nide8.com:233/28f8f58a8a7f11e88feb525400b59b6a/register";

    protected System.Media.SoundPlayer backgroundMusicPlayer;

    public MainWindow() : base(WindowType.Toplevel)
    {
        Build();
        ApplyLanguage();
        ApplyStyles();
        LoadVersions();
#if REMOTE_UPDATE
        CheckGit();
#endif
#if REMOTE_ENABLE
        CheckEnable();
#endif
#if REMOTE_LAUNCHER_UPDATE
        LauncherUpdate();
#endif
#if REMOTE_UPDATE
        ClientUpdate();
#else
        labelStatusIndicator.Visible = false;
        labelStatus.Visible = false;
#endif
#if REMOTE_NOTICE
        labelNotice.Text = Web.DownloadText(RemoteUrl + "/motd.txt");
#else
        labelNoticeIndicator.Visible = false;
        labelNotice.Visible = false;
#endif
        if (Website.Length == 0) buttonWebsite.Visible = false;
        PlayMisc();
    }

    #region Frontend Handler

    protected void ApplyLanguage()
    {
        if (Thread.CurrentThread.CurrentUICulture.Name.Contains("ja"))
        {
            Title = "Modern ランチャー";
            labelWelcome.Text = "こんにちは！";
            labelMemory.Text = "メモリ制限 (MB)：";
            labelNotice.Text = "読み込み中...";
            labelNoticeIndicator.Text = "お知らせ：";
            labelStatus.Text = "読み込み中...";
            labelStatusIndicator.Text = "状態：";
            labelPassword.Text = "パスワード：";
            labelUsername.Text = "ＩＤ：";
            labelFullscreen.Text = "全画面表示：";
            labelVersion.Text = "バージョン：";
            buttonAbout.Label = "について";
            buttonLaunch.Label = "行きましょう";
            buttonWebsite.Label = "サイト";
            buttonRegister.Label = "登録";
            togglebuttonMusic.Label = "音楽";
        }
        if (Thread.CurrentThread.CurrentUICulture.Name.Contains("zh"))
        {
            Title = "Modern 启动器";
            labelWelcome.Text = "欢迎！";
            labelMemory.Text = "内存限制 (MB)：";
            labelNotice.Text = "加载中...";
            labelNoticeIndicator.Text = "通知：";
            labelStatus.Text = "加载中...";
            labelStatusIndicator.Text = "状态：";
            labelPassword.Text = "密码：";
            labelUsername.Text = "ＩＤ：";
            labelFullscreen.Text = "全屏显示：";
            labelVersion.Text = "版本：";
            buttonAbout.Label = "关于";
            buttonLaunch.Label = "启动";
            buttonWebsite.Label = "网站";
            buttonRegister.Label = "注册";
            togglebuttonMusic.Label = "音乐";
        }
    }

    protected void ApplyStyles()
    {
        Opacity = 0.978;
        SetLabelForegroundToWhite(ref labelUsername);
        SetLabelForegroundToWhite(ref labelPassword);
        SetLabelForegroundToWhite(ref labelWelcome);
        SetLabelForegroundToWhite(ref labelStatus);
        SetLabelForegroundToWhite(ref labelNotice);
        SetLabelForegroundToWhite(ref labelStatusIndicator);
        SetLabelForegroundToWhite(ref labelNoticeIndicator);
        SetLabelForegroundToWhite(ref labelFullscreen);
        SetLabelForegroundToWhite(ref labelVersion);
        spinbuttonMemory.SetRange(128, new PerformanceCounter("Mono Memory", "Total Physical Memory").RawValue / 1048576 - 1024);
        labelWelcome.ModifyFont(new Pango.FontDescription
        {
            Size = Utility.GetScaledSize(65)
        });
    }

    protected void LoadVersions()
    {
        LauncherCore core = LauncherCore.Create();
        var versions = core.GetVersions().Cast<KMCCC.Launcher.Version>();
        if (versions.Any())
        {
            foreach (KMCCC.Launcher.Version version in versions) comboboxVersion.AppendText(version.Id);
            comboboxVersion.Model.IterNthChild(out TreeIter iter, 0);
            comboboxVersion.SetActiveIter(iter);
        }
    }

    protected void SetLabelForegroundToWhite(ref Label widget)
    {
        widget.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));
    }

    protected void PlayMisc()
    {
        backgroundMusicPlayer = new System.Media.SoundPlayer(Assembly.GetExecutingAssembly().GetManifestResourceStream("ModernLauncher.Resources.BackgroundMisc"));
        backgroundMusicPlayer.PlayLooping();
    }

    #endregion

    #region Event Handler

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnButtonWebsiteClicked(object sender, EventArgs e)
    {
        Process.Start(Website);
    }

    protected void OnButtonRegisterClicked(object sender, EventArgs e)
    {
        Process.Start(Register);
    }

    protected void OnButtonLaunchClicked(object sender, EventArgs e)
    {
        Launch.VersionName = comboboxVersion.ActiveText;
        if (Thread.CurrentThread.CurrentUICulture.Name.Contains("ja")) Launch.ChangeOption(Environment.CurrentDirectory, 1);
        else if (Thread.CurrentThread.CurrentUICulture.Name.Contains("zh")) Launch.ChangeOption(Environment.CurrentDirectory, 2);
        else Launch.ChangeOption(Environment.CurrentDirectory, 0);
        Launch.VLW(this, entryUsername.Text, entryPassword.Text, ((int)spinbuttonMemory.Value).ToString(), Environment.CurrentDirectory, checkbuttonFullScreen.Active);
        Sortie();
    }

    protected void OnTogglebuttonMusicToggled(object sender, EventArgs e)
    {
        switch (togglebuttonMusic.Active)
        {
            case false:
                backgroundMusicPlayer.Stop();
                break;
            case true:
                backgroundMusicPlayer.PlayLooping();
                break;
        }
    }

    protected void OnButtonAboutClicked(object sender, EventArgs e)
    {
        new ModernLauncher.AboutDialog().Show();
    }

    #endregion

    #region Backend Handler

    protected void LauncherUpdate()
    {
        AutoUpdater.Mandatory = true;
        AutoUpdater.AppTitle = "Modern Launcher Update Window";
        AutoUpdater.ReportErrors = true;
        AutoUpdater.Start(RemoteUrl + "/universal/UpdateInfo.xml");
    }

    protected void ClientUpdate()
    {
        Thread thread = new Thread(new ThreadStart(delegate
        {
            Update.OnUpdate += Update_OnUpdate;
            Update.Subete();
        }));
        thread.Start();
    }

    void Update_OnUpdate(UpdateEventArgs e)
    {
        if (e.GetSet())
        {
            switch (e.GetArg())
            {
                case 0:
                    labelWelcome.Text = e.GetCont();
                    break;
                case 1:
                    labelStatus.Text = e.GetCont();
                    break;
            }
        }
    }


    protected void CheckGit()
    {
        if (!Utility.CheckProcessSuccess("git", "--version"))
        {
            MessageDialog messageDialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "The program detected your git with installation was not supported.")
            {
                Title = "Missed Component"
            };
            messageDialog.Run();
            Sortie();
        }
    }

    protected void CheckEnable()
    {
        if (!Web.CheckFile(RemoteUrl + "/enable.txt"))
        {
            MessageDialog messageDialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "This launcher is disabled by the web administrator or you have disconnected from the Internet.")
            {
                Title = "Disabled"
            };
            messageDialog.Run();
            Sortie();
        }
    }

    protected void Sortie()
    {
        Application.Quit();
    }

    #endregion

    #region Outside Calling Handler

    protected void SetBigStatus(string status)
    {
        labelWelcome.Text = status;
    }

    protected void SetStatus(string status)
    {
        labelStatus.Text = status;
    }

    #endregion
}
