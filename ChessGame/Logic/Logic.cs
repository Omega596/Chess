using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ChessGame.Logic;

public class Logic
{
    public List<(int, int)> GetLegalMoves(Base.Board board, (int, int) Position, Base.Piece.PieceColor ownColor)
    {
        if ((Position.Item1 > 7 || Position.Item2 > 7) || (Position.Item1 < 0 || Position.Item2 < 0))
        {
            return new List<(int, int)>();
        }
        List<(int, int)> Moves = new();
        List<(int, int, bool)> vectors = board.boardMatrix[Position.Item1, Position.Item2].Piece.GetPieceVector(board);
        bool repeating = board.boardMatrix[Position.Item1, Position.Item2].Piece.pieceType != Base.Piece.PieceType.Pawn |
                         board.boardMatrix[Position.Item1, Position.Item2].Piece.pieceType != Base.Piece.PieceType.King |
                         board.boardMatrix[Position.Item1, Position.Item2].Piece.pieceType != Base.Piece.PieceType.Knight;
        if (repeating)
        {
            for (int n = 0; n < vectors.Count; ++n)
            {
                for (int j = 0; j < 8; ++j)
                {
                    (int, int, bool) currentVector = vectors[n];
                    if ((Position.Item1 + currentVector.Item1 >= 8 || Position.Item2 + currentVector.Item2 >= 8) 
                        || (Position.Item1 + currentVector.Item1 <= -1 || Position.Item2 + currentVector.Item2 <= -1))
                    {
                        break;
                    }
                    if (board.boardMatrix[Position.Item1 + currentVector.Item1, Position.Item2 + currentVector.Item2].Piece
                            .pieceColor == ownColor)
                    {
                        continue;
                    }
                    Moves.Add((Position.Item1 + currentVector.Item1, Position.Item2 + currentVector.Item2));
                    if (board.boardMatrix[Position.Item1, Position.Item2].Piece.pieceType !=
                        Base.Piece.PieceType.None)
                    {
                        break;
                    }
                }
            }
        }
        else
        {
            for (int n = 0; n < vectors.Count; ++n)
            {
                (int, int, bool) currentVector = vectors[n];
                if ((Position.Item1 + currentVector.Item1 >= 8 || Position.Item2 + currentVector.Item2 >= 8)
                    || (Position.Item1 + currentVector.Item1 <= -1 || Position.Item2 + currentVector.Item2 <= -1))
                {
                    continue;
                }

                if (board.boardMatrix[Position.Item1 + currentVector.Item1, Position.Item2 + currentVector.Item2].Piece
                        .pieceColor == ownColor)
                {
                    continue;
                }
                Moves.Add((Position.Item1 + currentVector.Item1, Position.Item2 + currentVector.Item2));
            }
        }

        return Moves;
    }

    public (int, int) FindHoveredSquare()
    {
        (int, int) Position;
        int X = Mouse.GetState().X;
        int Y = Mouse.GetState().Y;

        Position.Item1 = X / 60;
        Position.Item2 = Y / 60;
        return Position;
    }
}