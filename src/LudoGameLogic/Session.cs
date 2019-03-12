using Ludo.GameLogic.Extensions;
using System;
using System.Linq;
using System.Threading;

namespace Ludo.GameLogic
{
    // var försiktiga så ni inte blandar ihop distance och position!
    // NOT thread safe!
    internal class Session : ISession
    {
        // special constants:
        const int NON_BOARD_POSITION = -1; // base or goal position (distance tells them apart)
        const int NOT_STARTED = -2;
        const int RANDOM_START = int.MinValue;
        const int PIECE_COUNT = 4; // pieces per player

        // special rules TODO:
        const bool ALLOW_STACKING = false; // 'true' not implemented
        const bool ALLOW_UNLIMITED_ROLL_6 = true; // 'false' not implemented

        // <ctors>

        // ctor (new game)
        internal Session(BoardInfo boardInfo, Rules rules)
        {
            IsLoadedFromSavegame = false;
            pieceDistances = new int[boardInfo.SlotCount][];
            currentPieces = new PieceInfo[PIECE_COUNT];
            BoardInfo = boardInfo;
            Rules = rules;
        }

        // ctor (load game)
        internal Session(LudoSave save) // TODO: check that the save is valid?
        {
            IsLoadedFromSavegame = true;
            TurnCounter = save.TurnCounter;
            pieceDistances = save.pieceDistances.JaggedCopy();
            currentPieces = new PieceInfo[PIECE_COUNT];
            BoardInfo = new BoardInfo(save.BoardLength);
            Rules = save.Rules;
            CurrentSlot = save.CurrentSlot;
            CurrentDieRoll = save.CurrentDieRoll;
        }
        
        public bool IsBoardSlot(int slot)
            => unchecked((uint)slot < (uint)pieceDistances.Length);

        public bool TryAddPlayer(int slot)
            => !IsLoadedFromSavegame && IsBoardSlot(slot)
            && Interlocked.CompareExchange(ref pieceDistances[slot], new int[PIECE_COUNT], null) == null;

        public bool IsSlotOccupied(int slot)
            => pieceDistances[slot] != null;

        // returns false if already started
        public bool Start()
        {
            if (Interlocked.CompareExchange(ref _winner, -1, NOT_STARTED) != NOT_STARTED)
                return false;
            if (!IsLoadedFromSavegame)
            {
                RollDie();
                CurrentSlot = RandomNonEmptySlot();
                TurnCounter = 1;
            }
            ComputePieceInfo();
            InvokeInitialEvents();
            return true;
        }

        // </ctors> <events>

        public event EventHandler<TurnBegunEventArgs> TurnBegun;

        public event EventHandler PassingTurn;

        public event EventHandler<MovingPieceEventArgs> MovingPiece;

        public event EventHandler WinnerDeclared;

        public event EventHandler AcceptingInput;

        //</events> <public>

        public bool IsLoadedFromSavegame { get; }
        public bool HasStarted => _winner != NOT_STARTED;
        public bool IsAcceptingInput { get; private set; }
        public int TurnCounter { get; private set; }
        public int CurrentSlot { get; private set; } = -1;
        public int CurrentDieRoll { get; private set; } = -1;
        public int InBaseCount { get; private set; }
        public int InGoalCount { get; private set; }
        public int Winner => _winner < 0 ? -1 : _winner;
        public BoardInfo BoardInfo { get; }
        public Rules Rules { get; }

        public int PlayerCount
            => pieceDistances.Count(p => p != null);

        public int PieceCount
            => PIECE_COUNT;

        public PieceInfo GetPiece(int piece)
            => currentPieces[piece];

        public bool CanMovePiece(int piece)
            => currentPieces[piece].CanMove;

        public bool CanMove { get; private set; }
        public bool CanPass => !CanMove; // TODO: house-rules
        
        public void MovePiece(int piece)
        {
            if (!IsAcceptingInput)
                throw new InvalidOperationException("Session is not in an input accepting state!");
            if (!CanMovePiece(piece))
                throw new LudoRuleException("Rules does not allow the current player to " + $"move piece #{piece}.");
            BlockInput();
            if (currentPieces[piece].IsInBase)
                MoveBasePiece(piece);
            else
                MoveBoardPiece(piece);
        }

