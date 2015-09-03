using System.Collections.Generic;

namespace TetraSticks.Model
{
    public class TetraStick
    {
        public TetraStick(string tag, IEnumerable<Coords> interiorJunctionPoints, params IEnumerable<Coords>[] lines)
        {
            Tag = tag;
            Lines = lines;
            InteriorJunctionPoints = interiorJunctionPoints;
        }

        public string Tag { get; }
        public IEnumerable<Coords> InteriorJunctionPoints { get; }
        public IEnumerable<Coords>[] Lines { get; }
    }
}
