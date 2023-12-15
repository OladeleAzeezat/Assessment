using AssessmentMaui.ViewModels;

namespace AssessmentMaui;

public partial class SignupPage : ContentPage
{
    public SignupPage(SignupViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}