        public void PassTurn()
        {
            if (!IsAcceptingInput)
                throw new InvalidOperationException("Session is not in an input accepting state!");
            if (!CanPass)
                throw new LudoRuleException("Rules does not allow the current player to " + "pass the turn.");
            BlockInput();
            OnPassingTurn();
            NextTurn();
        }

        public int GetPiecePosition(int slot, int piece)
            => CalculatePosition(slot, piece);

        public SlotPiece? LookAtBoard(int position)
        {
            if (position >= 0 && position < BoardInfo.GoalPosition(3)) // quick range check.
                for (int slot = 0; slot < pieceDistances.Length; ++slot) // loop over slots...
                    if (IsSlotOccupied(slot)) // skip empty slots...
                        for (int piece = 0; piece < PIECE_COUNT; ++piece) // loop over pieces...
                            if (CalculatePosition(slot, piece) == position) // ...find match.
                                return new SlotPiece(slot, piece);
            return null;
        }

        public LudoSave GetSave()
            => new LudoSave(TurnCounter, CurrentSlot, CurrentDieRoll, BoardInfo.Length, Rules, pieceDistances);

        public bool IsLucky
            => CurrentDieRoll == 6; // TODO: implement rule that limits re-rolls to max three moves in a row.

        public int[][] CopyBoardState()
            => pieceDistances.JaggedCopy();

        // </public>  <protected>

        protected virtual void OnTurnBegun(TurnBegunEventArgs e = null)
            => TurnBegun?.Invoke(this, e ?? new TurnBegunEventArgs(TurnCounter, currentPieces));

        protected virtual void OnPassingTurn(EventArgs e = null)
            => PassingTurn?.Invoke(this, e ?? EventArgs.Empty);

        protected virtual void OnMovingPiece(MovingPieceEventArgs e)
            => MovingPiece?.Invoke(this, e);

        protected virtual void OnWinnerDeclared(EventArgs e = null)
            => WinnerDeclared?.Invoke(this, e ?? EventArgs.Empty);

        protected virtual void OnAcceptingInput(EventArgs e = null)
        {
            if (e == null)
                e = EventArgs.Empty;
            int tc = TurnCounter;
            var ai = AcceptingInput;
            if(ai != null)
                foreach (EventHandler eh in ai.GetInvocationList())
                {
                    eh(this, e);
                    if (tc != TurnCounter || !IsAcceptingInput)
                        return;
                }
        }

        // also returns true for all pieces of empty slots
        protected bool IsPieceInBase(int piece)
            => pieceDistances[CurrentSlot].IsNull(out var p) || p[piece] == 0;

        protected bool IsPieceInGoal(int piece)
            => pieceDistances[CurrentSlot]?[piece] == BoardInfo.GoalDistance;

        protected bool IsBaseRoll
            => CurrentDieRoll == 6 || (CurrentDieRoll == 1 && Rules.AllowBaseExitOnRoll1);

        protected int RandomNonEmptySlot()
        {
            var slots = pieceDistances;
            int player = random.Next() % slots.Count(p => p != null);
            for (int slot = 0; slot < slots.Length; ++slot)
                if (slots[slot] != null && player-- == 0)
                        return slot;
            throw new Exception("Doh!"); // should be unreachable
        }

        // </protected>  <private>

        private void InvokeInitialEvents()
        {
            OnTurnBegun();
            AcceptInput();
        }

        private void NextTurn()
        {
            if (!IsLucky)
                CurrentSlot = NextPlayerSlot();
            RollDie();
            ComputePieceInfo();
            ++TurnCounter;
            OnTurnBegun();
            AcceptInput();

            int NextPlayerSlot()
            {
                int cs = CurrentSlot;
                do cs = (cs + 1) % BoardInfo.SlotCount;
                while (!IsSlotOccupied(cs));
                return cs;
            }
        }

        private void RollDie()
        {
            CurrentDieRoll = random.Next(1, 7);
        }

        private void AcceptInput()
        {
            IsAcceptingInput = true;
            OnAcceptingInput();
        }

        private void BlockInput()
        {
            IsAcceptingInput = false;
        }

        private void MoveBasePiece(int piece)
        {
            if (pieceDistances[CurrentSlot].IsNull(out var p))
                throw new Exception("Invalid slot - there be race conditions?");
            OnMovingPiece(new MovingPieceEventArgs(piece));
            p[piece] = 1;
            HandleMoveCollision(piece);
            NextTurn();
        }

