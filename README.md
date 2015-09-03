
## Description

This project will be another demo program for [DlxLib](https://github.com/taylorjg/DlxLib).
Currently, [DlxLib](https://github.com/taylorjg/DlxLib) does not support secondary columns
as described in Donald E. Knuth's original
[Dancing Links](http://arxiv.org/pdf/cs/0011047v1.pdf "Dancing Links") paper around pages 17-18 when
he discusses puzzles involving 15 of the 16 possible tetra sticks in a 5 x 5 square:

![TetraSticks](https://raw.github.com/taylorjg/TetraSticks/master/Images/TetraSticks.png)

## TODO: Implementation Steps

- add basic graphical elements (grid, buttons, etc)
- ~~get basic grid working~~
  - ~~draw empty grid~~
- model a tetra stick
  - each tetra stick will have:
    - ~~a tag~~
    - ~~a list of lines each of which is a list of segments~~
    - a list of interior junction points
- model all 16 tetra sticks
- draw a tetra stick
  - ~~draw a line segment (0, 0) => (1, 0)~~
  - draw with given location
  - ~~draw several line segments~~
  - ~~draw several lines (i.e. a whole tetra stick)~~
  - draw with given colour
  - draw with given orientation
- need to enumerate all possible locations for each tetra stick
  - with all possible orientations
- need to be able to generate a matrix row for a given tetra stick/location/orientation
  - initially, ignore interior junction points
- build DLX matrix with all rows (no secondary columns)
  - we only want to do this for 15 of the 16 tetra sticks
    - initially, hardcode which 15 tetra sticks to use
- solve DLX matrix
  - initially on the main thread
  - then on a background thread
  - then display a dialog box during the action
  - then add the ability to cancel the action
- display (first) solution
- extend DlxLib to support secondary columns
- extend DLX matrix by adding secondary columns for the interior
  junction points when these are not on the edges of the grid
- add the ability to control which of the 16 tetra sticks should be omitted
  - simple dropdown showing the name of the tetra stick e.g. "L"
  - enhance this further by adding a tiny picture (with the correct colour)
    of the tetra stick next to the name
- display stats in status bar (size of matrix and elapsed time to solve it)
- add ability to step through all the solutions
- add ability to display a partial solution
- add ability to control speed at which partial solutions are displayed
- MVVM!

## Links

* [Dancing Links](http://arxiv.org/pdf/cs/0011047v1.pdf "Dancing Links")
* http://puzzler.sourceforge.net/docs/polysticks-intro.html#tetrasticks
