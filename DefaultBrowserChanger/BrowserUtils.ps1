function Set-DefaultBrowser($browserPopupPositionInTabsCount)
{
    Add-Type -AssemblyName 'System.Windows.Forms'
    Start-Process "ms-settings:defaultapps"
    Sleep 1
    # go to default browser option
    [System.Windows.Forms.SendKeys]::SendWait("{TAB}{TAB}{TAB}{TAB}{TAB}")

    # open all available apps
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")

    # wait till popup displayed
    Sleep .5

    # select browser  
    for($i = 0; $i -lt $browserPopupPositionInTabsCount; $i+=1)
    {
        [System.Windows.Forms.SendKeys]::SendWait("{TAB}")
    }
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")

    # wait until selection applied and close
    Sleep .6
    Stop-Process -Name "SystemSettings" -Force
}

function Get-CurrentDefaultBrowser
{
    $currentDefaultBrowser = (Get-ItemProperty HKCU:\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice -Name ProgId).ProgId
    $chromeBrowserId = "ChromeHTML"
    $firefoxBrowserId = "FirefoxURL-308046B0AF4A39CB"

    if($currentDefaultBrowser -eq $chromeBrowserId)
    {
        Write-Output "Chrome"
        exit
    }

    if($currentDefaultBrowser -eq $firefoxBrowserId)
    {
        Write-Output "Firefox"
        exit
    }

    Write-Output $currentDefaultBrowser
}

function Set-DefaultBrowserByName($name)
{
    if($name -eq "Firefox")
    {
        Set-DefaultBrowser 1
        Write-Output "Firefox"
        exit
    }

    if($name -eq "Chrome")
    {
        Set-DefaultBrowser 2
        Write-Output "Chrome"
        exit
    }

    Write-Output "Unknown browser name"
}

if($args[0] -eq "Get-CurrentDefaultBrowser")
{
    Get-CurrentDefaultBrowser
}
elseif($args[0] -eq "Set-DefaultBrowserByName")
{
    Set-DefaultBrowserByName $args[1]
}






