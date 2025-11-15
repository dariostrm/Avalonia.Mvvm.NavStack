using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mvvm.NestedNav;

public class Navigator : INavigator
{
    private readonly BehaviorSubject<IImmutableStack<Screen>> _stackSubject;
    
    public IObservable<IImmutableStack<Screen>> Stack => _stackSubject.AsObservable();
    public IImmutableStack<Screen> StackValue => _stackSubject.Value;
        
    public IObservable<Screen> CurrentScreen => _stackSubject.Select(stack => stack.Peek());
    public Screen CurrentScreenValue => _stackSubject.Value.Peek();
    public INavigator? ParentNavigator { get; }
    
    public Navigator(Screen initialScreen, INavigator? parentNavigator = null)
    {
        ParentNavigator = parentNavigator;
        var initialStack = ImmutableStack<Screen>.Empty.Push(initialScreen);
        _stackSubject = new BehaviorSubject<IImmutableStack<Screen>>(initialStack);
    }
    
    public void Navigate(Screen screen)
    {
        var newStack = StackValue.Push(screen);
        _stackSubject.OnNext(newStack);
    }

    public bool GoBack()
    {
        var currentStack = StackValue;
        if (currentStack.IsEmpty || currentStack.Count() == 1)
        {
            return false;
        }

        var newStack = currentStack.Pop();
        _stackSubject.OnNext(newStack);
        return true;
    }
}