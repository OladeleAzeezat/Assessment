using AssessmentMaui.ViewModels;

namespace AssessmentMaui.Views;

public partial class ItemPage : ContentPage
{
	public ItemPage(ItemViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}