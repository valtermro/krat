using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Krat;

public sealed class Win32Keyboard
{
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private const int WH_KEYBOARD_LL = 13;
    //private const int KEYEVENTF_KEYUP = 2;

    private static IntPtr _hookId = IntPtr.Zero;
    private static readonly IntPtr PreventDefault = (IntPtr)1;

    private readonly LowLevelKeyboardProc _hookCallback;
    private Func<KeyEvent, bool>? _onKeyInput = null;

    public Win32Keyboard()
    {
        _hookCallback = HookCallback;
    }

    public void Initialize(Func<KeyEvent, bool> onKeyInput)
    {
        if (_hookId != IntPtr.Zero)
            // TODO: Custom Exception
            throw new Exception($"{nameof(Win32Keyboard)} already hooked.");

        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;

        _onKeyInput = onKeyInput;
        _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _hookCallback, GetModuleHandle(curModule!.ModuleName), 0);
    }

    public void Shutdown()
    {
        UnhookWindowsHookEx(_hookId);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        const int eventKeyDown = 0x0100;
        const int eventKeyUp = 0x0101;
        //const int eventSysKeyDown = 0x0104;
        //const int eventSysKeyUp = 0x0105;

        var virtualKey = (Win32VirtualKey)Marshal.ReadInt32(lParam);
        var key = Win32KeyCodeMapper.Map(virtualKey);
        var kind = (int)wParam;

        // TODO: Tratar ALT+{KEY}. Isso é um `SysKeyDown` e quebra toda a lógica implementada aqui.

        var eventType = kind switch
        {
            eventKeyDown => KeyEventKind.KeyDown,
            eventKeyUp => KeyEventKind.KeyUp,
            _ => KeyEventKind.Unknown,
        };

        if (key != KeyCode.Unknown && eventType != KeyEventKind.Unknown)
        {
            var mods = ResolveKeyModifiers();

            var inputEvent = new KeyEvent(eventType, key, mods);
            if (_onKeyInput?.Invoke(inputEvent) == false)
                return PreventDefault;
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private static byte ResolveKeyModifiers()
    {
        byte result = 0;

        if (Has(Win32VirtualKey.Control))
            result |= KeyModifier.Control;

        if (Has(Win32VirtualKey.Shift))
            result |= KeyModifier.Shift;

        if (Has(Win32VirtualKey.Alt))
            result |= KeyModifier.Alt;

        if (Has(Win32VirtualKey.LeftWin) || Has(Win32VirtualKey.RightWin))
            result |= KeyModifier.Super;

        return result;

        static bool Has(Win32VirtualKey vk) => (GetAsyncKeyState((int)vk) & 0x8000) > 0;
    }

    #region dllImport

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(uint idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("User32.dll")]
    public static extern short GetAsyncKeyState(int nCode);

    #endregion
}