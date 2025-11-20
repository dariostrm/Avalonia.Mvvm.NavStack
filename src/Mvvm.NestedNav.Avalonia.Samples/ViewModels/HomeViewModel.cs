using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mvvm.NestedNav.Avalonia.Samples.Screens;

namespace Mvvm.NestedNav.Avalonia.Samples.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting = "Home not yet loaded!";
    
    public override async Task LoadAsync(INavigator navigator, Screen screen, CancellationToken cancellationToken = default)
    {
        await base.LoadAsync(navigator, screen, cancellationToken);
        await Task.Delay(2000, cancellationToken); // Simulate some loading time
        Greeting = "Home loaded!";
    }
    
    [RelayCommand]
    private void GoToDetails()
    {
        Navigator.Navigate(new DetailsScreen("Passed from Home!"));
    }
}