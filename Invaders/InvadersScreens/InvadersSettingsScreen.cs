using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameObjects.Screens;
using GameObjects.Managers;
using GameObjects.GameProperties;

namespace Invaders.GameScreens
{
    public abstract class InvadersSettingsScreen : MenuScreen
    {
        private static readonly string[] s_menuOptions = new string[] { string.Empty, string.Empty, string.Empty, "Done" };
        protected GameSettings m_gameSettings;

        /// <summary>
        /// InvadersSettingsScreen constructor.
        /// </summary>
        /// <param name="i_ScreensManager">Screen manager.</param>
        public InvadersSettingsScreen(ScreensManager i_ScreensManager)
            : base(i_ScreensManager, @"Backgrounds\BG_Space01_1024x768", "Screen Settings", s_menuOptions, eSounds.MenuMove)
        {
            m_gameSettings = Game.Services.GetService(typeof(GameSettings)) as GameSettings;
        }

        /// <summary>
        /// Set navigation keys.
        /// </summary>
        public sealed override void SetKeys()
        {
            base.SetKeys();
            bool v_continuedPressing = true;
            Action<GameTime> pageDownAction = new Action<GameTime>((GameTime) => changeSettings(Keys.PageDown));
            Action<GameTime> pageUpAction = new Action<GameTime>((GameTime) => changeSettings(Keys.PageUp));
            ScreensManager.KeyboardManager.AssignToKey(Keys.PageDown, pageDownAction, !v_continuedPressing);
            ScreensManager.KeyboardManager.AssignToKey(Keys.PageUp, pageUpAction, !v_continuedPressing);
            ScreensManager.KeyboardManager.AssignToKey(Keys.Enter, done, !v_continuedPressing);
        }

        /// <summary>
        /// Releasing navigation keys.
        /// </summary>
        public sealed override void EraseKeys()
        {
            base.EraseKeys();
            ScreensManager.KeyboardManager.RemoveAssigment(Keys.PageDown);
            ScreensManager.KeyboardManager.RemoveAssigment(Keys.PageUp);
            ScreensManager.KeyboardManager.RemoveAssigment(Keys.Enter);
        }

        /// <summary>
        /// Abstract method for changing settings.
        /// </summary>
        /// <param name="i_Sender"></param>
        protected abstract void changeSettings(Keys i_Sender);

        /// <summary>
        /// Closing the screen.
        /// </summary>
        /// <param name="i_GameTime"></param>
        private void done(GameTime i_GameTime)
        {
            if (ScreenMode.HasFlag(eScreenMode.Running))
            {
                if (m_menuCollection.Selected == 3)
                {
                    playSound();
                    Close();
                }
            }
        }
    }
}
