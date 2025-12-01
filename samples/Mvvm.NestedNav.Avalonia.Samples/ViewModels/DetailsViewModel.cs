using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mvvm.NestedNav.Avalonia.Samples.Routes;

namespace Mvvm.NestedNav.Avalonia.Samples.ViewModels;

public partial class DetailsViewModel(string message) : ViewModelBase
{
    [ObservableProperty] private string _message = message;
    
    [RelayCommand]
    private void GoBack()
    {
        Navigator.GoBack();
    }
}