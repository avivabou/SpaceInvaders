using System;
using Microsoft.Xna.Framework;

namespace GameObjects.Animations
{
    public class BlinkAnimation : SpriteAnimation
    {
        private float m_blinksForSecond = 0;
        private TimeSpan m_lastTimeChanged;
        private bool m_finishVisibility;

        /// <summary>
        /// BlinkAnimation constructor.
        /// </summary>
        /// <param name="i_AnimationName">The name of the animation.</param>
        /// <param name="i_AnimationLength">The length time for this animation.</param>
        /// <param name="i_BlinksForSecond">Amount of blinks in one second.</param>
        /// <param name="i_FinishVisibility">Last state of visibility.</param>
        public BlinkAnimation(string i_AnimationName, TimeSpan i_AnimationLength, float i_BlinksForSecond, bool i_FinishVisibility = false)
            : base(i_AnimationName, i_AnimationLength)
        {
            m_blinksForSecond = i_BlinksForSecond;
            m_finishVisibility = i_FinishVisibility;
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override object Clone()
        {
            return new BlinkAnimation(r_AnimationName, Length, m_blinksForSecond, m_finishVisibility);
        }

        /// <summary>
        /// Pause the animation.
        /// </summary>
        public override void Pause()
        {
            base.Pause();
            m_sprite.Visible = m_finishVisibility;
        }

        /// <summary>
        /// Apply the animation update.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        protected override void applyAnimation(GameTime i_GameTime)
        {
            if (i_GameTime.TotalGameTime.TotalSeconds - m_lastTimeChanged.TotalSeconds >= 1 / m_blinksForSecond)
            {
                m_lastTimeChanged = i_GameTime.TotalGameTime;
                m_sprite.Visible = !m_sprite.Visible;
            }
        }
    }
}
