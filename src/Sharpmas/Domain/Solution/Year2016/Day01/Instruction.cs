using System.Collections;
using Sharpmas.Domain.Solution.Common;

namespace Sharpmas.Domain.Solution.Year2016.Day01;

/// <summary>Which way to turn, then how far to walk. Parses from R2 or L3.</summary>
/// <remarks>
/// The letter is a <see cref="Common.Turn"/> rather than a direction, so U3 is
/// rejected instead of quietly turning nowhere and walking three.
/// </remarks>
internal sealed record Instruction(Turn Turn, int Distance)
{
    /// <summary>Reads one instruction, failing rather than defaulting.</summary>
    /// <exception cref="FormatException">
    /// Thrown when the text is empty, does not start with a turn, or has no
    /// number after it.
    /// </exception>
    public static Instruction Parse(string value)
    {
        var text = value.Trim();
        if (text.Length < 2)
        {
            throw new FormatException($"instruction too short: {value}");
        }

        if (!Common.Turn.TryParse(text[..1], out var turn))
        {
            throw new FormatException(
                $"instruction does not start with a turn: {text}. "
                    + "A heading is not a turn, so U3 is not an instruction."
            );
        }

        if (!int.TryParse(text[1..], out var distance))
        {
            throw new FormatException($"instruction has no distance: {text}");
        }

        return new Instruction(turn, distance);
    }
}

/// <summary>A whole input: comma separated instructions, all or nothing.</summary>
/// <remarks>
/// One bad instruction fails them all, since a partly parsed walk would give a
/// confidently wrong answer rather than an obviously missing one.
/// </remarks>
internal sealed class Instructions : IReadOnlyList<Instruction>
{
    readonly IReadOnlyList<Instruction> instructions;

    Instructions(IReadOnlyList<Instruction> instructions)
    {
        this.instructions = instructions;
    }

    /// <summary>Reads every instruction, trimming the spaces after each comma.</summary>
    /// <exception cref="FormatException">Thrown when any one of them will not parse.</exception>
    public static Instructions Parse(string value) =>
        new([.. value.Trim().Split(',').Select(Instruction.Parse)]);

    public Instruction this[int index] => instructions[index];

    public int Count => instructions.Count;

    public IEnumerator<Instruction> GetEnumerator() => instructions.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
