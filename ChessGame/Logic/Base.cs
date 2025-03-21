using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace ChessGame.Logic;

static class PieceExtensions
{
    public static List<(int, int, bool)> GetPieceVector(this Base.Piece piece, Base.Board board)
    {
        Base.Piece targetPiece = piece;
        var (x, _) = piece.Position;
        //    X    Y    CaptureFlag
        List<(int, int, bool)> vectors = new List<(int, int, bool)>();
        switch (targetPiece.pieceType)
        {
            default:
                return vectors;
            case Base.Piece.PieceType.Pawn:
                if (x == 1 && targetPiece.pieceColor == Base.Piece.PieceColor.Black)
                {
                    vectors.AddRange(new List<(int, int, bool)> { (0, -1, false), (0, -2, false), (1, -1, true), (-1, -1, true) });
                    return vectors;
                }

                if (x == 6 && targetPiece.pieceColor == Base.Piece.PieceColor.White)
                {
                    vectors.AddRange(new List<(int, int, bool)> { (0, 1, false), (0, 2, false), (-1, 1, true), (1, 1, true) });
                    return vectors;
                }

                if (targetPiece.pieceColor == Base.Piece.PieceColor.Black)
                {
                    vectors.AddRange(new List<(int, int, bool)> { (0, -1, false), (-1, -1, true), (1, -1, true) });
                    return vectors;
                }

                if (targetPiece.pieceColor == Base.Piece.PieceColor.White)
                {
                    vectors.AddRange(new List<(int, int, bool)> { (0, 1, false), (-1, 1, true), (1, 1, true) });
                    return vectors;
                }
                return vectors;
            case Base.Piece.PieceType.Bishop:
                vectors.AddRange(new List<(int, int, bool)> { (1, 1, true), (-1, 1, true), (1, -1, true), (-1, -1, true) });
                return vectors;
            case Base.Piece.PieceType.Knight:
                vectors.AddRange(new List<(int, int, bool)> { (1, -2, true), (1, 2, true), (-1, -2, true), (-1, 2, true), (-2, 1, true), (-2, -1, true), (2, -1, true), (2, 1, true) });
                return vectors;
            case Base.Piece.PieceType.Rook:
                vectors.AddRange((new List<(int, int, bool)> { (0, 1, true), (1, 0, true), (0, -1, true), (-1, 0, true) }));
                return vectors;
            case Base.Piece.PieceType.Queen:
                vectors.AddRange((new List<(int, int, bool)> { (0, 1, true), (1, 0, true), (0, -1, true), (-1, 0, true), (-1, -1, true), (1, -1, true), (-1, 1, true), (1, 1, true)}));
                return vectors;
            case Base.Piece.PieceType.King:
                vectors.AddRange((new List<(int, int, bool)> { (0, 1, true), (1, 0, true), (0, -1, true), (-1, 0, true), (-1, -1, true), (1, -1, true), (-1, 1, true), (1, 1, true) }));
                return vectors;
        }
    }
}

public static class MouseInput
{
    public static bool LeftClicked = false;

    public static bool RightClicked = false;
    
    private static MouseState ms = new MouseState(), oms;

    public static void Update()
    {
        oms = ms;
        ms = Mouse.GetState();
        LeftClicked = ms.LeftButton != ButtonState.Pressed && oms.LeftButton == ButtonState.Pressed;
        
        RightClicked = ms.RightButton != ButtonState.Pressed && oms.RightButton == ButtonState.Pressed;
    }
}
public class Base
{
    public class Piece
    {
        public (int, int) Position;
        public PieceColor pieceColor;
        public PieceType pieceType;

        public enum PieceColor
        {
            White,
            Black,
            None
        }
        public enum PiecePrice
        {
            None,
            Pawn = 1,
            Knight = 3,
            Bishop = 3,
            Rook = 5,
            Queen = 9,
            King
        }

        public enum PieceType
        {
            None,
            Pawn,
            Knight,
            Bishop,
            Rook,
            Queen,
            King
        }
    }
    public class Square
    {
        public Rectangle ClickBox;
        public Color Color;
        public Piece Piece;

        public Square(Point Position, Piece.PieceType PieceType)
        {
            ClickBox.Size = new Point(60);
            ClickBox.Location = Position;
            Piece = new Piece
            {
                pieceType = PieceType
            };
        }
    }

    public struct Board
    {
        public Square[,] boardMatrix = new Square[8, 8];

        public int MoveCount = 0;

        public int NoCaptureMoveCount = 0;

        public bool[] CastleAllowed = new bool[4] { true, true, true, true };
        
