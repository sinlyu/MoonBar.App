namespace MoonBar.App.WinApi.Structs;

public struct TaskBarInfo
{
    public IntPtr Handle;
    public bool IsPrimary;
    public IntPtr Monitor;
}