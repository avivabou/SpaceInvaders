using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Invaders.GameScreens;
using GameObjects.GameProperties;
using GameObjects.Managers;

namespace Invaders
{
    public class InvadersGame : Game
    {
        private SpriteBatch m_spriteBatch;
        private GraphicsDeviceManager m_graphics;
        private ScreensManager m_screensManager;

        /// <summary>
        /// InvadersGame constructor.
        /// </summary>
        public InvadersGame()
        {
            m_graphics = new GraphicsDeviceManager(this);
            m_graphics.PreferredBackBufferWidth = 795;
            m_graphics.PreferredBackBufferHeight = 550;
            m_graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            Services.AddService(typeof(GraphicsDeviceManager), m_graphics);

            GameSettings invadersSetting = new GameSettings(this);
            Services.AddService(typeof(GameSettings), invadersSetting);
            invadersSetting.ImportSounds(typeof(eSounds), eSounds.BGMusic);

            m_screensManager = new ScreensManager(this, endGame);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            LoadContent();
            WelcomeScreen welcomeScreen = new WelcomeScreen(m_screensManager);
            m_screensManager.MoveTo(welcomeScreen);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), m_spriteBatch);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="i_GameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime i_GameTime)
        {
            m_screensManager.Update(i_GameTime);
            this.Window.Title = Components.Count.ToString();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="i_GameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime i_GameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            m_screensManager.Draw(i_GameTime);
        }

        /// <summary>
        /// End of the game.
        /// </summary>
        private void endGame()
        {
            Environment.Exit(0);
            Exit();
        }
    }
}
