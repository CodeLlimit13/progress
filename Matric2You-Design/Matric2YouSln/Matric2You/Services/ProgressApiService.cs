using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Matric2You.Models;

namespace Matric2You.Services;

public interface IProgressApiService
{
 Task<UserProgressSummaryDto?> GetUserSummaryAsync(int userId, CancellationToken ct = default);
 Task<IReadOnlyList<SubjectProgressDto>> GetSubjectsProgressAsync(int userId, CancellationToken ct = default);
 Task<IReadOnlyList<LessonProgressDto>> GetSubjectProgressAsync(int userId, int subjectId, CancellationToken ct = default);
 Task<IReadOnlyList<SubjectTestResultDto>> GetTestResultsAsync(int userId, int page =1, int pageSize =20, CancellationToken ct = default);
 Task<bool> UpsertProgressAsync(UpsertProgressRequest request, CancellationToken ct = default);
}

public sealed class ProgressApiService : IProgressApiService
{
 private readonly HttpClient _http;
 private readonly IUserContext _userContext;
 private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

 public ProgressApiService(HttpClient http, IUserContext userContext)
 {
 _http = http;
 _userContext = userContext;
 }

 private async Task AddAuthAsync(HttpRequestMessage req, CancellationToken ct)
 {
 var token = await _userContext.GetTokenAsync(ct);
 if (!string.IsNullOrWhiteSpace(token))
 {
 req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
 }
 }

 public async Task<UserProgressSummaryDto?> GetUserSummaryAsync(int userId, CancellationToken ct = default)
 {
 using var req = new HttpRequestMessage(HttpMethod.Get, $"api/users/{userId}/progress/summary");
 await AddAuthAsync(req, ct);
 using var res = await _http.SendAsync(req, ct);
 res.EnsureSuccessStatusCode();
 return await res.Content.ReadFromJsonAsync<UserProgressSummaryDto>(JsonOptions, ct);
 }

 public async Task<IReadOnlyList<SubjectProgressDto>> GetSubjectsProgressAsync(int userId, CancellationToken ct = default)
 {
 using var req = new HttpRequestMessage(HttpMethod.Get, $"api/users/{userId}/subjects/progress");
 await AddAuthAsync(req, ct);
 using var res = await _http.SendAsync(req, ct);
 res.EnsureSuccessStatusCode();
 var data = await res.Content.ReadFromJsonAsync<List<SubjectProgressDto>>(JsonOptions, ct);
 return data ?? [];
 }

 public async Task<IReadOnlyList<LessonProgressDto>> GetSubjectProgressAsync(int userId, int subjectId, CancellationToken ct = default)
 {
 using var req = new HttpRequestMessage(HttpMethod.Get, $"api/subjects/{subjectId}/progress?userId={userId}");
 await AddAuthAsync(req, ct);
 using var res = await _http.SendAsync(req, ct);
 res.EnsureSuccessStatusCode();
 var data = await res.Content.ReadFromJsonAsync<List<LessonProgressDto>>(JsonOptions, ct);
 return data ?? [];
 }

 public async Task<IReadOnlyList<SubjectTestResultDto>> GetTestResultsAsync(int userId, int page =1, int pageSize =20, CancellationToken ct = default)
 {
 using var req = new HttpRequestMessage(HttpMethod.Get, $"api/users/{userId}/tests/results?page={page}&pageSize={pageSize}");
 await AddAuthAsync(req, ct);
 using var res = await _http.SendAsync(req, ct);
 res.EnsureSuccessStatusCode();
 var data = await res.Content.ReadFromJsonAsync<List<SubjectTestResultDto>>(JsonOptions, ct);
 return data ?? [];
 }

 public async Task<bool> UpsertProgressAsync(UpsertProgressRequest request, CancellationToken ct = default)
 {
 using var req = new HttpRequestMessage(HttpMethod.Post, "api/progress")
 {
 Content = new StringContent(JsonSerializer.Serialize(request, JsonOptions), Encoding.UTF8, "application/json")
 };
 await AddAuthAsync(req, ct);
 using var res = await _http.SendAsync(req, ct);
 return res.IsSuccessStatusCode;
 }
}
