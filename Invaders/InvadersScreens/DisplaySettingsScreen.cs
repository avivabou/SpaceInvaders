using Microsoft.Xna.Framework.Input;
using GameObjects.Screens;
using GameObjects.Managers;

namespace Invaders.GameScreens
{
    public class DisplaySettingsScreen : InvadersSettingsScreen
    {
        /// <summary>
        /// DisplaySettingsScreen constructor.
        /// </summary>
        /// <param name="i_ScreensManager">Screen manager.</param>
        public DisplaySettingsScreen(ScreensManager i_ScreensManager)
            : base(i_ScreensManager)
        {
            m_menuCollection.GetMenuLabelAt(0).ChangeText(string.Format("Allow Window Resizing: {0}", m_gameSettings.WindowResizing));
            m_menuCollection.GetMenuLabelAt(1).ChangeText(string.Format("Full Screen Mode: {0}", m_gameSettings.FullScreen));
            m_menuCollection.GetMenuLabelAt(2).ChangeText(string.Format("Mouse Visability: {0}", m_gameSettings.MouseVisibilty));
        }

        /// <summary>
        /// Change the setting of the selected label.
        /// </summary>
        /// <param name="i_Sender"></param>
        protected override void changeSettings(Keys i_Sender)
        {
            if (ScreenMode.HasFlag(eScreenMode.Running))
            {
                switch (m_menuCollection.Selected)
                {
                    case 0:
                        m_gameSettings.WindowResizing = 1 - m_gameSettings.WindowResizing;
                        m_menuCollection.GetMenuLabelAt(0).ChangeText(string.Format("Allow Window Resizing: {0}", m_gameSettings.WindowResizing));
                        break;
                    case 1:
                        m_gameSettings.FullScreen = 1 - m_gameSettings.FullScreen;
                        m_menuCollection.GetMenuLabelAt(1).ChangeText(string.Format("Full Screen Mode: {0}", m_gameSettings.FullScreen));
                        break;
                    case 2:
                        m_gameSettings.MouseVisibilty = 1 - m_gameSettings.MouseVisibilty;
                        m_menuCollection.GetMenuLabelAt(2).ChangeText(string.Format("Mouse Visability: {0}", m_gameSettings.MouseVisibilty));
                        break;
                }

                if (m_menuCollection.Selected != 3)
                {
                    playSound();
                }
            }
        }
    }
}
