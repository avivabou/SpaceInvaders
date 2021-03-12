using System;
using Microsoft.Xna.Framework;
using GameObjects;
using GameObjects.FightTools;
using GameObjects.Managers;
using GameObjects.Screens;

namespace Invaders
{
    public class Bullet : Sprite, IAttacker
    {
        public static readonly TimeSpan sr_ShotTimeInterval = TimeSpan.FromMilliseconds(500);
        private static readonly float sr_movementSpeed = 140;
        private static readonly float sr_chanceToBeDestroyedByBullet = 70;
        private static readonly string sr_texturePath = @"Sprites\Bullet";
        private float m_maxY;
        private Random m_random = new Random();

        public eTeam SelfTeam { get; private set; }

        /// <summary>
        /// Invoke when the bullet arrived to any destenition.
        /// </summary>
        public event Action<IAttacker> ArrivedToEnd;

        /// <summary>
        /// Invoke when the bullet attacked and should change the score.
        /// </summary>
        public event Action<int> GetScore;

        /// <summary>
        /// Bullet constructor.
        /// </summary>
        /// <param name="i_GameScreen">The game the bullet will live at.</param>
        /// <param name="i_StartLocation">Start location.</param>
        /// <param name="i_MaxY">Max Y for the bullet can be arrived.</param>
        public Bullet(GameScreen i_GameScreen, Vector2 i_StartLocation, float i_MaxY = 0)
            : base(i_GameScreen, sr_texturePath, i_MaxY == 0 ? Color.Red : Color.Blue, null)
        {
            m_maxY = i_MaxY;
            m_selfLocation = i_StartLocation;
            m_selfLocation.X += Enemy.sr_EnemySize / 2;
            if (m_maxY == 0)
            {
                SelfTeam = eTeam.Player;
            }
            else
            {
                SelfTeam = eTeam.Enemy;
            }
        }

        /// <summary>
        /// Update the bullet on game space.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        public override void Update(GameTime i_GameTime)
        {
            if (SelfTeam == eTeam.Player)
            {
                m_selfLocation.Y -= (float)i_GameTime.ElapsedGameTime.TotalSeconds * sr_movementSpeed;
                if (m_selfLocation.Y <= 0)
                {
                    Destroy(0);
                }
            }
            else
            {
                m_selfLocation.Y += (float)i_GameTime.ElapsedGameTime.TotalSeconds * sr_movementSpeed;
                if (m_selfLocation.Y >= Game.Window.ClientBounds.Height)
                {
                    Destroy(0);
                }
            }

            IntersectionManager intersectionManager = GameScreen.Services.GetService(typeof(IntersectionManager)) as IntersectionManager;
            intersectionManager.CheckAndActForIntersection(this);
        }

        /// <summary>
        /// Destroy the bullet and bring score.
        /// </summary>
        /// <param name="i_Score">The score should be got cause the destrosion.</param>
        public void Destroy(int i_Score)
        {
            if (ArrivedToEnd != null)
            {
                ArrivedToEnd.Invoke(this);
            }

            if (GetScore != null)
            {
                GetScore.Invoke(i_Score);
            }

            Game.Components.Remove(this);
            Dispose();
        }

        /// <summary>
        /// Kill the IHurtable which intersected with this bullet.
        /// </summary>
        /// <param name="i_Hurtable">IHurtable which intersected with this bullet.</param>
        public void Kill(IHurtable i_Hurtable)
        {
            i_Hurtable.GotHurt(this);
            if ((i_Hurtable is HurtableBullet) && (m_random.Next(100) < sr_chanceToBeDestroyedByBullet))
            {
                Destroy(0);
            }
        }
    }
}