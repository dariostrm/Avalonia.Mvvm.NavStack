using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mvvm.NestedNav.Avalonia.Samples.Screens;

namespace Mvvm.NestedNav.Avalonia.Samples.ViewModels;

public partial class DetailsViewModel : ViewModelBase
{
    [ObservableProperty] private string _message = "Details not yet loaded!";

    public override async Task LoadAsync(INavigator navigator, Screen screen, CancellationToken cancellationToken = default)
    {
        await base.LoadAsync(navigator, screen, cancellationToken);
        if (screen is DetailsScreen detailsScreen)
        {
            Message = detailsScreen.Message;
        }
    }
    
    [RelayCommand]
    private void GoBack()
    {
        Navigator.GoBack();
    }
}