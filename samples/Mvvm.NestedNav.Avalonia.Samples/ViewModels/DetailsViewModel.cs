using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mvvm.NestedNav.Avalonia.Samples.Screens;

namespace Mvvm.NestedNav.Avalonia.Samples.ViewModels;

public partial class DetailsViewModel : ScreenViewModel
{
    [ObservableProperty] private string _message = "Details not yet loaded!";

    public override void Initialize(INavigator navigator, Screen screen)
    {
        base.Initialize(navigator, screen);
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