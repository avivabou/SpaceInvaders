using GameObjects.Screens;
using GameObjects.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Invaders.GameScreens
{
    public class WelcomeScreen : GameScreen
    {
        private static readonly float sr_linesGapProportion = 0.75f;
        private MenuCollection m_menuCollection;

        /// <summary>
        /// WelcomeScreen constructor.
        /// </summary>
        /// <param name="i_ScreensManager">Screen manager.</param>
        public WelcomeScreen(ScreensManager i_ScreensManager)
            : base(i_ScreensManager, @"Backgrounds\BG_Space01_1024x768")
        {
            int midX = Game.Window.ClientBounds.Width / 2;
            string[] options = new string[3];
            options[0] = "Press 'Enter' to start play";
            options[1] = "Press 'Esc' to close the game";
            options[2] = "Press 'N' to view main menu";
            m_menuCollection = new MenuCollection(i_ScreensManager.Game, options, null, sr_linesGapProportion);
            addConstComponents(m_menuCollection);
        }

        /// <summary>
        /// Setting navigation keys.
        /// </summary>
        public override void SetKeys()
        {
            ScreensManager.KeyboardManager.AssignToKey(Keys.Enter, startGame, false);
            ScreensManager.KeyboardManager.AssignToKey(Keys.N, goToMainMenu, false);
        }

        /// <summary>
        /// Releasing navigation keys.
        /// </summary>
        public override void EraseKeys()
        {
            ScreensManager.KeyboardManager.RemoveAssigment(Keys.Enter);
            ScreensManager.KeyboardManager.RemoveAssigment(Keys.N);
        }

        /// <summary>
        /// Starting the game from the given stage screen.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        private void startGame(GameTime i_GameTime)
        {
            ScreensManager.RootScreen = new MainMenuScreen(ScreensManager);
            KeyboardManager keyboardManager = ScreensManager.KeyboardManager;
            GameScreen startTimer = StageScreen.GetStages(ScreensManager);
            ScreensManager.MoveTo(startTimer);
        }

        /// <summary>
        /// Going to main menu screen.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        private void goToMainMenu(GameTime i_GameTime)
        {
            ScreensManager.RootScreen = new MainMenuScreen(ScreensManager);
            ScreensManager.MoveBackToRootScreen();
        }
    }
}
