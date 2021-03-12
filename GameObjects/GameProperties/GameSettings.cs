using System;
using Microsoft.Xna.Framework;
using GameObjects.Managers;

namespace GameObjects.GameProperties
{
    public class GameSettings
    {
        private Game m_game;
        private GraphicsDeviceManager m_graphicsDeviceManager;
        private SoundManager m_soundManager;
        private int m_backgroundVolume = 100;
        private int m_soundEffectsVolume = 100;
        private eToggle m_isMusicOn = eToggle.On;

        public event Action MusicVolumeChanged;

        public eToggle MusicState
        {
            get
            {
                return m_isMusicOn;
            }

            set
            {
                if (value != m_isMusicOn)
                {
                    m_isMusicOn = value;
                    if (MusicVolumeChanged != null)
                    {
                        MusicVolumeChanged.Invoke();
                    }
                }
            }
        }

        public int BackgroundMusicVolume
        {
            get
            {
                return m_backgroundVolume;
            }

            set
            {
                m_backgroundVolume = (110 + value) % 110;
                if (MusicVolumeChanged != null)
                {
                    MusicVolumeChanged.Invoke();
                }
            }
        }

        public int SoundEffectsVolume
        {
            get
            {
                return m_soundEffectsVolume;
            }

            set
            {
                m_soundEffectsVolume = (110 + value) % 110;
            }
        }

        public eToggle WindowResizing
        {
            get
            {
                return m_game.Window.AllowUserResizing ? eToggle.On : eToggle.Off;
            }

            set
            {
                m_game.Window.AllowUserResizing = value == eToggle.On;
            }
        }

        public eToggle FullScreen
        {
            get
            {
                return m_graphicsDeviceManager.IsFullScreen ? eToggle.On : eToggle.Off;
            }

            set
            {
               if (FullScreen != value)
                {
                    m_graphicsDeviceManager.ToggleFullScreen();
                }
            }
        }

        public eVisible MouseVisibilty
        {
            get
            {
                return m_game.IsMouseVisible ? eVisible.Visible : eVisible.Invisible;
            }

            set
            {
                m_game.IsMouseVisible = value == eVisible.Visible;
            }
        }

        public ePlayersAmount PlayersAmount { get; set; } = ePlayersAmount.One;

        /// <summary>
        /// GameSettings constructor.
        /// </summary>
        /// <param name="i_Game">The game this GameSettings will control.</param>
        public GameSettings(Game i_Game)
        {
            m_game = i_Game;
            m_graphicsDeviceManager = m_game.Services.GetService(typeof(GraphicsDeviceManager)) as GraphicsDeviceManager;
        }

        /// <summary>
        /// Importing sound to sound manager.
        /// </summary>
        /// <param name="i_EnumType">Enum type which present sound bank names.</param>
        /// <param name="i_BackgroundName">Enum value which present the background sound.</param>
        public void ImportSounds(Type i_EnumType, Enum i_BackgroundName)
        {
            if (!i_EnumType.IsEnum)
            {
                throw new ArgumentException("Sound Manager can work only with Enum types");
            }

            m_soundManager = new SoundManager(m_game, i_EnumType, i_BackgroundName);
            m_game.Services.AddService(typeof(SoundManager), m_soundManager);
        }
    }
}
