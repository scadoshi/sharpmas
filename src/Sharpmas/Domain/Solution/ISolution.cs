namespace Sharpmas.Domain.Solution;

public interface ISolution<TSelf>
    where TSelf : ISolution<TSelf>
{
    public static abstract TSelf Parse(string input);
    public Answer PartOne();
    public Answer PartTwo();
}
