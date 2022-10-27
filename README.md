# MoonBar.App

[![](https://img.shields.io/badge/MoonBar_Is_Not_Working_Super_Well_Right_Now-red?style=for-the-badge)](#)

MoonBar moves the windows 11 taskbar to the top of your screen.

Since the last windows 11 update it is not possible anymore to move your taskbar to the top of your screen.
But since i am used to that, i created this little utility that moves the taskbar to the top.

How does it work:

- First we find the process that has the taskbar windows (explorer.exe)
- We inject MoonBar.ExplorerPatch into it.
- MoonBar.ExplorerPatch hooks the WndProc of the process and we skip handling *WM_WINDOWPOSCHANGED*, *WM_NCCALCSIZE*
- That removes flickering from SetWindowPos and also means we only have to set the taskbar position once instead of n amount of times every second to keep it there
- The main app ( this one ) then calculates the working area for each display and moves the taskbar via SetWindowPos
- Thats it!


missing features / bugs:

- No auto start implemented yet
- If the explorer process gets restarted, your taskbar will move down
- Child windows of the taskbar have the wrong screen position
- Icon tray acts weird
- right clicking the task bar crashes explorer
