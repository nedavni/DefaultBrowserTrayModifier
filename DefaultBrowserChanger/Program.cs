using System.Diagnostics;

NotifyIcon? _browserIcon = null;

ApplicationConfiguration.Initialize();

UpdateBrowserIcon();

Application.Run();

void ChangeBrowserHandler(object? sender, MouseEventArgs e)
{
    if (e.Button != MouseButtons.Left)
    {
        return;
    }

    var currentBrowser = CallBrowserScriptCommand("Get-CurrentDefaultBrowser");
    switch (currentBrowser)
    {
        case "Chrome":
            CallBrowserScriptCommand("Set-DefaultBrowserByName", "Firefox");
            break;
        case "Firefox":
            CallBrowserScriptCommand("Set-DefaultBrowserByName", "Chrome");
            break;
        default:
            throw new InvalidProgramException($"Unknown current browser {currentBrowser}");
    }

    UpdateBrowserIcon();
}

void UpdateBrowserIcon()
{
    if (_browserIcon != null)
    {
        _browserIcon.MouseClick -= ChangeBrowserHandler;
        _browserIcon.Visible = false;
        _browserIcon.ContextMenuStrip.Dispose();
        _browserIcon.Dispose();
    }

    var browser = CallBrowserScriptCommand("Get-CurrentDefaultBrowser");

    _browserIcon = new NotifyIcon
    {
        Visible = true,
        Icon = new Icon($"{browser}.ico"),
        ContextMenuStrip = MakeContextMenu()
    };
    _browserIcon.MouseClick += ChangeBrowserHandler;

    ContextMenuStrip MakeContextMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Close", null, (_, _) => Application.Exit());
        return menu;
    }
}

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