using System;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace MemoScope.Helpers;

public static class WindowsSettings
{
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;

    [DllImport("dwmapi.dll")]
    internal static extern bool DwmSetWindowAttribute(IntPtr hwnd, int attribut, ref int attrValeur, int attrSize);

    internal enum PreferredAppMode
    {
        APPMODE_DEFAULT = 0,
        APPMODE_ALLOWDARK = 1,
        APPMODE_FORCEDARK = 2,
        APPMODE_FORCELIGHT = 3,
        APPMODE_MAX = 4
    }

    [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern int SetPreferredAppMode(PreferredAppMode preferredAppMode);

    public static bool IsWindowsApplicationInDarkMode()
    {
        try
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
            if (reg != null)
            {
                int lightMode = int.Parse(reg.GetValue("AppsUseLightTheme").ToString());
                return (lightMode == 0);
            }
        }
        catch (Exception) { /* Ignore errors */ }
        return false;
    }

    public static bool IsWindows10OrGreater(int build = 0)
    {
        return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
    }

    public static bool IsWindows11OrGreater(int build = 22000)
    {
        if (build < 22000)
            return false;
        return IsWindows10OrGreater(build);
    }

    public static bool UseImmersiveDarkMode(IntPtr pointeurFenetre, bool activerDarkMode)
    {
        if (IsWindows10OrGreater())
        {
            int attribut = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
            if (IsWindows10OrGreater(18985))
            {
                attribut = DWMWA_USE_IMMERSIVE_DARK_MODE;
            }
            int immersiveActif = (activerDarkMode ? 1 : 0);
            SetPreferredAppMode(activerDarkMode ? PreferredAppMode.APPMODE_ALLOWDARK : PreferredAppMode.APPMODE_DEFAULT);
            return DwmSetWindowAttribute(pointeurFenetre, attribut, ref immersiveActif, IntPtr.Size);
        }
        return false;
    }
}
