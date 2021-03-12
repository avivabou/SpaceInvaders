using System;
using Microsoft.Xna.Framework;

namespace GameObjects.Animations
{
    public class FadeOutAnimation : SpriteAnimation
    {
        private float m_tint;

        /// <summary>
        /// FadeOutAnimation constructor.
        /// </summary>
        /// <param name="i_AnimationName">The name of the animation.</param>
        /// <param name="i_AnimationLength">The length time for this animation.</param>
        public FadeOutAnimation(string i_AnimationName, TimeSpan i_AnimationLength)
            : base(i_AnimationName, i_AnimationLength)
        {
        }

        /// <summary>
        /// Initialize the animation.
        /// </summary>
        /// <param name="i_Sprite">The sprite this animation will work on.</param>
        public override void Initialize(Sprite i_Sprite)
        {
            base.Initialize(i_Sprite);
            m_tint = i_Sprite.Tint;
        }

        /// <summary>
        /// Pause the animation.
        /// </summary>
        public override void Pause()
        {
            base.Pause();
            m_sprite.Tint = m_tint;
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override object Clone()
        {
            return new FadeOutAnimation(r_AnimationName, m_animationLength);
        }

        /// <summary>
        /// Apply the animation update.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        protected override void applyAnimation(GameTime i_GameTime)
        {
            m_sprite.Tint = (float)(m_timeLeft.TotalSeconds / m_animationLength.TotalSeconds);
        }
    }
}
