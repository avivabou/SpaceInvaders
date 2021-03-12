using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameObjects.FightTools;
using GameObjects.Screens;

namespace Invaders
{
    public class Barrier : HurtableSprite
    {
        public static readonly int sr_BarrierWidth = 44;
        public static readonly int sr_BarrierHeight = 32;
        private static readonly string sr_texturePath = @"Sprites\Barrier_44x32";
        private static readonly float sr_touchDamage = 0.35f;
        private static readonly Color sr_transparent = Color.White * 0;
        private Color[] m_textureColors;
        private bool[,] m_transparentMap;
        private int m_transparentPixels = 0;

        /// <summary>
        /// Barrier constructor.
        /// </summary>
        /// <param name="i_GameScreen">The game this Barrier live at.</param>
        /// <param name="i_BarrierLocation">Location of the Barrier.</param>
        public Barrier(GameScreen i_GameScreen, Vector2 i_BarrierLocation)
            : base(i_GameScreen, sr_texturePath, Color.White, 0)
        {
            m_selfLocation = i_BarrierLocation;
            Souls = int.MaxValue;
            HurtInCase |= eHurtInCase.Touch;
            DrawOrder = -1;
            m_attackedSound = eSounds.BarrierHit;
        }

        /// <summary>
        /// Initialize the Barrier.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            m_textureColors = new Color[m_SelfTexture.Width * m_SelfTexture.Height];
            m_SelfTexture.GetData(m_textureColors);
            m_SelfTexture = new Texture2D(GraphicsDevice, m_SelfTexture.Width, m_SelfTexture.Height);
            m_SelfTexture.SetData(m_textureColors);
            initializeTransparentMap();
            m_selfLocation.Y -= m_SelfTexture.Height;
        }

        /// <summary>
        /// When the Barrier got attacked, clear the eaten pixels.
        /// </summary>
        /// <param name="i_Attacker">The IAttacker</param>
        protected override void attacked(IAttacker i_Attacker)
        {
            eatPixels(i_Attacker);
            m_SelfTexture.SetData((Color[])m_textureColors.Clone());
            updateSoulsForDeath();
            base.attacked(i_Attacker);
        }

        /// <summary>
        /// When the Barrier got touched, clear the touching pixels.
        /// </summary>
        /// <param name="i_IntersectionPoints">The intersection points</param>
        protected override void touched(List<Vector2> i_IntersectionPoints)
        {
            for (int i = 0; i < i_IntersectionPoints.Count; i++)
            {
                int x = (int)(i_IntersectionPoints[i].X - CurrentLocation.X);
                int y = (int)(i_IntersectionPoints[i].Y - CurrentLocation.Y);
                if ((x < sr_BarrierWidth) && (y < sr_BarrierHeight))
                {
                    if (!m_transparentMap[x, y])
                    {
                        int index = x + (y * m_SelfTexture.Width);
                        m_textureColors[index] = sr_transparent;
                        m_transparentPixels++;
                        m_transparentMap[x, y] = true;
                    }
                }
            }

            m_SelfTexture.SetData((Color[])m_textureColors.Clone());
            updateSoulsForDeath();
        }

        /// <summary>
        /// Eat intersection pixels.
        /// </summary>
        /// <param name="i_Attacker">The IAttacker.</param>
        private void eatPixels(IAttacker i_Attacker)
        {
            int direction;
            Vector2 minVector, maxVector;
            getEatMinMaxVectors(i_Attacker.ShapeRectangle, out minVector, out maxVector, out direction);
            for (int i = 0; i < maxVector.Y; i++)
            {
                bool transparentLine = true;
                for (int x = (int)minVector.X; x < maxVector.X; x++)
                {
                    int y = (int)minVector.Y + (i * direction);
                    if ((y < 0) || (y >= m_SelfTexture.Height))
                    {
                        i = (int)maxVector.Y + 1;
                        break;
                    }

                    if (!m_transparentMap[x, y])
                    {
                        int index = x + (y * m_SelfTexture.Width);
                        transparentLine = false;
                        m_transparentMap[x, y] = true;
                        m_textureColors[index] = sr_transparent;
                        m_transparentPixels++;
                    }
                }

                if (transparentLine)
                {
                    maxVector.Y++;
                }
            }
        }

        /// <summary>
        /// Initialize a boolean map which present any pixel if it  
        /// transparent or not.
        /// </summary>
        private void initializeTransparentMap()
        {
            m_transparentMap = new bool[m_SelfTexture.Width, m_SelfTexture.Height];
            for (int y = 0; y < m_SelfTexture.Height; y++)
            {
                for (int x = 0; x < m_SelfTexture.Width; x++)
                {
                    int index = x + (y * m_SelfTexture.Width);
                    m_transparentMap[x, y] = m_textureColors[index] == sr_transparent;
                    if (!m_transparentMap[x, y])
                    {
                        m_transparentPixels++;
                    }
                }
            }

            m_transparentPixels = m_textureColors.Length - m_transparentPixels;
        }

        /// <summary>
        /// If all of the pixels are transparent, the barrier should be dead.
        /// </summary>
        private void updateSoulsForDeath()
        {
            if (m_transparentPixels == m_textureColors.Length)
            {
                Souls = 1;
            }
        }

        /// <summary>
        /// Get the boundries for checking which pixel is should be eaten.
        /// </summary>
        /// <param name="i_AttackerRectangle">The attacker rectangle.</param>
        /// <param name="o_MinVector">Min vector boundary.</param>
        /// <param name="o_MaxVector">Max vector boundary</param>
        /// <param name="o_Direction">The direction of eating.</param>
        private void getEatMinMaxVectors(Rectangle i_AttackerRectangle, out Vector2 o_MinVector, out Vector2 o_MaxVector, out int o_Direction)
        {
            o_MaxVector.Y = sr_touchDamage * i_AttackerRectangle.Height;
            o_MinVector.X = MathHelper.Clamp(i_AttackerRectangle.Left - CurrentLocation.X, 0, m_SelfTexture.Width);
            o_MaxVector.X = MathHelper.Clamp(i_AttackerRectangle.Right - CurrentLocation.X, 0, m_SelfTexture.Width);
            o_Direction = 1;
            o_MinVector.Y = 0;
            if (i_AttackerRectangle.Top > ShapeRectangle.Top )
            {
                o_Direction = -1;
                o_MinVector.Y = m_SelfTexture.Height - 1;
            }
        }
    }
}
