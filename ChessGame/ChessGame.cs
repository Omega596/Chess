using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChessGame.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChessGame;

public class ChessGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public Base.Board Board = new(new bool[4] {true, true, true, true}, Base.Piece.PieceColor.White); 
    public List<Texture2D> Sprites;
    public Dictionary<string, int> SpriteDict;
    public StockfishManager sfm = new();
    private int[] ponder = new int[4];
    private Dictionary<int, char> CoordToNotation = new()
    {
        { 1, 'a' },
        { 2, 'b' },
        { 3, 'c' },
        { 4, 'd' },
        { 5, 'e' },
        { 6, 'f' },
        { 7, 'g' },
        { 8, 'h' }
    };
    ((int, int), bool) clickInfo;
    bool fromPieceSelected;
    (int, int) fromPos;
    
    public Base.Piece.PieceColor TurnColor;
    public Base.Piece.PieceColor OwnColor;
    public ChessGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Logic.Logic logic = new Logic.Logic();
        clickInfo = ((1,1), false);
        SpriteDict = new Dictionary<string, int>()
        {
            {"square", 0},
            {"darkSquare", 1},
            {"pawn", 2},
            {"bishop", 3},
            {"knight", 4},
            {"rook", 5},
            {"queen", 6},
            {"king", 7},
            {"darkPawn", 8},
            {"darkBishop", 9},
            {"darkKnight", 10},
            {"darkRook", 11},
            {"darkQueen", 12},
            {"darkKing", 13},
            {"captureMoveHighlight", 14},
            {"moveHighlight", 15},
        };
        base.Initialize();
        sfm.Execute(@"/home/libc6/Projects/Rider/Chess/ChessGame/Logic/stockfish/stockfish-ubuntu-x86-64-sse41-popcnt");
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Sprites = new()
        {
            Content.Load<Texture2D>("square"), // 1
            Content.Load<Texture2D>("darkSquare"), // 2
            Content.Load<Texture2D>("pawn"), // 3
            Content.Load<Texture2D>("bishop"), // 4
            Content.Load<Texture2D>("knight"), // 5
            Content.Load<Texture2D>("rook"), // 6
            Content.Load<Texture2D>("queen"), // 7
            Content.Load<Texture2D>("king"), // 8
            Content.Load<Texture2D>("darkPawn"), // 9
            Content.Load<Texture2D>("darkBishop"), // 10
            Content.Load<Texture2D>("darkKnight"), // 11
            Content.Load<Texture2D>("darkRook"), // 12
            Content.Load<Texture2D>("darkQueen"), // 13
            Content.Load<Texture2D>("darkKing"), //14
            Content.Load<Texture2D>("captureMoveHighlight"), // 15
            Content.Load<Texture2D>("moveHighlight") // 16
        };
    }
    protected override void Update(GameTime gameTime)
    {
        MouseInput.Update();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        Logic.Logic logic = new Logic.Logic();
        TurnColor = Base.Piece.PieceColor.Black;
        OwnColor = Base.Piece.PieceColor.Black;
        if ((clickInfo.Item1.Item1 <= 7 && clickInfo.Item1.Item2 <= 7) && (clickInfo.Item1.Item1 >= 0 && clickInfo.Item1.Item2 >= 0))
        {
            if (MouseInput.LeftClicked && fromPieceSelected && TurnColor == Base.Piece.PieceColor.Black)
            {
                (int, int) toPos = logic.FindHoveredSquare();
                if (toPos != fromPos && 
                    Board.boardMatrix[fromPos.Item1, fromPos.Item2].Piece.pieceType != Base.Piece.PieceType.None &&
                    logic.GetLegalMoves(Board, fromPos, Board.boardMatrix[fromPos.Item1, fromPos.Item2].Piece.pieceColor).Contains(toPos))
                {
                    Board.Move(Board, toPos, fromPos);
                    fromPieceSelected = false;
                    fromPos = (0, 0);
                    TurnColor = Base.Piece.PieceColor.White;
                    var a = Task.Run(() => sfm.BestMoveWithPonder());
                    int[] bestMoveWithPonder = a.Result;
                    Board.Move(Board, (bestMoveWithPonder[2], bestMoveWithPonder[3]), (bestMoveWithPonder[0], bestMoveWithPonder[1]));
                }
                else if (Board.boardMatrix[toPos.Item1, toPos.Item2].Piece.pieceColor ==
                         Board.boardMatrix[fromPos.Item1, fromPos.Item2].Piece.pieceColor)
                {
                    fromPos = logic.FindHoveredSquare();
                }
            }
            if (MouseInput.LeftClicked && !fromPieceSelected && Board.boardMatrix[logic.FindHoveredSquare().Item1, logic.FindHoveredSquare().Item2].Piece.pieceColor == OwnColor)
            {
                fromPos = logic.FindHoveredSquare();
                fromPieceSelected = true;
            }

            if (MouseInput.RightClicked)
            {
                fromPieceSelected = false;
                fromPos = (0, 0);
            }
        }
        base.Update(gameTime);

    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        Drawing drawing = new Drawing();
        
        drawing.DrawBoard(_spriteBatch, Sprites, Board);
        
        drawing.DrawPieces(_spriteBatch, Sprites, Board);
        Logic.Logic logic = new Logic.Logic();
        if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed ||
            Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            clickInfo = (logic.FindHoveredSquare(), true);
        }
            if ((clickInfo.Item1.Item1 <= 7 && clickInfo.Item1.Item2 <= 7) && (clickInfo.Item1.Item1 >= 0 && clickInfo.Item1.Item2 >= 0))
            {
                if (clickInfo.Item2)
                {
                    _spriteBatch.Begin();
                    List<(int, int)> drawList = logic.GetLegalMoves(Board, clickInfo.Item1, Board.boardMatrix[clickInfo.Item1.Item1, clickInfo.Item1.Item2].Piece.pieceColor);
                    if (MouseInput.RightClicked)
                    {
                        clickInfo.Item2 = false;
                    }
                    if (drawList == null) { drawList = new(); }
                    foreach ((int,int) n in drawList)
                    {
                        if (Board.boardMatrix[n.Item1, n.Item2].Piece.pieceType != Base.Piece.PieceType.None)
                        {
                            _spriteBatch.Draw(Sprites[SpriteDict["captureMoveHighlight"]], new Vector2(n.Item1*60, n.Item2*60), Color.White);
                            continue;
                        }
                        _spriteBatch.Draw(Sprites[SpriteDict["moveHighlight"]], new Vector2(n.Item1*60, n.Item2*60), Color.White);
                    }
                    _spriteBatch.End();
                }
            }
        
        base.Draw(gameTime);
    }
}