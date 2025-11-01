using Matric2You.ViewModel;
using Matric2You.Helpers;
using Matric2You.Models;

namespace Matric2You.Views;

public partial class LearningPath : ContentPage
{
    private readonly LearningPathViewModel _vm;

    public LearningPath() : this(ResolveVm()) { }

    public LearningPath(LearningPathViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private static LearningPathViewModel ResolveVm()
    {
        return ServiceHelper.GetService<LearningPathViewModel>();
    }

    private async void OnPracticeClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MathTestPage(MathTestType.Practice));
    }

    private async void OnQuizClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MathTestPage(MathTestType.Quiz));
    }

    private async void OnExamClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MathTestPage(MathTestType.Exam));
    }

    private async void OnAlgebraClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AlgebraPage());
    }

    private async void OnGeometryClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GeometryPage());
    }
}