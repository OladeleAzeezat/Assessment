using AssessmentMaui.EmployeeControls;
using AssessmentMaui.Model;
using AssessmentMaui.Services;
using AssessmentMaui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;

namespace AssessmentMaui.ViewModels;
public partial class LoginPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string _password;

    readonly IAssessmentRepository assessment = new AssessmentService();

    //private string _jwtToken;

    //public string JwtToken
    //{
    //    get { return _jwtToken; }
    //    set
    //    {
    //        if (_jwtToken != value)
    //        {
    //            _jwtToken = value;
    //            OnPropertyChanged(nameof(JwtToken));
    //        }
    //    }
    //}

    [RelayCommand]
    public async void SignIn()
    {
        try
        {
            if(!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
            {
                Employee employee = await assessment.Login(new Employee
                {
                    Email = Email,
                    Password = Password
                });
                if (employee != null)
                {
                    if (Preferences.ContainsKey(nameof(App.employee)))
                    {
                        Preferences.Remove(nameof(App.employee));
                    }

                    //JwtToken = await assessment.GetTokenFromApi();

                    //await SecureStorage.SetAsync(nameof(App.Token), JwtToken);

                    string empDetails = JsonConvert.SerializeObject(employee);
                    Preferences.Set(nameof(App.employee), empDetails);
                    App.employee = employee;
                    Shell.Current.FlyoutHeader = new FlyoutHeaderControl();
                    await Shell.Current.GoToAsync(nameof(HomePage));
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Email or Password is incorrect", "Ok");
                    return;
                } 

            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "All fields required", "Ok");
                return;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            return;
        }
    }

    [RelayCommand]
    async Task Tap(string s)
    {
        await Shell.Current.GoToAsync("SignupPage");
    }

    //public async void Login()
    //{
    //    //to check if internet connection is active
    //    //if(Connectivity.Current.NetworkAccess == NetworkAccess.Internet)

    //    try
    //    {
    //        if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
    //        {
    //            Employee employee = await assessment.Login(Email, Password);
    //            if (employee != null)
    //            {
    //                if (Preferences.ContainsKey(nameof(App.employee)))
    //                {
    //                    Preferences.Remove(nameof(App.employee));
    //                }
    //                string empDetails = JsonConvert.SerializeObject(employee);
    //                Preferences.Set(nameof(App.employee), empDetails);
    //                App.employee = employee;
    //                Shell.Current.FlyoutHeader = new FlyoutHeaderControl();
    //                await Shell.Current.GoToAsync(nameof(HomePage));
    //            }
    //            else
    //            {
    //                await Shell.Current.DisplayAlert("Error", "Email or Password is incorrect", "Ok");
    //                return;
    //            }

    //        }
    //        else
    //        {
    //            await Shell.Current.DisplayAlert("Error", "All fields required", "Ok");
    //            return;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
    //        return;
    //    }
    //}
}

