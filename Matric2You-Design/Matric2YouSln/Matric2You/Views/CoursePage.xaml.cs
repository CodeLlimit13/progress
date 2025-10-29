namespace Matric2You.Views;

public partial class CoursePage : ContentPage
{
	public CoursePage()
	{
		InitializeComponent();
	}

    private async void OnNextClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CourseContentPage());
    }
}