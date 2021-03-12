using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameObjects.Screens;
using GameObjects.Managers;
using GameObjects.Animations;
using GameObjects.FightTools;
using GameObjects.GameProperties;

namespace Invaders.GameScreens
{
    internal class StageScreen : GameScreen
    {
        private static readonly TimeSpan sr_dyingTimeForEnemies = TimeSpan.FromMilliseconds(1700);
        private static readonly float sr_dyingSpinsForEnemies = 5;
        private static readonly TimeSpan sr_dyingTimeForSpaceships = TimeSpan.FromMilliseconds(2600);
        private static readonly float sr_dyingSpinsForSpaceships = 6;
        private static readonly TimeSpan sr_attackedTimeForMotherships = TimeSpan.FromMilliseconds(3000);
        private static readonly float sr_attackedBlinksForMotherships = 15;
        private static readonly TimeSpan sr_attackedTimeForSpaceships = TimeSpan.FromMilliseconds(2000);
        private static readonly float sr_attackedBlinksForSpaceships = 8;
        private static readonly Vector2 sr_commonOrigin = new Vector2(16, 16);
        private static AnimationsManager s_animationsManager = null;
        private static int s_killedSpaceships = 0;
        private int m_level;
        private bool m_isInitialized = false;
        private Spaceship[] m_spaceships;
        private Mothership m_mothership;
        private EnemiesMatrix m_enemies;
        private BarriersArray m_barriers;
        private VisualSoulsAndScore m_visualSouls;

        public bool StageFailed
        {
            get
            {
                return s_killedSpaceships == m_spaceships.Length;
            }
        }

        /// <summary>
        /// StageScreen stage 1 constructor.
        /// </summary>
        /// <param name="i_ScreensManager">Screen manager.</param>
        /// <param name="i_AmountOfSpaceships">Amount of spaceships.</param>
        private StageScreen(ScreensManager i_ScreensManager, int i_AmountOfSpaceships)
            : this(i_ScreensManager, 1, new Spaceship[i_AmountOfSpaceships])
        {
            Enemy.ResetScores();
            Spaceship.ResetSpaceshipIDs();
            s_killedSpaceships = 0;
        }

        /// <summary>
        /// Stage screen constructor.
        /// </summary>
        /// <param name="i_ScreensManager">Screen manager.</param>
        /// <param name="i_Level">Level of stage.</param>
        /// <param name="i_Spaceships">Spaceships array.</param>
        private StageScreen(ScreensManager i_ScreensManager, int i_Level, Spaceship[] i_Spaceships)
            : base(i_ScreensManager, @"Backgrounds\BG_Space01_1024x768")
        {
            m_level = i_Level;
            m_spaceships = i_Spaceships;
            IntersectionManager intersectionManager = new IntersectionManager(this);
            Services.AddService(typeof(IntersectionManager), intersectionManager);
            if (s_animationsManager == null)
            {
                s_animationsManager = new AnimationsManager();
                addDyingAnimation();
            }

            Services.AddService(typeof(AnimationsManager), s_animationsManager);
        }

        /// <summary>
        /// Stages sequence creator.
        /// </summary>
        /// <param name="i_ScreensManager">Screen manager.</param>
        /// <param name="i_PlayersAmount">Players amount.</param>
        /// <returns></returns>
        public static GameScreen GetStages(ScreensManager i_ScreensManager)
        {
            StageScreen stage = new StageScreen(i_ScreensManager, (int)i_ScreensManager.GameSettings.PlayersAmount);
            string stageString = "Stage: 1";
            TimerScreen timerScreen = new TimerScreen(stage, 3, stageString);
            return timerScreen;
        }

        /// <summary>
        /// Initialize StageScreen.
        /// </summary>
        public override void Initialize()
        {
            if (!m_isInitialized)
            {
                m_isInitialized = true;
                for (int i = 0; i < m_spaceships.Length; i++)
                {
                    if (m_level == 1)
                    {
                        m_spaceships[i] = new Spaceship(this);
                        m_spaceships[i].OutOfSouls += () => { s_killedSpaceships++; };
                    }
                    else
                    {
                        m_spaceships[i].Reset();
                        m_spaceships[i].GameScreen = this;
                    }

                    AddComponents(m_spaceships[i]);
                }

                m_visualSouls = new VisualSoulsAndScore(Game, m_spaceships);
                AddComponents(m_visualSouls);
                setBattleField();
                base.Initialize();
            }
        }

        /// <summary>
        /// Update StageScreen.
        /// </summary>
        /// <param name="i_GameTime"></param>
        public override void Update(GameTime i_GameTime)
        {
            if (!StageFailed)
            {
                base.Update(i_GameTime);
            }
            else
            {
                stageFailed();
            }
        }

        /// <summary>
        /// Set pause key.
        /// </summary>
        public override void SetKeys()
        {
            bool v_ContinuedPressing = true;
            ScreensManager.KeyboardManager.AssignToKey(Keys.P, pause, !v_ContinuedPressing);
        }

        /// <summary>
        /// Release pause key.
        /// </summary>
        public override void EraseKeys()
        {
            ScreensManager.KeyboardManager.RemoveAssigment(Keys.P);
        }

