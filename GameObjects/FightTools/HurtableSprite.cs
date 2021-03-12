using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameObjects.Managers;
using GameObjects.Screens;

namespace GameObjects.FightTools
{
    public class HurtableSprite : Sprite, IHurtable
    {
        protected int m_score = 0;
        protected eTeam m_selfTeam = eTeam.None;
        protected bool m_isFiniteSprite = true;
        protected Enum m_attackedSound;

        public eHurtInCase HurtInCase { get; protected set; } = eHurtInCase.Attack;

        public int Souls { get; protected set; } = 1;

        public eTeam SelfTeam
        {
            get
            {
                return m_selfTeam;
            }
        }

        public bool IsDead
        {
            get
            {
                return Souls <= 0;
            }
        }

        /// <summary>
        /// Invoke when the hurtable sprite will be killed.
        /// </summary>
        public event Action BeenKilled;

        /// <summary>
        /// Hurtable sprite constructor.
        /// </summary>
        /// <param name="i_GameScreen">The game this hurtable sprite will live at.</param>
        /// <param name="i_TexturePath">The path of the texture which defining this sprite.</param>
        /// <param name="i_Color">The color to cover with the texture.</param>
        /// <param name="i_SelfScore">The self bringing score if it will be dead.</param>
        public HurtableSprite(GameScreen i_GameScreen, string i_TexturePath, Color i_Color, int i_SelfScore)
            : base(i_GameScreen, i_TexturePath, i_Color, null)
        {
            m_score = i_SelfScore;
        }

        /// <summary>
        /// Initialize this HurtableSprite.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            AnimationsManager animationsManager = GameScreen.Services.GetService(typeof(AnimationsManager)) as AnimationsManager;
            animationsManager.InheritAnimation(this, m_animationName);
        }

        /// <summary>
        /// Update this HurtableSprite.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        public override void Update(GameTime i_GameTime)
        {
            base.Update(i_GameTime);
            if (HurtInCase.HasFlag(eHurtInCase.Touch))
            {
                IntersectionManager intersectionManager = GameScreen.Services.GetService(typeof(IntersectionManager)) as IntersectionManager;
                intersectionManager.CheckAndActForIntersection(this);
            }
        }

        /// <summary>
        /// This hurtable sprite got hurt by some Intersectable.
        /// </summary>
        /// <param name="i_Intersectable">The Intersectable that intersects this sprite.</param>
        /// /// <param name="i_IntersectionPoints">Intersection points.</param>
        public void GotHurt(IIntersectable i_Intersectable, List<Vector2> i_IntersectionPoints = null)
        {
            if (i_Intersectable is IAttacker)
            {
                attacked(i_Intersectable as IAttacker);
            }
            else if (i_IntersectionPoints != null)
            {
                touched(i_IntersectionPoints);
            }
        }

        /// <summary>
        /// Act for being attacked.
        /// </summary>
        /// <param name="i_Attacker">Attacker.</param>
        protected virtual void attacked(IAttacker i_Attacker)
        {
            if (i_Attacker.SelfTeam != SelfTeam)
            {
                SoundManager soundManager = Game.Services.GetService(typeof(SoundManager)) as SoundManager;
                soundManager.PlaySoundEffect(m_attackedSound);
                ApplyAnimation();
                i_Attacker.Destroy(m_score);
                Souls--;
                if (IsDead)
                {
                    if (!m_AnimationsLibrary.IsOnRun)
                    {
                        Visible = false;
                    }

                    if (BeenKilled != null)
                    {
                        BeenKilled.Invoke();
                    }

                    if (m_isFiniteSprite && m_AnimationsLibrary == null)
                    {
                        Game.Components.Remove(this);
                    }
                }
            }
        }

        /// <summary>
        /// Act for being touched.
        /// </summary>
        /// <param name="i_IntersectionPoints">Touching points.</param>
        protected virtual void touched(List<Vector2> i_IntersectionPoints)
        {
        }

        /// <summary>
        /// Turning off the animation.
        /// If the object is dead, than erase it from the game.
        /// </summary>
        protected override void turnOffAnimation()
        {
            base.turnOffAnimation();
            if (IsDead && m_isFiniteSprite)
            {
                Game.Components.Remove(this);
            }
        }
    }
}
