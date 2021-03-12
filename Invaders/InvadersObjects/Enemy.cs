using System;
using Microsoft.Xna.Framework;
using GameObjects.FightTools;
using GameObjects.Screens;
using GameObjects.Managers;

namespace Invaders
{
    public class Enemy : HurtableSprite, IIntersectable
    {
        public static readonly int sr_EnemySize = 32;
        private static readonly string sr_texturePath = @"Sprites\InvadersEnemies";
        private static readonly Color[] sr_enemiesColors = new Color[] { Color.LightPink, Color.LightCyan, Color.LightYellow };
        private static readonly int sr_randomShotInterval = 500;
        private static readonly int sr_minShotInterval = 1000;
        private static int[] s_enemiesScores = new int[] { 300, 200, 70 };
        private static int s_enemiesCounter = 0;
        private Shooter m_shooter = new Shooter(1, TimeSpan.Zero);
        private TimeSpan m_nextShotInterval;
        private TimeSpan m_lastShotTime = TimeSpan.Zero;
        private Random m_random;

        public override Rectangle ShapeRectangle
        {
            get
            {
                return new Rectangle((int)CurrentLocation.X, (int)CurrentLocation.Y, m_SourceRectangles[m_RectangleIndex].Width, m_SourceRectangles[m_RectangleIndex].Height);
            }
        }

        /// <summary>
        /// Enemy constructor.
        /// </summary>
        /// <param name="i_GameScreen">The game this enemy will live at.</param>
        /// <param name="i_EnemyIndex">The index of the enemy.</param>
        /// <param name="i_EnemyLocation">The location of the enemy.</param>
        /// <param name="i_TextureMode">Texture mode.</param>
        public Enemy(GameScreen i_GameScreen, int i_EnemyIndex, Vector2 i_EnemyLocation, int i_TextureMode = 0)
            : base(i_GameScreen, sr_texturePath, sr_enemiesColors[i_EnemyIndex], s_enemiesScores[i_EnemyIndex])
        {
            m_selfTeam = eTeam.Enemy;
            m_selfLocation = i_EnemyLocation;
            m_RectangleIndex = i_TextureMode;
            m_SourceRectangles = new Rectangle[2];
            m_SourceRectangles[0] = new Rectangle(0, 2 * sr_EnemySize * i_EnemyIndex, sr_EnemySize, sr_EnemySize);
            m_SourceRectangles[1] = new Rectangle(0, (2 * sr_EnemySize * i_EnemyIndex) + sr_EnemySize, sr_EnemySize, sr_EnemySize);
            m_animationName = "Enemy Dying";
            s_enemiesCounter++;
            m_attackedSound = eSounds.EnemyKill;
            m_random = new Random(s_enemiesCounter);
            BeenKilled += () => { s_enemiesCounter--; };
        }

        /// <summary>
        /// Raise all given scores by 100 points and reset enemies counter.
        /// </summary>
        public static void LevelUp()
        {
            s_enemiesCounter = 0;
            for (int i = 0; i < s_enemiesScores.Length; i++)
            {
                s_enemiesScores[i] += 100;
            }
        }

        /// <summary>
        /// Reset scores to be as original.
        /// </summary>
        public static void ResetScores()
        {
            s_enemiesScores[0] = 300;
            s_enemiesScores[1] = 200;
            s_enemiesScores[2] = 70;
        }

        /// <summary>
        /// Set a base vector to start from there (such as defining a new zero vector).
        /// </summary>
        /// <param name="i_ZeroVector">A base vector.</param>
        public override void SetZeroVector(Vector2 i_ZeroVector)
        {
            base.SetZeroVector(i_ZeroVector);
            m_RectangleIndex = 1 - m_RectangleIndex;
        } 

        /// <summary>
        /// Update the enemy.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        public override void Update(GameTime i_GameTime)
        {
            base.Update(i_GameTime);
            if (m_lastShotTime == TimeSpan.Zero)
            {
                m_lastShotTime = i_GameTime.TotalGameTime;
                updateNextShot();
            }
            else if (!IsDead)
            {
                if (i_GameTime.TotalGameTime - m_lastShotTime > m_nextShotInterval)
                {
                    m_lastShotTime = i_GameTime.TotalGameTime;
                    Bullet bullet = null;
                    bullet = new Bullet(GameScreen, CurrentLocation, Game.Window.ClientBounds.Bottom);
                    if (m_shooter.Shoot(i_GameTime.TotalGameTime, bullet))
                    {
                        SoundManager soundManager = Game.Services.GetService(typeof(SoundManager)) as SoundManager;
                        soundManager.PlaySoundEffect(eSounds.EnemyGunShot);
                        GameScreen.AddComponents(bullet);
                        updateNextShot();
                    }
                    else
                    {
                        bullet.Dispose();
                        bullet = null;
                    }
                }
            }
        }

        /// <summary>
        /// Update the timing of the next shot.
        /// </summary>
        private void updateNextShot()
        {
            int nextRand = sr_randomShotInterval * s_enemiesCounter;
            float randomTime = m_random.Next(nextRand);
            m_nextShotInterval = TimeSpan.FromMilliseconds(sr_minShotInterval + randomTime);
        }
    }
}
