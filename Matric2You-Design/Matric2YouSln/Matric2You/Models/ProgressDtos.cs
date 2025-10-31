using System.Text.Json.Serialization;

namespace Matric2You.Models;

public sealed class UserProgressSummaryDto
{
 [JsonPropertyName("totalLessons")] public int TotalLessons { get; set; }
 [JsonPropertyName("completedLessons")] public int CompletedLessons { get; set; }
 [JsonPropertyName("inProgressLessons")] public int InProgressLessons { get; set; }
 [JsonPropertyName("notStartedLessons")] public int NotStartedLessons { get; set; }
 [JsonPropertyName("completionPercent")] public double CompletionPercent { get; set; }
 [JsonPropertyName("timeSpentMinutes")] public int TimeSpentMinutes { get; set; }
 [JsonPropertyName("averageScore")] public double? AverageScore { get; set; }
 [JsonPropertyName("lastAccessedUtc")] public DateTime? LastAccessedUtc { get; set; }
}

public sealed class SubjectTestResultDto
{
 [JsonPropertyName("testId")] public int TestId { get; set; }
 [JsonPropertyName("subjectId")] public int SubjectId { get; set; }
 [JsonPropertyName("scorePercentage")] public double ScorePercentage { get; set; }
 [JsonPropertyName("timeTakenSeconds")] public int? TimeTakenSeconds { get; set; }
 [JsonPropertyName("completedAtUtc")] public DateTime? CompletedAtUtc { get; set; }
}

public sealed class SubjectProgressDto
{
 [JsonPropertyName("subjectId")] public int SubjectId { get; set; }
 [JsonPropertyName("subjectName")] public string SubjectName { get; set; } = string.Empty;
 [JsonPropertyName("completionPercent")] public double CompletionPercent { get; set; }
 [JsonPropertyName("timeSpentMinutes")] public int TimeSpentMinutes { get; set; }
 [JsonPropertyName("testsTaken")] public int TestsTaken { get; set; }
 [JsonPropertyName("averageScore")] public double? AverageScore { get; set; }
 [JsonPropertyName("recentScores")] public List<SubjectTestResultDto> RecentScores { get; set; } = new();
}

public sealed class LessonProgressDto
{
 [JsonPropertyName("lessonId")] public int LessonId { get; set; }
 [JsonPropertyName("lessonTitle")] public string LessonTitle { get; set; } = string.Empty;
 [JsonPropertyName("status")] public string Status { get; set; } = string.Empty; // completed | in-progress | not started
 [JsonPropertyName("timeSpentMinutes")] public int TimeSpentMinutes { get; set; }
 [JsonPropertyName("score")] public double? Score { get; set; }
 [JsonPropertyName("lastAccessedUtc")] public DateTime? LastAccessedUtc { get; set; }
}

public sealed class UpsertProgressRequest
{
 [JsonPropertyName("userId")] public int UserId { get; set; }
 [JsonPropertyName("lessonId")] public int LessonId { get; set; }
 [JsonPropertyName("status")] public string Status { get; set; } = "in-progress";
 [JsonPropertyName("timeSpentDeltaMinutes")] public int TimeSpentDeltaMinutes { get; set; } =0;
 [JsonPropertyName("score")] public double? Score { get; set; }
 [JsonPropertyName("lastAccessedUtc")] public DateTime? LastAccessedUtc { get; set; } = DateTime.UtcNow;
}
