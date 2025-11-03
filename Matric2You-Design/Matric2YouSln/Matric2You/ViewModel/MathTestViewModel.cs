using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Matric2You.Models;
using Matric2You.Services;

namespace Matric2You.ViewModel;

public partial class MathTestViewModel : ObservableObject
{
    private readonly ITestProgressService _progressService;
    private readonly MathTestType _type;

    public string Title => _type switch
    {
        MathTestType.Practice => "Mathematics Practice",
        MathTestType.Quiz => "Mathematics Quiz",
        MathTestType.Exam => "Mathematics Exam",
        _ => "Mathematics Test"
    };

    public ObservableCollection<QuestionItem> Questions { get; } = new();

    private int _currentIndex;
    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (SetProperty(ref _currentIndex, value))
            {
                NotifyComputed();
            }
        }
    }

    // Computed bindings for single-question flow
    public QuestionItem? CurrentQuestion => (_currentIndex >= 0 && _currentIndex < Questions.Count) ? Questions[_currentIndex] : null;
    public bool HasPrev => _currentIndex > 0;
    public bool HasNext => _currentIndex < Questions.Count - 1;
    public bool IsOnLast => Questions.Count > 0 && _currentIndex >= Questions.Count - 1;
    public string ProgressText => Questions.Count == 0 ? string.Empty : $"Question {_currentIndex + 1} of {Questions.Count}";

    private void NotifyComputed()
    {
        OnPropertyChanged(nameof(CurrentQuestion));
        OnPropertyChanged(nameof(HasPrev));
        OnPropertyChanged(nameof(HasNext));
        OnPropertyChanged(nameof(IsOnLast));
        OnPropertyChanged(nameof(ProgressText));
    }

    public MathTestViewModel(MathTestType type, ITestProgressService progressService)
    {
        _type = type;
        _progressService = progressService;
        LoadQuestions();
        // Ensure we start at the first question
        CurrentIndex = 0;
    }

    private void LoadQuestions()
    {
        var (q1, q2, q3, q4, q5) = _type switch
        {
            MathTestType.Practice => (
                new QuestionItem("What is 7 + 5?", new[] { "10", "11", "12", "13" }, 2),
                new QuestionItem("What is 9 - 4?", new[] { "6", "5", "4", "3" }, 1),
                new QuestionItem("What is 3 × 6?", new[] { "9", "18", "21", "24" }, 1),
                new QuestionItem("What is 16 ÷ 4?", new[] { "2", "3", "4", "5" }, 2),
                new QuestionItem("Simplify: 2x + 3x.", new[] { "5", "5x", "x^2", "6x" }, 1)
            ),
            MathTestType.Quiz => (
                new QuestionItem("Solve for x: 2x + 6 = 14.", new[] { "3", "4", "5", "6" }, 1),
                new QuestionItem("What is the area of a 5 by 3 rectangle?", new[] { "8", "12", "15", "30" }, 2),
                new QuestionItem("What is 5^2?", new[] { "10", "20", "25", "30" }, 2),
                new QuestionItem("Which of the following is a prime number?", new[] { "21", "27", "29", "39" }, 2),
                new QuestionItem("Simplify: a + a + a.", new[] { "a^3", "3a", "a+3", "3+a" }, 1)
            ),
            _ => (
                new QuestionItem("Solve: 3x - 5 = 10.", new[] { "x=3", "x=4", "x=5", "x=6" }, 2),
                new QuestionItem("Factor: x^2 - 9.", new[] { "(x-3)(x+3)", "(x-9)(x+1)", "(x-1)(x-9)", "(x+3)^2" }, 0),
                new QuestionItem("What is the derivative of x^2?", new[] { "x", "2x", "x^3", "2" }, 1),
                new QuestionItem("Simplify: 2/3 + 1/6.", new[] { "1/2", "2/9", "5/6", "3/4" }, 0),
                new QuestionItem("Solve: 2(x - 3) = 10.", new[] { "x=4", "x=5", "x=6", "x=8" }, 3)
            )
        };

        Questions.Clear();
        foreach (var q in new[] { q1, q2, q3, q4, q5 })
        {
            Questions.Add(q);
        }

        // Update computed bindings when questions change
        NotifyComputed();
    }

    [RelayCommand]
    private void Submit()
    {
        int total = Questions.Count;
        int correct = Questions.Count(q => q.SelectedIndex == q.CorrectIndex);
        _progressService.SaveResult(_type, total, correct);
        OnSubmitted?.Invoke(this, new TestSubmittedEventArgs(total, correct));
    }

    [RelayCommand]
    private void Next()
    {
        if (HasNext)
        {
            CurrentIndex = _currentIndex + 1;
        }
    }

    [RelayCommand]
    private void Prev()
    {
        if (HasPrev)
        {
            CurrentIndex = _currentIndex - 1;
        }
    }

    public event EventHandler<TestSubmittedEventArgs>? OnSubmitted;

    public sealed partial class QuestionItem : ObservableObject
    {
        public QuestionItem(string text, string[] options, int correctIndex)
        {
            Text = text;
            Options = options;
            CorrectIndex = correctIndex;
        }

        [ObservableProperty]
        private string text = string.Empty;

        [ObservableProperty]
        private string[] options = Array.Empty<string>();

        [ObservableProperty]
        private int correctIndex;

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (SetProperty(ref _selectedIndex, value))
                {
                    // Notify the parent (MathTestViewModel) to update the CurrentIndex
                    OnPropertyChanged(nameof(SelectedOption));
                }
            }
        }

        // Computed property to expose the currently selected option
        public string SelectedOption => _selectedIndex >= 0 && _selectedIndex < Options.Length ? Options[_selectedIndex] : string.Empty;
    }
}

public sealed class TestSubmittedEventArgs : EventArgs
{
    public int Total { get; }
    public int Correct { get; }

    public TestSubmittedEventArgs(int total, int correct)
    {
        Total = total;
        Correct = correct;
    }
}
