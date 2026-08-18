namespace Sharpmas.Domain.Solution.Year2015.Day01;

public class Puzzle : ISolution<Puzzle>
{
    public required string Input { get; init; }

    private int CharValue(char c)
    {
        return c switch
        {
            '(' => 1,
            ')' => -1,
            _ => 0,
        };
    }

    public static Puzzle Parse(string input)
    {
        return new Puzzle { Input = input };
    }

    public Answer PartOne()
    {
        return Answer.Solved(Input.Sum(CharValue).ToString());
    }

    public Answer PartTwo()
    {
        int total = 0;
        foreach ((int i, char c) in Input.Index())
        {
            total += CharValue(c);
            if (total < 0)
            {
                return Answer.Solved((i + 1).ToString());
            }
        }
        // The floor never reached the basement, which is not the same as
        // reaching it at the end. Returning the final floor would be part one's
        // answer wearing part two's hat.
        return new Answer.None();
    }
}
