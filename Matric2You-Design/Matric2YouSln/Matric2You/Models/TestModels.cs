namespace Matric2You.Models;

public enum MathTestType
{
 Practice,
 Quiz,
 Exam
}

public sealed class MathQuestion
{
 public string Text { get; set; } = string.Empty;
 public string[] Options { get; set; } = Array.Empty<string>();
 public int CorrectIndex { get; set; }
}

public sealed class MathTestProgress
{
 public MathTestType Type { get; set; }
 public int TotalQuestions { get; set; } =5;
 public int Answered { get; set; }
 public int Correct { get; set; }

 public bool Completed => Answered >= TotalQuestions;
 public double ScorePercent => TotalQuestions ==0 ?0 : (double)Correct / TotalQuestions *100.0;
}
