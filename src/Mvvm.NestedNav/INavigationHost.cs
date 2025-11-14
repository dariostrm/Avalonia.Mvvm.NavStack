using System.ComponentModel;

namespace Mvvm.NestedNav;

public interface INavigationHost : INotifyPropertyChanged, IAsyncDisposable
{
    IScreenViewModel? CurrentViewModel { get; }
    
    object? Content { get; }
}