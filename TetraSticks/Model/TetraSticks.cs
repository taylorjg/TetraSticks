using System.Collections.Generic;

namespace TetraSticks.Model
{
    public static class TetraSticks
    {
        // F, H, I, J, N, O, P, R, S, U, V, W, X, Y, Z
        // http://puzzler.sourceforge.net/docs/polysticks-intro.html
        public static readonly IEnumerable<TetraStick> All = new[]
        {
            new TetraStick(
                "F",
                new[]
                {
                    new Coords(0, 0),
                    new Coords(0, 1),
                    new Coords(1, 1)
                },
                new[]
                {
                    new Coords(0, 1),
                    new Coords(0, 2),
                    new Coords(1, 2)
                })
        };
    }
}
