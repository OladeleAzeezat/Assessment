namespace AssessmentMaui.EmployeeControls;

public partial class FlyoutHeaderControl : ContentView
{
	public FlyoutHeaderControl()
	{
		InitializeComponent();
		if(App.employee != null)
		{
			lblText.Text = "Login as: "; 
			lblEmail.Text = App.employee.Email; ;
		}
	}
}