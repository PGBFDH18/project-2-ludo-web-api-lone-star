namespace Ludo.GameService
{
    public class FinishedSession : IGameStateSession, ISharedGSS
    {
        private readonly IUserIdArray players;

        public FinishedSession(byte winner, IUserIdArray players)
        {
            WinnerSlot = winner;
            this.players = new UserArray(players);
        }

        // index of the winning slot.
        public byte WinnerSlot { get; }

        public string WinnerId => players[WinnerSlot];

        IUserIdArray ISharedGSS.Slots => players;

        #region --- IGameStateSession ---
        GameState IGameStateSession.State => GameState.finished;

        SetupSession IGameStateSession.Setup => null;

        IngameSession IGameStateSession.Ingame => null;

        FinishedSession IGameStateSession.Finished => this;

        ISharedGSS IGameStateSession.Shared => this;
        #endregion
    }
}
