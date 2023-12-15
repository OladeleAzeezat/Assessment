using AssessmentMaui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentMaui.ViewModels;
public partial class SignupViewModel : ObservableObject
{
    readonly IAssessmentRepository assessment = new AssessmentService();

    [RelayCommand]
    async Task LogIn(string m)
    {

        await Shell.Current.GoToAsync("//LoginPage");
    }
}

