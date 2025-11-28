using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mvvm.NestedNav.Avalonia.Samples.Routes;

namespace Mvvm.NestedNav.Avalonia.Samples.ViewModels;

public partial class ProfileViewModel : ViewModelBase<ProfileRoute>
{
    [ObservableProperty] private string _greeting = "Welcome to profile page!";
    
    [RelayCommand]
    private void GoToDetails()
    {
        Navigator.Navigate(new DetailsRoute("Passed from Profile!"));
    }
}