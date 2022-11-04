using System.Diagnostics;

namespace DefaultBrowserChanger;
internal static class Program
{
    private static NotifyIcon? _browserIcon;

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        UpdateBrowserIcon();

        Application.Run();
    }

    private static void ChangeBrowserHandler(object? sender, EventArgs e)
    {
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

    private static void UpdateBrowserIcon()
    {
        if (_browserIcon != null)
        {
            _browserIcon.Click -= ChangeBrowserHandler;
            _browserIcon.Dispose();
        }

        var browser = CallBrowserScriptCommand("Get-CurrentDefaultBrowser");

        _browserIcon = new NotifyIcon
        {
            Visible = true,
            Icon = new Icon($"{browser}.ico")
        };
        _browserIcon.Click += ChangeBrowserHandler;
    }

    private static string CallBrowserScriptCommand(params string[] commands)
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
}