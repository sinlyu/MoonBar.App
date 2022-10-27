using System.Runtime.InteropServices;

namespace MoonBar.App.WinAPI.Structs;

/// <summary>
/// The MONITORINFOEX structure contains information about a display monitor.
/// The GetMonitorInfo function stores information into a MONITORINFOEX structure or a MONITORINFO structure.
/// The MONITORINFOEX structure is a superset of the MONITORINFO structure. The MONITORINFOEX structure adds a string member to contain a name
/// for the display monitor.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct MonitorInfo
{
    public uint cbSize;
    
    public Rect rcMonitor;
    
    public Rect rcWork;
    
    public uint dwFlags;
}