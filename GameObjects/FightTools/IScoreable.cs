namespace GameObjects.FightTools
{
    public interface IScoreable
    {
        int Score { get; }

        void GetAdditionalScore(int i_Score);
    }
}
