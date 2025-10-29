using Matric2You.ViewModel;


namespace Matric2You.Views;

public partial class ChatBotPage : ContentPage
{
    public ChatBotPage(ChatViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
