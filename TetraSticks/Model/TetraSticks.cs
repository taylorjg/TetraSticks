using System.Collections.Generic;

namespace TetraSticks.Model
{
    public static class TetraSticks
    {
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
                    new Coords(1, 1),
                    new Coords(0, 1),
                    new Coords(0, 2),
                    new Coords(1, 2)
                }),

            new TetraStick(
                "H",
                new[]
                {
                    new Coords(0, 0),
                    new Coords(0, 1),
                    new Coords(1, 1),
                    new Coords(1, 0)
                },
                new[]
                {
                    new Coords(1, 0),
                    new Coords(1, 1),
                    new Coords(0, 1),
                    new Coords(0, 2)
                }),

            new TetraStick(
                "I",
                new[]
                {
                    new Coords(0, 0),
                    new Coords(0, 1),
                    new Coords(0, 2),
                    new Coords(0, 3)
                }),

            new TetraStick(
                "J",
                new[]
                {
                    new Coords(0, 1),
                    new Coords(0, 0),
                    new Coords(1, 0),
                    new Coords(1, 1),
                    new Coords(1, 2)
                }),

            new TetraStick(
                "L",
                new[]
                {
                    new Coords(0, 3),
                    new Coords(0, 2),
                    new Coords(0, 1),
                    new Coords(0, 0),
                    new Coords(1, 0)
                }),

            new TetraStick(
                "N",
                new[]
                {
                    new Coords(0, 0),
                    new Coords(0, 1),
                    new Coords(1, 1),
                    new Coords(1, 2),
                    new Coords(1, 3)
                }),

            new TetraStick(
                "O",
                new[]
                {
                    new Coords(0, 0),
                    new Coords(0, 1),
                    new Coords(1, 1),
                    new Coords(1, 0),
                    new Coords(0, 0)
                }),

            new TetraStick(
                "P",
                new[]
                {
                    new Coords(0, 0),
                    new Coords(0, 1),
                    new Coords(1, 1),
                    new Coords(1, 2),
                    new Coords(0, 2)
                }),

            new TetraStick(
                "R",
                new[]
                {
                    new Coords(1, 0),
                    new Coords(1, 1),
                    new Coords(0, 1)
                },
                new[]
                {
                    new Coords(0, 1),
                    new Coords(1, 1),
                    new Coords(1, 2),
                    new Coords(2, 2)
                }),

            new TetraStick(
                "T",
                new[]
                {
                    new Coords(0, 2),
                    new Coords(1, 2),
                    new Coords(1, 1),
                    new Coords(1, 0)
                },
                new[]
                {
                    new Coords(2, 2),
                    new Coords(1, 2),
                    new Coords(1, 1),
                    new Coords(1, 0)
                }),

            new TetraStick(
                "U",
                new[]
                {
                    new Coords(0, 1),
                    new Coords(0, 0),
                    new Coords(1, 0),
                    new Coords(2, 0),
                    new Coords(2, 1)
                }),

            new TetraStick(
                "V",
                new[]
                {
                    new Coords(0, 0),
                    new Coords(1, 0),
                    new Coords(2, 0),
                    new Coords(2, 1),
                    new Coords(2, 2)
                }),

            new TetraStick(
                "W",
                new[]
                {
                    new Coords(0, 0),
                    new Coords(1, 0),
                    new Coords(1, 1),
                    new Coords(2, 1),
                    new Coords(2, 2)
                }),

            new TetraStick(
                "X",
                new[]
                {
                    new Coords(0, 1),
                    new Coords(1, 1),
                    new Coords(1, 2)
                },
                new[]
                {
                    new Coords(1, 0),
                    new Coords(1, 1),
                    new Coords(2, 1)
                }),

            new TetraStick(
                "Y",
                new[]
                {
                    new Coords(0, 0),
                    new Coords(1, 0),
                    new Coords(2, 0),
                    new Coords(2, 1)
                },
                new[]
                {
                    new Coords(2, 1),
                    new Coords(2, 0),
                    new Coords(3, 0)
                }),

            new TetraStick(
                "Z",
                new[]
                {
                    new Coords(0, 2),
                    new Coords(1, 2),
                    new Coords(1, 1),
                    new Coords(1, 0),
                    new Coords(2, 0)
                })
        };
    }
}
