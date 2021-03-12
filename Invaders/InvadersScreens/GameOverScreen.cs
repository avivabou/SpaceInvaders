using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameObjects.Screens;

namespace Invaders.GameScreens
{
    public class GameOverScreen : DarkScreenCover
    {
        private TimeSpan m_mainTitleColorInterval = TimeSpan.Zero;

        /// <summary>
        /// GameOverScreen constructor.
        /// </summary>
        /// <param name="i_GameScreen">Game screen to cover.</param>
        /// <param name="i_Scores">Scores message.</param>
        public GameOverScreen(GameScreen i_GameScreen, string i_Scores)
            : base(i_GameScreen)
        {
            m_mainTitle.ChangeText("Game Over!");
            m_mainTitle.IsSelected = true;
            m_mainTitle.Enabled = false;
            m_subTitle.ChangeText(i_Scores);
            m_subTitle.SetColors(Color.Cyan, Color.Cyan);
            m_subTitle.Enabled = false;
            addKeysLabels();
        }

        /// <summary>
        /// Release this screen from being a single listener.
        /// </summary>
        public override void EraseKeys()
        {
            ScreensManager.KeyboardManager.FreeSingleListener();
        }

        /// <summary>
        /// Acting for keyboard state.
        /// </summary>
        /// <param name="i_KeyboardState">Keyboard state.</param>
        protected override void actForKeyboardState(KeyboardState i_KeyboardState)
        {
            if (i_KeyboardState.IsKeyDown(Keys.Escape))
            {
                ScreensManager.Quit();
            }
            else if (i_KeyboardState.IsKeyDown(Keys.Home))
            {
                bool v_EraseThisScreen = true;
                GameScreen startTimer = StageScreen.GetStages(ScreensManager);
                ScreensManager.MoveTo(startTimer, v_EraseThisScreen);
            }
            else if (i_KeyboardState.IsKeyDown(Keys.N))
            {
                ScreensManager.MoveBackToRootScreen();
            }
        }

        /// <summary>
        /// Adding guiding labels.
        /// </summary>
        private void addKeysLabels()
        {
            string[] options = new string[3];
            options[0] = "Press 'Esc' to close the game";
            options[1] = "Press 'Home' to restart";
            options[2] = "Press 'N' to view main menu";
            int thridHeightScreen = Game.Window.ClientBounds.Height / 3;
            Rectangle screenBoundries = new Rectangle(0, 2 * thridHeightScreen, Game.Window.ClientBounds.Width, thridHeightScreen);
            MenuCollection keysOptions = new MenuCollection(ScreensManager.Game, options, screenBoundries, 0.5f);
            keysOptions.Enabled = false;
            AddComponents(keysOptions);
        }
    }
}
