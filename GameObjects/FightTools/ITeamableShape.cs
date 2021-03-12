namespace GameObjects.FightTools
{
    public interface ITeamableShape : IIntersectable
    {
        eTeam SelfTeam { get; }
    }
}
