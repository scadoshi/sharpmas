using Sharpmas.Domain.Solution.Common;

namespace Sharpmas.Domain.Solution.Year2016.Day01;

/// <summary>2016 day 1: walk the instructions, then measure the walk.</summary>
public class Puzzle : ISolution<Puzzle>
{
    /// <summary>Parsed here, so a bad input fails before either part runs.</summary>
    readonly Instructions instructions;

    Puzzle(Instructions instructions)
    {
        this.instructions = instructions;
    }

    public static Puzzle Parse(string input) => new(Instructions.Parse(input));

    /// <summary>How far the end of the walk is from where it started.</summary>
    public Answer PartOne()
    {
        var pose = Pose.Start;
        foreach (var instruction in instructions)
        {
            pose = pose.Turned(instruction.Turn).SaturatingMoved(instruction.Distance);
        }
        return Answer.Solved(pose.Position.DistanceFromOrigin().ToString());
    }

    /// <summary>How far the first place visited twice is from the start.</summary>
    /// <remarks>
    /// Walks a block at a time rather than a segment at a time, since a repeat
    /// can happen part way along one. The position is checked before stepping,
    /// so each segment's endpoint is tested at the start of the next one rather
    /// than being skipped or counted twice.
    /// </remarks>
    public Answer PartTwo()
    {
        var pose = Pose.Start;
        var visited = new HashSet<Point>();

        foreach (var instruction in instructions)
        {
            pose = pose.Turned(instruction.Turn);
            for (var step = 0; step < instruction.Distance; step++)
            {
                if (!visited.Add(pose.Position))
                {
                    return Answer.Solved(pose.Position.DistanceFromOrigin().ToString());
                }
                pose = pose.SaturatingMoved(1);
            }
        }

        // Nothing repeated. Returning the final distance here would be part
        // one's answer wearing part two's hat.
        return new Answer.None();
    }
}
