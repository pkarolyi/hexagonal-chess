using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Grid))]
public class BoardScript : MonoBehaviour
{
    public GameObject HexPrefab;
    public GameObject PiecePrefab;
    public GameObject CirclePrefab;

    public Color Black;
    public Color White;
    public Color Grey;
    public Color Select;
    public Color Highlight;

    private Grid _grid;

    private Dictionary<Hex, GameObject> _board;
    private Dictionary<Hex, Piece> _pieces;

    private Hex _selectedHex;
    private PieceColor _currentPlayerColor;

    private Hex _enPassantPosition;
    private Hex _enPassantPawn;

    Sprite[] _blackSprites;
    Sprite[] _whiteSprites;

    private King _blackKing;
    private King _whiteKing;

    void OnGUI()
    {
        var piecesString = string.Join(Environment.NewLine, _pieces);
        var enPassantPString = _enPassantPosition is not null ? _enPassantPosition.ToString() : "";
        var enPassantPwString = _enPassantPawn is not null ? _enPassantPawn.ToString() : "";
        GUILayout.Label(
            string.Join(Environment.NewLine, piecesString, enPassantPString, enPassantPwString)
        );
    }

    private Color getBaseColor(Hex hex)
    {
        Color[] _baseColors = { Grey, White, Black };
        Color color = _baseColors[Helper.mod(Helper.mod(hex.Q * 2, 3) + Helper.mod(hex.R, 3), 3)];
        return new Color(color.r, color.g, color.b); // no idea why this is needed.
    }

    private void initializeBoard()
    {
        _board = new Dictionary<Hex, GameObject>();

        const int boardRadius = 5;
        for (int q = -boardRadius; q <= boardRadius; q++)
        {
            int r1 = Math.Max(-boardRadius, -q - boardRadius);
            int r2 = Math.Min(boardRadius, -q + boardRadius);
            for (int r = r1; r <= r2; r++)
            {
                Hex hex = new Hex(q, r);
                Vector3Int offsetCoords = hex.ToOffset();
                GameObject newTile = Instantiate(
                    HexPrefab,
                    _grid.CellToWorld(offsetCoords),
                    transform.rotation
                );

                newTile.transform.Find("HexSprite").GetComponent<SpriteRenderer>().color =
                    getBaseColor(hex);
                //newTile.GetComponentInChildren<TextMesh>().text = $"({hex.Q}, {hex.R})";
                _board.Add(hex, newTile);
            }
        }
    }

    private void initializePieces()
    {
        _pieces = new Dictionary<Hex, Piece>();

        // for checkmate checking
        _whiteKing = new King(PieceColor.White);
        _blackKing = new King(PieceColor.Black);

        var initialPieces = new (Hex, Piece)[]
        {
            (new Hex(-1, -4), _whiteKing),
            (new Hex(1, -5), new Queen(PieceColor.White)),
            (new Hex(0, -3), new Bishop(PieceColor.White)),
            (new Hex(0, -4), new Bishop(PieceColor.White)),
            (new Hex(0, -5), new Bishop(PieceColor.White)),
            (new Hex(-3, -2), new Rook(PieceColor.White)),
            (new Hex(3, -5), new Rook(PieceColor.White)),
            (new Hex(-2, -3), new Knigth(PieceColor.White)),
            (new Hex(2, -5), new Knigth(PieceColor.White)),
            (new Hex(-4, -1), new Pawn(PieceColor.White)),
            (new Hex(-3, -1), new Pawn(PieceColor.White)),
            (new Hex(-2, -1), new Pawn(PieceColor.White)),
            (new Hex(-1, -1), new Pawn(PieceColor.White)),
            (new Hex(0, -1), new Pawn(PieceColor.White)),
            (new Hex(1, -2), new Pawn(PieceColor.White)),
            (new Hex(2, -3), new Pawn(PieceColor.White)),
            (new Hex(3, -4), new Pawn(PieceColor.White)),
            (new Hex(4, -5), new Pawn(PieceColor.White)),
            (new Hex(1, 4), _blackKing),
            (new Hex(-1, 5), new Queen(PieceColor.Black)),
            (new Hex(0, 3), new Bishop(PieceColor.Black)),
            (new Hex(0, 4), new Bishop(PieceColor.Black)),
            (new Hex(0, 5), new Bishop(PieceColor.Black)),
            (new Hex(-3, 5), new Rook(PieceColor.Black)),
            (new Hex(3, 2), new Rook(PieceColor.Black)),
            (new Hex(-2, 5), new Knigth(PieceColor.Black)),
            (new Hex(2, 3), new Knigth(PieceColor.Black)),
            (new Hex(-4, 5), new Pawn(PieceColor.Black)),
            (new Hex(-3, 4), new Pawn(PieceColor.Black)),
            (new Hex(-2, 3), new Pawn(PieceColor.Black)),
            (new Hex(-1, 2), new Pawn(PieceColor.Black)),
            (new Hex(0, 1), new Pawn(PieceColor.Black)),
            (new Hex(1, 1), new Pawn(PieceColor.Black)),
            (new Hex(2, 1), new Pawn(PieceColor.Black)),
            (new Hex(3, 1), new Pawn(PieceColor.Black)),
            (new Hex(4, 1), new Pawn(PieceColor.Black))
        };

        foreach (var initialPiece in initialPieces)
        {
            Vector3Int offsetCoords = initialPiece.Item1.ToOffset();
            GameObject pieceObject = Instantiate(
                PiecePrefab,
                _grid.CellToWorld(offsetCoords),
                transform.rotation
            );

            if (initialPiece.Item2.Color == PieceColor.Black)
                pieceObject.GetComponentInChildren<SpriteRenderer>().sprite = _blackSprites[
                    (int)initialPiece.Item2.Sprite
                ];
            else
                pieceObject.GetComponentInChildren<SpriteRenderer>().sprite = _whiteSprites[
                    (int)initialPiece.Item2.Sprite
                ];

            initialPiece.Item2.GameObject = pieceObject;
            _pieces.Add(initialPiece.Item1, initialPiece.Item2);
        }
    }

