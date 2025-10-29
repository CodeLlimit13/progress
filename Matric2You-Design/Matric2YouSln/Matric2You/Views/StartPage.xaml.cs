namespace Matric2You.Views;

public partial class StartPage : ContentPage
{
	public StartPage()
	{
		InitializeComponent();
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
    }
    private async void OnNextClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Onbording1());
    }
    private async void OnNextClicked1(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }
}