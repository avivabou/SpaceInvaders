using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameObjects.FightTools
{
    public class VisualSoulsAndScore : DrawableGameComponent
    {
        private Vector2 m_soulsLocation, m_scoreLocation;
        private HurtableSprite[] m_sprites;
        private SpriteFont m_consolasFont;
        private Color[] m_playersColors = new Color[] { Color.Blue, Color.Green };

        /// <summary>
        /// VisualSoulsAndScore constructor.
        /// </summary>
        /// <param name="i_Game">The game this component will live at.</param>
        /// <param name="i_SoulsLocation">Visual souls location on screen.</param>
        /// <param name="i_ScoreLocation">Visual score location on screen.</param>
        /// <param name="i_Sprites">The HurtableSprite should be viewed.</param>
        public VisualSoulsAndScore(Game i_Game, params HurtableSprite[] i_Sprites)
            : base(i_Game)
        {
            m_soulsLocation = new Vector2(Game.Window.ClientBounds.Width, 0);
            m_scoreLocation = new Vector2(16, 16);
            m_sprites = i_Sprites;
            Game.Window.ClientSizeChanged += window_ClientSizeChanged;
        }

        /// <summary>
        /// Initialize this visuals.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            m_consolasFont = Game.Content.Load<SpriteFont>(@"Fonts\Consolas");
        }

        /// <summary>
        /// Draw the score board and souls.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        public override void Draw(GameTime i_GameTime)
        {
            SpriteBatch spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            int y = 0;
            for (int i = 0; i < m_sprites.Length; i++)
            {
                printScore(spriteBatch, i, y);
                drawSouls(spriteBatch, m_sprites[i], y);
                y += (int)(m_sprites[i].ShapeRectangle.Height / 2 * 1.5f);
            }
        }

        /// <summary>
        /// Drawing the score board on screen.
        /// </summary>
        /// <param name="i_SpriteBatch">Sprite batch to draw with.</param>
        /// <param name="i_Index">The index of the HurtableSprite should be printed.</param>
        /// <param name="i_AdditionalY">The additional Y to the const location.</param>
        private void printScore(SpriteBatch i_SpriteBatch, int i_Index, int i_AdditionalY)
        {
            string scoreText = string.Format("Player {0}: {1}", i_Index + 1, (m_sprites[i_Index] as IScoreable).Score);
            Vector2 textLocation = m_scoreLocation;
            textLocation.Y += i_AdditionalY;
            i_SpriteBatch.DrawString(m_consolasFont, scoreText, textLocation, m_playersColors[i_Index % 2]);
        }

        /// <summary>
        /// Drawing the souls on screen.
        /// </summary>
        /// <param name="i_SpriteBatch">Sprite batch to draw with.</param>
        /// <param name="i_Sprite">The sprite that his souls should be drawed.</param>
        /// <param name="i_AdditionalY">The additional Y to the const location.</param>
        private void drawSouls(SpriteBatch i_SpriteBatch, HurtableSprite i_Sprite, int i_AdditionalY)
        {
            if (i_Sprite.Souls > 0)
            {
                float x_location = -i_Sprite.ShapeRectangle.Width / 2;
                float y_location = i_AdditionalY + (i_Sprite.ShapeRectangle.Height / 2);
                Vector2 location = new Vector2(x_location, y_location);
                location += m_soulsLocation;
                for (int j = 0; j < i_Sprite.Souls; j++)
                {
                    location.X -= i_Sprite.ShapeRectangle.Width / 2 * 1.2f;
                    i_SpriteBatch.Draw(i_Sprite.m_SelfTexture, location, i_Sprite.m_SourceRectangles[i_Sprite.m_RectangleIndex], Color.White * 0.5f, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                }
            }
        }

        /// <summary>
        /// Adapting location of GUI to the new window size.
        /// </summary>
        /// <param name="i_Sender">Sender.</param>
        /// <param name="i_ArgsHolder">Arguments holder.</param>
        private void window_ClientSizeChanged(object i_Sender, System.EventArgs i_ArgsHolder)
        {
            m_soulsLocation = new Vector2(Game.Window.ClientBounds.Width, 0);
        }
    }
}