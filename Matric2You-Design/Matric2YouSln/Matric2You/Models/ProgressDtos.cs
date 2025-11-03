using System.Text.Json.Serialization;

namespace Matric2You.Models;

public sealed class UserProgressSummaryDto
{
    [JsonPropertyName("totalLessons")] public int TotalLessons { get; set; } // all lessons available
    [JsonPropertyName("completedLessons")] public int CompletedLessons { get; set; } // finished lessons
    [JsonPropertyName("inProgressLessons")] public int InProgressLessons { get; set; } // currently in progress
    [JsonPropertyName("notStartedLessons")] public int NotStartedLessons { get; set; } // not started yet
    [JsonPropertyName("completionPercent")] public double CompletionPercent { get; set; } //0..100
    [JsonPropertyName("timeSpentMinutes")] public int TimeSpentMinutes { get; set; } // total minutes
    [JsonPropertyName("averageScore")] public double? AverageScore { get; set; } // null if no tests
    [JsonPropertyName("lastAccessedUtc")] public DateTime? LastAccessedUtc { get; set; } // last activity time
}

public sealed class SubjectTestResultDto
{
    [JsonPropertyName("testId")] public int TestId { get; set; }
    [JsonPropertyName("subjectId")] public int SubjectId { get; set; }
    [JsonPropertyName("scorePercentage")] public double ScorePercentage { get; set; } //0..100
    [JsonPropertyName("timeTakenSeconds")] public int? TimeTakenSeconds { get; set; }
    [JsonPropertyName("completedAtUtc")] public DateTime? CompletedAtUtc { get; set; }
}

public sealed class SubjectProgressDto
{
    [JsonPropertyName("subjectId")] public int SubjectId { get; set; }
    [JsonPropertyName("subjectName")] public string SubjectName { get; set; } = string.Empty;
    [JsonPropertyName("completionPercent")] public double CompletionPercent { get; set; } //0..100
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
    [JsonPropertyName("status")] public string Status { get; set; } = "in-progress"; // desired state
    [JsonPropertyName("timeSpentDeltaMinutes")] public int TimeSpentDeltaMinutes { get; set; } =0; // minutes to add
    [JsonPropertyName("score")] public double? Score { get; set; }
    [JsonPropertyName("lastAccessedUtc")] public DateTime? LastAccessedUtc { get; set; } = DateTime.UtcNow; // client timestamp
}
