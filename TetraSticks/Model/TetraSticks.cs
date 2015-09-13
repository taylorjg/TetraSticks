using System.Collections.Immutable;

// ReSharper disable MemberCanBePrivate.Global

namespace TetraSticks.Model
{
    public static class TetraSticks
    {
        public static readonly IImmutableList<TetraStick> All = ImmutableList.Create(
            F, H, I, J, L, N, O, P, R, T, U, V, W, X, Y, Z);

        public static TetraStick F => new TetraStick(
            "F",
            ImmutableList<Coords>.Empty,
            ImmutableList.Create(
                new Coords(0, 0),
                new Coords(0, 1),
                new Coords(1, 1)
                ),
            ImmutableList.Create(
                new Coords(1, 1),
                new Coords(0, 1),
                new Coords(0, 2),
                new Coords(1, 2)
                ));

        public static TetraStick H => new TetraStick(
            "H",
            ImmutableList<Coords>.Empty,
            ImmutableList.Create(
                new Coords(0, 0),
                new Coords(0, 1),
                new Coords(1, 1),
                new Coords(1, 0)
                ),
            ImmutableList.Create(
                new Coords(1, 0),
                new Coords(1, 1),
                new Coords(0, 1),
                new Coords(0, 2)
                ));

        public static TetraStick I => new TetraStick(
            "I",
            ImmutableList.Create(
                new Coords(0, 1),
                new Coords(0, 2),
                new Coords(0, 3)
                ),
            ImmutableList.Create(
                new Coords(0, 0),
                new Coords(0, 1),
                new Coords(0, 2),
                new Coords(0, 3),
                new Coords(0, 4)
                ));

        public static TetraStick J => new TetraStick(
            "J",
            ImmutableList.Create(
                new Coords(1, 1)),
            ImmutableList.Create(
                new Coords(0, 1),
                new Coords(0, 0),
                new Coords(1, 0),
                new Coords(1, 1),
                new Coords(1, 2)
                ));

        public static TetraStick L => new TetraStick(
            "L",
            ImmutableList.Create(
                new Coords(0, 2),
                new Coords(0, 1)
                ),
            ImmutableList.Create(
                new Coords(0, 3),
                new Coords(0, 2),
                new Coords(0, 1),
                new Coords(0, 0),
                new Coords(1, 0)
                ));

        public static TetraStick N => new TetraStick(
            "N",
            ImmutableList.Create(
                new Coords(1, 2)),
            ImmutableList.Create(
                new Coords(0, 0),
                new Coords(0, 1),
                new Coords(1, 1),
                new Coords(1, 2),
                new Coords(1, 3)
                ));

        public static TetraStick O => new TetraStick(
            "O",
            ImmutableList<Coords>.Empty,
            //ImmutableList.Create(
            //    new Coords(0, 0),
            //    new Coords(0, 1),
            //    new Coords(1, 1),
            //    new Coords(1, 0),
            //    new Coords(0, 0)
            //    ));
            ImmutableList.Create(
                new Coords(0, 0),
                new Coords(1, 0),
                new Coords(1, 1),
                new Coords(0, 1),
                new Coords(0, 0)
                ));

        public static TetraStick P => new TetraStick(
            "P",
            ImmutableList<Coords>.Empty,
            ImmutableList.Create(
                new Coords(0, 0),
                new Coords(0, 1),
                new Coords(1, 1),
                new Coords(1, 2),
                new Coords(0, 2)
                ));

        public static TetraStick R => new TetraStick(
            "R",
            ImmutableList<Coords>.Empty,
            ImmutableList.Create(
                new Coords(1, 0),
                new Coords(1, 1),
                new Coords(0, 1)
                ),
            ImmutableList.Create(
                new Coords(0, 1),
                new Coords(1, 1),
                new Coords(1, 2),
                new Coords(2, 2)
                ));

        public static TetraStick T => new TetraStick(
            "T",
            ImmutableList.Create(
                new Coords(1, 1)),
            ImmutableList.Create(
                new Coords(0, 2),
                new Coords(1, 2),
                new Coords(1, 1),
                new Coords(1, 0)
                ),
            ImmutableList.Create(
                new Coords(2, 2),
                new Coords(1, 2),
                new Coords(1, 1),
                new Coords(1, 0)
                ));

        public static TetraStick U => new TetraStick(
            "U",
            ImmutableList.Create(
                new Coords(1, 0)),
            ImmutableList.Create(
                new Coords(0, 1),
                new Coords(0, 0),
                new Coords(1, 0),
                new Coords(2, 0),
                new Coords(2, 1)
                ));

        public static TetraStick V => new TetraStick(
            "V",
            ImmutableList.Create(
                new Coords(1, 0),
                new Coords(2, 1)
                ),
            ImmutableList.Create(
                new Coords(0, 0),
                new Coords(1, 0),
                new Coords(2, 0),
                new Coords(2, 1),
                new Coords(2, 2)
                ));

        public static TetraStick W => new TetraStick(
            "W",
            ImmutableList<Coords>.Empty,
            ImmutableList.Create(
                new Coords(0, 0),
                new Coords(1, 0),
                new Coords(1, 1),
                new Coords(2, 1),
                new Coords(2, 2)
                ));

        public static TetraStick X => new TetraStick(
            "X",
            ImmutableList<Coords>.Empty,
            ImmutableList.Create(
                new Coords(0, 1),
                new Coords(1, 1),
                new Coords(1, 2)
                ),
            ImmutableList.Create(
                new Coords(1, 0),
                new Coords(1, 1),
                new Coords(2, 1)
                ),
            ImmutableList.Create(
                new Coords(0, 1),
                new Coords(1, 1),
                new Coords(1, 0)
                ),
            ImmutableList.Create(
                new Coords(1, 2),
                new Coords(1, 1),
                new Coords(2, 1)
                ));

        public static TetraStick Y => new TetraStick(
            "Y",
            ImmutableList.Create(
                new Coords(1, 0)),
            ImmutableList.Create(
                new Coords(0, 0),
                new Coords(1, 0),
                new Coords(2, 0),
                new Coords(2, 1)
                ),
            ImmutableList.Create(
                new Coords(2, 1),
                new Coords(2, 0),
                new Coords(3, 0)
                ));

        public static TetraStick Z => new TetraStick(
            "Z",
            ImmutableList.Create(
                new Coords(1, 1)
                ),
            ImmutableList.Create(
                new Coords(0, 2),
                new Coords(1, 2),
                new Coords(1, 1),
                new Coords(1, 0),
                new Coords(2, 0)
                ));
    }
}
