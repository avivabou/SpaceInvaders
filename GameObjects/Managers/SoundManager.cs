using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using GameObjects.GameProperties;
using Microsoft.Xna.Framework;

namespace GameObjects.Managers
{
    public class SoundManager
    {
        private Dictionary<string, SoundEffect> m_soundEffects = new Dictionary<string, SoundEffect>();
        private SoundEffectInstance m_backgroundSong;
        private GameSettings m_gameSettings;

        /// <summary>
        /// SoundManager constructor.
        /// </summary>
        /// <param name="i_Game">The game this SoundManager will be playing at.</param>
        /// <param name="i_EnumType">Enum type for sound bank.</param>
        /// <param name="i_BackgroundName">The Enum value which presents background sound.</param>
        public SoundManager(Game i_Game, Type i_EnumType, Enum i_BackgroundName)
        {
            m_gameSettings = i_Game.Services.GetService(typeof(GameSettings)) as GameSettings;
            m_gameSettings.MusicVolumeChanged += backgroundMusicVolumeChanged;
            foreach (string eValue in Enum.GetNames(i_EnumType))
            {
                if (eValue == i_BackgroundName.ToString())
                {
                    SoundEffect background = i_Game.Content.Load<SoundEffect>(string.Format(@"Sounds\{0}", i_BackgroundName));
                    m_backgroundSong = background.CreateInstance();
                    m_backgroundSong.IsLooped = true;
                    m_backgroundSong.Play();
                }
                else
                {
                    SoundEffect soundEffect = i_Game.Content.Load<SoundEffect>(string.Format(@"Sounds\{0}", eValue));
                    m_soundEffects.Add(eValue, soundEffect);
                }
            }
        }

        /// <summary>
        /// Play the sound effect by given Enum Value.
        /// </summary>
        /// <param name="i_SoundEffect">Enum value.</param>
        public void PlaySoundEffect(Enum i_SoundEffect)
        {
            if (m_gameSettings.MusicState == eToggle.On)
            {
                if (i_SoundEffect != null && m_soundEffects.ContainsKey(i_SoundEffect.ToString()))
                {
                    SoundEffectInstance instance = m_soundEffects[i_SoundEffect.ToString()].CreateInstance();
                    instance.IsLooped = false;
                    instance.Volume = m_gameSettings.SoundEffectsVolume / 100f;
                    instance.Play();
                }
            }
        }

        /// <summary>
        /// Updating background volume.
        /// </summary>
        private void backgroundMusicVolumeChanged()
        {
            if (m_gameSettings.MusicState == eToggle.Off)
            {
                m_backgroundSong.Volume = 0;
            }
            else
            {
                m_backgroundSong.Volume = m_gameSettings.BackgroundMusicVolume / 100f;
            }
        }
    }
}
