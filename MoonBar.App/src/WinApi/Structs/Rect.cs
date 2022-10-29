using System.Runtime.InteropServices;

namespace MoonBar.App.WinApi.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct Rect
{
   public int Left, Top, Right, Bottom;

   public Rect(int left, int top, int right, int bottom)
   {
       Left = left;
       Top = top;
       Right = right;
       Bottom = bottom;
   }

   public bool Intersects(Rect other)
   {
       return Left >= other.Left && Right <= other.Right && Top >= other.Top && 
              Bottom <= other.Bottom;
   }
}