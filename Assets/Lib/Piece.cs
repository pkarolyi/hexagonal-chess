using System;
using System.Collections.Generic;
using UnityEngine;

public enum PieceColor
{
    Black,
    White
}

// Numbers match spritesheet order
public enum PieceSprite
{
    Pawn = 0,
    Knight = 1,
    Rook = 2,
    Bishop = 3,
    Queen = 4,
    King = 5
}

public abstract class Piece
{
    public PieceSprite Sprite { get; set; }
    public PieceColor Color { get; set; }
    public GameObject GameObject { get; set; }

    public Piece(PieceSprite sprite, PieceColor color, GameObject gameObject)
    {
        Sprite = sprite;
        Color = color;
        GameObject = gameObject;
    }

    public virtual List<Hex> getValidMoves(
        Hex position,
        Dictionary<Hex, Piece> board,
        Hex enPassantPosition
    )
    {
        throw new NotImplementedException();
    }
}

public class Pawn : Piece
{
    private List<Hex> _whiteStartingPositions = new List<Hex>()
    {
        new Hex(-4, -1),
        new Hex(-3, -1),
        new Hex(-2, -1),
        new Hex(-1, -1),
        new Hex(0, -1),
        new Hex(1, -2),
        new Hex(2, -3),
        new Hex(3, -4),
        new Hex(4, -5)
    };

    private List<Hex> _blackStartingPositions = new List<Hex>()
    {
        new Hex(-4, 5),
        new Hex(-3, 4),
        new Hex(-2, 3),
        new Hex(-1, 2),
        new Hex(0, 1),
        new Hex(1, 1),
        new Hex(2, 1),
        new Hex(3, 1),
        new Hex(4, 1)
    };

    public Pawn(PieceColor color, GameObject gameObject = null)
        : base(PieceSprite.Pawn, color, gameObject) { }

    public override List<Hex> getValidMoves(
        Hex position,
        Dictionary<Hex, Piece> board,
        Hex enPassantPosition
    )
    {
        List<Hex> validMoves = new List<Hex>();

        if (Color == PieceColor.White)
        {
            if (!board.ContainsKey(position.Neighbor(Hex.Up)))
            {
                validMoves.Add(position.Neighbor(Hex.Up));
                if (
                    _whiteStartingPositions.Contains(position)
                    && !board.ContainsKey(position.Neighbor(Hex.Up * 2))
                )
                {
                    validMoves.Add(position.Neighbor(Hex.Up * 2));
                }
            }

            if (
                position.Neighbor(Hex.RightUp) == enPassantPosition
                || (
                    board.ContainsKey(position.Neighbor(Hex.RightUp))
                    && board[position.Neighbor(Hex.RightUp)].Color == PieceColor.Black
                )
            )
            {
                validMoves.Add(position.Neighbor(Hex.RightUp));
            }

            if (
                position.Neighbor(Hex.LeftUp) == enPassantPosition
                || (
                    board.ContainsKey(position.Neighbor(Hex.LeftUp))
                    && board[position.Neighbor(Hex.LeftUp)].Color == PieceColor.Black
                )
            )
            {
                validMoves.Add(position.Neighbor(Hex.LeftUp));
            }
        }
        else
        {
            if (!board.ContainsKey(position.Neighbor(Hex.Down)))
            {
                validMoves.Add(position.Neighbor(Hex.Down));
                if (
                    _blackStartingPositions.Contains(position)
                    && !board.ContainsKey(position.Neighbor(Hex.Down * 2))
                )
                {
                    validMoves.Add(position.Neighbor(Hex.Down * 2));
                }
            }

            if (
                position.Neighbor(Hex.RightDown) == enPassantPosition
                || (
                    board.ContainsKey(position.Neighbor(Hex.RightDown))
                    && board[position.Neighbor(Hex.RightDown)].Color == PieceColor.White
                )
            )
            {
                validMoves.Add(position.Neighbor(Hex.RightDown));
            }

            if (
                position.Neighbor(Hex.LeftDown) == enPassantPosition
                || (
                    board.ContainsKey(position.Neighbor(Hex.LeftDown))
                    && board[position.Neighbor(Hex.LeftDown)].Color == PieceColor.White
                )
            )
            {
                validMoves.Add(position.Neighbor(Hex.LeftDown));
            }
        }

        return validMoves;
    }

    public override string ToString()
    {
        return $"Pawn ({Sprite}, {Color})";
    }
}

public class Knigth : Piece
{
    public Knigth(PieceColor color, GameObject gameObject = null)
        : base(PieceSprite.Knight, color, gameObject) { }

    public override List<Hex> getValidMoves(
        Hex position,
        Dictionary<Hex, Piece> board,
        Hex enPassantPosition
    )
    {
        List<Hex> validMoves = new List<Hex>();

        validMoves.Add(position.Neighbor(Hex.Up * 2).Neighbor(Hex.LeftUp));
        validMoves.Add(position.Neighbor(Hex.Up * 2).Neighbor(Hex.RightUp));
        validMoves.Add(position.Neighbor(Hex.RightUp * 2).Neighbor(Hex.Up));
        validMoves.Add(position.Neighbor(Hex.RightUp * 2).Neighbor(Hex.RightDown));
        validMoves.Add(position.Neighbor(Hex.RightDown * 2).Neighbor(Hex.RightUp));
        validMoves.Add(position.Neighbor(Hex.RightDown * 2).Neighbor(Hex.Down));
        validMoves.Add(position.Neighbor(Hex.Down * 2).Neighbor(Hex.RightDown));
        validMoves.Add(position.Neighbor(Hex.Down * 2).Neighbor(Hex.LeftDown));
        validMoves.Add(position.Neighbor(Hex.LeftDown * 2).Neighbor(Hex.Down));
        validMoves.Add(position.Neighbor(Hex.LeftDown * 2).Neighbor(Hex.LeftUp));
        validMoves.Add(position.Neighbor(Hex.LeftUp * 2).Neighbor(Hex.LeftDown));
        validMoves.Add(position.Neighbor(Hex.LeftUp * 2).Neighbor(Hex.Up));

        validMoves.RemoveAll((Hex hex) => board.ContainsKey(hex) && board[hex].Color == Color);

        return validMoves;
    }

