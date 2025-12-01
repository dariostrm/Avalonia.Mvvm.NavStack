using CommunityToolkit.Mvvm.ComponentModel;

namespace Mvvm.NestedNav.Avalonia.Samples.ViewModels;

public partial class MainViewModel(IViewModelFactory viewModelFactory) : ObservableObject
{
    [ObservableProperty] private IViewModelFactory _viewModelFactory = viewModelFactory;
}