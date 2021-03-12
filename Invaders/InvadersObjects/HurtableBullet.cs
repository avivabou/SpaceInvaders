using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameObjects.FightTools;
using GameObjects.Screens;

namespace Invaders
{
    public class HurtableBullet : Bullet, IHurtable
    {
        public eHurtInCase HurtInCase { get; } = eHurtInCase.Attack | eHurtInCase.Touch;

        public int Souls { get; } = 1;

        /// <summary>
        /// HurtableBullet constructor.
        /// </summary>
        /// <param name="i_GameScreen">The game this object will live at.</param>
        /// <param name="i_StartLocation">Start location.</param>
        /// <param name="i_MaxY">Max Y cordinate to arrive.</param>
        public HurtableBullet(GameScreen i_GameScreen, Vector2 i_StartLocation, float i_MaxY = 0)
            : base(i_GameScreen, i_StartLocation, i_MaxY)
        {
        }

        /// <summary>
        /// When this HurtableBullet got hurt it will be destroyed.
        /// </summary>
        /// <param name="i_Intersectable">The IIntersectable that hurt this HurtableBullet.</param>
        /// <param name="i_IntersectionPoints">All of the intersection points.</param>
        public void GotHurt(IIntersectable i_Intersectable, List<Vector2> i_IntersectionPoints)
        {
            Destroy(0);
        }
    }
}
