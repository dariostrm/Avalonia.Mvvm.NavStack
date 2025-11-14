using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mvvm.NestedNav;

public class ScreenNavigator : IScreenNavigator
{
    private readonly BehaviorSubject<Screen> _currentScreenSubject;
    private readonly IScreenFactory _screenFactory;

    public IObservable<Screen> CurrentScreen => _currentScreenSubject.AsObservable();
    public Screen CurrentScreenValue => _currentScreenSubject.Value;
    public INavigator? ParentNavigator { get; }
    public IImmutableStack<Screen> Stack { get; private set; }
    
    public ScreenNavigator(Route initialRoute, INavigator? parentNavigator, IScreenFactory screenFactory)
    {
        _screenFactory = screenFactory;
        ParentNavigator = parentNavigator;
        var initialScreen = _screenFactory.CreateScreen(initialRoute, this);
        Stack = ImmutableStack<Screen>.Empty.Push(initialScreen);
        _currentScreenSubject = new BehaviorSubject<Screen>(initialScreen);
    }

    public void Navigate(Route route)
    {
        Stack = Stack.Push(_screenFactory.CreateScreen(route, this));
        _currentScreenSubject.OnNext(Stack.Peek());
    }

    public bool GoBack()
    {
        if (Stack.Count() <= 1)
        {
            return ParentNavigator?.GoBack() ?? false;
        }
        Stack = Stack.Pop();
        _currentScreenSubject.OnNext(Stack.Peek());
        return true;
    }
}