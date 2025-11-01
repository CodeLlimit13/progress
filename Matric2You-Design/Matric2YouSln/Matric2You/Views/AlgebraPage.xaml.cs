using Matric2You.Models;
using Matric2You.Services;
using Matric2You.Helpers;

namespace Matric2You.Views;

public partial class AlgebraPage : ContentPage
{
    private readonly IStudyProgressService _study;
    private readonly string _user = "Llimit";
    private readonly int _totalSections = 4;
    private int _currentSection;

    public AlgebraPage()
    {
        InitializeComponent();
        _study = ServiceHelper.GetService<IStudyProgressService>();
        var saved = _study.Get(_user, StudyTopic.Algebra);
        _currentSection = Math.Clamp(saved.CurrentSection, 0, _totalSections - 1);
        ApplySectionVisibility();
        UpdateUi();
    }

    private void ApplySectionVisibility()
    {
        Sec1.IsVisible = _currentSection == 0;
        Sec2.IsVisible = _currentSection == 1;
        Sec3.IsVisible = _currentSection == 2;
        Sec4.IsVisible = _currentSection == 3;
        // Scroll to top on section change
        _ = Scroller.ScrollToAsync(0, 0, true);
    }

    private void UpdateUi()
    {
        // Update progress and label
        Progress.Progress = (_currentSection + 1) / (double)_totalSections;
        SectionLabel.Text = $"Section {_currentSection + 1} of {_totalSections}";
        // Persist
        _study.Save(_user, StudyTopic.Algebra, _totalSections, _currentSection);
    }

    private void OnPrevClicked(object sender, EventArgs e)
    {
        if (_currentSection <= 0) return;
        _currentSection--;
        ApplySectionVisibility();
        UpdateUi();
    }

    private void OnNextClicked(object sender, EventArgs e)
    {
        if (_currentSection >= _totalSections - 1) return;
        _currentSection++;
        ApplySectionVisibility();
        UpdateUi();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
