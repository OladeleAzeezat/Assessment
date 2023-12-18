using AssessmentMaui.Model;
using AssessmentMaui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentMaui.ViewModels;
public partial class SignupViewModel : ObservableObject
{
    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _name;

    readonly IAssessmentRepository assessment = new AssessmentService();

    [RelayCommand]
    public async Task Signup()
    {
        try
        {
            //if(string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            //{
                Employee employee = await assessment.Signup(new Employee
                {
                    Email = Email,
                    Password = Password,
                    Username = Username,
                    Name = Name
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
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Parameters required", "Ok");
                    return;
                }
            //}
            //else
            //{
            //    await Shell.Current.DisplayAlert("Error", "Fields required", "Ok");
            //    return;
            //}
        }
        catch
        {

        }
    }

    [RelayCommand]
    async Task LogIn(string m)
    {

        await Shell.Current.GoToAsync("//LoginPage");
    }
}

