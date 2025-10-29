namespace Matric2You.Views;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
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