namespace Mvvm.NestedNav;

public interface IScreenViewModel
{
    void Initialize(INavigator navigator, Screen screen);
    Task LoadAsync(CancellationToken cancellationToken = default);
}