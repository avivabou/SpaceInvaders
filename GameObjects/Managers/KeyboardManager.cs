using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameObjects.GameProperties;

namespace GameObjects.Managers
{
    public class KeyboardManager : GameComponent
    {
        private Dictionary<Keys, Action<GameTime>> m_keysActions = new Dictionary<Keys, Action<GameTime>>();
        private Dictionary<Keys, bool?> m_keysFullClick = new Dictionary<Keys, bool?>();
        private Action<KeyboardState> m_singleListener = null;
        private GameSettings m_gameSettings;
        private bool m_rightMouseButtonPressed = false;
        private bool m_leftMouseButtonPressed = false;
        private float m_lastScrollWheelValue = 0;
        private Keys m_mouseLeftClick, m_mouseRightClick, m_mouseScrollUp, m_mouseScrollDown;

        /// <summary>
        /// KeyboardManager constructor.
        /// </summary>
        /// <param name="i_Game">The game this manager live at.</param>
        public KeyboardManager(Game i_Game)
            : base(i_Game)
        {
            m_gameSettings = Game.Services.GetService(typeof(GameSettings)) as GameSettings;
        }

        /// <summary>
        /// Assign an action to keyboard key pressed.
        /// </summary>
        /// <param name="i_Key">The key should be react.</param>
        /// <param name="i_Action">The action for the given key.</param>
        public void AssignToKey(Keys i_Key, Action<GameTime> i_Action, bool i_ContinuedPressing = true)
        {
            if (m_keysActions.ContainsKey(i_Key))
            {
                m_keysActions[i_Key] += i_Action;
            }
            else
            {
                m_keysActions.Add(i_Key, i_Action);
            }

            if (!i_ContinuedPressing)
            {
                m_keysFullClick[i_Key] = false;
            }
            else
            {
                m_keysFullClick[i_Key] = null;
            }
        }

        /// <summary>
        /// Remove the action from the given key.
        /// </summary>
        /// <param name="i_Key">The key that invoking the action.</param>
        /// <param name="i_Action">The action should be removed.</param>
        public void RemoveAssigment(Keys i_Key)
        {
            m_keysActions[i_Key] = null;
            m_keysFullClick[i_Key] = null;
        }

        /// <summary>
        /// Invoking the actions for each key which pressed.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        public override void Update(GameTime i_GameTime)
        {
            if (Enabled)
            {
                actForMouse(i_GameTime);
                KeyboardState keyboardState = Keyboard.GetState();
                if (m_singleListener == null)
                {
                    invokePressedKeys(i_GameTime, keyboardState);
                }
                else
                {
                    m_singleListener.Invoke(keyboardState);
                }

                base.Update(i_GameTime);
            }
        }

        /// <summary>
        /// Set a method to be the only method which get update by keyboard.
        /// </summary>
        /// <param name="i_Action"></param>
        public void SetASingleListener(Action<KeyboardState> i_Action)
        {
            m_singleListener = i_Action;
        }

        /// <summary>
        /// Releasing the single method which get update by keyboard.
        /// </summary>
        public void FreeSingleListener()
        {
            m_singleListener = null;
        }

        /// <summary>
        /// Set mouse parallel keyboard pressing.
        /// </summary>
        /// <param name="i_LeftClick">Mouse left click key action.</param>
        /// <param name="i_RightClick">Mouse right click key action.</param>
        /// <param name="i_ScrollUp">Mouse scroll up key action.</param>
        /// <param name="i_ScrollDown">Mouse scroll down key action.</param>
        public void SetMouseParallelKeys(Keys i_LeftClick, Keys i_RightClick, Keys i_ScrollUp, Keys i_ScrollDown)
        {
            m_mouseLeftClick = i_LeftClick;
            m_mouseRightClick = i_RightClick;
            m_mouseScrollUp = i_ScrollUp;
            m_mouseScrollDown = i_ScrollDown;
        }

        /// <summary>
        /// Applying action by key.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        /// <param name="i_Key">Pressed key.</param>
        private void applyAction(GameTime i_GameTime, Keys i_Key)
        {
            if (m_keysActions.ContainsKey(i_Key) && (m_keysActions[i_Key] != null))
            {
                m_keysActions[i_Key].Invoke(i_GameTime);
            }
        }

