using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameObjects.FightTools
{
    public interface IHurtable : ITeamableShape
    {
        int Souls { get; }

        eHurtInCase HurtInCase { get; }

        void GotHurt(IIntersectable i_Intersectable, List<Vector2> i_IntersectionPoints = null);
    }
}
