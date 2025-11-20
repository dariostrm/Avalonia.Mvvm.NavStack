namespace Mvvm.NestedNav;

public interface IViewModel
{
    Screen Screen { get; }
    INavigator Navigator { get; }
    Task LoadAsync(CancellationToken cancellationToken = default);
}