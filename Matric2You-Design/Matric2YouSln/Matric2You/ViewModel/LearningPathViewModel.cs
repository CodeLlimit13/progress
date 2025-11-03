// ViewModel powering the Learning Path page including dashboard aggregates and refresh logic.
using System.Collections.ObjectModel;
using Matric2You.Models;
using Matric2You.Services;
using Microsoft.Maui.Controls;

namespace Matric2You.ViewModel;

public sealed class LearningPathViewModel : BaseViewModel
{
 private readonly IProgressApiService _api;
 private readonly IUserContext _userContext;
 private readonly ITestProgressService _testProgress;
 private readonly IStudyProgressService _studyProgress;

 private UserProgressSummaryDto? _summary;
 public UserProgressSummaryDto? Summary { get => _summary; set => SetProperty(ref _summary, value); }

 // Display name for this test scenario
 public string UserName { get; } = "Llimit";

 // Maths test progress
 private MathTestProgress _practiceProgress = new() { Type = MathTestType.Practice, TotalQuestions =5 };
 public MathTestProgress PracticeProgress
 {
 get => _practiceProgress;
 set
 {
 if (SetProperty(ref _practiceProgress, value))
 {
 OnPropertyChanged(nameof(CompletedActivities));
 OnPropertyChanged(nameof(TotalRewards));
 OnPropertyChanged(nameof(AverageScore));
 }
 }
 }
 private MathTestProgress _quizProgress = new() { Type = MathTestType.Quiz, TotalQuestions =5 };
 public MathTestProgress QuizProgress
 {
 get => _quizProgress;
 set
 {
 if (SetProperty(ref _quizProgress, value))
 {
 OnPropertyChanged(nameof(CompletedActivities));
 OnPropertyChanged(nameof(TotalRewards));
 OnPropertyChanged(nameof(AverageScore));
 }
 }
 }
 private MathTestProgress _examProgress = new() { Type = MathTestType.Exam, TotalQuestions =5 };
 public MathTestProgress ExamProgress
 {
 get => _examProgress;
 set
 {
 if (SetProperty(ref _examProgress, value))
 {
 OnPropertyChanged(nameof(CompletedActivities));
 OnPropertyChanged(nameof(TotalRewards));
 OnPropertyChanged(nameof(AverageScore));
 }
 }
 }

 // Study topic progress
 private StudyProgress _algebraStudy = new() { Topic = StudyTopic.Algebra, TotalSections =4, CurrentSection =0 };
 public StudyProgress AlgebraStudy
 {
 get => _algebraStudy;
 set
 {
 if (SetProperty(ref _algebraStudy, value))
 {
 OnPropertyChanged(nameof(CompletedActivities));
 OnPropertyChanged(nameof(TotalRewards));
 }
 }
 }
 private StudyProgress _geometryStudy = new() { Topic = StudyTopic.Geometry, TotalSections =4, CurrentSection =0 };
 public StudyProgress GeometryStudy
 {
 get => _geometryStudy;
 set
 {
 if (SetProperty(ref _geometryStudy, value))
 {
 OnPropertyChanged(nameof(CompletedActivities));
 OnPropertyChanged(nameof(TotalRewards));
 }
 }
 }

 public ObservableCollection<SubjectProgressDto> Subjects { get; } = new();
 public ObservableCollection<SubjectTestResultDto> RecentTests { get; } = new();

 // Dashboard aggregates now include2 learning activities +3 tests
 public int TotalActivities =>5;
 public int CompletedActivities => (PracticeProgress.Completed ?1 :0) + (QuizProgress.Completed ?1 :0) + (ExamProgress.Completed ?1 :0)
 + (AlgebraStudy.Completed ?1 :0) + (GeometryStudy.Completed ?1 :0);

 // Rewards:1 point per correct answer +5 points per study topic completion (once)
 public int TotalRewards => PracticeProgress.Correct + QuizProgress.Correct + ExamProgress.Correct
 + (AlgebraStudy.RewardGranted ?5 :0) + (GeometryStudy.RewardGranted ?5 :0);

 public double AverageScore
 {
 get
 {
 // Only tests count for average score
 var sum = PracticeProgress.ScorePercent + QuizProgress.ScorePercent + ExamProgress.ScorePercent;
 const int testCount =3;
 return System.Math.Round(sum / testCount,1);
 }
 }

 public Command RefreshCommand { get; }

 public LearningPathViewModel(IProgressApiService api, IUserContext userContext, ITestProgressService testProgress, IStudyProgressService studyProgress)
 {
 _api = api;
 _userContext = userContext;
 _testProgress = testProgress;
 _studyProgress = studyProgress;
 RefreshCommand = new Command(async () => await LoadAsync());
 }

 public async Task LoadAsync(CancellationToken ct = default)
 {
 if (IsBusy) return;
 IsBusy = true;
 Error = null;
 try
 {
 // Local maths test progress
 PracticeProgress = _testProgress.GetProgress(MathTestType.Practice);
 QuizProgress = _testProgress.GetProgress(MathTestType.Quiz);
 ExamProgress = _testProgress.GetProgress(MathTestType.Exam);

 // Local study progress
 AlgebraStudy = _studyProgress.Get(UserName, StudyTopic.Algebra);
 GeometryStudy = _studyProgress.Get(UserName, StudyTopic.Geometry);

 var userId = await _userContext.GetUserIdAsync(ct);
 if (userId is not null)
 {
 Summary = await _api.GetUserSummaryAsync(userId.Value, ct);

 Subjects.Clear();
 var subjects = await _api.GetSubjectsProgressAsync(userId.Value, ct);
 foreach (var s in subjects) Subjects.Add(s);

 RecentTests.Clear();
 var tests = await _api.GetTestResultsAsync(userId.Value, page:1, pageSize:10, ct);
 foreach (var t in tests) RecentTests.Add(t);
 }
 else
 {
 Error = "User not authenticated.";
 }
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
