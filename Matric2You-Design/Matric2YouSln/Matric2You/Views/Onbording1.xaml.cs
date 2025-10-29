namespace Matric2You.Views;

public partial class Onbording1 : ContentPage
{
	public Onbording1()
	{
		InitializeComponent();
	}
    private async void OnNextClicked(object sender, EventArgs e)
    {
        // Go directly to TabBar (MainApp route)
        await Shell.Current.GoToAsync("//HomePage");
    }
}