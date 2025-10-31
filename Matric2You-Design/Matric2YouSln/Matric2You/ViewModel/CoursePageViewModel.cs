using System.Collections.ObjectModel;
using Matric2You.Models;
using Matric2You.Services;

namespace Matric2You.ViewModel;

public sealed class CoursePageViewModel : BaseViewModel
{
 private readonly IProgressApiService _api;
 private readonly IUserContext _userContext;

 public ObservableCollection<SubjectProgressDto> Subjects { get; } = new();
 public ObservableCollection<SubjectTestResultDto> RecentTests { get; } = new();

 public CoursePageViewModel(IProgressApiService api, IUserContext userContext)
 {
 _api = api;
 _userContext = userContext;
 }

 public async Task LoadAsync(CancellationToken ct = default)
 {
 if (IsBusy) return;
 IsBusy = true;
 Error = null;
 try
 {
 var userId = await _userContext.GetUserIdAsync(ct);
 if (userId is null) { Error = "User not authenticated."; return; }

 Subjects.Clear();
 var subjects = await _api.GetSubjectsProgressAsync(userId.Value, ct);
 foreach (var s in subjects) Subjects.Add(s);

 RecentTests.Clear();
 var tests = await _api.GetTestResultsAsync(userId.Value, page:1, pageSize:10, ct);
 foreach (var t in tests) RecentTests.Add(t);
 }
 catch (Exception ex)
 {
 Error = ex.Message;
 }
 finally
 {
 IsBusy = false;
 }
 }
}
