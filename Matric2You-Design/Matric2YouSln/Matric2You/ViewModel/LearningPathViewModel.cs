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

    private UserProgressSummaryDto? _summary;
    public UserProgressSummaryDto? Summary { get => _summary; set => SetProperty(ref _summary, value); }

    // Display name for this test scenario
    public string UserName { get; } = "Llimit";

    // Maths test progress
    private MathTestProgress _practiceProgress = new() { Type = MathTestType.Practice, TotalQuestions = 5 };
    public MathTestProgress PracticeProgress
    {
        get => _practiceProgress;
        set
        {
            if (SetProperty(ref _practiceProgress, value))
            {
                OnPropertyChanged(nameof(CompletedTests));
                OnPropertyChanged(nameof(TotalRewards));
                OnPropertyChanged(nameof(AverageScore));
            }
        }
    }
    private MathTestProgress _quizProgress = new() { Type = MathTestType.Quiz, TotalQuestions = 5 };
    public MathTestProgress QuizProgress
    {
        get => _quizProgress;
        set
        {
            if (SetProperty(ref _quizProgress, value))
            {
                OnPropertyChanged(nameof(CompletedTests));
                OnPropertyChanged(nameof(TotalRewards));
                OnPropertyChanged(nameof(AverageScore));
            }
        }
    }
    private MathTestProgress _examProgress = new() { Type = MathTestType.Exam, TotalQuestions = 5 };
    public MathTestProgress ExamProgress
    {
        get => _examProgress;
        set
        {
            if (SetProperty(ref _examProgress, value))
            {
                OnPropertyChanged(nameof(CompletedTests));
                OnPropertyChanged(nameof(TotalRewards));
                OnPropertyChanged(nameof(AverageScore));
            }
        }
    }

    public ObservableCollection<SubjectProgressDto> Subjects { get; } = new();
    public ObservableCollection<SubjectTestResultDto> RecentTests { get; } = new();

    // Aggregates for the dashboard blocks
    public int TotalTests => 3;
    public int CompletedTests => (PracticeProgress.Completed ? 1 : 0) + (QuizProgress.Completed ? 1 : 0) + (ExamProgress.Completed ? 1 : 0);
    public int TotalRewards => PracticeProgress.Correct + QuizProgress.Correct + ExamProgress.Correct; // simple reward model:1 point per correct
    public double AverageScore
    {
        get
        {
            var sum = PracticeProgress.ScorePercent + QuizProgress.ScorePercent + ExamProgress.ScorePercent;
            return System.Math.Round(sum / TotalTests, 1);
        }
    }

    public Command RefreshCommand { get; }

    public LearningPathViewModel(IProgressApiService api, IUserContext userContext, ITestProgressService testProgress)
    {
        _api = api;
        _userContext = userContext;
        _testProgress = testProgress;
        RefreshCommand = new Command(async () => await LoadAsync());
    }

    public async Task LoadAsync(CancellationToken ct = default)
    {
        if (IsBusy) return;
        IsBusy = true;
        Error = null;
        try
        {
            // Always refresh local maths test progress first (works offline/unauthenticated)
            PracticeProgress = _testProgress.GetProgress(MathTestType.Practice);
            QuizProgress = _testProgress.GetProgress(MathTestType.Quiz);
            ExamProgress = _testProgress.GetProgress(MathTestType.Exam);

            // Then try to load remote summary/subjects/tests if user is available
            var userId = await _userContext.GetUserIdAsync(ct);
            if (userId is not null)
            {
                Summary = await _api.GetUserSummaryAsync(userId.Value, ct);

                Subjects.Clear();
                var subjects = await _api.GetSubjectsProgressAsync(userId.Value, ct);
                foreach (var s in subjects) Subjects.Add(s);

                RecentTests.Clear();
                var tests = await _api.GetTestResultsAsync(userId.Value, page: 1, pageSize: 10, ct);
                foreach (var t in tests) RecentTests.Add(t);
            }
            else
            {
                // Keep previous server-backed data, but ensure aggregates reflect local tests
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
