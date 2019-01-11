using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Gtk;
using ModernLauncher;

public partial class MainWindow : Gtk.Window
{
    protected const bool RemoteEnable = false;
    protected const bool RemoteUpdate = false;
    protected const bool RemoteNotice = true;
    protected const string RemoteUrl = "https://vl.cstu.gq/support/launcher";
    protected const string Website = "https://vl.cstu.gq";
    protected const string Register = "https://login2.nide8.com:233/28f8f58a8a7f11e88feb525400b59b6a/register";

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        ApplyStyles();
        if(RemoteUpdate) CheckGit();
        if(RemoteEnable) CheckEnable();
        if(RemoteUpdate) LauncherUpdate();
        if(RemoteNotice) labelNotice.Text = Web.DownloadText(RemoteUrl + "/motd.txt");
        else
        {
            labelNoticeIndicator.Visible = false;
            labelNotice.Visible = false;
        }
        if (Website.Length == 0) buttonWebsite.Visible = false;
        labelStatus.Text = "Done.";
    }

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

    private void SetLabelForegroundToWhite(ref Label widget)
    {
        widget.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));
    }

    #region Component Handler

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        // Application.Quit();
        // a.RetVal = true;
        Sortie();
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
        Launch.VLW(this, entryUsername.Text, entryPassword.Text, spinbuttonMemory.Digits.ToString(), Environment.CurrentDirectory, checkbuttonFullScreen.Active);
        Sortie();
    }

    #endregion

    protected void LauncherUpdate()
    {
        if (FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion != Web.DownloadText(RemoteUrl + "/ver.txt"))
        {
            MessageDialog messageDialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "This launcher is outdated, confirm to update it.\n\nDetails:\n" + Web.DownloadText("https://vl.cstu.gq/support/launcher/upd_log.txt"))
            {
                Title = "Update Required"
            };
            messageDialog.Run();
            // To be implemented.
            Sortie();
        }
    }

    protected void CheckGit()
    {
        if(!Utility.CheckProcessSuccess("git", "--version")) {
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

    protected void CleanUpdateLog()
    {
        try
        {
            Utility.DeleteFile(Environment.CurrentDirectory + "/update.log");
            Utility.DeleteFile(Environment.CurrentDirectory + "/update.err");
        }
        catch
        {
            labelStatus.Text = "DEL_TRASH_ERROR";
        }
    }

    protected void Sortie()
    {
        CleanUpdateLog();
        Environment.Exit(0);
    }
}
