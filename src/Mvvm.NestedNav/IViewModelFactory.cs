using Mvvm.NestedNav;

namespace Mvvm.NestedNav;

public interface IViewModelFactory
{
    IViewModel CreateViewModel(Route route, INavigator navigator);
}