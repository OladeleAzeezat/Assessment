using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentMaui.ViewModels;

public partial class AppShellViewModel : ObservableObject 
{
    [RelayCommand]
    async void Logout()
    {
        if (Preferences.ContainsKey(nameof(App.employee)))
        {
            Preferences.Remove(nameof(App.employee));
        }
        await Shell.Current.GoToAsync("//LoginPage");
    }
} 