        private void MoveBoardPiece(int piece)
        {
            if (pieceDistances[CurrentSlot].IsNull(out var p))
                throw new Exception("Invalid slot - there be race conditions?");
            OnMovingPiece(new MovingPieceEventArgs(piece));
            int distance = p[piece] + CurrentDieRoll;
            if (distance > BoardInfo.GoalDistance) // Piece moved too far - it bounces back. (Rules.AllowGoalBouncing)
            {
                p[piece] = Bounce(distance);
            }
            else
            {
                p[piece] = distance;
                if (CheckVictoryCondition())
                    return; // <-- Game finished !!!
            }
            HandleMoveCollision(piece);
            NextTurn();
        }

        private int Bounce(int distance)
            => BoardInfo.GoalDistance * 2 - distance;

        private void HandleMoveCollision(int piece)
        {
            if (currentPieces[piece].Collision is SlotPiece ci)
            {
                if (ci.Slot != CurrentSlot)
                    KnockOut(ci);
            }
        }

        private void KnockOut(SlotPiece pp)
        {
            pieceDistances[pp.Slot][pp.Piece] = 0;
        }

        private int CalculatePosition(int slot, int piece)
        {
            int distance = (pieceDistances[slot]?[piece]).GetValueOrDefault();
            if (distance == 0 || distance == BoardInfo.GoalDistance)
            {
                // piece is in base or in goal.
                return NON_BOARD_POSITION;
            }
            if (BoardInfo.IsInEndZone(distance))
            {
                // we are in a collision-free end-zone.
                return distance + slot * BoardInfo.EndZoneLength;
            }
            else
            {
                // we are out on the competative board where collisions are possible!
                return (BoardInfo.StartPosition(slot) + distance - 1) % BoardInfo.Length;
            }
        }
        
        private int CalculateNewPosition(int slot, int piece)
        {
            if (pieceDistances[slot].IsNull(out var p))
                return NON_BOARD_POSITION; // empty slot... throw?
            int distance = p[piece] + CurrentDieRoll;
            if (distance == BoardInfo.GoalDistance)
                return NON_BOARD_POSITION; // goal
            if (distance > BoardInfo.GoalDistance)
            {
                if (Rules.AllowGoalBouncing)
                    distance = Bounce(distance);
                else
                    distance -= CurrentDieRoll; // we can not move, so return where we are currently.
            }
            if (BoardInfo.IsInEndZone(distance))
            {
                return distance + slot * BoardInfo.EndZoneLength; // end-zone
            }
            else
            {
                // we are out on the competative board where collisions are possible!
                return (BoardInfo.StartPosition(slot) + distance - 1) % BoardInfo.Length;
            }
        }

        private bool CheckVictoryCondition()
        {
            int goal = BoardInfo.GoalDistance;
            if (currentPieces.All(p => p.CurrentDistance == goal))
            {
                _winner = CurrentSlot;
                // update all PieceInfo so IsInGoal is true:
                for (int i = 0; i < currentPieces.Length; ++i)
                    currentPieces[i] = new PieceInfo(goal, NON_BOARD_POSITION);
                CanMove = false;
                // Game has ended! (no further state changes should be allowed!)
                OnWinnerDeclared();
                return true;
            }
            return false;
        }