        public Piece.PieceColor ColorsTurn;

        public int[] FromChessNotation(string notation)
        {
            Dictionary<char, int> NotationToCoord = new Dictionary<int, char>()
            {
                { 1, 'a' },
                { 2, 'b' },
                { 3, 'c' },
                { 4, 'd' },
                { 5, 'e' },
                { 6, 'f' },
                { 7, 'g' },
                { 8, 'h' }
            }.ToDictionary(x => x.Value, x => x.Key);
            int[] result = new int[4];
            char[] notationchararray = notation.ToCharArray();
            result[0] = NotationToCoord[notationchararray[0]];
            result[1] = int.Parse(notationchararray[1].ToString());
            result[2] = NotationToCoord[notationchararray[2]];
            result[3] = int.Parse(notationchararray[3].ToString());
            return result;
        }

        public void Move(Board board, (int, int) ToPos, (int, int) FromPos)
        {
            board.boardMatrix[ToPos.Item1, ToPos.Item2].Piece.pieceType = board.boardMatrix[FromPos.Item1, FromPos.Item2].Piece.pieceType;
            board.boardMatrix[ToPos.Item1, ToPos.Item2].Piece.pieceColor = board.boardMatrix[FromPos.Item1, FromPos.Item2].Piece.pieceColor;
            board.boardMatrix[FromPos.Item1, FromPos.Item2].Piece.pieceType = Base.Piece.PieceType.None;
            board.boardMatrix[FromPos.Item1, FromPos.Item2].Piece.pieceColor = Piece.PieceColor.None;
        }
        public Board(string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {
            Board board = FENParser(FEN);
            MoveCount = board.MoveCount;

            NoCaptureMoveCount = board.NoCaptureMoveCount;

            CastleAllowed = board.CastleAllowed;

            ColorsTurn = board.ColorsTurn;

            if (FEN.Length == 0)
            {
                for (int i = 0; i < 8; ++i)
                {
                    for (int j = 0; j < 8; ++j)
                    {

                        Point Position;
                        Position.X = i * 60;
                        Position.Y = j * 60;
                        boardMatrix[i, j] = new Square(Position, Piece.PieceType.None);
                        if (j is 0 or 7)
                        {
                            if (i is 0 or 7)
                            {
                                boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Rook;
                            }
                            if (i is 1 or 6)
                            {
                                boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Knight;
                            }
                            if (i is 2 or 5)
                            {
                                boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Bishop;
                            }
                            if (i == 3)
                            {
                                boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Queen;
                            }
                            if (i == 4)
                            {
                                boardMatrix[i, j].Piece.pieceType = Piece.PieceType.King;
                            }
                        }
                        if (j is 1 or 6)
                        {
                            boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Pawn;
                        }

                        if (j is 0 or 1)
                        {
                            boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.White;
                        }
                        else boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.Black;
                    }

                }

                // Square color assigning
                for (int i = 0; i < 8; i = i + 2)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        int whitei = j % 2 == 0 && j != 0 ? i + 1 : i;
                        int blacki = j % 2 != 0 && j != 0 ? i : i + 1;
                        boardMatrix[whitei, j].Color = Color.White;
                        boardMatrix[blacki, j].Color = Color.Black;
                    }
                }
            }
            else boardMatrix = FENParser(FEN).boardMatrix;
        }
        public Board(bool[] castleAllowed, Piece.PieceColor colorsTurn, int moveCount = 0, int noCaptureMoveCount = 0)
        {
            MoveCount = moveCount;

            NoCaptureMoveCount = noCaptureMoveCount;

            CastleAllowed = castleAllowed;

            ColorsTurn = colorsTurn;

            boardMatrix = new Square[8,8];
            
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {

                    Point Position;
                    Position.X = i * 60;
                    Position.Y = j * 60;
                    boardMatrix[i, j] = new Square(Position, Piece.PieceType.None);
                    if (j is 0 or 7)
                    {
                        if (i is 0 or 7)
                        {
                            boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Rook;
                        }

                        if (i is 1 or 6)
                        {
                            boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Knight;
                        }
                        if (i is 2 or 5)
                        {
                            boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Bishop;
                        }

                        if (i is 3)
                        {
                            boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Queen;
                        }

                        if (i is 4)
                        {
                            boardMatrix[i, j].Piece.pieceType = Piece.PieceType.King;
                        }
                    }

                    if (j is 1 or 6)
                    { 
                        boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Pawn;
                    }
                    if (j is 0 or 1)
                    {
                            boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.White;
                    }

                    if (j is 6 or 7)
                    {
                        boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.Black;
                    }
                    if (j is 5 or 4 or 3 or 2 ) boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.None;
                }
            }
            // Square color assigning
            for (int i = 0; i < 8; i = i + 2)
            {
                for (int j = 0; j < 8; ++j)
                {
                    int whitei = j % 2 == 0 && j != 0 ? i + 1 : i;
                    int blacki = j % 2 != 0 && j != 0 ? i : i + 1;
                    boardMatrix[whitei, j].Color = Color.White;
                    boardMatrix[blacki, j].Color = Color.Black;
                }
            }
        }
        }

        public static Board FENParser(string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {
            Board board = new(new bool[4] {true, true, true, true}, Piece.PieceColor.White);
            int j = 0;
            int i = 0;
            bool reachedflags = false;
            bool reachedcounters = false;
            for (int stringi = 0; stringi < FEN.Length; stringi++)
            {
                if (!reachedflags)
                {
                    if (int.TryParse(FEN[stringi].ToString(), out var result))
                    {
                        for (int ii = 0; ii < result; ii++)
                        {
                            board.boardMatrix[ii, j].Piece.pieceType = Piece.PieceType.None;
                            board.boardMatrix[ii, j].Piece.pieceColor = Piece.PieceColor.None;
                        }
                    }
                    switch (FEN[stringi])
                    {
                        case '\0': break;
                        case 'r':
                            board.boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Rook;
                            board.boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.Black;
                            i++;
                            break;
                        case 'R':
                            board.boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Rook;
                            board.boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.White;
                            i++;
                            break;
                        case 'p':
                            board.boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Pawn;
                            board.boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.Black;
                            i++;
                            break;
                        case 'P':
                            board.boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Pawn;
                            board.boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.White;
                            i++;
                            break;
                        case 'k':
                            board.boardMatrix[i, j].Piece.pieceType = Piece.PieceType.King;
                            board.boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.Black;
                            i++;
                            break;
                        case 'K':
                            board.boardMatrix[i, j].Piece.pieceType = Piece.PieceType.King;
                            board.boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.White;
                            i++;
                            break;
                        case 'n':
                            board.boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Knight;
                            board.boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.Black;
                            i++;
                            break;
                        case 'N':
                            board.boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Knight;
                            board.boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.White;
                            i++;
                            break;
                        case 'b':
                            board.boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Bishop;
                            board.boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.Black;
                            i++;
                            break;
                        case 'B':
                            board.boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Bishop;
                            board.boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.White;
                            i++;
                            break;
                        case 'q':
                            board.boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Queen;
                            board.boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.Black;
                            i++;
                            break;
                        case 'Q':
                            board.boardMatrix[i, j].Piece.pieceType = Piece.PieceType.Queen;
                            board.boardMatrix[i, j].Piece.pieceColor = Piece.PieceColor.White;
                            i++;
                            break;
                        case '/':
                            j++;
                            i = 0;
                            break;
                        case ' ':
                            reachedflags = true;
                            break;
                    }
                }
                if (reachedflags)
                {
                    switch (FEN[stringi])
                    {
                        case '\0': break;
                        case 'w':
                            board.ColorsTurn = Piece.PieceColor.White; break;
                        case 'b':
                            board.ColorsTurn = Piece.PieceColor.Black; break;
                        case 'K':
                            board.CastleAllowed[0] = true;
                            if (char.IsWhiteSpace(FEN[stringi + 1]))
                            {
                                reachedcounters = true; break;
                            }
                            break;
                        case 'k':
                            board.CastleAllowed[2] = true;
                            if (char.IsWhiteSpace(FEN[stringi + 1]))
                            {
                                reachedcounters = true; break;
                            }
                            break;
                        case 'Q':
                            board.CastleAllowed[1] = true;
                            if (char.IsWhiteSpace(FEN[stringi + 1]))
                            {
                                reachedcounters = true; break;
                            }
                            break;
                        case 'q':
                            board.CastleAllowed[3] = true;
                            if (char.IsWhiteSpace(FEN[stringi + 1]))
                            {
                                reachedcounters = true; break;
                            }
                            break;
                    }
                }
                if (reachedcounters)
                {
                    board.MoveCount = int.Parse(FEN[stringi + 1].ToString());
                    board.NoCaptureMoveCount = int.Parse(FEN[stringi + 3].ToString());
                    return board;
                }
                return board;
            }
            return board;
    }
    
}