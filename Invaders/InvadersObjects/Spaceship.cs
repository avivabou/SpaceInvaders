using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameObjects.FightTools;
using GameObjects.Managers;
using GameObjects.Screens;

namespace Invaders
{
    public class Spaceship : HurtableSprite, IScoreable
    {
        public static readonly int sr_SpaceshipSize = 32;
        private static readonly int sr_deadScoreLoss = -600;
        private static readonly float sr_spaceshipKeyboardMovementSpeed = 140;
        private static readonly string sr_texturePath = @"Sprites\InvadersSpacships";
        private static int s_spaceshipCounter = 0;
        private int m_spaceshipID;
        private float m_maxXOnScreen;
        private bool m_isReactingForMouse = false;
        private Shooter m_shooter;
        private MouseState? m_prevMouseState = null;

        public static int SpaceshipsY { get; private set; }

        public int Score { get; private set; } = 0;

        /// <summary>
        /// Invoke when the spaceship left of souls.
        /// </summary>
        public event Action OutOfSouls;

        /// <summary>
        /// Spaceship constructor.
        /// </summary>
        /// <param name="i_GameScreen">The game this spaceship will live at.</param>
        public Spaceship(GameScreen i_GameScreen)
            : base(i_GameScreen, sr_texturePath, Color.White, sr_deadScoreLoss)
        {
            m_selfTeam = eTeam.Player;
            Souls = 3;
            m_spaceshipID = s_spaceshipCounter;
            if (m_spaceshipID == 0)
            {
                m_isReactingForMouse = true;
            }

            s_spaceshipCounter++;
            m_SourceRectangles = new Rectangle[] { new Rectangle(0, (m_spaceshipID % 2) * sr_SpaceshipSize, sr_SpaceshipSize, sr_SpaceshipSize) };
            m_animationName = "Spaceship Attacked";
            m_isFiniteSprite = false;
            m_attackedSound = eSounds.LifeDie;
            Game.Window.ClientSizeChanged += window_ClientSizeChanged;
        }

        /// <summary>
        /// Reset spaceships IDs.
        /// </summary>
        public static void ResetSpaceshipIDs()
        {
            s_spaceshipCounter = 0;
        }

        /// <summary>
        /// Intialize the spaceship components.
        /// </summary>
        public override void Initialize()
        {
            if (!m_isIinitialized)
            {
                base.Initialize();
                m_maxXOnScreen = Game.Window.ClientBounds.Width - m_SelfTexture.Width;
                m_shooter = new Shooter(2, Bullet.sr_ShotTimeInterval);
                Reset();
                if (m_spaceshipID % 2 == 1)
                {
                    SetKeys(i_MoveLeft: Keys.Q, i_MoveRight: Keys.E, i_Shoot: Keys.D2);
                }
                else
                {
                    SetKeys(i_MoveLeft: Keys.Left, i_MoveRight: Keys.Right, i_Shoot: Keys.Up);
                }
            }
        }

        /// <summary>
        /// Update the compomnent.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        public override void Update(GameTime i_GameTime)
        {
            if (Enabled)
            {
                base.Update(i_GameTime);
                if (m_isReactingForMouse)
                {
                    reactForMouse(i_GameTime);
                }
            }
        }

        /// <summary>
        /// Getting score.
        /// Total score can't be negative.
        /// </summary>
        /// <param name="i_Score">Amount of score to reach.</param>
        public void GetAdditionalScore(int i_Score)
        {
            Score += i_Score;
            if (Score < 0)
            {
                Score = 0;
            }
        }

        /// <summary>
        /// Reset spaceship location and shooter.
        /// </summary>
        public void Reset()
        {
            SpaceshipsY = Game.Window.ClientBounds.Height - m_SelfTexture.Height;
            m_selfLocation = new Vector2(0, SpaceshipsY);
            m_selfLocation.X += m_spaceshipID * m_SelfTexture.Width;
            m_shooter.ClearShooter();
            if (Souls == 0)
            {
                Visible = false;
                Enabled = false;
            }
        }

