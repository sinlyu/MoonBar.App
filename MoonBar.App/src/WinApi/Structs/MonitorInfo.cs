using System.Runtime.InteropServices;

namespace MoonBar.App.WinApi.Structs;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct MonitorInfo
{
    public uint cbSize;
    public Rect rcMonitor;
    public Rect rcWork;
    public uint dwFlags;
}