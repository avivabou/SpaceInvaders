using System;

namespace GameObjects.Screens
{
    [Flags]
    public enum eScreenMode
    {
        Inactive = 0b00,
        Visible = 0b01,        
        Running = 0b10,
        Active = 0b11
    }
}