        // here we do the heavy lifting! (checking the rules and updating PieceInfo!)
        private void ComputePieceInfo()
        {
            // these are updated by ComputePieceInfo(i)
            InBaseCount = 0;
            InGoalCount = 0;
            CanMove = false;

            // cache'ar resultat här så vi slipper räkna ut base-exit reglerna flera ggr:
            PieceInfo? baseExitInfo = null;

            for (int i = 0; i < PIECE_COUNT; ++i)
                ComputePieceInfo(i);

            // << här tar ComputePieceInfo() metoden slut, koden under är "bara" lokala hjälpmetoder >>

            void ComputePieceInfo(int piece)
            {
                if (IsPieceInBase(piece)) // (distance == 0)
                {
                    ++InBaseCount;
                    if (baseExitInfo == null) // räkna ut och cache'a värdet om det inte finns...
                        ComputeBaseExitInfo();
                    currentPieces[piece] = baseExitInfo.Value; // <-- använd cache'ade värdet.
                }
                else if (IsPieceInGoal(piece)) // (distance == BoardInfo.GoalDistance)
                {
                    ++InGoalCount;
                    currentPieces[piece] = new PieceInfo(BoardInfo.GoalDistance, NON_BOARD_POSITION);
                }
                else if (ALLOW_STACKING)
                {
                    throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                    // TODO: ...
                }
                else
                {
                    ComputeBoardPieceInfo(piece);
                }
            }

            void ComputeBoardPieceInfo(int piece)
            {
                int oldDistance = pieceDistances[CurrentSlot][piece];
                int newDistance = oldDistance + CurrentDieRoll;
                int oldPosition = CalculatePosition(CurrentSlot, piece);
                if (newDistance == BoardInfo.GoalDistance)
                {
                    currentPieces[piece] = new PieceInfo(oldDistance, oldPosition, NON_BOARD_POSITION); // goal!
                    CanMove = true;
                    return; // <---
                }
                if (newDistance > BoardInfo.GoalDistance)
                {
                    if (Rules.AllowGoalBouncing)
                    {
                        newDistance = Bounce(newDistance);
                    }
                    else
                    {
                        currentPieces[piece] = new PieceInfo(oldDistance, oldPosition); // cant move.
                        return; // <---
                    }
                }
                int newPosition = CalculateNewPosition(CurrentSlot, piece);
                if (LookAtBoard(newPosition) is SlotPiece pp)
                {
                    //^ the new position collides with something...
                    if (pp.Slot == CurrentSlot)
                    {
                        //^ the new position collides with one of our own pieces
                        if (ALLOW_STACKING)
                        {
                            throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                            // TODO: ...
                        }
                        else
                        {
                            currentPieces[piece] = new PieceInfo(oldDistance, oldPosition, pp); // cant move.
                        }
                    }
                    else
                    {
                        //^ new position collides with another players piece
                        if (ALLOW_STACKING)
                        {
                            throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                            // TODO: ... check if it is a double piece
                        }
                        else
                        {
                            //^ we can kill it!
                            currentPieces[piece] = new PieceInfo(oldDistance, oldPosition, newPosition, pp);
                            CanMove = true;
                        }
                    }
                }
                else
                {
                    //^ new position is empty / no collision
                    currentPieces[piece] = new PieceInfo(oldDistance, oldPosition, newPosition);
                    CanMove = true;
                }
            }

            void ComputeBaseExitInfo()
            {
                if (IsBaseRoll)
                {
                    int startPosition = BoardInfo.StartPosition(CurrentSlot);
                    if (LookAtBoard(startPosition) is SlotPiece collider)
                    {
                        //^ another piece is occupying our startPosition...
                        if (collider.Slot == CurrentSlot)
                        {
                            //^ we already have a piece on our startingPosition...
                            if (ALLOW_STACKING)
                            {
                                throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                                // TODO: ...
                            }
                            else
                            {
                                baseExitInfo = new PieceInfo(0, NON_BOARD_POSITION); // we can not move out of base!
                            }
                        }
                        else
                        {
                            //^ another players piece is on our startPosition...
                            if (ALLOW_STACKING)
                            {
                                throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                                // TODO: check if the collison target is stacked / a double-piece
                            }
                            else
                            {
                                //^ we can kill it!
                                baseExitInfo = new PieceInfo(0, NON_BOARD_POSITION, startPosition, collider);
                                CanMove = true;
                            }
                        }
                    }
                    else
                    {
                        //^ startPosition is empty / no collision...
                        baseExitInfo = new PieceInfo(0, NON_BOARD_POSITION, startPosition);
                        CanMove = true;
                    }
                }
                else // (isBaseRoll == false)
                {
                    baseExitInfo = new PieceInfo(0, NON_BOARD_POSITION); // we can not move out of base!
                }
            }
        }

        // <fields>

        // how far each piece has moved. [slot, piece]
        // inner array is null if slot is empty.
        private readonly int[][] pieceDistances;
        private readonly PieceInfo[] currentPieces;
        private readonly Random random = new Random();
        private int _winner = NOT_STARTED; // NOT_STARTED (-2): game not started, -1: no winner yet.
    }
}
