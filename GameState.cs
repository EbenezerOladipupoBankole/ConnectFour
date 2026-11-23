namespace ConnectFour;

/// <summary>
/// Holds the state and rules of a Connect Four game.
/// The board is 7 columns x 6 rows, stored as 42 cells where index 0 is the
/// top-left cell and index 41 is the bottom-right. 0 = empty, 1/2 = player.
/// </summary>
public class GameState
{
    public const int Columns = 7;
    public const int Rows = 6;

    public enum WinState
    {
        No_Winner = 0,
        Player1_Wins = 1,
        Player2_Wins = 2,
        Tie = 3
    }

    /// <summary>Every group of 4 cell indexes that counts as a win.</summary>
    private static readonly List<int[]> WinningPlaces = CalculateWinningPlaces();

    public List<int> TheBoard { get; private set; } = new(new int[Columns * Rows]);

    /// <summary>Number of pieces played so far (0-based index of the next piece).</summary>
    public int CurrentTurn => TheBoard.Count(cell => cell != 0);

    /// <summary>The player whose turn it is: 1 or 2.</summary>
    public int PlayerTurn => CurrentTurn % 2 + 1;

    public void ResetBoard()
    {
        TheBoard = new List<int>(new int[Columns * Rows]);
    }

    /// <summary>
    /// Drops a piece for the current player into <paramref name="column"/>.
    /// Returns the row it landed in, 1 (top) to 6 (bottom).
    /// </summary>
    public byte PlayPiece(int column)
    {
        if (CheckForWin() != WinState.No_Winner)
            throw new ArgumentException("Game is over");

        if (column < 0 || column >= Columns)
            throw new ArgumentException("Invalid column");

        if (TheBoard[column] != 0)
            throw new ArgumentException("Column is full");

        // Start at the bottom row and move up until an empty cell is found.
        var landingSpot = column + (Rows - 1) * Columns;
        while (TheBoard[landingSpot] != 0)
            landingSpot -= Columns;

        TheBoard[landingSpot] = PlayerTurn;
        return (byte)(landingSpot / Columns + 1);
    }

    public WinState CheckForWin()
    {
        foreach (var places in WinningPlaces)
        {
            var first = TheBoard[places[0]];
            if (first != 0 && places.All(p => TheBoard[p] == first))
                return first == 1 ? WinState.Player1_Wins : WinState.Player2_Wins;
        }

        return CurrentTurn == Columns * Rows ? WinState.Tie : WinState.No_Winner;
    }

    private static List<int[]> CalculateWinningPlaces()
    {
        var places = new List<int[]>();

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Columns; col++)
            {
                int Cell(int r, int c) => r * Columns + c;

                // Horizontal
                if (col <= Columns - 4)
                    places.Add([Cell(row, col), Cell(row, col + 1), Cell(row, col + 2), Cell(row, col + 3)]);

                // Vertical
                if (row <= Rows - 4)
                    places.Add([Cell(row, col), Cell(row + 1, col), Cell(row + 2, col), Cell(row + 3, col)]);

                // Diagonal down-right
                if (row <= Rows - 4 && col <= Columns - 4)
                    places.Add([Cell(row, col), Cell(row + 1, col + 1), Cell(row + 2, col + 2), Cell(row + 3, col + 3)]);

                // Diagonal down-left
                if (row <= Rows - 4 && col >= 3)
                    places.Add([Cell(row, col), Cell(row + 1, col - 1), Cell(row + 2, col - 2), Cell(row + 3, col - 3)]);
            }
        }

        return places;
    }
}
