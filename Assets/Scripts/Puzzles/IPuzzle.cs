/// <summary>
/// Interface for puzzle components to work with the terminal system
/// </summary>
public interface IPuzzle
{
    bool IsSolved();
    string GetPuzzleInstructions();
}