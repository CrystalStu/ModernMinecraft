#define REMOTE_ENABLE
#undef REMOTE_UPDATE
#define REMOTE_LAUNCHER_UPDATE
#define REMOTE_NOTICE

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using AutoUpdaterDotNET;
using Gtk;
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
        Mono.Unix.Catalog.Init("i8n1", "./locale");
        Build();
        ApplyStyles();
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

    protected void ApplyStyles()
    {
        SetLabelForegroundToWhite(ref labelUsername);
        SetLabelForegroundToWhite(ref labelPassword);
        SetLabelForegroundToWhite(ref labelWelcome);
        SetLabelForegroundToWhite(ref labelStatus);
        SetLabelForegroundToWhite(ref labelNotice);
        SetLabelForegroundToWhite(ref labelStatusIndicator);
        SetLabelForegroundToWhite(ref labelNoticeIndicator);
        spinbuttonMemory.SetRange(128, new PerformanceCounter("Mono Memory", "Total Physical Memory").RawValue / 1048576 - 1024);
        checkbuttonFullScreen.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));
        labelWelcome.ModifyFont(new Pango.FontDescription
        {
            Size = Utility.GetScaledSize(65)
        });
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
        if (Thread.CurrentThread.CurrentUICulture.Name.Contains("ja")) Launch.ChangeOption(Environment.CurrentDirectory, 1);
        else if (Thread.CurrentThread.CurrentUICulture.Name.Contains("zh")) Launch.ChangeOption(Environment.CurrentDirectory, 2);
        else Launch.ChangeOption(Environment.CurrentDirectory, 0);
        Launch.VLW(this, entryUsername.Text, entryPassword.Text, ((int)spinbuttonMemory.Value).ToString(), Environment.CurrentDirectory, checkbuttonFullScreen.Active);
        Sortie();
    }

    protected void OnTogglebuttonMusicToggled(object sender, EventArgs e)
    {
        switch(togglebuttonMusic.Active)
        {
            case false:
                backgroundMusicPlayer.Stop();
                break;
            case true:
                backgroundMusicPlayer.PlayLooping();
                break;
        }
    }

#endregion

#region Backend Handler

    protected void LauncherUpdate()
    {
        AutoUpdater.Mandatory = true;
        AutoUpdater.AppTitle = "Modern Launcher Update Window";
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
        if(e.GetSet())
        {
            switch(e.GetArg()) {
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
