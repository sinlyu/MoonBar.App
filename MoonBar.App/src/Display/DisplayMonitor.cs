using MoonBar.App.WinApi.Structs;

namespace MoonBar.App.Display;

internal sealed class DisplayMonitor
{
    public MonitorInfo MonitorInfo { get; }
    public IntPtr Handle { get; }

    internal DisplayMonitor(IntPtr handle, MonitorInfo monitorInfo)
    {
        Handle = handle;
        MonitorInfo = monitorInfo;
    }
}