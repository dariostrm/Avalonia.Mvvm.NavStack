namespace Mvvm.NestedNav;

public interface INavigator
{
    IObservable<Route> CurrentRoute { get; }
    Route CurrentRouteValue { get; }
    INavigator? ParentNavigator { get; }
    bool GoBack();
}