using System;

namespace GameObjects.FightTools
{
    [Flags]
    public enum eHurtInCase
    {
        None = 0,
        Touch = 1,
        Attack = 2, 
    }
}
