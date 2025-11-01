using Matric2You.Models;
using Matric2You.Services;
using Microsoft.Maui.Controls; // Add this using directive

namespace Matric2You.Views;

public partial class HomePage : ContentPage
{
    private readonly ITestProgressService _testProgress;

    public HomePage()
    {
        InitializeComponent();
        _testProgress = Matric2You.Helpers.ServiceHelper.GetService<ITestProgressService>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateLearningPathProgress();
    }

    private void UpdateLearningPathProgress()
    {
        // Progress reflects fraction of tests completed (Practice, Quizz, Exam)
        var pDone = _testProgress.GetProgress(MathTestType.Practice).Completed ? 1 : 0;
        var qDone = _testProgress.GetProgress(MathTestType.Quiz).Completed ? 1 : 0;
        var eDone = _testProgress.GetProgress(MathTestType.Exam).Completed ? 1 : 0;
        var completed = pDone + qDone + eDone;
        var normalized = (double)completed / 3.0; //0.0,0.33,0.66,1.0

        // Resolve the ProgressBar by name to avoid any ambiguity with generated members
        var bar = this.FindByName<ProgressBar>("LearningPathProgress");
        if (bar != null)
            bar.Progress = normalized;
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LearningPath());
    }
    private async void OnBorderTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CourseContentPage());
    }
    private async void OnBorderTapped1(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LearningPath());
    }
    private async void OnBorderTapped2(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LocationOfCenters());
    }
}