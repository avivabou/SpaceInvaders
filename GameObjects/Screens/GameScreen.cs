using System;
using Microsoft.Xna.Framework;
using GameObjects.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace GameObjects.Screens
{
    public abstract class GameScreen : DrawableGameComponent
    {
        protected eScreenMode m_screenMode = eScreenMode.Active;
        protected GameComponentCollection m_gameComponentCollection;
        protected GameComponentCollection m_gameDrawableComponentCollection;
        protected GameComponentCollection m_constGameComponentCollection;
        protected Sprite m_background;

        public ScreensManager ScreensManager { get; }

        public eScreenMode ScreenMode { get; set; }

        public GameServiceContainer Services { get; } = new GameServiceContainer();

        /// <summary>
        /// Invoke when a component added to the screen.
        /// </summary>
        public event Action<IGameComponent[]> ComponentAdded;

        /// <summary>
        /// Invoke when a component removed from the screen.
        /// </summary>
        public event Action<IGameComponent> ComponentRemoved;

        /// <summary>
        /// GameScreen abstract constructor.
        /// </summary>
        /// <param name="i_ScreensManager">Screen Manager.</param>
        /// <param name="i_BackgroundTexture">Background texture name.</param>
        public GameScreen(ScreensManager i_ScreensManager, string i_BackgroundTexture)
            : base(i_ScreensManager.Game)
        {
            Enabled = false;
            Visible = false;
            m_gameComponentCollection = new GameComponentCollection();
            m_gameDrawableComponentCollection = new GameComponentCollection();
            m_constGameComponentCollection = new GameComponentCollection();
            Game.Components.ComponentRemoved += components_ComponentRemoved;
            ScreensManager = i_ScreensManager;
            Vector2 backgroundProportion = Vector2.One;
            m_background = new Sprite(this, i_BackgroundTexture, Color.White, backgroundProportion);
            m_background.DrawOrder = -2;
            addConstComponents(m_background);
        }

        /// <summary>
        /// Add component to the screen and game.
        /// </summary>
        /// <param name="i_GameComponents">Game components.</param>
        public void AddComponents(params IGameComponent[] i_GameComponents)
        {
            foreach (GameComponent component in i_GameComponents)
            {
                m_gameComponentCollection.Add(component);
                component.Initialize();
                if (component is DrawableGameComponent)
                {
                    m_gameDrawableComponentCollection.Add(component);
                }

                if (!Game.Components.Contains(component))
                {
                    Game.Components.Add(component);
                }
            }

            if (ComponentAdded != null)
            {
                ComponentAdded.Invoke(i_GameComponents);
            }
        }

        /// <summary>
        /// Initialize GameScreen.
        /// </summary>
        public override void Initialize()
        {
            for (int i = 0; i < m_gameComponentCollection.Count; i++)
            {
                m_gameComponentCollection[i].Initialize();
            }
        }

        /// <summary>
        /// Update any component in this screen.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        public override void Update(GameTime i_GameTime)
        {
            if (m_screenMode.HasFlag(eScreenMode.Running))
            {
                for (int i = 0; i < m_gameComponentCollection.Count; i++)
                {
                    (m_gameComponentCollection[i] as GameComponent).Update(i_GameTime);
                }
            }
        }

        /// <summary>
        /// Drawing any drawable component in this screen.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        public override void Draw(GameTime i_GameTime)
        {
            SpriteBatch spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            spriteBatch.Begin();
            if (m_screenMode.HasFlag(eScreenMode.Visible))
            {
                for (int i = 0; i < m_gameDrawableComponentCollection.Count; i++)
                {
                    DrawableGameComponent component = m_gameDrawableComponentCollection[i] as DrawableGameComponent;
                    component.Draw(i_GameTime);
                }
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Close this screen.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        public virtual void Close(GameTime i_GameTime = null)
        {
            ScreensManager.PopBack();
            Dispose();
        }

        /// <summary>
        /// Set keys from keyboard to listen.
        /// </summary>
        public abstract void SetKeys();

        /// <summary>
        /// Clear listening keys.
        /// </summary>
        public abstract void EraseKeys();

        /// <summary>
        /// Add const componenet (won't be removed from screen if it will be removed from the game)
        /// </summary>
        /// <param name="i_GameComponent"></param>
        protected void addConstComponents(IGameComponent i_GameComponent)
        {
            m_constGameComponentCollection.Add(i_GameComponent);
            AddComponents(i_GameComponent);
        }

        /// <summary>
        /// When a componenet removed from the game, it will be also removed from the screen.
        /// </summary>
        /// <param name="i_Sender">Sender object.</param>
        /// <param name="i_ArgsHolder">Arguments holder.</param>
        private void components_ComponentRemoved(object i_Sender, GameComponentCollectionEventArgs i_ArgsHolder)
        {
            IGameComponent component = i_ArgsHolder.GameComponent;
            if (!m_constGameComponentCollection.Contains(component))
            {
                if (m_gameComponentCollection.Contains(component))
                {
                    m_gameComponentCollection.Remove(component);
                }

                if (m_gameDrawableComponentCollection.Contains(component))
                {
                    m_gameDrawableComponentCollection.Remove(component);
                }

                if (ComponentRemoved != null)
                {
                    ComponentRemoved.Invoke(component);
                }
            }
        }
    }
}
