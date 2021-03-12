using System;
using Microsoft.Xna.Framework;
using GameObjects.FightTools;
using GameObjects.Screens;

namespace Invaders
{
    public class Mothership : HurtableSprite
    {
        public static readonly float sr_MovementSpeed = 95;
        private static readonly int sr_mothershipScore = 600;
        private static readonly string sr_texturePath = @"Sprites\MotherShip_32x120";
        private static readonly int sr_mothershipMaxRandomSeconds = 10;
        private static readonly int sr_mothershipMinSeconds = 5;
        private bool m_firstSpawn = true;
        private Random m_random = new Random();
        private TimeSpan m_nextMothershipSpawn;
        private TimeSpan m_lastMothershipSpawn;

        /// <summary>
        /// Mothership constructor.
        /// </summary>
        /// <param name="i_GameScreen">The game this mothership will live at.</param>
        public Mothership(GameScreen i_GameScreen)
            : base(i_GameScreen, sr_texturePath, Color.Red, sr_mothershipScore)
        {
            BeenKilled += updateNextMothershipSpawn;
            Visible = false;
            m_animationName = "Mothership Attacked";
            m_selfTeam = eTeam.Enemy;
            m_isFiniteSprite = false;
            m_AnimationsLibrary.Finished += () => { Visible = false; };
            m_attackedSound = eSounds.MotherShipKill;
        }

        /// <summary>
        /// Intialize the components of this mothership.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            m_selfLocation = new Vector2(0, m_SelfTexture.Bounds.Height);
        }

        /// <summary>
        /// Update the mothership.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        public override void Update(GameTime i_GameTime)
        {
            if (Enabled)
            {
                base.Update(i_GameTime);
                if (m_firstSpawn)
                {
                    m_firstSpawn = false;
                    m_lastMothershipSpawn = i_GameTime.TotalGameTime;
                    updateNextMothershipSpawn();
                }

                if (!m_AnimationsLibrary.IsOnRun && Visible)
                {
                    m_selfLocation.X += (float)i_GameTime.ElapsedGameTime.TotalSeconds * sr_MovementSpeed;
                    if (m_selfLocation.X >= Game.Window.ClientBounds.Width)
                    {
                        Souls--;
                        Visible = false;
                        updateNextMothershipSpawn();
                    }

                    m_lastMothershipSpawn = TimeSpan.FromSeconds(i_GameTime.TotalGameTime.TotalSeconds);
                }
                else if (i_GameTime.TotalGameTime.TotalSeconds - m_nextMothershipSpawn.TotalSeconds > 0)
                {
                    respawn();
                }
            }
        }

        /// <summary>
        /// Turning off the animation.
        /// </summary>
        protected override void turnOffAnimation()
        {
            base.turnOffAnimation();
            Visible = false;
        }

        /// <summary>
        /// Manage the spawning of the mothership. 
        /// </summary>
        private void updateNextMothershipSpawn()
        {
            int seconds = (int)(sr_mothershipMinSeconds + m_random.Next(sr_mothershipMaxRandomSeconds) + m_lastMothershipSpawn.TotalSeconds);
            m_nextMothershipSpawn = TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Respawn a mothership.
        /// </summary>
        private void respawn()
        {
            Visible = true;
            m_selfLocation.X = 0;
            Souls = 1;
        }
    }
}
