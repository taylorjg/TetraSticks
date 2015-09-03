using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global

namespace TetraSticks.Model
{
    public static class TetraSticks
    {
        public static readonly IEnumerable<TetraStick> All = new[]
        {
            F, H, I, J, L, N, O, P, R, T, U, V, W, X, Y, Z
        };

        public static TetraStick F => new TetraStick(
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
            });

        public static TetraStick H => new TetraStick(
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
            });


        public static TetraStick I => new TetraStick(
            "I",
            new[]
            {
                new Coords(0, 0),
                new Coords(0, 1),
                new Coords(0, 2),
                new Coords(0, 3)
            });

        public static TetraStick J => new TetraStick(
            "J",
            new[]
            {
                new Coords(0, 1),
                new Coords(0, 0),
                new Coords(1, 0),
                new Coords(1, 1),
                new Coords(1, 2)
            });

        public static TetraStick L => new TetraStick(
            "L",
            new[]
            {
                new Coords(0, 3),
                new Coords(0, 2),
                new Coords(0, 1),
                new Coords(0, 0),
                new Coords(1, 0)
            });

        public static TetraStick N => new TetraStick(
            "N",
            new[]
            {
                new Coords(0, 0),
                new Coords(0, 1),
                new Coords(1, 1),
                new Coords(1, 2),
                new Coords(1, 3)
            });

        public static TetraStick O => new TetraStick(
            "O",
            new[]
            {
                new Coords(0, 0),
                new Coords(0, 1),
                new Coords(1, 1),
                new Coords(1, 0),
                new Coords(0, 0)
            });

        public static TetraStick P => new TetraStick(
            "P",
            new[]
            {
                new Coords(0, 0),
                new Coords(0, 1),
                new Coords(1, 1),
                new Coords(1, 2),
                new Coords(0, 2)
            });

        public static TetraStick R => new TetraStick(
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
            });

        public static TetraStick T => new TetraStick(
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
            });

        public static TetraStick U => new TetraStick(
            "U",
            new[]
            {
                new Coords(0, 1),
                new Coords(0, 0),
                new Coords(1, 0),
                new Coords(2, 0),
                new Coords(2, 1)
            });

        public static TetraStick V => new TetraStick(
            "V",
            new[]
            {
                new Coords(0, 0),
                new Coords(1, 0),
                new Coords(2, 0),
                new Coords(2, 1),
                new Coords(2, 2)
            });

        public static TetraStick W => new TetraStick(
            "W",
            new[]
            {
                new Coords(0, 0),
                new Coords(1, 0),
                new Coords(1, 1),
                new Coords(2, 1),
                new Coords(2, 2)
            });

        public static TetraStick X => new TetraStick(
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
            });

        public static TetraStick Y => new TetraStick(
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
            });

        public static TetraStick Z => new TetraStick(
            "Z",
            new[]
            {
                new Coords(0, 2),
                new Coords(1, 2),
                new Coords(1, 1),
                new Coords(1, 0),
                new Coords(2, 0)
            });
    }
}
