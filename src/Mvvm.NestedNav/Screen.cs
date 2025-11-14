namespace Mvvm.NestedNav;

public record Screen(
    Route Route,
    IScreenNavigator Navigator
)
{
    public static Screen Empty(IScreenNavigator navigator) => new(Route.Empty, navigator);
    
    public bool IsEmpty => Route is EmptyRoute;
}