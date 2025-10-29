namespace Matric2You.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void OnNextClicked1(object sender, EventArgs e)
    {
        // Go directly to TabBar (MainApp route)
        await Shell.Current.GoToAsync("//HomePage");
    }
}