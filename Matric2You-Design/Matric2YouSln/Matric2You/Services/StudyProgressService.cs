using System.Text.Json;
using Matric2You.Models;

namespace Matric2You.Services;

public interface IStudyProgressService
{
    StudyProgress Get(string userName, StudyTopic topic);
    void Save(string userName, StudyTopic topic, int totalSections, int currentSection);
    void MarkCompletedAndReward(string userName, StudyTopic topic);
}

public sealed class StudyProgressService : IStudyProgressService
{
    // Preference key prefix
    private const string KeyPrefix = "study_";

    // Json options for compact storage
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    // Load or initialize progress for a user/topic
    public StudyProgress Get(string userName, StudyTopic topic)
    {
        var key = BuildKey(userName, topic);
        var json = Preferences.Default.Get(key, string.Empty);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new StudyProgress
            {
                Topic = topic,
                TotalSections = 5,
                CurrentSection = 0,
                RewardGranted = false
            };
        }

        try
        {
            return JsonSerializer.Deserialize<StudyProgress>(json, Options) ?? new StudyProgress
            {
                Topic = topic,
                TotalSections = 5,
                CurrentSection = 0,
                RewardGranted = false
            };
        }
        catch
        {
            return new StudyProgress
            {
                Topic = topic,
                TotalSections = 5,
                CurrentSection = 0,
                RewardGranted = false
            };
        }
    }

    // Persist current section and keep reward flag
    public void Save(string userName, StudyTopic topic, int totalSections, int currentSection)
    {
        var key = BuildKey(userName, topic);
        var data = new StudyProgress
        {
            Topic = topic,
            TotalSections = totalSections,
            CurrentSection = currentSection,
            RewardGranted = Get(userName, topic).RewardGranted
        };

        Preferences.Default.Set(key, JsonSerializer.Serialize(data, Options));
    }

    // Mark completed and grant reward once
    public void MarkCompletedAndReward(string userName, StudyTopic topic)
    {
        var existing = Get(userName, topic);

        if (existing.RewardGranted)
        {
            return; // reward once
        }

        existing.RewardGranted = true;

        var key = BuildKey(userName, topic);
        Preferences.Default.Set(key, JsonSerializer.Serialize(existing, Options));
    }

    private static string BuildKey(string user, StudyTopic topic) => $"{KeyPrefix}{user}_{topic}";
}
