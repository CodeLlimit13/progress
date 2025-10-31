using Matric2You.Models;
using Matric2You.Services;

namespace Matric2You.ViewModel;

public sealed class LearningPathViewModel : BaseViewModel
{
 private readonly IProgressApiService _api;
 private readonly IUserContext _userContext;

 private UserProgressSummaryDto? _summary;
 public UserProgressSummaryDto? Summary { get => _summary; set => SetProperty(ref _summary, value); }

 public LearningPathViewModel(IProgressApiService api, IUserContext userContext)
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

 Summary = await _api.GetUserSummaryAsync(userId.Value, ct);
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
