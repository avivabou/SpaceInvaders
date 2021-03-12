using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameObjects.Managers;

namespace GameObjects.Screens
{
    public class MenuScreen : GameScreen
    {
        protected MenuCollection m_menuCollection;
        protected MenuLabel m_title;
        private Enum m_sound;

        /// <summary>
        /// MenuScreen constructor.
        /// </summary>
        /// <param name="i_ScreensManager">Screen manager.</param>
        /// <param name="i_Background">Background content name.</param>
        /// <param name="i_Title">Main title.</param>
        /// <param name="i_Options">Options strings.</param>
        /// <param name="i_Sound">Sound of any action.</param>
        public MenuScreen(ScreensManager i_ScreensManager, string i_Background, string i_Title, string[] i_Options, Enum i_Sound)
            : base(i_ScreensManager, i_Background)
        {
            m_title = new MenuLabel(Game, i_Title);
            m_title.SetColors(Color.Cyan, Color.Cyan);
            m_title.Enabled = false;
            AddComponents(m_title);
            m_menuCollection = new MenuCollection(Game, i_Options, getFittedOptionsRectangle());
            AddComponents(m_menuCollection);
            m_menuCollection.Selected = 0;
            m_sound = i_Sound;
            Game.Window.ClientSizeChanged += window_ClientSizeChanged;
        }

        /// <summary>
        /// Initialize MenuScreen.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            fitMainTitle();
        }

        /// <summary>
        /// Set keys for acting in the MenuScreen.
        /// </summary>
        public override void SetKeys()
        {
            bool v_ContinuedPressing = true;
            ScreensManager.KeyboardManager.AssignToKey(Keys.Up, selectedDecrease, !v_ContinuedPressing);
            ScreensManager.KeyboardManager.AssignToKey(Keys.Down, selectedIncrease, !v_ContinuedPressing);
        }

        /// <summary>
        /// Release keys.
        /// </summary>
        public override void EraseKeys()
        {
            ScreensManager.KeyboardManager.RemoveAssigment(Keys.Up);
            ScreensManager.KeyboardManager.RemoveAssigment(Keys.Down);
        }

        /// <summary>
        /// Play the sound member.
        /// </summary>
        protected void playSound()
        {
            SoundManager soundManager = Game.Services.GetService(typeof(SoundManager)) as SoundManager;
            soundManager.PlaySoundEffect(m_sound);
        }

        /// <summary>
        /// Selected change to be larger by one. (cycly)
        /// </summary>
        /// <param name="i_GameTime"></param>
        private void selectedIncrease(GameTime i_GameTime)
        {
            if (ScreenMode.HasFlag(eScreenMode.Running))
            {
                m_menuCollection.Selected = (m_menuCollection.Selected + 1) % m_menuCollection.Count;
                playSound();
            }
        }

        /// <summary>
        /// Selected change to be smaller by one. (cycly)
        /// </summary>
        private void selectedDecrease(GameTime i_GameTime)
        {
            if (ScreenMode.HasFlag(eScreenMode.Running))
            {
                m_menuCollection.Selected = (m_menuCollection.Count + m_menuCollection.Selected - 1) % m_menuCollection.Count;
                playSound();
            }
        }

        /// <summary>
        /// Refit titles to new window size.
        /// </summary>
        /// <param name="i_Sender">Sender.</param>
        /// <param name="i_ArgsHolder">Arguments.</param>
        private void window_ClientSizeChanged(object i_Sender, EventArgs i_ArgsHolder)
        {
            m_menuCollection.SetRectangleBoundries(getFittedOptionsRectangle());
            fitMainTitle();
        }

        /// <summary>
        /// Get location rectangle of the options.
        /// </summary>
        /// <returns>Rectangle of options location.</returns>
        private Rectangle getFittedOptionsRectangle()
        {
            int screenWidth = Game.Window.ClientBounds.Width;
            return new Rectangle(screenWidth / 2, 0, screenWidth / 2, Game.Window.ClientBounds.Height);
        }

        /// <summary>
        /// Fit main title to window size.
        /// </summary>
        private void fitMainTitle()
        {
            float proportion = 0.4f * Game.Window.ClientBounds.Width / m_title.Measuring.X;
            Vector2 location;
            location.X = ((0.5f * Game.Window.ClientBounds.Width) - (m_title.Measuring.X * proportion)) / 2;
            location.Y = (Game.Window.ClientBounds.Height - (m_title.Measuring.Y * proportion)) / 2;
            m_title.SetProportion(location, proportion);
        }
    }
}
