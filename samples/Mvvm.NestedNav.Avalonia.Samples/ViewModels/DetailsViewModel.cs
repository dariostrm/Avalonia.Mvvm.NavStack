using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mvvm.NestedNav.Avalonia.Samples.Routes;

namespace Mvvm.NestedNav.Avalonia.Samples.ViewModels;

public partial class DetailsViewModel : ViewModelBase<DetailsRoute>
{
    [ObservableProperty] private string _message = "Details not yet loaded!";

    public override void Initialize(INavigator navigator, DetailsRoute route)
    {
        base.Initialize(navigator, route);
        Message = route.Message;
    }
    
    [RelayCommand]
    private void GoBack()
    {
        Navigator.GoBack();
    }
}