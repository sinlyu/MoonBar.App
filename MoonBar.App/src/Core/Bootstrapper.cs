using System.Diagnostics;
using Ardalis.GuardClauses;
using Lunar;
using MoonBar.App.Display;
using MoonBar.App.Guards;
using MoonBar.App.WinApi;
using MoonBar.App.WinApi.Interop;

namespace MoonBar.App.Core;

internal sealed class Bootstrapper
{
    private IEnumerable<DisplayMonitor> _monitors;
    private IEnumerable<Taskbar> _taskbars;

    public void Run()
    {
        _monitors = Guard.Against.NullOrEmpty(DisplayManager.Instance.DisplayMonitors, nameof(_monitors));
        _taskbars = Guard.Against.NullOrEmpty(TaskbarManager.Instance.Taskbars, nameof(_taskbars));

        InjectDll();
        UpdateWorkingAreas();
        MoveTaskbars();
    }

    private void InjectDll()
    {
        const string dllName = "MoonBar.ExplorerPatch.dll";
        Guard.Against.FileExists(dllName, nameof(dllName));

        // Get explorer pid from primary taskbar handle
        var primaryTaskbar = _taskbars.First();
        UnmanagedMethods.GetWindowThreadProcessId(primaryTaskbar.Handle, out var pid);
        var process = Process.GetProcessById((int)pid);

        // inject dll into explorer
        var bytes = File.ReadAllBytes(dllName);
        var mapper = new LibraryMapper(process, bytes);
        mapper.MapLibrary();
    }

    private void UpdateWorkingAreas()
    {
        const TaskbarPosition position = TaskbarPosition.Top;
        foreach (var taskbar in _taskbars)
        {
            taskbar.UpdateWorkingArea(position);
        }
    }

    private void MoveTaskbars()
    {
        const TaskbarPosition position = TaskbarPosition.Top;
        foreach (var taskbar in _taskbars)
        {
            taskbar.Move(position);
        }
    }
}