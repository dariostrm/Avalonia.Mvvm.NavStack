namespace Mvvm.NestedNav;

public interface INavigationHost
{
    INavigator Navigator { get; }
    
    IObservable<IViewModel> CurrentViewModel { get; }
    
    IViewModel CurrentViewModelValue { get; }
}