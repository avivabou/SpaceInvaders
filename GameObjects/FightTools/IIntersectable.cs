using Microsoft.Xna.Framework;

namespace GameObjects.FightTools
{
    public interface IIntersectable
    {
        Rectangle ShapeRectangle { get; }

        Color[] ObjectColors { get; }
    }
}
