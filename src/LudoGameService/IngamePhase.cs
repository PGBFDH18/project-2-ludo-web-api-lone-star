using Ludo.API.Models;
using Ludo.API.Service.Extensions;
using System.Linq;

namespace Ludo.API.Service
{
    public class IngamePhase : IGamePhase
    {
        private readonly GameLogic.ISession session;
        private readonly ISlotArray slots;
        private readonly object sessionLocker = new object();

        public IngamePhase(ISlotArray slots, int startingSlot = -1)
        {
            this.slots = new SlotArray(slots); // <-- makes a copy
            session = GameLogic.SessionFactory.New(playerCount: slots.PlayerCount, startingPlayer: startingSlot);
        }

        internal bool Start()
        {
            lock(sessionLocker)
                return session.Start();
        }

        public ISlotArray Slots => slots;

        public bool IsValidSlot(int slot)
            => unchecked((uint)slot < (uint)slots.Length);

        public TurnSlotDie GetCurrent()
        {
            lock (sessionLocker)
                return new TurnSlotDie
                {
                    Turn = session.TurnCounter,
                    Slot = session.CurrentPlayer,
                    Die = session.CurrentDieRoll
        };      }

        // returns null if 
        public TurnInfo TryGetTurnInfo(int slot)
        {
            lock (sessionLocker)
                return session.CurrentPlayer == slot
                    ? new TurnInfo
                    {
                        CanPass = session.CanPass,
                        IsLucky = session.IsLucky,
                        Pieces = session
                            .Loop(session.PieceCount, session.GetPiece)
                            .Select(pi => new PieceInfo
                            {
                                Distance = pi.CurrentDistance,
                                Position = pi.CurrentPosition,
                                Moved = pi.MovedPosition,
                                Collision = pi.Collision.HasValue
                                ? new PlayerPiece
                                {
                                    Player = pi.Collision.Value.Player,
                                    Piece = pi.Collision.Value.Piece,
                                } : null
                            }).ToArray()
                    } : null;
        }

        public BoardState GetBoardState()
        {
            lock (sessionLocker)
                return new BoardState(session.CopyBoardState());
        }

        #region --- IGameStateSession ---
        GamePhase IGamePhase.Phase => GamePhase.ingame;

        SetupPhase IGamePhase.Setup => null;

        IngamePhase IGamePhase.Ingame => this;

        FinishedPhase IGamePhase.Finished => null;

        string IGamePhase.Winner => null;
        #endregion
    }
}
