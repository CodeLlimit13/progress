using System.Text.Json;
using Matric2You.Models;

namespace Matric2You.Services;

public interface ITestProgressService
{
 MathTestProgress GetProgress(MathTestType type);
 void SaveResult(MathTestType type, int totalQuestions, int correct);
 void Reset(MathTestType type);
}

public sealed class TestProgressService : ITestProgressService
{
 private const string KeyPrefix = "math_test_progress_";
 private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

 public MathTestProgress GetProgress(MathTestType type)
 {
 var key = KeyPrefix + type;
 var json = Preferences.Default.Get(key, string.Empty);
 if (string.IsNullOrWhiteSpace(json))
 {
 return new MathTestProgress { Type = type, TotalQuestions =5, Answered =0, Correct =0 };
 }
 try
 {
 var p = JsonSerializer.Deserialize<MathTestProgress>(json, JsonOptions);
 return p ?? new MathTestProgress { Type = type, TotalQuestions =5 };
 }
 catch
 {
 return new MathTestProgress { Type = type, TotalQuestions =5 };
 }
 }

 public void SaveResult(MathTestType type, int totalQuestions, int correct)
 {
 var p = new MathTestProgress
 {
 Type = type,
 TotalQuestions = totalQuestions,
 Answered = totalQuestions,
 Correct = correct
 };
 var key = KeyPrefix + type;
 var json = JsonSerializer.Serialize(p, JsonOptions);
 Preferences.Default.Set(key, json);
 }

 public void Reset(MathTestType type)
 {
 var key = KeyPrefix + type;
 Preferences.Default.Remove(key);
 }
}
