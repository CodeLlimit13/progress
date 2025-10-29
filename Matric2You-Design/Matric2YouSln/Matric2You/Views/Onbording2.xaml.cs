namespace Matric2You.Views;

public partial class Onbording2 : ContentPage
{
	public Onbording2()
	{
		InitializeComponent();
	}

    private async void OnNextClicked(object sender, EventArgs e)
    {
        // Go directly to TabBar (MainApp route)
        await Shell.Current.GoToAsync("//HomePage");
    }
}