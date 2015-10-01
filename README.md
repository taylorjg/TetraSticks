

## Description

This project is another demo program for [DlxLib](https://github.com/taylorjg/DlxLib).
I recently added support for secondary columns as described in Donald E. Knuth's original
[Dancing Links](http://arxiv.org/pdf/cs/0011047v1.pdf "Dancing Links")
paper around pages 17-18 where he discusses puzzles involving tetra sticks.

A tetra stick is a shape composed of four short line segments of equal length.
There are 16 distinct tetra sticks known as `F H I J L N O P R T U V W X Y Z`
because of the similarity of the shapes to these letters.
It turns out that if one of `H J L N Y` is omitted, the remaining 15 tetra sticks
can be arranged to form a 5 x 5 square. Here is one possible solution (omitting `L`):

![TetraSticks](https://raw.github.com/taylorjg/TetraSticks/master/Images/TetraSticks.png)

Secondary columns are needed to prevent the tetra sticks from crossing:

![TetraSticks2](https://raw.github.com/taylorjg/TetraSticks/master/Images/TetraSticks2.png)

## Screenshot

![Screenshot](https://raw.github.com/taylorjg/TetraSticks/master/Images/Screenshot.png)

## Building the Internal Data Structure

Each tetra stick can be placed on the board with:

* a specific location e.g. `(0, 0)`
* a specific orientation i.e. `North`, `South`, `East` or `West`
* a specific reflection mode i.e. `Normal` or `MirrorY`

When building the internal data structure, which in turn will be used to build the DLX matrix, we need to consider each valid location/orientation/reflection mode of each tetra stick. This is achieved with the following code (see [RowBuilder.cs](https://github.com/taylorjg/TetraSticks/blob/master/TetraSticks/Model/RowBuilder.cs)):

```C#
public static IImmutableList<PlacedTetraStick> BuildRows(IImmutableList<TetraStick> tetraSticks)
{
    var locations = (
        from x in Enumerable.Range(0, 5)
        from y in Enumerable.Range(0, 5)
        select new Coords(x, y)
        ).ToImmutableList();
    var orientations = Enum.GetValues(typeof (Orientation)).Cast<Orientation>().ToImmutableList();
    var reflectionModes = Enum.GetValues(typeof (ReflectionMode)).Cast<ReflectionMode>().ToImmutableList();

    var placedTetraSticks =
        from tetraStick in tetraSticks
        from location in locations
        from orientation in orientations
        from reflectionMode in reflectionModes
        let placedTetraStick = new PlacedTetraStick(tetraStick, location, orientation, reflectionMode)
        where IsFullyWithinGrid(placedTetraStick)
        select placedTetraStick;

    placedTetraSticks = placedTetraSticks.Distinct(new PlacedTetraStickComparer());
    placedTetraSticks = PinTetraStickToBeOrientedNorth(tetraSticks.First(), placedTetraSticks);

    return placedTetraSticks.ToImmutableList();
}
```

Notes:

* We use `Distinct` with a specific comparer to remove duplicate rows e.g. `O` covers the same points in all orientations.

* We arbitrarily pick one tetra stick (the first one) and pin it so that we only consider it's `North` orientation. This is to avoid finding each solution four times where the only difference is the orientation of the board.

## TODO: Implementation Steps

- ~~get basic grid working~~
  - ~~draw empty grid~~
- ~~model a tetra stick~~
  - ~~each tetra stick will have:~~
    - ~~a tag~~
    - ~~a list of lines each of which is a list of segments~~
    - ~~a list of interior junction points~~
- ~~model all 16 tetra sticks~~
- ~~draw a tetra stick~~
  - ~~draw a line segment (0, 0) => (1, 0)~~
  - ~~draw with given location~~
  - ~~draw several line segments~~
  - ~~draw several lines (i.e. a whole tetra stick)~~
  - ~~draw with correct colour~~
  - ~~draw with given orientation~~
  - ~~draw with given reflection mode~~
- ~~need to enumerate all possible locations for each tetra stick~~
  - ~~with all possible orientations and reflection modes~~
- ~~need to be able to generate a matrix row for a given tetra stick/location/orientation~~
  - ~~initially, ignore interior junction points~~
- ~~build the DLX matrix with all rows (but no secondary columns)~~
  - ~~we only want to do this for 15 of the 16 tetra sticks~~
    - ~~initially, hardcode which 15 tetra sticks to use~~
- ~~solve the DLX matrix~~
  - ~~initially on the main thread~~
  - ~~display first solution~~
  - ~~then solve on a background thread~~
  - ~~then add the ability to cancel the solving~~
- ~~extend DlxLib to support secondary columns~~
- ~~extend DLX matrix by adding secondary columns for the interior
  junction points when these are not on the edges of the grid~~
- add the ability to select which of the tetra sticks (H, J, L, N, Y) to omit
  - ~~simple dropdown showing the name of the tetra stick e.g. "L"~~
  - enhance this by adding a tiny picture of the tetra stick next to the name (drawn North with the correct colour)
- ~~add the ability to step forward through each solution~~
  - ~~add Next Solution and Cancel buttons~~
  - ~~enable/disable the buttons appropriately~~
  - ~~the background thread should invoke onSolutionFound for each solution~~
  - ~~view model needs a CurrentSolutionIndex property (initially null)~~
  - ~~onSolutionFound impl will just add each solution to a list in the view model~~
    - ~~if this is the first solution then automatically display it~~
  - ~~OnNextSolution displays the next available solution~~
    - ~~only enabled when there is another solution to display~~
  - ~~add a label to indicate we are currently displaying solution 'n' of 'total'
  where 'total' auto-updates as the background thread finds more solutions~~
- ~~improve the drawing of the tetra sticks on the board (currently it is very basic and looks a bit naff)~~
- ~~add the ability to display each step of the algorithm~~
- add a status bar
- add an app icon
- display messages in the status bar
- display stats in the status bar (size of matrix and elapsed time to solve it)
- ~~Use the MVVM pattern~~

## Links

* [Dancing Links](http://arxiv.org/pdf/cs/0011047v1.pdf "Dancing Links")
* http://puzzler.sourceforge.net/docs/polysticks-intro.html#tetrasticks
