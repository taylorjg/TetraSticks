﻿namespace TetraSticks.Model
{
    public class Coords
    {
        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as Coords;
            if (other == null) return false;
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return 33 * X.GetHashCode() + Y.GetHashCode();
        }
    }
}
