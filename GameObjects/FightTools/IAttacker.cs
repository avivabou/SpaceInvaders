using System;

namespace GameObjects.FightTools
{
    public interface IAttacker : ITeamableShape
    {
        event Action<IAttacker> ArrivedToEnd;

        void Destroy(int i_Score);

        void Kill(IHurtable i_Hurtable);
    }
}