    void Start()
    {
        _grid = GetComponent<Grid>();

        _whiteSprites = Resources.LoadAll<Sprite>("WhitePiecesWood-Sheet");
        _blackSprites = Resources.LoadAll<Sprite>("BlackPiecesWood-Sheet");

        initializeBoard();
        initializePieces();

        _currentPlayerColor = PieceColor.White;
    }

    void Update()
    {
        // Paint board
        foreach (var tile in _board)
        {
            tile.Value.transform.Find("HexSprite").GetComponent<SpriteRenderer>().color =
                getBaseColor(tile.Key);
        }

        // // checkmate?
        if (_whiteKing.isCheckMate(_pieces.First(x => x.Value == _whiteKing).Key, _pieces))
        {
            Debug.Log("Black win.");
        }

        if (_blackKing.isCheckMate(_pieces.First(x => x.Value == _blackKing).Key, _pieces))
        {
            Debug.Log("White win.");
        }

        // color valid moves for selected piece
        if (_selectedHex is not null)
        {
            if (_pieces.ContainsKey(_selectedHex))
            {
                // TODO expensive?
                List<Hex> validMoves = _pieces[_selectedHex].getValidMoves(
                    _selectedHex,
                    _pieces,
                    _enPassantPosition
                );
                foreach (var move in validMoves)
                {
                    if (_board.ContainsKey(move))
                    {
                        _board[move].GetComponentInChildren<SpriteRenderer>().color = Highlight;
                    }
                }
            }

            if (_board.ContainsKey(_selectedHex))
            {
                _board[_selectedHex].GetComponentInChildren<SpriteRenderer>().color = Select;
            }
        }

        // Paint pieces
        foreach (var piece in _pieces)
        {
            GameObject pieceObject = piece.Value.GameObject;
            if (pieceObject is not null)
            {
                Vector3Int offsetCoords = piece.Key.ToOffset();
                pieceObject.transform.position = _grid.CellToWorld(offsetCoords);
            }
        }

        // Handle mouse
        var mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePosition = mouse.position.ReadValue();
            Vector3 pz = Camera.main.ScreenToWorldPoint(mousePosition);
            Hex clickedHex = Hex.FromOffset(_grid.WorldToCell(pz));

            if (clickedHex is not null)
            {
                if (
                    _pieces.ContainsKey(clickedHex)
                    && _pieces[clickedHex].Color == _currentPlayerColor
                )
                {
                    _selectedHex = clickedHex;
                }
                else if (
                    _selectedHex is not null
                    && _board.ContainsKey(clickedHex)
                    && _pieces.ContainsKey(_selectedHex)
                )
                {
                    Piece selectedPiece = _pieces[_selectedHex];

                    List<Hex> validMoves = selectedPiece.getValidMoves(
                        _selectedHex,
                        _pieces,
                        _enPassantPosition
                    );
                    if (validMoves.Contains(clickedHex))
                    {
                        // take
                        if (
                            _pieces.ContainsKey(clickedHex)
                            && _pieces[clickedHex].Color != _currentPlayerColor
                        )
                        {
                            Destroy(_pieces[clickedHex].GameObject);
                            _pieces.Remove(clickedHex);
                        }

                        if (_pieces[_selectedHex] is Pawn && clickedHex == _enPassantPosition)
                        {
                            // en passant
                            Destroy(_pieces[_enPassantPawn].GameObject);
                            _pieces.Remove(_enPassantPawn);
                            _enPassantPosition = null;
                            _enPassantPawn = null;
                        }

                        //  enpassant clear up if not taken in one turn
                        if (_enPassantPosition is not null)
                        {
                            _enPassantPosition = null;
                            _enPassantPawn = null;
                        }

                        // en passant setup
                        if (selectedPiece is Pawn && _selectedHex.DistanceOf(clickedHex) == 2)
                        {
                            _enPassantPosition = new Hex(
                                _selectedHex.Q,
                                (_selectedHex.R + clickedHex.R) / 2
                            );
                            _enPassantPawn = clickedHex;
                        }

                        _pieces.Add(clickedHex, selectedPiece);
                        _pieces.Remove(_selectedHex);

                        if (_currentPlayerColor == PieceColor.White)
                        {
                            _currentPlayerColor = PieceColor.Black;
                        }
                        else
                        {
                            _currentPlayerColor = PieceColor.White;
                        }
                    }
                }
            }
        }
    }
}
