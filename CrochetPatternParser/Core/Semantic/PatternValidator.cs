using CrochetPatternParser.Core.Ast;

public class PatternValidator
{
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