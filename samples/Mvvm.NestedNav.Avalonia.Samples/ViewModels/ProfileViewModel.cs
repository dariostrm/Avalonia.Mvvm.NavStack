using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mvvm.NestedNav.Avalonia.Samples.Screens;

namespace Mvvm.NestedNav.Avalonia.Samples.ViewModels;

public partial class ProfileViewModel : ScreenViewModel
{
    [ObservableProperty] private string _greeting = "Welcome to profile page!";
    
    [RelayCommand]
    private void GoToDetails()
    {
        Navigator.Navigate(new DetailsScreen("Passed from Profile!"));
    }
}