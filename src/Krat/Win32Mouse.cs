using System.Runtime.InteropServices;

namespace Krat;

[StructLayout(LayoutKind.Sequential)]
public record struct MousePoint(int X, int Y);

public sealed class Win32Mouse
{
    [DllImport("User32.Dll")]
    public static extern long SetCursorPos(int x, int y);

    // [DllImport("User32.Dll")]
    // public static extern bool ClientToScreen(IntPtr hWnd, ref MousePoint mousePoint);

}