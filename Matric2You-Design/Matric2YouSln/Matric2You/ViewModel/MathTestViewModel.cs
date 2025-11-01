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
 MathTestType.Quiz => "Mathematics Quizz",
 MathTestType.Exam => "Mathematics Exam",
 _ => "Mathematics Test"
 };

 public ObservableCollection<QuestionItem> Questions { get; } = new();

 public MathTestViewModel(MathTestType type, ITestProgressService progressService)
 {
 _type = type;
 _progressService = progressService;
 LoadQuestions();
 }

 private void LoadQuestions()
 {
 var (q1,q2,q3,q4,q5) = _type switch
 {
 MathTestType.Practice => (
 new QuestionItem("What is7 +5?", new[]{"10","11","12","13"},2),
 new QuestionItem("What is9 -4?", new[]{"6","5","4","3"},1),
 new QuestionItem("What is3 ×6?", new[]{"9","18","21","24"},1),
 new QuestionItem("What is16 ÷4?", new[]{"2","3","4","5"},2),
 new QuestionItem("Simplify:2x +3x", new[]{"5","5x","x^2","6x"},1)
 ),
 MathTestType.Quiz => (
 new QuestionItem("Solve for x:2x +6 =14", new[]{"3","4","5","6"},1),
 new QuestionItem("Area of5 by3 rectangle?", new[]{"8","12","15","30"},2),
 new QuestionItem("What is5^2?", new[]{"10","20","25","30"},2),
 new QuestionItem("Which is a prime number?", new[]{"21","27","29","39"},2),
 new QuestionItem("Simplify: (a+a+a)", new[]{"a^3","3a","a+3","3+a"},1)
 ),
 _ => (
 new QuestionItem("Solve:3x -5 =10", new[]{"x=3","x=4","x=5","x=6"},2),
 new QuestionItem("Factor: x^2 -9", new[]{"(x-3)(x+3)","(x-9)(x+1)","(x-1)(x-9)","(x+3)^2"},0),
 new QuestionItem("Derivative of x^2", new[]{"x","2x","x^3","2"},1),
 new QuestionItem("Simplify:2/3 +1/6", new[]{"1/2","2/9","5/6","3/4"},0),
 new QuestionItem("Solve:2(x-3)=10", new[]{"x=4","x=5","x=6","x=8"},2)
 )
 };

 Questions.Clear();
 foreach (var q in new[]{q1,q2,q3,q4,q5}) Questions.Add(q);
 }

 [RelayCommand]
 private void Submit()
 {
 int total = Questions.Count;
 int correct = Questions.Count(q => q.SelectedIndex == q.CorrectIndex);
 _progressService.SaveResult(_type, total, correct);
 OnSubmitted?.Invoke(this, new TestSubmittedEventArgs(total, correct));
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

 [ObservableProperty]
 private int selectedIndex = -1;
 }
}

public sealed class TestSubmittedEventArgs : EventArgs
{
 public int Total { get; }
 public int Correct { get; }
 public TestSubmittedEventArgs(int total, int correct)
 {
 Total = total; Correct = correct;
 }
}
