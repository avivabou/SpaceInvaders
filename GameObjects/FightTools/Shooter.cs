using System;

namespace GameObjects.FightTools
{
    public class Shooter
    {
        private IAttacker[] m_attackers;
        private TimeSpan m_lastTimeAttacked;

        public TimeSpan ShotTimeInterval { private get; set; }

        /// <summary>
        /// Shooter cunstructor.
        /// </summary>
        /// <param name="i_MaxAttackers">Max IAttackers on screen.</param>
        /// <param name="i_ShotTimeInterval">Shot time interval.</param>
        public Shooter(int i_MaxAttackers, TimeSpan i_ShotTimeInterval)
        {
            m_attackers = new IAttacker[i_MaxAttackers];
            ShotTimeInterval = i_ShotTimeInterval;
        }

        /// <summary>
        /// Erasing all attackers.
        /// </summary>
        public void ClearShooter()
        {
            for (int i = 0; i < m_attackers.Length; i++)
            {
                m_attackers[i] = null;
            }
        }

        /// <summary>
        /// Try to shoot the given IAttacker.
        /// </summary>
        /// <param name="i_CurrentTime">Current time span.</param>
        /// <param name="i_Attacker">The IAttacker should be shoot</param>
        /// <returns>True if the shot were successed, false othwerwise.</returns>
        public bool Shoot(TimeSpan i_CurrentTime, IAttacker i_Attacker)
        {
            bool wasShoot = false;
            if (i_CurrentTime - m_lastTimeAttacked >= ShotTimeInterval)
            {
                for (int k = 0; k < m_attackers.Length; k++)
                {
                    if (m_attackers[k] == null)
                    {
                        m_attackers[k] = i_Attacker;
                        i_Attacker.ArrivedToEnd += clearAttackerSpace;
                        m_lastTimeAttacked = i_CurrentTime;
                        wasShoot = true;
                        break;
                    }
                }
            }

            return wasShoot;
        }

        /// <summary>
        /// Update the IAttacker array when a IAttacker were earased from the game screen.
        /// </summary>
        /// <param name="i_Attacker">The IAttacker which erased.</param>
        private void clearAttackerSpace(IAttacker i_Attacker)
        {
            for (int k = 0; k < m_attackers.Length; k++)
            {
                if (ReferenceEquals(m_attackers[k], i_Attacker))
                {
                    m_attackers[k] = null;
                    break;
                }
            }
        }
    }
}
