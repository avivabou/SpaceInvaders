using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameObjects.Screens;

namespace Invaders
{
    public class EnemiesMatrix : GameComponent
    {
        private static readonly int sr_deadEnemiesForSpeedIncreasing = 5;
        private static readonly float sr_deadEnemiesSpeedIncreasement = 1.03f;
        private static readonly float sr_downLineSpeedIncreasement = 1.05f;
        private Enemy[,] m_enemies;
        private Vector2 m_baseVectorForEnemiesLocation = new Vector2(0, Enemy.sr_EnemySize * 3);
        private int m_totalDeadEnemies = 0;
        private float m_jumpsForSecond;
        private int m_movementDirection = 1;
        private int m_lastEnemiesLine, m_lastEnemiesColumn;
        private int m_firstEnemiesColumn = 0;
        private double m_lastMove = 0;
        private bool m_alreadyTouchedWall = true;
        private GameScreen m_gameScreen;
        private bool m_isInitialized = false;

        /// <summary>
        /// Will invoke when there are no more anemies alive at the matrix.
        /// </summary>
        public event Action AllEnemiesDead;

        /// <summary>
        /// Will invoke when the lowest line of enemies arrived to the max Y.
        /// </summary>
        public event Action ArrivedMaxY;

        /// <summary>
        /// EnemiesMatrix constructor.
        /// </summary>
        /// <param name="i_GameScreen">The game the enemies matrix will live in.</param>
        /// <param name="i_Columns">Amount of enemies columns.</param>
        /// <param name="i_Rows">Amount of enemies rows.</param>
        /// <param name="i_MovementSpeed">Enemies movement speed.</param>
        public EnemiesMatrix(GameScreen i_GameScreen, int i_Columns, int i_Rows, float i_JumpsForSecond)
            : base(i_GameScreen.Game)
        {
            m_gameScreen = i_GameScreen;
            m_lastEnemiesLine = i_Rows - 1;
            m_lastEnemiesColumn = i_Columns - 1;
            m_jumpsForSecond = i_JumpsForSecond;
            m_enemies = new Enemy[i_Columns, i_Rows];
            int enemyMode = 1;
            for (int j = 0; j < i_Rows; j++)
            {
                enemyMode = 1 - enemyMode;
                for (int i = 0; i < i_Columns; i++)
                {
                    int enemyIndex = 0;
                    float x = (float)(i * 1.6 * Enemy.sr_EnemySize);
                    float y = (float)(j * 1.6 * Enemy.sr_EnemySize);
                    Vector2 currentVector = new Vector2(x, y);
                    if (j >= 2 * i_Rows / 3)
                    {
                        enemyIndex = 2;
                    }
                    else if (j >= i_Rows / 3)
                    {
                        enemyIndex = 1;
                    }

                    m_enemies[i, j] = new Enemy(i_GameScreen, enemyIndex, currentVector, enemyMode);
                    m_enemies[i, j].BeenKilled += updateLastLine;
                    m_enemies[i, j].BeenKilled += updateColumnsBounderies;
                    m_enemies[i, j].SetZeroVector(m_baseVectorForEnemiesLocation);
                }
            }
        }

        /// <summary>
        /// Initialize EnemiesMatrix components.
        /// </summary>
        public override void Initialize()
        {
            if (!m_isInitialized)
            {
                m_isInitialized = true;
                foreach (Enemy enemy in m_enemies)
                {
                    m_gameScreen.AddComponents(enemy);
                    enemy.BeenKilled += () => enemyDead(enemy);
                }
            }
        }

