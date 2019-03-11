using Ludo.API.Models;
using Ludo.API.Service.Extensions;
using System;
using System.Linq;

namespace Ludo.API.Service
{
    public class IngamePhase : IGamePhase
    {
        private readonly GameLogic.ISession session;
        private readonly ISlotArray slots;
        private readonly object sessionLocker = new object();

        public IngamePhase(ISlotArray slots)
        {
            var s = new SlotArray(slots); // <-- makes a copy
            this.slots = s;
            session = GameLogic.SessionFactory.New();

            for (int i = 0; i < s.Length; ++i)
                if (s[i] != null)
                    session.TryAddPlayer(i); // should never fail here.
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
                                ? new PlayerPiece
                                {
                                    Player = pi.Collision.Value.Slot,
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
            int slot = slots.IndexOf(userId); // pre-lock search
            if (slot != -1)
                lock (sessionLocker)
                {
                    if (slots[slot] == userId) // post-lock check
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

        #region --- IGameStateSession ---
        GamePhase IGamePhase.Phase => GamePhase.ingame;

        SetupPhase IGamePhase.Setup => null;

        IngamePhase IGamePhase.Ingame => this;

        FinishedPhase IGamePhase.Finished => null;

        string IGamePhase.Winner => null;
        #endregion
    }
}
