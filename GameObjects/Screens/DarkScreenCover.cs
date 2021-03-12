using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameObjects.Screens
{
    public class DarkScreenCover : GameScreen
    {
        private static readonly float sr_backScreenTint = 0.4f;
        protected GameScreen m_backgroundGameScreen;
        protected MenuLabel m_mainTitle = null;
        protected MenuLabel m_subTitle = null;
        protected Vector2 m_textLocation;
        private bool m_isPausing;

        public float Scale { get; set; } = 6;

        /// <summary>
        /// DarkScreenCover constructor.
        /// </summary>
        /// <param name="i_GameScreen">Background screen (which will be covered)</param>
        /// <param name="i_IsPausing">Is this cover pause the background screen updates?</param>
        public DarkScreenCover(GameScreen i_GameScreen, bool i_IsPausing = true)
            : base(i_GameScreen.ScreensManager, @"Screens\gradient")
        {
            m_isPausing = i_IsPausing;
            m_backgroundGameScreen = i_GameScreen;
            if (i_IsPausing)
            {
                m_mainTitle = new MenuLabel(Game, "Pause");
                m_subTitle = new MenuLabel(Game, "Press 'R' to resume");
                AddComponents(m_mainTitle, m_subTitle);
            }
        }

        /// <summary>
        /// Initialize DarkScreenCover.
        /// </summary>
        public override void Initialize()
        {
            m_backgroundGameScreen.Initialize();
            m_backgroundGameScreen.ScreenMode |= eScreenMode.Visible;
            base.Initialize();
            m_background.Tint = 1 - sr_backScreenTint;
            fitSubTitleToCenter();
            fitMainTitle();
        }

        /// <summary>
        /// Draw background screen with dark cover.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        public override void Draw(GameTime i_GameTime)
        {
            m_backgroundGameScreen.Draw(i_GameTime);
            base.Draw(i_GameTime);
        }

        /// <summary>
        /// Set this screen as single listener.
        /// </summary>
        public override void SetKeys()
        {
            ScreensManager.KeyboardManager.SetASingleListener(actForKeyboardState);
        }

        /// <summary>
        /// Free this screen from listening to keyboard.
        /// </summary>
        public override void EraseKeys()
        {
            ScreensManager.KeyboardManager.FreeSingleListener();
        }

        /// <summary>
        /// Fitting subtitle location to be at the center of the screen.
        /// </summary>
        protected void fitSubTitleToCenter()
        {
            if (m_subTitle != null)
            {
                float subScale = Scale / 1.5f;
                float textWidth = m_subTitle.Measuring.X * subScale;
                m_textLocation.X = (Game.Window.ClientBounds.Width - textWidth) / 2;
                float textHeight = m_subTitle.Measuring.Y * subScale;
                m_textLocation.Y = (Game.Window.ClientBounds.Height - textHeight) / 2;
                m_subTitle.SetProportion(m_textLocation, subScale);
            }
        }

        /// <summary>
        /// Acting for keyboard.
        /// </summary>
        /// <param name="i_KeyboardState"></param>
        protected virtual void actForKeyboardState(KeyboardState i_KeyboardState)
        {
            if (i_KeyboardState.IsKeyDown(Keys.R))
            {
                resume();
            }
        }

        /// <summary>
        /// Release the background screen from covering.
        /// </summary>
        protected void resume()
        {
            if (m_isPausing)
            {
                ScreensManager.PopBack();
            }
            else
            {
                bool v_EraseThisCover = true;
                m_backgroundGameScreen.ScreensManager.MoveTo(m_backgroundGameScreen, v_EraseThisCover);
            }
        }

        /// <summary>
        /// Fitting main title location if it exists.
        /// </summary>
        private void fitMainTitle()
        {
            if (m_mainTitle != null)
            {
                Vector2 location;
                float textX = m_mainTitle.Measuring.X * Scale;
                location.X = (Game.Window.ClientBounds.Width - textX) / 2;
                location.Y = m_mainTitle.Measuring.Y * Scale / 2;
                m_mainTitle.SetProportion(location, Scale);
            }
        }
    }
}