        /// <summary>
        /// Adding dying animation for each Invader object.
        /// </summary>
        private static void addDyingAnimation()
        {
            /// Dying animation for enemies
            SpinAnimation spinAnimationForEnemies = new SpinAnimation("Spin", sr_dyingTimeForEnemies, sr_dyingSpinsForEnemies, sr_commonOrigin);
            ShrinkAnimation shrinkAnimationForEnemies = new ShrinkAnimation("Shrink", sr_dyingTimeForEnemies);
            AnimationsLibrary enemyDyingAnimation = new AnimationsLibrary("Enemy Dying", sr_dyingTimeForEnemies);
            enemyDyingAnimation.AddAnimation(spinAnimationForEnemies, shrinkAnimationForEnemies);
            s_animationsManager.AddAnimationToManager(enemyDyingAnimation);
            /// Attacked animation for spaceships
            bool v_FinalVisibility = true;
            BlinkAnimation blinkAnimationForSpaceships = new BlinkAnimation("Blink", sr_attackedTimeForSpaceships, sr_attackedBlinksForSpaceships, v_FinalVisibility);
            AnimationsLibrary spaceshipAttackedAnimation = new AnimationsLibrary("Spaceship Attacked", sr_attackedTimeForSpaceships);
            spaceshipAttackedAnimation.AddAnimation(blinkAnimationForSpaceships);
            s_animationsManager.AddAnimationToManager(spaceshipAttackedAnimation);
            /// Dying animation for spaceships
            SpinAnimation spinAnimationForSpaceships = new SpinAnimation("Spin", sr_dyingTimeForSpaceships, sr_dyingSpinsForSpaceships, sr_commonOrigin);
            FadeOutAnimation fadeOutAnimationForSpaceship = new FadeOutAnimation("Fade Out", sr_dyingTimeForSpaceships);
            AnimationsLibrary spaceshipDyingAnimation = new AnimationsLibrary("Spaceship Dying", sr_dyingTimeForSpaceships);
            spaceshipDyingAnimation.AddAnimation(spinAnimationForSpaceships, fadeOutAnimationForSpaceship);
            s_animationsManager.AddAnimationToManager(spaceshipDyingAnimation);
            /// Attacked animation for motherships
            ShrinkAnimation shrinkAnimationForMotherships = new ShrinkAnimation("Shrink", sr_attackedTimeForMotherships);
            BlinkAnimation blinkAnimationForMotherships = new BlinkAnimation("Blink", sr_attackedTimeForMotherships, sr_attackedBlinksForMotherships);
            FadeOutAnimation fadeOutAnimationForMothership = new FadeOutAnimation("Fade Out", sr_attackedTimeForMotherships);
            AnimationsLibrary mothershipDyingAnimation = new AnimationsLibrary("Mothership Attacked", sr_attackedTimeForMotherships);
            mothershipDyingAnimation.AddAnimation(shrinkAnimationForMotherships, blinkAnimationForMotherships, fadeOutAnimationForMothership);
            s_animationsManager.AddAnimationToManager(mothershipDyingAnimation);
        }

        /// <summary>
        /// Set battle field by current level.
        /// </summary>
        private void setBattleField()
        {
            m_mothership = new Mothership(this);
            AddComponents(m_mothership);
            int fieldLevel = (m_level - 1) % 4;
            float enemiesJumpsForSecond = 2;
            int enemiesRows = 5;
            int enemiesColumns = 9 + fieldLevel;
            m_enemies = new EnemiesMatrix(this, enemiesColumns, enemiesRows, enemiesJumpsForSecond);
            m_enemies.AllEnemiesDead += nextStage;
            m_enemies.ArrivedMaxY += stageFailed;
            AddComponents(m_enemies);
            m_barriers = new BarriersArray(this, 4, fieldLevel);
            AddComponents(m_barriers);
            if (fieldLevel == 0)
            {
                Enemy.ResetScores();
            }
        }

        /// <summary>
        /// Going to pausing screen.
        /// </summary>
        /// <param name="gameTime"></param>
        private void pause(GameTime gameTime)
        {
            DarkScreenCover pauseScreen = new DarkScreenCover(this);
            ScreensManager.MoveTo(pauseScreen);
        }

        /// <summary>
        /// Going to next stage screen.
        /// </summary>
        private void nextStage()
        {
            clearStageComponents();
            SoundManager soundManager = Game.Services.GetService(typeof(SoundManager)) as SoundManager;
            soundManager.PlaySoundEffect(eSounds.LevelWin);
            StageScreen nextStage = new StageScreen(ScreensManager, m_level + 1, m_spaceships);
            string stageString = string.Format("Stage: {0}", m_level + 1);
            TimerScreen timerScreen = new TimerScreen(nextStage, 3, stageString);
            Enemy.LevelUp();
            bool v_EraseLastStage = true;
            ScreensManager.MoveTo(timerScreen, v_EraseLastStage);
        }

        /// <summary>
        /// Going to game over screen.
        /// </summary>
        private void stageFailed()
        {
            clearStageComponents();
            SoundManager soundManager = Game.Services.GetService(typeof(SoundManager)) as SoundManager;
            soundManager.PlaySoundEffect(eSounds.GameOver);
            string scores = string.Format("Player 1: {0}", m_spaceships[0].Score);
            for (int i = 1; i < m_spaceships.Length; i++)
            {
                scores = string.Format("{0}{1}Player {2}: {3}", scores, Environment.NewLine, i + 1, m_spaceships[i].Score);
            }

            bool v_EraseThisScreen = true;
            GameOverScreen gameOver = new GameOverScreen(this, scores);
            ScreensManager.MoveTo(gameOver, v_EraseThisScreen);
        }

        /// <summary>
        /// Erasing any stage component from the game.
        /// </summary>
        private void clearStageComponents()
        {
            m_enemies.ArrivedMaxY -= stageFailed;
            m_enemies.AllEnemiesDead -= nextStage;
            Game.Components.Remove(m_mothership);
            Game.Components.Remove(m_enemies);
            Game.Components.Remove(m_barriers);
            Game.Components.Remove(m_visualSouls);
        }
    }
}