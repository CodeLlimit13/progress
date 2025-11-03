using Matric2You.Models;
using Matric2You.ViewModel;
using Matric2You.Helpers;

namespace Matric2You.Views;

public partial class MathTestPage : ContentPage
{
 private readonly MathTestViewModel _vm;
 public MathTestPage(MathTestType type) : this(new MathTestViewModel(type, ServiceHelper.GetService<Matric2You.Services.ITestProgressService>())) { }
 public MathTestPage(MathTestViewModel vm)
 {
 InitializeComponent();
 BindingContext = _vm = vm;
 TitleLabel.Text = _vm.Title;
 _vm.OnSubmitted += OnSubmitted;
 }

 private void OnOptionCheckedChanged(object? sender, CheckedChangedEventArgs e)
 {
 if (e.Value is true && sender is RadioButton rb && _vm.CurrentQuestion is not null)
 {
 var options = _vm.CurrentQuestion.Options;
 var selectedText = rb.Content?.ToString();
 var index = Array.IndexOf(options, selectedText);
 if (index >=0)
 {
 _vm.CurrentQuestion.SelectedIndex = index;
 }
 }
 }

 private async void OnSubmitClicked(object sender, EventArgs e)
 {
 await MainThread.InvokeOnMainThreadAsync(() => _vm.SubmitCommand.Execute(null));
 }

 private async void OnSubmitted(object? sender, TestSubmittedEventArgs e)
 {
 await DisplayAlert("Submitted", $"You answered {e.Correct} / {e.Total} correctly.", "OK");
 // Navigate back and force LearningPath to refresh
 if (Navigation.NavigationStack.LastOrDefault() is LearningPath lp)
 {
 // call LoadAsync to immediately update UI
 await lp.Dispatcher.DispatchAsync(async () => await (lp.BindingContext as LearningPathViewModel)!.LoadAsync());
 }
 await Navigation.PopAsync();
 }
}
