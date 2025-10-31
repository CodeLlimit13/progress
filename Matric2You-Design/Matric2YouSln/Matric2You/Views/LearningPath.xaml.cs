using Matric2You.ViewModel;
using Matric2You.Helpers;

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
}