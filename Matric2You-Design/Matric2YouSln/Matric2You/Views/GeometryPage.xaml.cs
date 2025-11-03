using Matric2You.Models;
using Matric2You.Services;
using Matric2You.Helpers;
using Microsoft.Maui.Controls;

namespace Matric2You.Views;

public partial class GeometryPage : ContentPage
{
    private readonly IStudyProgressService _study;
    private readonly string _user = "Llimit";
    private readonly int _totalSections = 4;
    private int _currentSection;

    public GeometryPage()
    {
        InitializeComponent();
        _study = ServiceHelper.GetService<IStudyProgressService>();
        var saved = _study.Get(_user, StudyTopic.Geometry);
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
        Scroller.ScrollToAsync(0, 0, true);
    }

    private void UpdateUi()
    {
        Progress.Progress = (_currentSection + 1) / (double)_totalSections;
        SectionLabel.Text = $"Section {_currentSection + 1} of {_totalSections}";
        _study.Save(_user, StudyTopic.Geometry, _totalSections, _currentSection);
        var primaryBtn = this.FindByName<Button>("PrimaryActionButton");
        var nextBtn = this.FindByName<Button>("NextButton");
        bool onLast = _currentSection >= _totalSections - 1;
        if (onLast)
        {
            _study.MarkCompletedAndReward(_user, StudyTopic.Geometry);
            if (primaryBtn is not null) primaryBtn.Text = "Complete";
            if (nextBtn is not null) nextBtn.IsVisible = false;
        }
        else
        {
            if (primaryBtn is not null) primaryBtn.Text = "Back";
            if (nextBtn is not null) nextBtn.IsVisible = true;
        }
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

    private async void OnPrimaryActionClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
