using GameObjects.Screens;
using GameObjects.Managers;
using Microsoft.Xna.Framework;
using GameObjects.GameProperties;
using Microsoft.Xna.Framework.Input;

namespace Invaders.GameScreens
{
    public class MainMenuScreen : MenuScreen
    {
        private static readonly string[] m_mainMenuOptions = new string[] { "Screen Settings", "Players: One", "Sound Settings", "Play", "Quit" };

        /// <summary>
        /// MainMenuScreen constructor.
        /// </summary>
        /// <param name="i_ScreensManager">Screen manager.</param>
        public MainMenuScreen(ScreensManager i_ScreensManager)
            : base(i_ScreensManager, @"Backgrounds\BG_Space01_1024x768", "Main Menu", m_mainMenuOptions, eSounds.MenuMove)
        {
        }

        /// <summary>
        /// Set navigation keys.
        /// </summary>
        public override void SetKeys()
        {
            base.SetKeys();
            bool v_continuedPressing = true;
            ScreensManager.KeyboardManager.AssignToKey(Keys.PageDown, togglePlayersAmount, !v_continuedPressing);
            ScreensManager.KeyboardManager.AssignToKey(Keys.PageUp, togglePlayersAmount, !v_continuedPressing);
            ScreensManager.KeyboardManager.AssignToKey(Keys.Enter, selectionChosed, !v_continuedPressing);
        }

        /// <summary>
        /// Releasing navigation keys.
        /// </summary>
        public override void EraseKeys()
        {
            base.EraseKeys();
            ScreensManager.KeyboardManager.RemoveAssigment(Keys.PageDown);
            ScreensManager.KeyboardManager.RemoveAssigment(Keys.PageUp);
            ScreensManager.KeyboardManager.RemoveAssigment(Keys.Enter);
        }

        /// <summary>
        /// Toggle players amount Enum.
        /// </summary>
        /// <param name="i__GameTime"></param>
        private void togglePlayersAmount(GameTime i__GameTime)
        {
            if (ScreenMode.HasFlag(eScreenMode.Running))
            {
                if (m_menuCollection.Selected == 1)
                {
                    ScreensManager.GameSettings.PlayersAmount = 3 - ScreensManager.GameSettings.PlayersAmount;
                    m_menuCollection.GetMenuLabelAt(1).ChangeText(string.Format("Players: {0}", ScreensManager.GameSettings.PlayersAmount.ToString()));
                    playSound();
                }
            }
        }

        /// <summary>
        /// Selected label were chosed.
        /// </summary>
        /// <param name="i_GameTime"></param>
        private void selectionChosed(GameTime i_GameTime)
        {
            if (ScreenMode.HasFlag(eScreenMode.Running))
            {
                switch (m_menuCollection.Selected)
                {
                    case 0:
                        DisplaySettingsScreen displaySettings = new DisplaySettingsScreen(ScreensManager);
                        ScreensManager.MoveTo(displaySettings);
                        break;
                    case 2:
                        SoundSettingsScreen soundSettings = new SoundSettingsScreen(ScreensManager);
                        ScreensManager.MoveTo(soundSettings);
                        break;
                    case 3:
                        GameScreen startGame = StageScreen.GetStages(ScreensManager);
                        ScreensManager.MoveTo(startGame);
                        break;
                    case 4:
                        Close();
                        break;
                }

                if (m_menuCollection.Selected != 1)
                {
                    playSound();
                }
            }
        }
    }
}
