using System.Text.Json;
using Matric2You.Models;

namespace Matric2You.Services;

public interface IStudyProgressService
{
 StudyProgress Get(string userName, StudyTopic topic);
 void Save(string userName, StudyTopic topic, int totalSections, int currentSection);
}

public sealed class StudyProgressService : IStudyProgressService
{
 private const string KeyPrefix = "study_";
 private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

 public StudyProgress Get(string userName, StudyTopic topic)
 {
 var key = BuildKey(userName, topic);
 var json = Preferences.Default.Get(key, string.Empty);
 if (string.IsNullOrWhiteSpace(json))
 return new StudyProgress { Topic = topic, TotalSections =5, CurrentSection =0 };
 try
 {
 return JsonSerializer.Deserialize<StudyProgress>(json, Options) ?? new StudyProgress { Topic = topic, TotalSections =5, CurrentSection =0 };
 }
 catch
 {
 return new StudyProgress { Topic = topic, TotalSections =5, CurrentSection =0 };
 }
 }

 public void Save(string userName, StudyTopic topic, int totalSections, int currentSection)
 {
 var key = BuildKey(userName, topic);
 var data = new StudyProgress { Topic = topic, TotalSections = totalSections, CurrentSection = currentSection };
 Preferences.Default.Set(key, JsonSerializer.Serialize(data, Options));
 }

 private static string BuildKey(string user, StudyTopic topic) => $"{KeyPrefix}{user}_{topic}";
}
