using System.Collections.Generic;

namespace TetraSticks.Model
{
    public class TetraStick
    {
        public TetraStick(string tag, params IEnumerable<Coords>[] lines)
        {
            Lines = lines;
            Tag = tag;
        }

        public string Tag { get; }
        public IEnumerable<Coords>[] Lines { get; }
    }
}
