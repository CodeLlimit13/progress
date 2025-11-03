// Home landing page code-behind: updates Learning Path aggregate progress and navigation hooks.
using Matric2You.Models;
using Matric2You.Services;
using Microsoft.Maui.Controls; // Add this using directive

namespace Matric2You.Views;

public partial class HomePage : ContentPage
{
    private readonly ITestProgressService _testProgress;
    private readonly IStudyProgressService _studyProgress;

    public HomePage()
    {
        InitializeComponent();
        // Resolve services from DI
        _testProgress = Matric2You.Helpers.ServiceHelper.GetService<ITestProgressService>();
        _studyProgress = Matric2You.Helpers.ServiceHelper.GetService<IStudyProgressService>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateLearningPathProgress();
    }

    private void UpdateLearningPathProgress()
    {
        // Tests (3 activities)
        var pDone = _testProgress.GetProgress(MathTestType.Practice).Completed ? 1 : 0;
        var qDone = _testProgress.GetProgress(MathTestType.Quiz).Completed ? 1 : 0;
        var eDone = _testProgress.GetProgress(MathTestType.Exam).Completed ? 1 : 0;

        // Study topics (2 activities)
        var algebra = _studyProgress.Get("Llimit", StudyTopic.Algebra).Completed ? 1 : 0;
        var geometry = _studyProgress.Get("Llimit", StudyTopic.Geometry).Completed ? 1 : 0;

        // Normalize to0..1 for ProgressBar
        var completed = pDone + qDone + eDone + algebra + geometry;
        const int totalActivities = 5; //3 tests +2 studies
        var normalized = (double)completed / totalActivities; //0..1

        // Update the UI
        var bar = this.FindByName<ProgressBar>("LearningPathProgress");
        if (bar != null)
            bar.Progress = normalized;
    }

    // Navigation handlers
    private async void OnNextClicked(object sender, EventArgs e) => await Navigation.PushAsync(new LearningPath());
    private async void OnBorderTapped(object sender, EventArgs e) => await Navigation.PushAsync(new CourseContentPage());
    private async void OnBorderTapped1(object sender, EventArgs e) => await Navigation.PushAsync(new LearningPath());
    private async void OnBorderTapped2(object sender, EventArgs e) => await Navigation.PushAsync(new LocationOfCenters());
}