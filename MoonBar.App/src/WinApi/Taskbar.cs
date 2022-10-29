using MoonBar.App.Display;
using MoonBar.App.WinApi.Interop;
using MoonBar.App.WinApi.Structs;

namespace MoonBar.App.WinApi;

public sealed class Taskbar
{
    public IntPtr Handle { get; private set; }
    
    private readonly TaskBarInfo _taskBarInfo;
    private readonly DisplayMonitor _monitor;
    public Taskbar(TaskBarInfo taskBarInfo)
    {
        _taskBarInfo = taskBarInfo;
        _monitor = DisplayManager.Instance.FromHandle(_taskBarInfo.Monitor);
        Handle = _taskBarInfo.Handle;
    }

    public unsafe void UpdateWorkingArea(TaskbarPosition position)
    {
        var monitorInfo = _monitor.MonitorInfo;
        var rcMonitor = monitorInfo.rcMonitor;
        var rcWork = rcMonitor;

        var offsetTop = 0;

        switch (position)
        {
            case TaskbarPosition.Top:
                offsetTop += 48;
                break;
            case TaskbarPosition.Bottom:
            case TaskbarPosition.Left:
            case TaskbarPosition.Right:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(position), position, null);
        }
        
        // remove 48 pixels from top of the work area
        rcWork.Top += offsetTop;

        // set new work area
        UnmanagedMethods.SystemParametersInfo(UnmanagedMethods.SPI_SETWORKAREA, 0, (IntPtr)(&rcWork),
            UnmanagedMethods.SPIF_UPDATEINIFILE);
    }

    public void Move(TaskbarPosition direction)
    {
        switch (direction)
        {
            case TaskbarPosition.Top:
                MoveToTop();
                break;
            case TaskbarPosition.Bottom:
            case TaskbarPosition.Left:
            case TaskbarPosition.Right:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    private void MoveToTop()
    {
        var rcMonitor = _monitor.MonitorInfo.rcMonitor;

        UnmanagedMethods.SetWindowPos(Handle, IntPtr.Zero, rcMonitor.Left, 0, rcMonitor.Left, rcMonitor.Right,
            UnmanagedMethods.SWP_NOSIZE | UnmanagedMethods.SWP_NOZORDER | UnmanagedMethods.SWP_NOSENDCHANGING |
            UnmanagedMethods.SWP_NOCOPYBITS);
    }

    private void MoveWindowsIntoScreen()
    {
        UnmanagedMethods.EnumWindows(EnumWindowsCallback, IntPtr.Zero);
    }

    private static bool EnumWindowsCallback(IntPtr hwnd, IntPtr lparam)
    {
        // check if the window is a valid visible window
        if (IsWindowInvalid(hwnd))
        {
            return true;
        }

        var monitorHandle = UnmanagedMethods.MonitorFromWindow(hwnd, UnmanagedMethods.MONITOR_DEFAULTTONEAREST);
        var monitor = DisplayManager.Instance.FromHandle(monitorHandle);
        
        // fix work area (magic numbers for top alignment)
        var rcWork = monitor.MonitorInfo.rcWork;
        rcWork.Top += 40;
        rcWork.Left -= 6;

        UnmanagedMethods.GetWindowRect(hwnd, out var windowRect);

        if (rcWork.Intersects(windowRect))
        {
            return true;
        }

        var x = windowRect.Left;
        var y = windowRect.Top;
        var width = windowRect.Right - windowRect.Left;
        var height = windowRect.Bottom - windowRect.Top;

        if (windowRect.Left < rcWork.Left)
        {
            x = rcWork.Left;
        }

        if (windowRect.Right > rcWork.Right)
        {
            x = rcWork.Right - width;
        }

        if (windowRect.Top < rcWork.Top)
        {
            y = rcWork.Top;
        }

        if (windowRect.Bottom > rcWork.Bottom)
        {
            y = rcWork.Bottom - height;
        }

        UnmanagedMethods.SetWindowPos(hwnd, IntPtr.Zero, x, y, width, height, UnmanagedMethods.SWP_NOSENDCHANGING);
        return true;
    }
        
    
      private static bool IsWindowInvalid(IntPtr hwnd)
        {
            return 
                UnmanagedMethods.IsWindowVisible(hwnd) == false || 
                UnmanagedMethods.GetText(hwnd) == string.Empty ||
                UnmanagedMethods.GetDesktopWindow() == hwnd ||
                UnmanagedMethods.GetShellWindow() == hwnd ||
                UnmanagedMethods.IsIconic(hwnd);
    
        }
}