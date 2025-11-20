namespace Mvvm.NestedNav;

public interface IViewModel
{
    Task LoadAsync(INavigator navigator, Screen screen, CancellationToken cancellationToken = default);
}