        /// <summary>
        /// Checking for any key pressing.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        /// <param name="i_KeyboardState">Keyboard state.</param>
        private void invokePressedKeys(GameTime i_GameTime, KeyboardState i_KeyboardState)
        {
            Keys[] keys = new Keys[m_keysActions.Keys.Count];
            m_keysActions.Keys.CopyTo(keys, 0);
            foreach (Keys key in keys)
            {
                if (i_KeyboardState.IsKeyDown(key))
                {
                    if (m_keysFullClick[key] == null)
                    {
                        applyAction(i_GameTime, key);
                    }
                    else
                    {
                        m_keysFullClick[key] = true;
                    }
                }
                else if (i_KeyboardState.IsKeyUp(key))
                {
                    if ((m_keysFullClick[key] != null) && m_keysFullClick[key].Value)
                    {
                        m_keysFullClick[key] = false;
                        applyAction(i_GameTime, key);
                    }
                }
            }
        }

        /// <summary>
        /// Act for mouse current state.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        /// <returns>True if mouse can act instead of keyboard.</returns>
        private void actForMouse(GameTime i_GameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (m_gameSettings.MouseVisibilty == eVisible.Visible)
            {
                actForMouseLeftClick(i_GameTime, mouseState);
                actForMouseRightClick(i_GameTime, mouseState);
            }

            actForMouseScrollWheel(i_GameTime, mouseState);
            m_lastScrollWheelValue = mouseState.ScrollWheelValue;
        }

        /// <summary>
        /// Act for mouse left click.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        /// <param name="i_MouseState">Mouse state.</param>
        /// <returns>True if it can be clicked.</returns>
        private bool actForMouseLeftClick(GameTime i_GameTime, MouseState i_MouseState)
        {
            bool canBeClicked = false;
            if (m_keysActions[m_mouseLeftClick] != null)
            {
                canBeClicked = true;
                if (i_MouseState.LeftButton == ButtonState.Pressed)
                {
                    m_leftMouseButtonPressed = true;
                }
                else if (m_leftMouseButtonPressed && i_MouseState.LeftButton == ButtonState.Released)
                {
                    m_leftMouseButtonPressed = false;
                    m_keysActions[m_mouseLeftClick].Invoke(i_GameTime);
                }
            }

            return canBeClicked;
        }

        /// <summary>
        /// Act for mouse right click.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        /// <param name="i_MouseState">Mouse state.</param>
        /// <returns>True if it can be clicked.</returns>
        private bool actForMouseRightClick(GameTime i_GameTime, MouseState i_MouseState)
        {
            bool canBeClicked = false;
            if (m_keysActions[m_mouseRightClick] != null)
            {
                canBeClicked = true;
                if (i_MouseState.RightButton == ButtonState.Pressed)
                {
                    m_rightMouseButtonPressed = true;
                }
                else if (m_rightMouseButtonPressed && i_MouseState.RightButton == ButtonState.Released)
                {
                    m_rightMouseButtonPressed = false;
                    m_keysActions[m_mouseRightClick].Invoke(i_GameTime);
                }
            }

            return canBeClicked;
        }

        /// <summary>
        /// Act for mouse scroll wheel.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        /// <param name="i_MouseState">Mouse state.</param>
        /// <returns>True if it can be scrolled.</returns>
        private void actForMouseScrollWheel(GameTime i_GameTime, MouseState i_MouseState)
        {
            if (m_keysActions.ContainsKey(m_mouseScrollUp) && m_keysActions.ContainsKey(m_mouseScrollDown))
            {
                if ((m_keysActions[m_mouseScrollUp] != null) && (m_keysActions[m_mouseScrollDown] != null))
                {
                    if (i_MouseState.ScrollWheelValue < m_lastScrollWheelValue)
                    {
                        m_keysActions[m_mouseScrollUp].Invoke(i_GameTime);
                    }
                    else if (i_MouseState.ScrollWheelValue > m_lastScrollWheelValue)
                    {
                        m_keysActions[m_mouseScrollDown].Invoke(i_GameTime);
                    }
                }
            }
        }
    }
}
