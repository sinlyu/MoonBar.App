using System.Runtime.InteropServices;
using MoonBar.App.Core;
using MoonBar.App.WinApi.Interop;
using MoonBar.App.WinApi.Structs;

namespace MoonBar.App.Display;

internal sealed class DisplayManager : Singleton<DisplayManager>
{
    public IList<DisplayMonitor> DisplayMonitors { get; }

    public DisplayManager()
    {
        DisplayMonitors = new List<DisplayMonitor>();
        RefreshDisplayMonitors();
    }

    public DisplayMonitor FromHandle(IntPtr handle)
    {
        return DisplayMonitors.First(monitor => monitor.Handle == handle);
    }
    
    private void RefreshDisplayMonitors()
    {
        DisplayMonitors.Clear();
        UnmanagedMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            EnumDisplayMonitorsCallback(), IntPtr.Zero);
    }

    private UnmanagedMethods.EnumMonitorsDelegate EnumDisplayMonitorsCallback()
    {
        return delegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
        {
            var monitorInfo = new MonitorInfo();
            monitorInfo.cbSize = (uint)Marshal.SizeOf(monitorInfo);
                
            UnmanagedMethods.GetMonitorInfo(hMonitor, ref monitorInfo);

            var displayMonitor = new DisplayMonitor(hMonitor, monitorInfo);
            DisplayMonitors.Add(displayMonitor);

            return true;
        };
    }
}