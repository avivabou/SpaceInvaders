using System;
using System.Collections.Generic;
using GameObjects.Animations;

namespace GameObjects.Managers
{
    public class AnimationsManager
    {
        private Dictionary<string, SpriteAnimation> m_spriteAnimationsDictionary = new Dictionary<string, SpriteAnimation>();

        /// <summary>
        /// AnimationsManager constructor.
        /// </summary>
        public AnimationsManager()
        {
        }

        /// <summary>
        /// Add the animation to the manager.
        /// </summary>
        /// <param name="i_SpriteAnimation">The animation.</param>
        /// <param name="i_Override">Should it override an animation with the same name?</param>
        public void AddAnimationToManager(SpriteAnimation i_SpriteAnimation, bool i_Override = true)
        {
            if (i_Override && m_spriteAnimationsDictionary.ContainsKey(i_SpriteAnimation.r_AnimationName))
            {
                m_spriteAnimationsDictionary[i_SpriteAnimation.r_AnimationName] = i_SpriteAnimation;
            }
            else
            {
                m_spriteAnimationsDictionary.Add(i_SpriteAnimation.r_AnimationName, i_SpriteAnimation);
            }
        }

        /// <summary>
        /// Inherit the animation into the given sprite.
        /// </summary>
        /// <param name="i_Sprite">The sprite that would inherit the animation.</param>
        /// <param name="i_AnimationName">The animation that we want to inherit.</param>
        /// <param name="i_TimeSpan">A new time span for the animation.</param>
        /// <param name="i_Override">Shuld it overrid an animation with the same name?</param>
        public void InheritAnimation(Sprite i_Sprite, string i_AnimationName, bool i_Override = false, TimeSpan? i_TimeSpan = null)
        {
            if (m_spriteAnimationsDictionary.ContainsKey(i_AnimationName))
            {
                if (i_Override)
                {
                    i_Sprite.m_AnimationsLibrary.Clear();
                }

                SpriteAnimation animation = m_spriteAnimationsDictionary[i_AnimationName].Clone() as SpriteAnimation;
                if (i_TimeSpan.HasValue && (!i_TimeSpan.Equals(animation.Length)))
                {
                    animation.Length = (TimeSpan)i_TimeSpan;
                }

                if (i_Sprite.m_AnimationsLibrary == null)
                {
                    i_Sprite.m_AnimationsLibrary = new AnimationsLibrary();
                }

                i_Sprite.m_AnimationsLibrary.AddAnimation(animation);
                i_Sprite.m_AnimationsLibrary.Initialize(i_Sprite, i_AnimationName);
            }
        }
    }
}