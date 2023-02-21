namespace ROTten_Text;

public partial class HowItWorksPage : ContentPage
{
	public HowItWorksPage()
	{
		InitializeComponent();
	}
    async void GotItBtn(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}