    public override string ToString()
    {
        return $"Knigth ({Sprite}, {Color})";
    }
}

public class Rook : Piece
{
    public Rook(PieceColor color, GameObject gameObject = null)
        : base(PieceSprite.Rook, color, gameObject) { }

    public override List<Hex> getValidMoves(
        Hex position,
        Dictionary<Hex, Piece> board,
        Hex enPassantPosition
    )
    {
        List<Hex> validMoves = new List<Hex>();

        for (int i = 0; i < 6; i++)
        {
            Hex direction = Hex.GetDirection((Hex.Direction)i);
            for (int j = 1; j < 12; j++)
            {
                Hex tileToCheck = position.Neighbor(direction * j);
                if (board.ContainsKey(tileToCheck))
                {
                    if (board[tileToCheck].Color != Color)
                    {
                        validMoves.Add(tileToCheck);
                    }
                    break;
                }
                else
                {
                    validMoves.Add(tileToCheck);
                }
            }
        }

        return validMoves;
    }

    public override string ToString()
    {
        return $"Rook ({Sprite}, {Color})";
    }
}

public class Bishop : Piece
{
    public Bishop(PieceColor color, GameObject gameObject = null)
        : base(PieceSprite.Bishop, color, gameObject) { }

    public override List<Hex> getValidMoves(
        Hex position,
        Dictionary<Hex, Piece> board,
        Hex enPassantPosition
    )
    {
        List<Hex> validMoves = new List<Hex>();

        for (int i = 0; i < 6; i++)
        {
            Hex direction =
                Hex.GetDirection((Hex.Direction)i) + Hex.GetDirection((Hex.Direction)((i + 1) % 6));
            for (int j = 1; j < 12; j++)
            {
                Hex tileToCheck = position.Neighbor(direction * j);
                if (board.ContainsKey(tileToCheck))
                {
                    if (board[tileToCheck].Color != Color)
                    {
                        validMoves.Add(tileToCheck);
                    }
                    break;
                }
                else
                {
                    validMoves.Add(tileToCheck);
                }
            }
        }

        return validMoves;
    }

    public override string ToString()
    {
        return $"Bishop ({Sprite}, {Color})";
    }
}

public class Queen : Piece
{
    public Queen(PieceColor color, GameObject gameObject = null)
        : base(PieceSprite.Queen, color, gameObject) { }

    public override List<Hex> getValidMoves(
        Hex position,
        Dictionary<Hex, Piece> board,
        Hex enPassantPosition
    )
    {
        List<Hex> validMoves = new List<Hex>();

        for (int i = 0; i < 6; i++)
        {
            Hex direction =
                Hex.GetDirection((Hex.Direction)i) + Hex.GetDirection((Hex.Direction)((i + 1) % 6));
            for (int j = 1; j < 12; j++)
            {
                Hex tileToCheck = position.Neighbor(direction * j);
                if (board.ContainsKey(tileToCheck))
                {
                    if (board[tileToCheck].Color != Color)
                    {
                        validMoves.Add(tileToCheck);
                    }
                    break;
                }
                else
                {
                    validMoves.Add(tileToCheck);
                }
            }

            direction = Hex.GetDirection((Hex.Direction)i);
            for (int j = 1; j < 12; j++)
            {
                Hex tileToCheck = position.Neighbor(direction * j);
                if (board.ContainsKey(tileToCheck))
                {
                    if (board[tileToCheck].Color != Color)
                    {
                        validMoves.Add(tileToCheck);
                    }
                    break;
                }
                else
                {
                    validMoves.Add(tileToCheck);
                }
            }
        }

        return validMoves;
    }

    public override string ToString()
    {
        return $"Queen ({Sprite}, {Color})";
    }
}

public class King : Piece
{
    public King(PieceColor color, GameObject gameObject = null)
        : base(PieceSprite.King, color, gameObject) { }

    public override List<Hex> getValidMoves(
        Hex position,
        Dictionary<Hex, Piece> board,
        Hex enPassantPosition
    )
    {
        List<Hex> validMoves = new List<Hex>();

        for (int i = 0; i < 6; i++)
        {
            Hex direction =
                Hex.GetDirection((Hex.Direction)i) + Hex.GetDirection((Hex.Direction)((i + 1) % 6));

            Hex tileToCheck = position.Neighbor(direction);
            if (board.ContainsKey(tileToCheck))
            {
                if (board[tileToCheck].Color != Color)
                {
                    validMoves.Add(tileToCheck);
                }
            }
            else
            {
                validMoves.Add(tileToCheck);
            }

            direction = Hex.GetDirection((Hex.Direction)i);
            tileToCheck = position.Neighbor(direction);
            if (board.ContainsKey(tileToCheck))
            {
                if (board[tileToCheck].Color != Color)
                {
                    validMoves.Add(tileToCheck);
                }
            }
            else
            {
                validMoves.Add(tileToCheck);
            }
        }

        return validMoves;
    }

    public bool isCheckMate(Hex position, Dictionary<Hex, Piece> board)
    {
        return false;
    }

    public override string ToString()
    {
        return $"King ({Sprite}, {Color})";
    }
}
