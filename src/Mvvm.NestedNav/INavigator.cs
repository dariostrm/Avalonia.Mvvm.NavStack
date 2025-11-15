namespace Mvvm.NestedNav;

public interface INavigator
{
    IObservable<Screen> CurrentScreen { get; }
    Screen CurrentScreenValue { get; }
    INavigator? ParentNavigator { get; }
    bool GoBack();
}