        /// <summary>
        /// Update the EnemiesMatrix.
        /// </summary>
        /// <param name="i_GameTime">Game time.</param>
        public override void Update(GameTime i_GameTime)
        {
                double currentTime = i_GameTime.TotalGameTime.TotalSeconds;
                if (currentTime - m_lastMove >= 1 / m_jumpsForSecond)
                {
                    m_lastMove = i_GameTime.TotalGameTime.TotalSeconds;
                    float minX = -1.6f * Enemy.sr_EnemySize * m_firstEnemiesColumn;
                    float maxX = Game.Window.ClientBounds.Width - Enemy.sr_EnemySize;
                    maxX -= 1.6f * Enemy.sr_EnemySize * m_lastEnemiesColumn;
                    if (((m_baseVectorForEnemiesLocation.X == minX) || (m_baseVectorForEnemiesLocation.X == maxX)) && !m_alreadyTouchedWall)
                    {
                        m_alreadyTouchedWall = true;
                        enemiesLevelDown();
                    }
                    else
                    {
                        m_alreadyTouchedWall = false;
                        float movement = 0.5f * Enemy.sr_EnemySize * m_movementDirection;
                        m_baseVectorForEnemiesLocation.X += movement;
                        m_baseVectorForEnemiesLocation.X = MathHelper.Clamp(m_baseVectorForEnemiesLocation.X, minX, maxX);
                    }

                    foreach (Enemy enemy in m_enemies)
                    {
                        enemy.SetZeroVector(m_baseVectorForEnemiesLocation);
                    }
                }
        }

        /// <summary>
        /// Change the base vector for enemies location to be lower on screen.
        /// Raising the movement speed by sr_downLineSpeedIncreasement and sweep the movement direction.
        /// </summary>
        private void enemiesLevelDown()
        {
            if (m_totalDeadEnemies < m_enemies.Length)
            {
                m_baseVectorForEnemiesLocation.Y += Enemy.sr_EnemySize / 2;
                m_jumpsForSecond *= sr_downLineSpeedIncreasement;
                m_movementDirection *= -1;
                checkForArrivingMaxY();
            }
        }

        /// <summary>
        /// Updating the last line index.
        /// </summary>
        private void updateLastLine()
        {
            bool allDead = true;
            for (int i = 0; i < m_enemies.GetLength(0); i++)
            {
                allDead &= m_enemies[i, m_lastEnemiesLine].IsDead;
                if (!allDead)
                {
                    break;
                }
            }

            if (allDead)
            {
                m_lastEnemiesLine--;
                if (m_lastEnemiesLine < 0)
                {
                    if (AllEnemiesDead != null)
                    {
                        AllEnemiesDead.Invoke();
                    }
                }
                else
                {
                    updateLastLine();
                }
            }
        }

        /// <summary>
        /// Updating the first and last columns indices.
        /// </summary>
        private void updateColumnsBounderies()
        {
            bool allDeadFirst = true;
            bool allDeadLast = true;
            for (int i = 0; i < m_enemies.GetLength(1); i++)
            {
                allDeadFirst &= m_enemies[m_firstEnemiesColumn, i].IsDead;
                allDeadLast &= m_enemies[m_lastEnemiesColumn, i].IsDead;
                if (!allDeadFirst && !allDeadLast)
                {
                    break;
                }
            }

            if (allDeadFirst || allDeadLast)
            {
                if (allDeadFirst)
                {
                    m_firstEnemiesColumn++;
                }

                if (allDeadLast)
                {
                    m_lastEnemiesColumn--;
                }

                if (m_firstEnemiesColumn > m_lastEnemiesColumn)
                {
                    if (AllEnemiesDead != null)
                    {
                        AllEnemiesDead.Invoke();
                    }
                }
                else
                {
                    updateColumnsBounderies();
                }
            }
        }

        /// <summary>
        /// Checking if there is a collision with the spaceships Y location.
        /// If there is, the action ArrivedMaxY will be invoke.
        /// </summary>
        private void checkForArrivingMaxY()
        {
            float highest_y = m_baseVectorForEnemiesLocation.Y;
            highest_y += (float)((m_lastEnemiesLine + 1) * 1.6 * Enemy.sr_EnemySize);
            if (highest_y > Spaceship.SpaceshipsY)
            {
                m_jumpsForSecond = 0;
                if (ArrivedMaxY != null)
                {
                    ArrivedMaxY.Invoke();
                }
            }
        }

        /// <summary>
        /// Get known for the dead enemy.
        /// </summary>
        /// <param name="i_enemy">The enemy which is dead.</param>
        private void enemyDead(Enemy i_enemy)
        {
            m_totalDeadEnemies++;
            if (m_totalDeadEnemies % sr_deadEnemiesForSpeedIncreasing == 0)
            {
                m_jumpsForSecond *= sr_deadEnemiesSpeedIncreasement;
            }
        }
    }
}