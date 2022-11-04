using System.Diagnostics;

const string chromeName = "Chrome";
const string firefoxName = "Firefox";
HashSet<string> SupportedBrowserNames = new HashSet<string> { chromeName, firefoxName };

NotifyIcon? browserIcon = null;

ApplicationConfiguration.Initialize();

UpdateBrowserIcon();

Application.Run();

void ChangeBrowserHandler(object? sender, MouseEventArgs e)
{
    if (e.Button != MouseButtons.Left)
    {
        return;
    }

    var currentBrowser = GetCurrentBrowserName();
    switch (currentBrowser)
    {
        case chromeName:
            SetAsDefaultBrowser(firefoxName);
            break;
        case firefoxName:
            SetAsDefaultBrowser(chromeName);
            break;
        default:
            throw new InvalidProgramException($"Unknown current browser {currentBrowser}");
    }

    UpdateBrowserIcon();
}

void UpdateBrowserIcon()
{
    if (browserIcon != null)
    {
        browserIcon.MouseClick -= ChangeBrowserHandler;
        browserIcon.Visible = false;
        browserIcon.ContextMenuStrip.Dispose();
        browserIcon.Dispose();
    }

    var browser = GetCurrentBrowserName();

    if (!SupportedBrowserNames.Contains(browser))
    {
        SetAsDefaultBrowser(chromeName);
        return;
    }

    browserIcon = new NotifyIcon
    {
        Visible = true,
        Icon = new Icon($"{browser}.ico"),
        ContextMenuStrip = MakeContextMenu()
    };
    browserIcon.MouseClick += ChangeBrowserHandler;

    ContextMenuStrip MakeContextMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Close", null, (_, _) => Application.Exit());
        return menu;
    }
}

string GetCurrentBrowserName() => CallBrowserScriptCommand("Get-CurrentDefaultBrowser");

void SetAsDefaultBrowser(string browserName) => CallBrowserScriptCommand("Set-DefaultBrowserByName", browserName);

string CallBrowserScriptCommand(params string[] commands)
{
    var processInfo = new ProcessStartInfo("powershell.exe")
    {
        CreateNoWindow = true,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        Arguments = $"-File BrowserUtils.ps1 {string.Join(' ', commands.Select(c => $"\"{c}\""))}"
    };

    using var process = Process.Start(processInfo);
    process!.WaitForExit();
    return process.StandardOutput.ReadToEnd().Trim();
}