using MoonBar.App.Core;
using MoonBar.App.WinApi.Interop;
using MoonBar.App.WinApi.Structs;

namespace MoonBar.App.WinApi;

public sealed class TaskbarManager : Singleton<TaskbarManager>
{
    public IList<Taskbar> Taskbars { get; }
    public TaskbarManager()
    {
        Taskbars = new List<Taskbar>();
        RefreshTaskbarList();
    }

    private void RefreshTaskbarList()
    {
        Taskbars.Clear();   
        UnmanagedMethods.EnumWindows(EnumerateWindowCallback, IntPtr.Zero);
    }
    
    private bool EnumerateWindowCallback(IntPtr wnd, IntPtr param)
    {
        var isTaskbar = UnmanagedMethods.FindWindowEx(wnd, IntPtr.Zero, "Start", null);
        var isPrimaryTaskbar = UnmanagedMethods.FindWindowEx(wnd, IntPtr.Zero, "RebarWindow32", null);

        if (isTaskbar.Equals(IntPtr.Zero)) return true;

        var taskbarInfo = new TaskBarInfo()
        {
            Handle = wnd,
            IsPrimary = !isPrimaryTaskbar.Equals(IntPtr.Zero),
            Monitor = UnmanagedMethods.MonitorFromWindow(wnd, UnmanagedMethods.MONITOR_DEFAULTTONEAREST)
        };
        
        var taskbar = new Taskbar(taskbarInfo);
        Taskbars.Add(taskbar);

        return true;
    }
}