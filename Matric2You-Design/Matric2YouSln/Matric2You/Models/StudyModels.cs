namespace Matric2You.Models;

public enum StudyTopic
{
 Algebra,
 Geometry
}

public sealed class StudyProgress
{
 public StudyTopic Topic { get; set; }
 public int TotalSections { get; set; }
 public int CurrentSection { get; set; }
 public bool Completed => CurrentSection >= TotalSections -1 && TotalSections >0;
 public double CompletionPercent => TotalSections <=0 ?0 : ((double)(CurrentSection +1) / TotalSections) *100.0;
}
