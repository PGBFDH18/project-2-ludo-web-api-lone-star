using Ludo.API.Models;
using Ludo.API.Service.Extensions;
using System;
using System.Linq;

namespace Ludo.API.Service
{
    public partial class IngamePhase : IGamePhase
    {
        private readonly GameLogic.ISession session;
        private readonly object sessionLocker = new object();

        // NEVER MODIFY THESE OUTSIDE OF SESSION EVENT LISTENERS!
        private readonly TurnCache[] turnCache; // cyclic buffer
        private int turnCacheIndex = -1; // current buffer index

        private TurnCache CurrentTurn
            => turnCacheIndex == -1 ? null : turnCache[turnCacheIndex];

        public IngamePhase(ISlotArray slots)
        {
            var s = new SlotArray(slots); // <-- makes a copy
            Slots = s;
            session = GameLogic.SessionFactory.New();

            int players = 0;
            for (int i = 0; i < s.Length; ++i)
                if (s[i] != null)
                {
                    ++players;
                    session.TryAddPlayer(i); // should never fail here.
                }

            turnCache = new TurnCache[players * 2];
            session.TurnBegun += Session_TurnBegun;
            session.WinnerDeclared += Session_WinnerDeclared;
        }

        public ISlotArray Slots { get; }
        public bool HasStarted => session.HasStarted;
        public bool IsLoadedFromSave => session.IsLoadedFromSavegame;

        internal bool Start()
        {
            lock(sessionLocker)
                return session.Start();
        }

        internal event Action<IngamePhase> WinnerDeclared;

        private void Session_WinnerDeclared(object sender, EventArgs e)
            => WinnerDeclared?.Invoke(this);

        private void Session_TurnBegun(object sender, GameLogic.TurnBegunEventArgs e)
        {
            // Since this is the only code that updates this array and
            // we ASSUME that session is handled correctly elsewhere...
            // so no race condition on the event trigger...
            // ...then we should not need a lock here.
            // (Importantly, reference assignment is atomic in .Net)

            int i = turnCacheIndex + 1 % turnCache.Length;
            turnCache[i] = new TurnCache(e, session.CurrentSlot);
            turnCacheIndex = i;
        }

        public bool IsValidSlot(int slot)
            => unchecked((uint)slot < (uint)Slots.Length);

        public bool IsValidPiece(int piece)
            => unchecked((uint)piece < (uint)session.PieceCount);

        // Who won? (SlotIndex 0-3 or -1 if no one has won yet.)
        public int Winner => session.Winner;

        public TurnSlotDie GetCurrent()
        {
            lock (sessionLocker)
                return new TurnSlotDie
                {
                    Turn = session.TurnCounter,
                    Slot = session.CurrentSlot,
                    Die = session.CurrentDieRoll
        };      }

        // returns null if 
        public TurnInfo TryGetTurnInfo(int slot)
        {
            lock (sessionLocker)
                return session.CurrentSlot == slot
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
                                ? new SlotPiece
                                {
                                    Slot = pi.Collision.Value.Slot,
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

        public bool TryPassTrun(int slot)
        {
            lock (sessionLocker)
                return (session.CurrentSlot == slot && session.CanPass)
                    .OnTrue(session.PassTurn);
        }

        // remainingUserCount is -1 if no change occurred.
        internal void Concede(string userId, out int remainingUserCount)
        {
            int slot = Slots.IndexOf(userId); // pre-lock search
            if (slot != -1)
                lock (sessionLocker)
                {
                    if (Slots[slot] == userId) // post-lock check
                    {
                        //TODO/FIXME
                        // 1. change slot to reflect a concede*
                        // 2. set remainingUserCount
                        // 3. if rUC < 2, return;
                        // 4. change slot to reflect that it's a bot*
                        // 5. create bot
                        // 6. attach bot as listener to session events
                        // 7. make sure the bot acts now if needed
                         //*TODO: how to handle this? Redesign required!
                         // Want to remember the old userId AND see the bot.
                        throw new NotImplementedException("Ingame.Concede");
                    }
                }
            remainingUserCount = -1;
        }

        // returns (0,-1) if the game has not started yet.
        // returns (t,-1) if piece is out of range.
        public (int turn, int slot) GetPieceInfo(int piece, out PieceInfo pieceInfo)
        {
            pieceInfo = null;
            // must grab a local reference to our cache object to be safe:
            TurnCache turn = CurrentTurn;
            if (turn == null) // game not started yet.
                return (0,-1);
            if (unchecked((uint)piece >= (uint)session.PieceCount))
                return (turn.Turn, -1);
            pieceInfo = turn.pieces[piece];
            return (turn.Turn, turn.Slot);

        }

        public Error MovePiece(int slot, int piece)
        {
            if (!IsValidSlot(slot))
                return Error.Codes.E10InvalidSlotIndex;
            if (!IsValidPiece(piece))
                return Error.Codes.E18InvalidPieceIndex;
            if (Slots[slot] == null)
                return Error.Codes.E16SlotIsEmpty;
            lock (sessionLocker)
            {
                if (session.CurrentSlot != slot || !session.IsAcceptingInput)
                    return Error.Codes.E15NotYourTurn;
                if (!session.CanMovePiece(piece))
                    return Error.Codes.E19GameRuleViolation;
                session.MovePiece(piece);
            }
            return Error.Codes.E00NoError;
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
