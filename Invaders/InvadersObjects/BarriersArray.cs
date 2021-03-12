using Microsoft.Xna.Framework;
using GameObjects.Screens;

namespace Invaders
{
    public class BarriersArray : GameComponent
    {
        private static readonly int sr_movementSpeed = 35;
        private Barrier[] m_barriers;
        private Vector2 m_baseVectorForBarriersLocation;
        private float m_movementSpeed;
        private int m_movementDirection = 1;
        private GameScreen m_gameScreen;
        private bool m_isInitialized = false;
        private float m_totalWidth, m_midX;

        /// <summary>
        /// BarriersArray constructor.
        /// </summary>
        /// <param name="i_GameScreen">The game this BarriersArray will live it.</param>
        /// <param name="i_Count">The amount of Barriers in the array.</param>
        public BarriersArray(GameScreen i_GameScreen, int i_Count, int i_Level)
            : base(i_GameScreen.Game)
        {
            m_gameScreen = i_GameScreen;
            m_barriers = new Barrier[i_Count];
            Game.Window.ClientSizeChanged += window_ClientSizeChanged;
            if (i_Level == 0)
            {
                m_movementSpeed = 0;
            }
            else
            {
                m_movementSpeed = (1 + ((i_Level - 1) * 0.06f)) * sr_movementSpeed;
            }

            for (int i = 0; i < i_Count; i++)
            {
                float x = (float)(i * 2.3 * Barrier.sr_BarrierWidth);
                Vector2 currentVector = new Vector2(x, 0);
                m_barriers[i] = new Barrier(i_GameScreen, currentVector);
                m_totalWidth = x + Barrier.sr_BarrierWidth;
            }
        }

        /// <summary>
        /// Initialize the BarriersArray.
        /// </summary>
        public override void Initialize()
        {
            if (!m_isInitialized)
            {
                m_isInitialized = true;
                m_gameScreen.AddComponents(m_barriers);
                m_midX = (Game.Window.ClientBounds.Width - m_totalWidth) / 2;
                m_baseVectorForBarriersLocation.X = m_midX;
                m_baseVectorForBarriersLocation.Y = Game.Window.ClientBounds.Height - Spaceship.sr_SpaceshipSize - Barrier.sr_BarrierHeight;
                foreach (Barrier barrier in m_barriers)
                {
                    barrier.SetZeroVector(m_baseVectorForBarriersLocation);
                }
            }
        }

        /// <summary>
        /// Update the BarriersArray.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        public override void Update(GameTime i_GameTime)
        {
            float minX = m_midX - (Barrier.sr_BarrierWidth / 2);
            float maxX = m_midX + (Barrier.sr_BarrierWidth / 2);
            m_baseVectorForBarriersLocation.X += (float)(m_movementSpeed * i_GameTime.ElapsedGameTime.TotalSeconds * m_movementDirection);
            m_baseVectorForBarriersLocation.X = MathHelper.Clamp(m_baseVectorForBarriersLocation.X, minX, maxX);
            if ((m_baseVectorForBarriersLocation.X == minX) || (m_baseVectorForBarriersLocation.X == maxX))
            {
                m_movementDirection *= -1;
            }

            foreach (Barrier barrier in m_barriers)
            {
                barrier.SetZeroVector(m_baseVectorForBarriersLocation);
            }
        }

        private void window_ClientSizeChanged(object sender, System.EventArgs e)
        {
            m_midX = (Game.Window.ClientBounds.Width - m_totalWidth) / 2;
            m_baseVectorForBarriersLocation.Y = Game.Window.ClientBounds.Height - Spaceship.sr_SpaceshipSize - Barrier.sr_BarrierHeight;
        }
    }
}
