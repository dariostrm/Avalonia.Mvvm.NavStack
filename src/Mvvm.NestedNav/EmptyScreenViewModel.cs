namespace Mvvm.NestedNav;

public class EmptyScreenViewModel(Route route, IScreenNavigator navigator) : IScreenViewModel
{
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Route Route { get; } = route;
    public IScreenNavigator ScreenNavigator { get; } = navigator;
    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}