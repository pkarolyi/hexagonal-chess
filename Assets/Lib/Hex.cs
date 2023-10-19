using System;
using UnityEngine;

public class Hex
{
    private Vector3Int _v;

    public int Q
    {
        get => _v[0];
        set => _v[0] = value;
    }

    public int R
    {
        get => _v[1];
        set => _v[1] = value;
    }

    public int S
    {
        get => _v[2];
        set => _v[2] = value;
    }

    public Hex(int q, int r, int s)
    {
        if (q + r + s != 0)
            throw new ArgumentException("q + r + s must be 0.");
        _v = new Vector3Int(q, r, s);
    }

    public Hex(Vector3Int v)
        : this(v[0], v[1], v[2]) { }

    public Hex(int q, int r)
        : this(q, r, -q - r) { }

    public Hex(Hex hex)
        : this(hex.Q, hex.R, hex.S) { }

    public static Hex FromOffset(Vector3Int offset)
    {
        int q = offset.y;
        int r = offset.x - (int)((offset.y - (Helper.mod(offset.y, 2))) / 2);
        return new Hex(q, r);
    }

    public Vector3Int ToOffset()
    {
        int x = R + (int)((Q - (Helper.mod(Q, 2))) / 2);
        int y = Q;
        return new Vector3Int(x, y);
    }

    public override string ToString()
    {
        return $"Hex ({Q}, {R}, {S})";
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;

        Hex b = obj as Hex;
        return !(b == null) && _v.Equals(b._v);
    }

    public override int GetHashCode()
    {
        return _v.GetHashCode();
    }

    public static Boolean operator ==(Hex a, Hex b)
    {
        return a.Equals(b);
    }

    public static Boolean operator !=(Hex a, Hex b)
    {
        return !a.Equals(b);
    }

    public static Hex operator +(Hex a, Hex b)
    {
        return new Hex(a._v + b._v);
    }

    public static Hex operator -(Hex a, Hex b)
    {
        return new Hex(a._v - b._v);
    }

    public static Hex operator *(Hex a, Hex b)
    {
        return new Hex(a._v * b._v);
    }

    public static Hex operator *(Hex a, int b)
    {
        return new Hex(a._v * b);
    }

    public int DistanceOf(Hex b)
    {
        return (int)Math.Floor((this - b)._v.magnitude);
    }

    public enum Direction
    {
        Up,
        RightUp,
        RightDown,
        Down,
        LeftDown,
        LeftUp
    };

    public static Hex GetDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return new Hex(0, 1);
            case Direction.RightUp:
                return new Hex(1, 0);
            case Direction.RightDown:
                return new Hex(1, -1);
            case Direction.Down:
                return new Hex(0, -1);
            case Direction.LeftDown:
                return new Hex(-1, 0);
            case Direction.LeftUp:
                return new Hex(-1, 1);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static Hex Up = GetDirection(Direction.Up);
    public static Hex RightUp = GetDirection(Direction.RightUp);
    public static Hex RightDown = GetDirection(Direction.RightDown);
    public static Hex Down = GetDirection(Direction.Down);
    public static Hex LeftDown = GetDirection(Direction.LeftDown);
    public static Hex LeftUp = GetDirection(Direction.LeftUp);

    public Hex Neighbor(Hex direction)
    {
        return this + direction;
    }
}
