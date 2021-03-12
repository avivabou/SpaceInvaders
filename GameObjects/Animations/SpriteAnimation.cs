using System;
using Microsoft.Xna.Framework;

namespace GameObjects.Animations
{
    public abstract class SpriteAnimation : ICloneable
    {
        internal readonly string r_AnimationName;
        protected TimeSpan m_animationLength;
        protected TimeSpan m_timeLeft;
        protected Sprite m_sprite;
        private Vector2? m_originalPosition;
        private Vector2 m_originalOrigin, m_originalScale;
        private float m_originalRotation;

        public bool Enabeled { get; set; }

        public bool IsOnRun { get; protected set; } = false;

        public virtual TimeSpan Length
        {
            get
            {
                return m_animationLength;
            }

            set
            {
                if (!IsOnRun)
                {
                    m_animationLength = value;
                }
            }
        }

        /// <summary>
        /// Invoke when the animation is finished.
        /// </summary>
        public event Action Finished;

        /// <summary>
        /// SpriteAnimation abstract constructor.
        /// </summary>
        /// <param name="i_AnimationName">The name of the animation.</param>
        /// <param name="i_AnimationLength">The length time for this animation.</param>
        protected SpriteAnimation(string i_AnimationName, TimeSpan i_AnimationLength)
        {
            r_AnimationName = i_AnimationName;
            m_animationLength = i_AnimationLength;
            m_timeLeft = TimeSpan.Zero;
            Finished += () => { Enabeled = false; };
        }

        /// <summary>
        /// Start the animation.
        /// </summary>
        public virtual void Start()
        {
            m_timeLeft = m_animationLength;
            IsOnRun = true;
            Enabeled = true;
        }

        /// <summary>
        /// Pause the animation.
        /// </summary>
        public virtual void Pause()
        {
            IsOnRun = false;
        }

        /// <summary>
        /// Update the animation.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        public void Update(GameTime i_GameTime)
        {
            if (IsOnRun)
            {
                m_timeLeft -= i_GameTime.ElapsedGameTime;
                if ((m_timeLeft > TimeSpan.Zero) || (m_animationLength == TimeSpan.Zero))
                {
                    applyAnimation(i_GameTime);
                }
                else
                {
                    Pause();
                    if (Finished != null)
                    {
                        Finished.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Reset the animation.
        /// </summary>
        public virtual void Reset()
        {
            Pause();
            m_timeLeft = m_animationLength;
            m_sprite.Origin = m_originalOrigin;
            m_sprite.Position = m_originalPosition;
            m_sprite.Rotation = m_originalRotation;
            m_sprite.Scale = m_originalScale;
        }

        /// <summary>
        /// Initialize the animation.
        /// </summary>
        /// <param name="i_Sprite">The sprite this animation will work on.</param>
        public virtual void Initialize(Sprite i_Sprite)
        {
            m_sprite = i_Sprite;
            m_originalOrigin = i_Sprite.Origin;
            m_originalPosition = i_Sprite.Position; 
            m_originalRotation = i_Sprite.Rotation;
            m_originalScale = i_Sprite.Scale;
        }

        /// <summary>
        /// Abstract method for cloning SpriteAnimation object.
        /// </summary>
        /// <returns>A clone of the SpriteAnimation object.</returns>
        public abstract object Clone();

        /// <summary>
        /// Abstract method for applying the animation update.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        protected abstract void applyAnimation(GameTime i_GameTime);
    }
}