        /// <summary>
        /// Set the spaceship keys.
        /// </summary>
        /// <param name="i_MoveLeft">Key to move left.</param>
        /// <param name="i_MoveRight">Key to move right.</param>
        /// <param name="i_Shoot">Key to shoot.</param>
        public void SetKeys(Keys i_MoveLeft, Keys i_MoveRight, Keys i_Shoot)
        {
            KeyboardManager keyboardManager = GameScreen.ScreensManager.KeyboardManager;
            Action<GameTime> moveLeft = (GameTime i_gameTime) => { move(i_gameTime, -1); };
            Action<GameTime> moveRight = (GameTime i_gameTime) => { move(i_gameTime, 1); };
            keyboardManager.AssignToKey(i_MoveLeft, moveLeft);
            keyboardManager.AssignToKey(i_MoveRight, moveRight);
            keyboardManager.AssignToKey(i_Shoot, shoot);
            BeenKilled += () => keyboardManager.RemoveAssigment(i_MoveLeft);
            BeenKilled += () => keyboardManager.RemoveAssigment(i_MoveRight);
            BeenKilled += () => keyboardManager.RemoveAssigment(i_Shoot);
        }

        /// <summary>
        /// Attacked by a IAttacker.
        /// </summary>
        /// <param name="i_Attacker">The IAttacker that attacked this spaceship.</param>
        protected override void attacked(IAttacker i_Attacker)
        {
            if (!m_AnimationsLibrary.IsOnRun)
            {
                base.attacked(i_Attacker);
                if (Souls > 0)
                {
                    m_selfLocation.X = 0;
                }

                GetAdditionalScore(sr_deadScoreLoss);
            }
        }

        /// <summary>
        /// Turning off the quit animation.
        /// </summary>
        protected override void turnOffAnimation()
        {
            base.turnOffAnimation();
            if (Souls == 1)
            {
                m_AnimationsLibrary.Pause();
                m_AnimationsLibrary.Clear();
                AnimationsManager animationsManager = GameScreen.Services.GetService(typeof(AnimationsManager)) as AnimationsManager;
                m_animationName = "Spaceship Dying";
                bool v_Override = true;
                animationsManager.InheritAnimation(this, m_animationName, v_Override);
                m_isFiniteSprite = true;
            }
            else if (IsDead && OutOfSouls != null)
            {
                OutOfSouls.Invoke();
            }
        }

        /// <summary>
        /// Reacting for mouse state.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        private void reactForMouse(GameTime i_GameTime)
        {
            if (Souls > 0)
            {
                MouseState mouseState = Mouse.GetState();
                if (!m_prevMouseState.HasValue && (mouseState.X != 0))
                {
                    m_prevMouseState = mouseState;
                }

                if (m_prevMouseState.HasValue)
                {
                    m_selfLocation.X += mouseState.X - m_prevMouseState.Value.X;
                    m_selfLocation.X = MathHelper.Clamp(m_selfLocation.X, 0, Game.Window.ClientBounds.Width - sr_SpaceshipSize);
                    m_prevMouseState = mouseState;
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        shoot(i_GameTime);
                    }
                }
            }
        }

        /// <summary>
        /// Move the spaceship by the given elapsed game time.
        /// </summary>
        /// <param name="i_ElapsedGameTime">Elapsed Game Time.</param>
        private void move(GameTime i_GameTime, int i_Direction = 1)
        {
            if (Souls > 0)
            {
                m_selfLocation.X += i_Direction * (float)(i_GameTime.ElapsedGameTime.TotalSeconds * sr_spaceshipKeyboardMovementSpeed);
                m_selfLocation.X = MathHelper.Clamp(m_selfLocation.X, 0, m_maxXOnScreen);
            }
        }

        /// <summary>
        /// Allow the spaceship to shoot a HurtableBullet.
        /// </summary>
        /// <param name="i_GameTime">Current time.</param>
        /// <returns>The HurtableBullet that were shot.</returns>
        private void shoot(GameTime i_GameTime)
        {
            HurtableBullet bullet = new HurtableBullet(GameScreen, m_selfLocation);
            if (m_shooter.Shoot(i_GameTime.TotalGameTime, bullet))
            {
                bullet.GetScore += GetAdditionalScore;
                GameScreen.AddComponents(bullet);
                SoundManager soundManager = Game.Services.GetService(typeof(SoundManager)) as SoundManager;
                soundManager.PlaySoundEffect(eSounds.SSGunShot);
            }
            else
            {
                bullet.Dispose();
                bullet = null;
            }
        }

        /// <summary>
        /// Refit spaceship to new window size.
        /// </summary>
        /// <param name="i_Sender">Sender.</param>
        /// <param name="i_ArgsHolder">Arguments.</param>
        private void window_ClientSizeChanged(object sender, EventArgs e)
        {
            SpaceshipsY = Game.Window.ClientBounds.Height - m_SelfTexture.Height;
            m_maxXOnScreen = Game.Window.ClientBounds.Width - m_SelfTexture.Width;
            m_selfLocation = new Vector2(m_selfLocation.X, SpaceshipsY);
        }
    }
}
