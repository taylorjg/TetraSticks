using System.Collections.Immutable;

namespace TetraSticks.Model
{
    public class TetraStick
    {
        public TetraStick(string tag, IImmutableList<Coords> interiorJunctionPoints, params IImmutableList<Coords>[] lines)
        {
            Tag = tag;
            InteriorJunctionPoints = interiorJunctionPoints;
            Lines = ImmutableList.CreateRange(lines);
        }

        public string Tag { get; }
        public IImmutableList<Coords> InteriorJunctionPoints { get; }
        public IImmutableList<IImmutableList<Coords>> Lines { get; }
    }
}
