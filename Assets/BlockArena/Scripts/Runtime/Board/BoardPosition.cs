using System;

public readonly struct BoardPosition : IEquatable<BoardPosition>
{
    public BoardPosition(int x, int z)
    {
        X = x;
        Z = z;
    }

    public int X { get; }
    public int Z { get; }

    public bool Equals(BoardPosition other)
    {
        return X == other.X && Z == other.Z;
    }

    public override bool Equals(object obj)
    {
        return obj is BoardPosition other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (X * 397) ^ Z;
        }
    }

    public override string ToString()
    {
        return $"({X}, {Z})";
    }
}
