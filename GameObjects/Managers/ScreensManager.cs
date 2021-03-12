using System;
using System.Collections.Generic;
using GameObjects.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameObjects.GameProperties;

namespace GameObjects.Managers
{
    public class ScreensManager : DrawableGameComponent
    {
        private Stack<GameScreen> m_gameScreensStack = new Stack<GameScreen>();
        private Action m_quit;
        private GameScreen m_rootScreen;

        public GameScreen RootScreen
        {
            set
            {
                value.Initialize();
                m_rootScreen = value;
            }
        }

        public KeyboardManager KeyboardManager { get; }

        public GameSettings GameSettings { get; } 

        public GameScreen Current
        {
            get
            {
                return m_gameScreensStack.Peek();
            }
        }

        /// <summary>
        /// ScreensManager constructor.
        /// </summary>
        /// <param name="i_Game">The game this ScreensManager will manage.</param>
        /// <param name="i_Quit">Quit method (can not be null)</param>
        public ScreensManager(Game i_Game, Action i_Quit)
            : base(i_Game)
        {
            bool v_ContinuedPressing = true;
            m_quit = i_Quit ?? throw new ArgumentNullException("Quit action can not get a null value");
            KeyboardManager = new KeyboardManager(i_Game);
            KeyboardManager.AssignToKey(Keys.Escape, PopBack, !v_ContinuedPressing);
            KeyboardManager.AssignToKey(Keys.M, toggleSoundState, !v_ContinuedPressing);
            KeyboardManager.SetMouseParallelKeys(i_LeftClick: Keys.Enter, i_RightClick: Keys.PageDown, i_ScrollDown: Keys.PageDown, i_ScrollUp: Keys.PageUp);
            Enabled = false;
            GameSettings = Game.Services.GetService(typeof(GameSettings)) as GameSettings;
        }

        /// <summary>
        /// Update the current screen.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        public sealed override void Update(GameTime i_GameTime)
        {
            if (Enabled)
            {
                Current.Update(i_GameTime);
                KeyboardManager.Update(i_GameTime);
            }
        }

        /// <summary>
        /// Draw current screen.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        public sealed override void Draw(GameTime i_GameTime)
        {
            if (Visible)
            {
                Current.Draw(i_GameTime);
            }
        }

        /// <summary>
        /// Moving to the given screen.
        /// </summary>
        /// <param name="i_GameScreen">Screen will be shown.</param>
        /// <param name="i_EraseLast">Erasing the current screen from the stack if true.</param>
        public void MoveTo(GameScreen i_GameScreen, bool i_EraseLast = false)
        {
            if (m_gameScreensStack.Count > 0)
            {
                Current.EraseKeys();
                if (i_EraseLast)
                {
                    Current.ScreenMode = eScreenMode.Inactive;
                    m_gameScreensStack.Pop();
                }
                else
                {
                    Current.ScreenMode = eScreenMode.Visible;
                }
            }

            i_GameScreen.Initialize();
            m_gameScreensStack.Push(i_GameScreen);
            Current.ScreenMode = eScreenMode.Active;
            Current.SetKeys();
            Enabled = true;
        }

        /// <summary>
        /// Moving back to root screen (erasing from the stack any screen until arriving the root screen)
        /// </summary>
        public void MoveBackToRootScreen()
        {
            Current.EraseKeys();
            while (Current != m_rootScreen)
            {
                m_gameScreensStack.Pop();
                if (m_gameScreensStack.Count == 0)
                {
                    m_gameScreensStack.Push(m_rootScreen);
                }
            }

            m_rootScreen.SetKeys();
            m_rootScreen.ScreenMode = eScreenMode.Active;
        }

        /// <summary>
        /// Quit the screen manager.
        /// </summary>
        public void Quit()
        {
            m_gameScreensStack.Clear();
            m_quit.Invoke();
            Dispose();
        }

        /// <summary>
        /// Pop the current screen from the stack.
        /// If the stack lefts empty, the screen manger will quit.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        internal void PopBack(GameTime i_GameTime = null)
        {
            Current.ScreenMode = eScreenMode.Inactive;
            Current.EraseKeys();
            m_gameScreensStack.Pop();
            if (m_gameScreensStack.Count == 0)
            {
                m_quit.Invoke();
            }

            Current.ScreenMode = eScreenMode.Active;
            Current.SetKeys();
        }

        /// <summary>
        /// Toggle sound state. 
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        private void toggleSoundState(GameTime i_GameTime)
        {
            GameSettings.MusicState = 1 - GameSettings.MusicState;
        }
    }
}
