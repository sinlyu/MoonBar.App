using MoonBar.App.WinAPI.Display;
using MoonBar.App.WinAPI.Structs;

namespace MoonBar.App.WinAPI;

public static class Taskbar
{

    public static List<TaskBarInfo> GetTaskbars()
    {
        var _taskBarWindows = new List<TaskBarInfo>();
        User32.EnumWindows(EnumerateWindow, IntPtr.Zero);

        bool EnumerateWindow(IntPtr wnd, IntPtr param)
        {
            var isTaskbar = User32.FindWindowEx(wnd, IntPtr.Zero, "Start", null);
            var isPrimaryTaskbar = User32.FindWindowEx(wnd, IntPtr.Zero, "RebarWindow32", null);

            if (isTaskbar.Equals(IntPtr.Zero)) return true;

            var taskbarWnd = new TaskBarInfo()
            {
                Handle = wnd,
                IsPrimary = !isPrimaryTaskbar.Equals(IntPtr.Zero),
                Monitor = User32.MonitorFromWindow(wnd, User32.MONITOR_DEFAULTTONEAREST)
            };

            _taskBarWindows.Add(taskbarWnd);

            return true;
        }

        return _taskBarWindows;
    }


    public static void UpdateMonitorWorkingAreas(IReadOnlyList<DisplayMonitor> displayMonitors)
    {
        unsafe
        {
            for (var i = 0; i < displayMonitors.Count; i++)
            {
                var monitor = displayMonitors[i];

                var rcMonitor = monitor.MonitorInfo.rcMonitor;
                var rcWork = monitor.MonitorInfo.rcWork;

                // reset work area to use full monitor
                rcWork.Bottom = rcMonitor.Bottom;
                rcWork.Left = rcMonitor.Left;
                rcWork.Right = rcMonitor.Right;
                rcWork.Top = rcMonitor.Top;

                // remove 48 pixels from top of work area
                rcWork.Top += 48;
                //rcWork.Bottom;

                // create ptr
                var rcWorkPtr = (IntPtr)(&rcWork);

                // set work area
                User32.SystemParametersInfo(User32.SPI_SETWORKAREA, 0, rcWorkPtr, User32.SPIF_UPDATEINIFILE);
            }
        }
    }


    public static void MoveWindowsIntoValidScreenSpace(IReadOnlyList<DisplayMonitor> displayMonitors)
    {
        // update each window
        User32.EnumWindows(delegate(IntPtr wnd, IntPtr param)
        {
            // skip if window is not visible or if the window title is an empty string   
            if (!User32.IsWindowVisible(wnd) && User32.GetText(wnd) != "")
            {
                return true;
            }
            
            // skip if window is desktop window
            if (wnd == User32.GetDesktopWindow())
            {
                return true;
            }

            // skip if the specified window is minimized
            if (User32.IsIconic(wnd))
            {
                return true;
            }

            // skip if the specified window is a desktop window
            if (wnd == User32.GetShellWindow())
            {
                return true;
            }

            // get monitor handle from window
            var hMonitor = User32.MonitorFromWindow(wnd, User32.MONITOR_DEFAULTTONEAREST);

            // find display monitor
            var displayMonitor = displayMonitors.FirstOrDefault(monitor => monitor.Handle == hMonitor);

            // calculate right rcWork
            var rcWork = displayMonitor.MonitorInfo.rcWork;
            rcWork.Top += 40;
            rcWork.Left -= 6;

            User32.GetWindowRect(wnd, out var rect);

            // check if window is in work area
            if (rect.Left >= rcWork.Left && rect.Right <= rcWork.Right && rect.Top >= rcWork.Top &&
                rect.Bottom <= rcWork.Bottom)
            {
                return true;
            }

            // move window into work area
            var x = rect.Left;
            var y = rect.Top;
            var width = rect.Right - rect.Left;
            var height = rect.Bottom - rect.Top;

            // check if window is outside of work area
            if (rect.Left < rcWork.Left)
            {
                x = rcWork.Left;
            }
            else if (rect.Right > rcWork.Right)
            {
                x = rcWork.Right - width;
            }

            if (rect.Top < rcWork.Top)
            {
                y = rcWork.Top;
            }
            else if (rect.Bottom > rcWork.Bottom)
            {
                y = rcWork.Bottom - height;
            }

            User32.SetWindowPos(wnd, IntPtr.Zero, x, y, width, height, User32.SWP_NOSENDCHANGING);
            User32.UpdateWindow(wnd);

            return true;
        }, IntPtr.Zero);
    }

    public static void MoveTaskbarsToTheTop(IEnumerable<TaskBarInfo> taskBarInfos, IReadOnlyList<DisplayMonitor> displayMonitors)
    {
        foreach (var taskBarInfo in taskBarInfos)
        {
            // find displayMonitor for taskbar
            var displayMonitor = displayMonitors.FirstOrDefault(monitor => monitor.Handle.Equals(taskBarInfo.Monitor));
            var rcMonitor = displayMonitor.MonitorInfo.rcMonitor;

            User32.SetWindowPos(taskBarInfo.Handle, IntPtr.Zero, rcMonitor.Left, 0, rcMonitor.Left, rcMonitor.Right,
                User32.SWP_NOSIZE | User32.SWP_NOZORDER | User32.SWP_NOSENDCHANGING | User32.SWP_NOCOPYBITS);
        }
    }
}