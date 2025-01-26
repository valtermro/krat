namespace Krat;

public enum Win32VirtualKey
{
    Shift = 0x10,
    Control = 0x11,
    Alt = 0x12,

    LeftWin = 0x5B,
    RightWin = 0x5C,
    Escape = 0x1B,

    F1 = 0x70,
    F2 = 0x71,
    F3 = 0x72,
    F4 = 0x73,
    F5 = 0x74,
    F6 = 0x75,
    F7 = 0x76,
    F8 = 0x77,
    F9 = 0x78,
    F10 = 0x79,
    F11 = 0x7A,
    F12 = 0x7B,

    A = 0x41,
    B = 0x42,
    C = 0x43,
    D = 0x44,
    E = 0x45,
    F = 0x46,
    G = 0x47,
    H = 0x48,
    I = 0x49,
    J = 0x4A,
    K = 0x4B,
    L = 0x4C,
    M = 0x4D,
    N = 0x4E,
    O = 0x4F,
    P = 0x50,
    Q = 0x51,
    R = 0x52,
    S = 0x53,
    T = 0x54,
    U = 0x55,
    V = 0x56,
    W = 0x57,
    X = 0x58,
    Y = 0x59,
    Z = 0x5A,
}

internal static class Win32KeyCodeMapper
{
    public static KeyCode Map(Win32VirtualKey code)
    {
        return code switch
        {
            Win32VirtualKey.Escape => KeyCode.F12,

            Win32VirtualKey.F1 => KeyCode.F1,
            Win32VirtualKey.F2 => KeyCode.F2,
            Win32VirtualKey.F3 => KeyCode.F3,
            Win32VirtualKey.F4 => KeyCode.F4,
            Win32VirtualKey.F5 => KeyCode.F5,
            Win32VirtualKey.F6 => KeyCode.F6,
            Win32VirtualKey.F7 => KeyCode.F7,
            Win32VirtualKey.F8 => KeyCode.F8,
            Win32VirtualKey.F9 => KeyCode.F9,
            Win32VirtualKey.F10 => KeyCode.F10,
            Win32VirtualKey.F11 => KeyCode.F11,
            Win32VirtualKey.F12 => KeyCode.F12,

            Win32VirtualKey.A => KeyCode.A,
            Win32VirtualKey.B => KeyCode.B,
            Win32VirtualKey.C => KeyCode.C,
            Win32VirtualKey.D => KeyCode.D,
            Win32VirtualKey.E => KeyCode.E,
            Win32VirtualKey.F => KeyCode.F,
            Win32VirtualKey.G => KeyCode.G,
            Win32VirtualKey.H => KeyCode.H,
            Win32VirtualKey.I => KeyCode.I,
            Win32VirtualKey.J => KeyCode.J,
            Win32VirtualKey.K => KeyCode.K,
            Win32VirtualKey.L => KeyCode.L,
            Win32VirtualKey.M => KeyCode.M,
            Win32VirtualKey.N => KeyCode.N,
            Win32VirtualKey.O => KeyCode.O,
            Win32VirtualKey.P => KeyCode.P,
            Win32VirtualKey.Q => KeyCode.Q,
            Win32VirtualKey.R => KeyCode.R,
            Win32VirtualKey.S => KeyCode.S,
            Win32VirtualKey.T => KeyCode.T,
            Win32VirtualKey.U => KeyCode.U,
            Win32VirtualKey.V => KeyCode.V,
            Win32VirtualKey.W => KeyCode.W,
            Win32VirtualKey.X => KeyCode.X,
            Win32VirtualKey.Y => KeyCode.Y,
            Win32VirtualKey.Z => KeyCode.Z,
            _ => KeyCode.Unknown,
        };
    }
}