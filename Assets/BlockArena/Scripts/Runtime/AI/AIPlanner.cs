using System;
using System.Collections.Generic;

public static class AIPlanner
{
    public static T SelectHighestScoring<T>(
        IReadOnlyList<T> options,
        Func<T, int> score
    )
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (score == null)
        {
            throw new ArgumentNullException(nameof(score));
        }

        if (options.Count == 0)
        {
            throw new ArgumentException(
                "At least one option is required.",
                nameof(options)
            );
        }

        T bestOption = options[0];
        int bestScore = score(bestOption);

        for (int index = 1; index < options.Count; index++)
        {
            T option = options[index];
            int optionScore = score(option);

            if (optionScore > bestScore)
            {
                bestOption = option;
                bestScore = optionScore;
            }
        }

        return bestOption;
    }

    public static T SelectLowestScoring<T>(
        IReadOnlyList<T> options,
        Func<T, int> score
    )
    {
        return SelectHighestScoring(
            options,
            option => -score(option)
        );
    }
}
