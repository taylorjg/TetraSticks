
## Description

This project will be another demo program for [DlxLib](https://github.com/taylorjg/DlxLib).
Currently, [DlxLib](https://github.com/taylorjg/DlxLib) does not support secondary columns
as described in Donald E. Knuth's original
[Dancing Links](http://arxiv.org/pdf/cs/0011047v1.pdf "Dancing Links") paper around pages 17-18 when
he discusses puzzles involving 15 of the 16 possible tetra sticks in a 5 x 5 square:

![TetraSticks](https://raw.github.com/taylorjg/TetraSticks/master/Images/TetraSticks.png)

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
- improve the drawing of the tetra sticks on the board (currently it is very basic and looks a bit naff)
- add the ability to display each step of the algorithm ?
- add a status bar
- display messages in the status bar
- display stats in the status bar (size of matrix and elapsed time to solve it)
- Use the MVVM pattern (to at least some extent)

## Links

* [Dancing Links](http://arxiv.org/pdf/cs/0011047v1.pdf "Dancing Links")
* http://puzzler.sourceforge.net/docs/polysticks-intro.html#tetrasticks
