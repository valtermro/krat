namespace Krat;

public enum KeyEventKind
{
    Unknown,
    KeyDown,
    KeyUp,
    // SysKeyDown,
    // SysKeyUp
}

public enum KeyCode
{
    Unknown,

    A = 0x61,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z,

    Escape,
    F1,
    F2,
    F3,
    F4,
    F5,
    F6,
    F7,
    F8,
    F9,
    F10,
    F11,
    F12,
}

public static class KeyModifier
{
    public const byte Alt = 0x01;
    public const byte Shift = 0x02;
    public const byte Control = 0x04;
    public const byte Super = 0x08;
}

public readonly record struct KeyEvent(KeyEventKind Kind, KeyCode Key, byte Mods)
{
    public bool HasMod(byte mod)
    {
        return (Mods & mod) != 0;
    }

    public char KeyChar()
    {
        return (char)(Key);
    }

    public bool IsAlphaKey()
    {
        return Key is >= KeyCode.A and <= KeyCode.Z;
    }
}