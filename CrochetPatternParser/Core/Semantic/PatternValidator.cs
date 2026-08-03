using CrochetPatternParser.Core.Ast;

public class PatternValidator
{
    public RoundValidationResult ValidateRound(RoundNode round, int roundIndex, int? expectedStitchConsumed = null)
    {
        int output = round.GetProducedStitches();
        int input = round.GetConsumedStitches();

        var roundResult = new RoundValidationResult
        {
            RoundIndex = roundIndex,
            StitchCount = output
        };

        if (roundIndex == 1)
        {
            if (output < 1)
                roundResult.Error = "Round 1 must produce at least 1 stitch";

            return roundResult;
        }

        roundResult.ExpectedStitchConsumed = expectedStitchConsumed;

        if (expectedStitchConsumed.HasValue && input != expectedStitchConsumed.Value)
        {
            roundResult.Error = $"Expected to use {expectedStitchConsumed.Value} stitches, but used {input}";
        }

        return roundResult;
    }

    public PatternValidationResult Validate(PatternNode pattern)
    {
        var result = new PatternValidationResult();

        int previousOutput = 0;

        for (int i = 0; i < pattern.Rounds.Count; i++)
        {
            var round = pattern.Rounds[i];

            int output = round.GetProducedStitches();
            int input = round.GetConsumedStitches();

            var roundResult = new RoundValidationResult
            {
                RoundIndex = i + 1,
                StitchCount = output
            };

            // Round 1: must produce ≥1 stitch
            if (i == 0 && output < 1)
            {
                roundResult.Error = "Round 1 must produce at least 1 stitch";
                result.Rounds.Add(roundResult);
                break;
            }

            // Subsequent rounds: must consume exactly previous output
            if (i > 0 && input != previousOutput)
            {
                roundResult.ExpectedStitchConsumed = previousOutput;
                roundResult.Error = $"Expected to use {previousOutput} stitches, but used {input}";
                result.Rounds.Add(roundResult);
                break;
            }

            // Everything OK
            if (i > 0)
                roundResult.ExpectedStitchConsumed = previousOutput;

            result.Rounds.Add(roundResult);
            previousOutput = output;
        }

        return result;
    }
}