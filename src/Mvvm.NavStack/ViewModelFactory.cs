namespace Mvvm.NavStack;

public class ViewModelFactory(Func<Route, IViewModel> viewModelResolver) : IViewModelFactory
{
    public IViewModel CreateViewModel(Route route, INavigator navigator)
    {
        var vm = viewModelResolver(route);
        if (vm is ViewModelBase vmBase)
            vmBase.Navigator = navigator;
        return vm;
    }
}