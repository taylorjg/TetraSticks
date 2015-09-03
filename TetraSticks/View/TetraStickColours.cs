using System;
using System.Windows.Media;

namespace TetraSticks.View
{
    public static class TetraStickColours
    {
        public static Color TagToColour(string tag)
        {
            switch (tag)
            {
                case "F":
                    return Color.FromRgb(0xFF, 0x73, 0x66);
                case "H":
                    return Color.FromRgb(0x00, 0xE6, 0x1A);
                case "I":
                    return Color.FromRgb(0x66, 0x00, 0x66);
                case "J":
                    return Color.FromRgb(0xE6, 0xE6, 0xFF);
                case "L":
                    return Color.FromRgb(0x59, 0x66, 0x73);
                case "N":
                    return Color.FromRgb(0xFF, 0xFF, 0x00);
                case "O":
                    return Color.FromRgb(0xCC, 0xCC, 0x1A);
                case "P":
                    return Color.FromRgb(0x99, 0x4D, 0x33);
                case "R":
                    return Color.FromRgb(0x99, 0x26, 0xB2);
                case "T":
                    return Color.FromRgb(0x33, 0x00, 0xB2);
                case "U":
                    return Color.FromRgb(0xFF, 0x26, 0x99);
                case "V":
                    return Color.FromRgb(0x00, 0xFF, 0xFF);
                case "W":
                    return Color.FromRgb(0xCC, 0xFF, 0x00);
                case "X":
                    return Color.FromRgb(0xE6, 0x00, 0x00);
                case "Y":
                    return Color.FromRgb(0x66, 0x59, 0xE6);
                case "Z":
                    return Color.FromRgb(0x00, 0x80, 0x00);
                default:
                    throw new InvalidOperationException($"Unknown tetra stick tag, \"{tag}\".");
            }
        }
    }
}
