using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameObjects.Animations;
using GameObjects.Screens;

namespace GameObjects
{
    public class Sprite : DrawableGameComponent
    {
        protected internal AnimationsLibrary m_AnimationsLibrary = new AnimationsLibrary();
        protected internal Texture2D m_SelfTexture;
        protected internal Rectangle[] m_SourceRectangles;
        protected internal int m_RectangleIndex = 0;
        protected internal Color m_TextureColor = Color.White;
        protected Vector2 m_selfLocation;
        protected Vector2 m_zeroPoint = Vector2.Zero;
        protected Vector2? m_screenProportion = null;
        protected bool m_isIinitialized = false;
        protected string m_animationName = string.Empty;
        private string m_texturePath;

        protected internal Vector2? Position { get; set; } = null;

        protected internal Vector2 Origin { get; set; }

        protected internal Vector2 Scale { get; set; } = Vector2.One;

        protected internal float Rotation { get; set; } = 0;

        protected internal float Tint { get; set; } = 1;

        public virtual Rectangle ShapeRectangle
        {
            get
            {
                Point location = new Point((int)(m_zeroPoint.X + m_selfLocation.X), (int)(m_zeroPoint.Y + m_selfLocation.Y));
                Point proportion;
                if (m_screenProportion.HasValue)
                {
                    Vector2 screenProportion = (Vector2)m_screenProportion;
                    proportion = new Point((int)(Game.Window.ClientBounds.Width * screenProportion.X), (int)(Game.Window.ClientBounds.Height * screenProportion.Y));
                }
                else
                {
                    proportion = new Point(m_SourceRectangles[m_RectangleIndex].Width, m_SourceRectangles[m_RectangleIndex].Height);
                }

                return new Rectangle(location, proportion);
            }
        }

        public GameScreen GameScreen { get; set; }

        public Color[] ObjectColors
        {
            get
            {
                Color[] colors = new Color[m_SelfTexture.Width * m_SelfTexture.Height];
                m_SelfTexture.GetData(colors);
                return colors;
            }
        }

        public Vector2 CurrentLocation
        {
            get
            {
                return m_zeroPoint + m_selfLocation;
            }
        }

        /// <summary>
        /// Sprite constructor.
        /// </summary>
        /// <param name="i_GameScreen">The game this sprite will live at.</param>
        /// <param name="i_TexturePath">The path of the the texture which present this sprite.</param>
        /// <param name="i_Color">The color which will cover the texture.</param>
        /// <param name="i_ScreenProportion">Define the size of the sprite by given screen proportion.</param>
        public Sprite(GameScreen i_GameScreen, string i_TexturePath, Color i_Color, Vector2? i_ScreenProportion)
            : base(i_GameScreen.Game)
        {
            GameScreen = i_GameScreen;
            m_texturePath = i_TexturePath;
            m_TextureColor = i_Color;
            DrawOrder = 0;
            Visible = true;
            m_screenProportion = i_ScreenProportion;
        }

        /// <summary>
        /// Initialize sprite components.
        /// </summary>
        public override void Initialize()
        {
            if (!m_isIinitialized)
            {
                m_isIinitialized = true;
                m_SelfTexture = Game.Content.Load<Texture2D>(m_texturePath);
                if (m_SourceRectangles == null)
                {
                    m_SourceRectangles = new Rectangle[1];
                    m_SourceRectangles[0] = new Rectangle(0, 0, m_SelfTexture.Width, m_SelfTexture.Height);
                }

                Origin = new Vector2(m_SourceRectangles[m_RectangleIndex].Width / 2, m_SourceRectangles[m_RectangleIndex].Height / 2);
            }
        }

        /// <summary>
        /// Draw the sprite.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        public override void Draw(GameTime i_GameTime)
        {
            SpriteBatch spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            if (m_AnimationsLibrary.IsOnRun)
            {
                Vector2 position = CurrentLocation + Origin;
                if (Position.HasValue)
                {
                    position = Position.Value;
                }

                if (Visible)
                {
                    spriteBatch.Draw(m_SelfTexture, position, m_SourceRectangles[m_RectangleIndex], m_TextureColor * Tint, Rotation, Origin, Scale, SpriteEffects.None, 0);
                }
            }
            else if (Visible)
            {
                spriteBatch.Draw(m_SelfTexture, ShapeRectangle, m_SourceRectangles[m_RectangleIndex], m_TextureColor * Tint);
            }
        }

        /// <summary>
        /// Updating the sprite.
        /// If there is animation on runing, apply it.
        /// </summary>
        /// <param name="i_GameTime"></param>
        public override void Update(GameTime i_GameTime)
        {
            if ((m_AnimationsLibrary != null) && m_AnimationsLibrary.IsOnRun)
            {
                m_AnimationsLibrary.Update(i_GameTime);
            }
        }

        /// <summary>
        /// Set a base vector to start from there (such as defining a new zero vector).
        /// </summary>
        /// <param name="i_ZeroVector">A base vector.</param>
        public virtual void SetZeroVector(Vector2 i_ZeroVector)
        {
            m_zeroPoint = i_ZeroVector;
        }

        /// <summary>
        /// Applying the quit animation (such as dying animation).
        /// </summary>
        public void ApplyAnimation()
        {
            SpriteAnimation animation = m_AnimationsLibrary.GetAnimation(m_animationName);
            if (animation != null)
            {
                m_AnimationsLibrary.Start();
                animation.Finished += turnOffAnimation;
            }
        }

        /// <summary>
        /// Turn off the quit animation (end of the animation).
        /// </summary>
        protected virtual void turnOffAnimation()
        {
            m_AnimationsLibrary.Reset();
        }
    }
}