using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mvvm.NestedNav;

public class ScreenNavigator : IScreenNavigator
{
    private readonly BehaviorSubject<Route> _currentRouteSubject;

    public IObservable<Route> CurrentRoute => _currentRouteSubject.AsObservable();
    public Route CurrentRouteValue => _currentRouteSubject.Value;
    public INavigator? ParentNavigator { get; }
    public IImmutableStack<Route> Stack { get; private set; }
    
    public ScreenNavigator(Route initialRoute, INavigator? parentNavigator)
    {
        ParentNavigator = parentNavigator;
        Stack = ImmutableStack<Route>.Empty.Push(initialRoute);
        _currentRouteSubject = new BehaviorSubject<Route>(initialRoute);
    }

    public void Navigate(Route route)
    {
        Stack = Stack.Push(route);
        _currentRouteSubject.OnNext(Stack.Peek());
    }

    public bool GoBack()
    {
        if (Stack.Count() <= 1)
        {
            return ParentNavigator?.GoBack() ?? false;
        }
        Stack = Stack.Pop();
        _currentRouteSubject.OnNext(Stack.Peek());
        return true;
    }
}