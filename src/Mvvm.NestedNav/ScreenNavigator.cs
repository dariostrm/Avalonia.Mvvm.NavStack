using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mvvm.NestedNav;

public class ScreenNavigator : IScreenNavigator
{
    private readonly BehaviorSubject<Screen> _currentRouteSubject;

    public IObservable<Screen> CurrentScreen => _currentRouteSubject.AsObservable();
    public Screen CurrentScreenValue => _currentRouteSubject.Value;
    public INavigator? ParentNavigator { get; }
    public IImmutableStack<Screen> Stack { get; private set; }
    
    public ScreenNavigator(Screen initialScreen, INavigator? parentNavigator)
    {
        ParentNavigator = parentNavigator;
        Stack = ImmutableStack<Screen>.Empty.Push(initialScreen);
        _currentRouteSubject = new BehaviorSubject<Screen>(initialScreen);
    }

    public void Navigate(Screen screen)
    {
        Stack = Stack.Push(screen);
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