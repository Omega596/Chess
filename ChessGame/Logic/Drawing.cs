using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChessGame.Logic;

public class Drawing
{
    public void DrawBoard(SpriteBatch _spriteBatch, List<Texture2D> Sprites, Base.Board Board)
    {
        if (Board.boardMatrix == null)
        {
            return;
        }
        _spriteBatch.Begin();
        
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Vector2 Vector;
                
                Vector.X = Board.boardMatrix[i, j].ClickBox.X;
                Vector.Y = Board.boardMatrix[i, j].ClickBox.Y;

                _spriteBatch.Draw(Board.boardMatrix[i, j].Color == Color.Black ? Sprites[1] : Sprites[0], Vector, Color.White);
            }
        }
        
        _spriteBatch.End();
    }
    public void DrawPieces(SpriteBatch _spriteBatch, List<Texture2D> Sprites, Base.Board Board)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Vector2 Vector;
                Vector.X = Board.boardMatrix[i,j].ClickBox.X;
                Vector.Y = Board.boardMatrix[i,j].ClickBox.Y;

                _spriteBatch.Begin();
                
                switch (Board.boardMatrix[i, j].Piece.pieceType)
                {
                    case Base.Piece.PieceType.Pawn:
                        if (Board.boardMatrix[i, j].Piece.pieceColor == Base.Piece.PieceColor.Black)
                        {
                            _spriteBatch.Draw(Sprites[8], Vector, Color.White);
                            break;
                        }
                        _spriteBatch.Draw(Sprites[2], Vector, Color.White);
                        break;
                    case Base.Piece.PieceType.Bishop:
                        if (Board.boardMatrix[i, j].Piece.pieceColor == Base.Piece.PieceColor.Black)
                        {
                            _spriteBatch.Draw(Sprites[9], Vector, Color.White);
                            break;
                        }
                        _spriteBatch.Draw(Sprites[3], Vector, Color.White);
                        break;
                    case Base.Piece.PieceType.Knight:
                        if (Board.boardMatrix[i, j].Piece.pieceColor == Base.Piece.PieceColor.Black)
                        {
                            _spriteBatch.Draw(Sprites[10], Vector, Color.White);
                            break;
                        }
                        _spriteBatch.Draw(Sprites[4], Vector, Color.White);
                        break;
                    case Base.Piece.PieceType.Rook:
                        if (Board.boardMatrix[i, j].Piece.pieceColor == Base.Piece.PieceColor.Black)
                        {
                            _spriteBatch.Draw(Sprites[11], Vector, Color.White);
                            break;
                        }
                        _spriteBatch.Draw(Sprites[5], Vector, Color.White);
                        break;
                    case Base.Piece.PieceType.Queen:
                        if (Board.boardMatrix[i, j].Piece.pieceColor == Base.Piece.PieceColor.Black)
                        {
                            _spriteBatch.Draw(Sprites[12], Vector, Color.White);
                            break;
                        }
                        _spriteBatch.Draw(Sprites[6], Vector, Color.White);
                        break;
                    case Base.Piece.PieceType.King:
                        if (Board.boardMatrix[i, j].Piece.pieceColor == Base.Piece.PieceColor.Black)
                        {
                            _spriteBatch.Draw(Sprites[13], Vector, Color.White);
                            break;
                        }
                        _spriteBatch.Draw(Sprites[7], Vector, Color.White);
                        break;
                }
                
                _spriteBatch.End();
            }
        }
    }
}