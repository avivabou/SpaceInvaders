using System;
using Microsoft.Xna.Framework;

namespace GameObjects.Animations
{
    public class ShrinkAnimation : SpriteAnimation
    {
        /// <summary>
        /// ShrinkAnimation constructor.
        /// </summary>
        /// <param name="i_AnimationName">The name of the animation.</param>
        /// <param name="i_AnimationLength">The length time for this animation.</param>
        public ShrinkAnimation(string i_AnimationName, TimeSpan i_AnimationLength)
            : base(i_AnimationName, i_AnimationLength)
        {
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override object Clone()
        {
            return new ShrinkAnimation(r_AnimationName, Length);
        }

        /// <summary>
        /// Apply the animation update.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        protected override void applyAnimation(GameTime i_GameTime)
        {
            float percentage = (float)(m_timeLeft.TotalSeconds / m_animationLength.TotalSeconds);
            m_sprite.Scale = new Vector2(percentage, percentage);
            m_sprite.Position = m_sprite.CurrentLocation + m_sprite.Origin;
        }
    }
}
