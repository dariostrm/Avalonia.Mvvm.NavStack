namespace Mvvm.NavStack;

public interface IViewModelFactory
{
    IViewModel CreateViewModel(Route route, INavigator navigator);
}