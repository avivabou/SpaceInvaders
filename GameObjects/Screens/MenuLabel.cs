using System;
using GameObjects.GameProperties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameObjects.Screens
{
    public class MenuLabel : DrawableGameComponent
    {
        private GameSettings m_gameSettings;
        private SpriteFont m_consolas;
        private Vector2 m_selfLocation = Vector2.Zero;
        private Color m_neturalColor = Color.Yellow;
        private Color m_selectedColor = Color.Red;
        private string m_text;
        private float m_baseScale, m_changinScale;
        private float m_breathDirection;
        private Vector2 m_baseLocation;
        private bool m_isSelected = false;
        private Point m_lastMouseLocation;

        private Color Color
        {
            get
            {
                Color color = m_neturalColor;
                if (IsSelected)
                {
                    color = m_selectedColor;
                }

                return color;
            }
        }

        public Vector2 Measuring
        {
            get
            {
                return m_consolas.MeasureString(m_text);
            }
        }

        public Rectangle LabelRectangle
        {
            get
            {
                Vector2 size = m_baseScale * Measuring;
                Vector2 start = m_baseLocation;
                return new Rectangle((int)start.X, (int)start.Y, (int)size.X, (int)size.Y);
            }
        }

        public bool IsSelected
        {
            get
            {
                return m_isSelected;
            }

            set
            {
                m_isSelected = value;
                if (value == false)
                {
                    stopBreathingWEffect();
                }
            }
        }

        /// <summary>
        /// MenuLabel constructor.
        /// </summary>
        /// <param name="i_Game">Game.</param>
        /// <param name="i_Text">Label text.</param>
        public MenuLabel(Game i_Game, string i_Text)
            : base(i_Game)
        {
            m_text = i_Text;
        }

        /// <summary>
        /// Initialize MenuLabel.
        /// </summary>
        public override void Initialize()
        {
            m_consolas = Game.Content.Load<SpriteFont>(@"Fonts\Consolas");
            m_gameSettings = Game.Services.GetService(typeof(GameSettings)) as GameSettings;
        }

        /// <summary>
        /// Drawing the menu label.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        public override void Draw(GameTime i_GameTime)
        {
            SpriteBatch spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            float rotaion = 0;
            Vector2 origin = Vector2.Zero;
            float layerDepth = 0;
            spriteBatch.DrawString(m_consolas, m_text, m_selfLocation, Color, rotaion, origin, m_changinScale, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        /// Update the menu label.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        public override void Update(GameTime i_GameTime)
        {
            if (Enabled)
            {
                MouseState mouseState = Mouse.GetState();
                if (mouseState.Position != m_lastMouseLocation)
                {
                    if (m_gameSettings.MouseVisibilty == eVisible.Visible)
                    {
                        setSelectedIfMouseIntersects(mouseState);
                    }

                    m_lastMouseLocation = mouseState.Position;
                }
            }

            if (IsSelected)
            {
                applyBreathingEffect(i_GameTime);
            }
        }

        /// <summary>
        /// Set the proportion of label.
        /// </summary>
        /// <param name="i_Location">Location of on screen.</param>
        /// <param name="i_Scale">Scale of label.</param>
        public void SetProportion(Vector2 i_Location, float i_Scale)
        {
            m_selfLocation = i_Location;
            m_baseScale = i_Scale;
            m_changinScale = i_Scale;
            m_baseLocation = i_Location;
            m_breathDirection = 0.2f * i_Scale;
        }

        /// <summary>
        /// Set colors of label.
        /// </summary>
        /// <param name="i_NeturalColor">Default color.</param>
        /// <param name="i_SelectedColor">While selected color.</param>
        public void SetColors(Color i_NeturalColor, Color i_SelectedColor)
        {
            m_neturalColor = i_NeturalColor;
            m_selectedColor = i_SelectedColor;
        }

        /// <summary>
        /// Change the text of the label.
        /// </summary>
        /// <param name="i_Text">New Text.</param>
        public void ChangeText(string i_Text)
        {
            m_text = i_Text;
        }

        /// <summary>
        /// Applying breathing effect.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        private void applyBreathingEffect(GameTime i_GameTime)
        {
            m_changinScale += (float)i_GameTime.ElapsedGameTime.TotalSeconds * m_breathDirection;
            m_selfLocation.X = m_baseLocation.X - ((m_changinScale - m_baseScale) * Measuring.X / 2);
            m_selfLocation.Y = m_baseLocation.Y - ((m_changinScale - m_baseScale) * Measuring.Y / 2);
            if ((m_changinScale >= 1.05f * m_baseScale) || (m_changinScale <= 0.95f * m_baseScale))
            {
                m_breathDirection *= -1;
            }
        }

        /// <summary>
        /// Reset scale and location to unselected properties.
        /// </summary>
        private void stopBreathingWEffect()
        {
            m_changinScale = m_baseScale;
            m_selfLocation = m_baseLocation;
        }

        /// <summary>
        /// Define label selection according to mouse location.
        /// </summary>
        private void setSelectedIfMouseIntersects(MouseState i_MouseState)
        {
            if (LabelRectangle.Contains(i_MouseState.Position))
            {
                IsSelected = true;
            }
            else
            {
                IsSelected = false;
            }
        }
    }
}
