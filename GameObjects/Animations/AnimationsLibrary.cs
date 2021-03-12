using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameObjects.Animations
{
    public class AnimationsLibrary : SpriteAnimation
    {
        private Dictionary<string, SpriteAnimation> m_spriteAnimations = new Dictionary<string, SpriteAnimation>();

        public override TimeSpan Length
        {
            set
            {
                if (!IsOnRun)
                {
                    m_animationLength = value;
                    foreach (string animationName in m_spriteAnimations.Keys)
                    {
                        m_spriteAnimations[animationName].Length = value;
                    }
                }
            }
        }

        /// <summary>
        /// AnimationsLibrary constructor.
        /// </summary>
        public AnimationsLibrary()
            : base("Animations Library", TimeSpan.Zero)
        {
        }

        /// <summary>
        /// AnimationsLibrary consturctor.
        /// </summary>
        /// <param name="i_AnimationName">The name of the animation.</param>
        /// <param name="i_AnimationLength">The length time for this animation.</param>
        public AnimationsLibrary(string i_AnimationName, TimeSpan i_AnimationLength)
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
            foreach (string animationName in m_spriteAnimations.Keys)
            {
                m_spriteAnimations[animationName].Initialize(i_Sprite);
            }
        }

        /// <summary>
        /// Initialize a specific animation from the library.
        /// </summary>
        /// <param name="i_Sprite">The sprite this animation will work on.</param>
        /// <param name="i_AnimationName">The specific animation name.</param>
        public void Initialize(Sprite i_Sprite, string i_AnimationName)
        {
            base.Initialize(i_Sprite);
            m_spriteAnimations[i_AnimationName].Initialize(i_Sprite);
        }

        /// <summary>
        /// Start run the animation.
        /// </summary>
        public override void Start()
        {
            foreach (string animationName in m_spriteAnimations.Keys)
            {
                m_spriteAnimations[animationName].Start();
            }

            base.Start();
        }

        /// <summary>
        /// Pause the animation.
        /// </summary>
        public override void Pause()
        {
            foreach (string animationName in m_spriteAnimations.Keys)
            {
                m_spriteAnimations[animationName].Pause();
            }

            base.Pause();
        }

        /// <summary>
        /// Reset the animation.
        /// </summary>
        public override void Reset()
        {
            foreach (string animationName in m_spriteAnimations.Keys)
            {
                m_spriteAnimations[animationName].Reset();
            }

            base.Reset();
        }

        /// <summary>
        /// Get a specific animation from the library.
        /// </summary>
        /// <param name="i_Name">The name of the animation to export.</param>
        /// <returns></returns>
        public SpriteAnimation GetAnimation(string i_Name)
        {
            SpriteAnimation spriteAnimation = null;
            if (m_spriteAnimations.ContainsKey(i_Name))
            {
                spriteAnimation = m_spriteAnimations[i_Name];
            }

            return spriteAnimation;
        }

        /// <summary>
        /// Add animation to the library.
        /// </summary>
        /// <param name="i_SpriteAnimations">The animation which will be added.</param>
        public void AddAnimation(params SpriteAnimation[] i_SpriteAnimations)
        {
            foreach (SpriteAnimation animation in i_SpriteAnimations)
            {
                if (m_spriteAnimations.ContainsKey(animation.r_AnimationName))
                {
                    m_spriteAnimations.Remove(animation.r_AnimationName);
                }

                m_spriteAnimations.Add(animation.r_AnimationName, animation);
            }
        }

        /// <summary>
        /// Clear all of the animations that in the library.
        /// </summary>
        public void Clear()
        {
            m_spriteAnimations.Clear();
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override object Clone()
        {
            AnimationsLibrary combintaion = new AnimationsLibrary(r_AnimationName, Length);
            foreach (string animationName in m_spriteAnimations.Keys)
            {
                combintaion.AddAnimation(m_spriteAnimations[animationName].Clone() as SpriteAnimation);
            }

            return combintaion;
        }

        /// <summary>
        /// Apply the animation update.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        protected sealed override void applyAnimation(GameTime i_GameTime)
        {
            string[] keys = new string[m_spriteAnimations.Keys.Count];
            m_spriteAnimations.Keys.CopyTo(keys, 0);
            for (int i = 0; i < keys.Length; i++)
            {
                string animationName = keys[i];
                IsOnRun |= m_spriteAnimations[animationName].IsOnRun;
                if (m_spriteAnimations[animationName].Enabeled)
                {
                    m_spriteAnimations[animationName].Update(i_GameTime);
                }
            }
        }
    }
}
