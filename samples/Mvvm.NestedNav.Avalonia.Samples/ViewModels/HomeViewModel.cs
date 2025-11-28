using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mvvm.NestedNav.Avalonia.Samples.Routes;

namespace Mvvm.NestedNav.Avalonia.Samples.ViewModels;

public partial class HomeViewModel : ViewModelBase<HomeRoute>
{
    [ObservableProperty] private string _greeting = "Home not yet loaded!";

    public override void Initialize(INavigator navigator, HomeRoute route)
    {
        base.Initialize(navigator, route);
        Greeting = "Welcome to home page!";
    }

    [RelayCommand]
    private void GoToDetails()
    {
        Navigator.Navigate(new DetailsRoute("Passed from Home!"));
    }
}