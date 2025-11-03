// Models representing study topics and progress including computed completion percentage.
namespace Matric2You.Models;

public enum StudyTopic
{
    Algebra,
    Geometry
}

public sealed class StudyProgress
{
    // Topic this progress applies to
    public StudyTopic Topic { get; set; }

    // Total number of sections in the topic
    public int TotalSections { get; set; }

    // Zero-based current section index
    public int CurrentSection { get; set; }

    // Whether a completion reward has been granted
    public bool RewardGranted { get; set; }

    // Consider completed on last section
    public bool Completed => CurrentSection >= TotalSections - 1 && TotalSections > 0;

    // Percentage across sections:0% at start,100% on last
    public double CompletionPercent
    {
        get
        {
            if (TotalSections <= 0)
                return 0.0;

            if (TotalSections == 1)
                return Completed ? 100.0 : 0.0;

            var denom = TotalSections - 1;
            var pct = ((double)CurrentSection / denom) * 100.0;

            if (pct < 0)
                pct = 0;

            if (pct > 100)
                pct = 100;

            return pct;
        }
    }
}
