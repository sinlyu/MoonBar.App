using System.Runtime.InteropServices;
using MoonBar.App.WinAPI.Structs;

namespace MoonBar.App.WinAPI.Display;

public sealed class DisplayMonitor
{
    public MonitorInfo MonitorInfo;
    public IntPtr Handle;

    private DisplayMonitor(IntPtr handle, MonitorInfo monitorInfo)
    {
        Handle = handle;
        MonitorInfo = monitorInfo;
    }

    public static List<DisplayMonitor> GetDisplayMonitors()
    {
        var monitors = new List<DisplayMonitor>();
        
        User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            delegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
            {
                var monitorInfo = new MonitorInfo();
                monitorInfo.cbSize = (uint)Marshal.SizeOf(monitorInfo);
                
                User32.GetMonitorInfo(hMonitor, ref monitorInfo);

                var displayMonitor = new DisplayMonitor(hMonitor, monitorInfo);
                monitors.Add(displayMonitor);

                return true;
            }, IntPtr.Zero);

        return monitors;
    }
}