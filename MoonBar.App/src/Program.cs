using System.Diagnostics;
using Lunar;
using MoonBar.App.WinAPI;
using MoonBar.App.WinAPI.Display;


var displayMonitors = DisplayMonitor.GetDisplayMonitors();
var taskBarInfos = Taskbar.GetTaskbars();

// Get explorer pid which has the taskbar
User32.GetWindowThreadProcessId(taskBarInfos[0].Handle, out var pid);
var process = Process.GetProcessById((int)pid);

// inject dll into explorer
var bytes = File.ReadAllBytes("MoonBar.ExplorerPatch.dll");
var mapper = new LibraryMapper(process, bytes);
mapper.MapLibrary();

// update work area and task bar position
Taskbar.UpdateMonitorWorkingAreas(displayMonitors);
Taskbar.MoveWindowsIntoValidScreenSpace(displayMonitors);
Taskbar.MoveTaskbarsToTheTop(taskBarInfos, displayMonitors);
