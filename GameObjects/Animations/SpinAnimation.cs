using System;
using Microsoft.Xna.Framework;

namespace GameObjects.Animations
{
    public class SpinAnimation : SpriteAnimation
    {
        private float m_roundsInASecond;
        private Vector2 m_origin;

        /// <summary>
        /// SpinAnimation constructor.
        /// </summary>
        /// <param name="i_AnimationName">The name of the animation.</param>
        /// <param name="i_AnimationLength">The length time for this animation.</param>
        /// <param name="i_RoundsInASecond">Amount of spins for a one second.</param>
        /// <param name="i_Origin">The origin of the texture.</param>
        public SpinAnimation(string i_AnimationName, TimeSpan i_AnimationLength, float i_RoundsInASecond, Vector2 i_Origin)
            : base(i_AnimationName, i_AnimationLength)
        {
            m_roundsInASecond = i_RoundsInASecond;
            m_origin = i_Origin;
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override object Clone()
        {
            return new SpinAnimation(r_AnimationName, Length, m_roundsInASecond, m_origin);
        }

        /// <summary>
        /// Apply the animation update.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        protected override void applyAnimation(GameTime i_GameTime)
        {
            m_sprite.Origin = m_origin;
            float timePassed = (float)(m_animationLength - m_timeLeft).TotalSeconds;
            m_sprite.Rotation = timePassed * m_roundsInASecond * MathHelper.TwoPi;
            m_sprite.Position = m_sprite.CurrentLocation + m_sprite.Origin;
        }
    }
}
