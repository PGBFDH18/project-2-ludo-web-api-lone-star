using System;

namespace Ludo.GameLogic
{
    public interface ISession
    {
        // A new turn has begun. (Informational: Not accepting input.)
        event EventHandler<TurnBegunEventArgs> TurnBegun;
        // The CurrentPlayer is passing their turn. (Turn has not passed yet!) (Informational: Not accepting input.)
        event EventHandler PassingTurn;
        // The CurrentPlayer is moving a piece. (The piece has not moved yet!) (Informational: Not accepting input.)
        event EventHandler<MovingPieceEventArgs> MovingPiece;
        // A piece has been knocked out by another piece (and thus moved back to its base). (Informational: Not accepting input.)
        //event EventHandler<TODO> PieceKnockedOut;
        // A player has won the game! (Informational: Not accepting input.)
        event EventHandler WinnerDeclared;

        // Started accepting input. (Useful for implementing bots!)
        // IMPORTANT: Remaining subscribers are NOT invoked if a subscriber supplies input during invokation.
        event EventHandler AcceptingInput;
        // Calling MovePiece and PassTurn is only valid while this is true.
        bool IsAcceptingInput { get; }

        // Has the game started? (Need to call Start method?)
        bool HasStarted { get; }
        // Checks if a slot index is valid for the game board. (See BoardInfo.SlotCount)
        bool IsBoardSlot(int slot);
        // Tries to add a player to the slot.
        // Fails if the game has already started, or the slot is occupied / out of range.
        bool TryAddPlayer(int slot);
        // Checks if a slot is occupied.
        bool IsSlotOccupied(int slot);
        // Starts the game, or returns false.
        // GUARANTEE: Returns true only once, even if called from multiple threads.
        // WARNING: It is not guaranteed to mutate internal state atomically; you still need a lock.
        bool Start();

        // The current turn number (i.e. a counter for how many times the die has been rolled).
        // When creating a new game this is zero until the game has been started.
        int TurnCounter { get; }
        // (Loading a game also loads the old turn counter.)
        bool IsLoadedFromSavegame { get; }
        
        // The slot index of the current player.
        int CurrentSlot { get; }
        // The current die roll.
        int CurrentDieRoll { get; }

        // The number of pieces the current player has in their base.
        int InBaseCount { get; }
        // The number of pieces the current player has reach the goal with.
        int InGoalCount { get; }
        // True if the current player has at least one piece that can move. (GetPiece(x).CanMove == TRUE for some x)
        bool CanMove { get; }
        // Typically only true if no piece can move. (Unless house-rules are applied that allow passing anyway.)
        bool CanPass { get; }
        // True if the current player has another move after their current move.
        bool IsLucky { get; }

        // Static info about the board (size etc.)
        BoardInfo BoardInfo { get; }
        // Number of players. (Invariant: PlayerCount <= BoardInfo.SlotCount)
        int PlayerCount { get; }
        // Number of pieces per player.
        int PieceCount { get; }
        // Who won? (SlotIndex 0-3 or -1 if no one has won yet.)
        int Winner { get; }

        // Get info about piece [0-3] for the current player.
        PieceInfo GetPiece(int piece);
        // Same as GetPiece(piece).CanMove without returning the whole struct.
        bool CanMovePiece(int piece);
        // Move piece [0-3] for the current player (and proceed to the next turn / roll die).
        void MovePiece(int piece);
        // Call this to pass the turn to the next player, or if lucky, simply re-roll the die.
        void PassTurn();

        // Get the board position of any piece.
        int GetPiecePosition(int player, int piece);
        // Get the piece at a board position [1 - BoardInfo.Length] (or NULL if position is empty).
        SlotPiece? LookAtBoard(int position);

        // Returns the current gamestate.
        LudoSave GetSave();

        // Returns the current boardstate.
        int[][] CopyBoardState();
    }
}
