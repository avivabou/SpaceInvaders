using GameObjects.Screens;
using GameObjects.Managers;
using Microsoft.Xna.Framework.Input;

namespace Invaders.GameScreens
{
    public class SoundSettingsScreen : InvadersSettingsScreen
    {
        /// <summary>
        /// SoundSettingsScreen constructor.
        /// </summary>
        /// <param name="i_ScreensManager">Screen manager.</param>
        public SoundSettingsScreen(ScreensManager i_ScreensManager)
            : base(i_ScreensManager)
        {
            m_gameSettings.MusicVolumeChanged += m_gameSettings_MusicVolumeChanged;
            m_gameSettings_MusicVolumeChanged();
        }

        /// <summary>
        /// Change settings of selected label.
        /// </summary>
        /// <param name="i_Sender">Key pressed.</param>
        protected override void changeSettings(Keys i_Sender)
        {
            if (ScreenMode.HasFlag(eScreenMode.Running))
            {
                int changeDirection = 10;
                if (i_Sender.Equals(Keys.PageDown))
                {
                    changeDirection = -10;
                }

                switch (m_menuCollection.Selected)
                {
                    case 0:
                        m_gameSettings.MusicState = 1 - m_gameSettings.MusicState;
                        break;
                    case 1:
                        m_gameSettings.BackgroundMusicVolume += changeDirection;
                        break;
                    case 2:
                        m_gameSettings.SoundEffectsVolume += changeDirection;
                        m_gameSettings_MusicVolumeChanged();
                        break;
                }

                if (m_menuCollection.Selected != 3)
                {
                    playSound();
                }
            }
        }

        /// <summary>
        /// Updating labels when some setting changed.
        /// </summary>
        private void m_gameSettings_MusicVolumeChanged()
        {
            m_menuCollection.GetMenuLabelAt(0).ChangeText(string.Format("Toggle Sound: {0}", m_gameSettings.MusicState));
            m_menuCollection.GetMenuLabelAt(1).ChangeText(string.Format("Background Music Volume: {0}", m_gameSettings.BackgroundMusicVolume));
            m_menuCollection.GetMenuLabelAt(2).ChangeText(string.Format("Sound Effects Volume: {0}", m_gameSettings.SoundEffectsVolume));
        }
    }
}
