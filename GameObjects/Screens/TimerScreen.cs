using System;
using Microsoft.Xna.Framework;

namespace GameObjects.Screens
{
    public class TimerScreen : DarkScreenCover
    {
        private static readonly float sr_minScale = 1;
        private static readonly float sr_maxScale = 8;
        private double m_timeToChange = 1;
        private int m_currentDigit;
        private TimeSpan m_length;

        /// <summary>
        /// TimerScreen consturcor.
        /// </summary>
        /// <param name="i_GameScreen">Game screen to cover.</param>
        /// <param name="i_Seconds">Seconds to release the timer cover screen.</param>
        /// <param name="i_MainTitle">Main title while counting down.</param>
        public TimerScreen(GameScreen i_GameScreen, int i_Seconds, string i_MainTitle)
            : base(i_GameScreen, false)
        {
            m_backgroundGameScreen.ScreenMode = eScreenMode.Visible;
            m_currentDigit = i_Seconds;
            m_length = TimeSpan.FromSeconds(i_Seconds - 0.5f);
            m_mainTitle = new MenuLabel(Game, i_MainTitle);
            AddComponents(m_mainTitle);
            m_subTitle = new MenuLabel(Game, m_currentDigit.ToString());
            AddComponents(m_subTitle);
        }

        /// <summary>
        /// Initialize TimerScreen.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            ScreensManager.KeyboardManager.Enabled = false;
        }

        /// <summary>
        /// Update labels and release this screen when the time is over.
        /// </summary>
        /// <param name="i_GameTime"></param>
        public override void Update(GameTime i_GameTime)
        {
            m_length -= i_GameTime.ElapsedGameTime;
            m_timeToChange -= i_GameTime.ElapsedGameTime.TotalSeconds;
            if (m_length.TotalSeconds <= 0)
            {
                resume();
                ScreensManager.KeyboardManager.Enabled = true;
            }

            if (m_timeToChange <= 0)
            {
                changeDigit();
            }

            Scale -= (float)i_GameTime.ElapsedGameTime.TotalSeconds * (sr_maxScale - sr_minScale);
            fitSubTitleToCenter();
        }

        /// <summary>
        /// Change the current digit for counting down.
        /// </summary>
        private void changeDigit()
        {
            m_timeToChange += 1;
            m_currentDigit--;
            Game.Components.Remove(m_subTitle);
            m_subTitle = new MenuLabel(Game, m_currentDigit.ToString());
            AddComponents(m_subTitle);
            Scale = sr_maxScale;
        }
    }
}
