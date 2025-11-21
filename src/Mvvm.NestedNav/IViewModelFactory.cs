using Mvvm.NestedNav;

namespace Mvvm.NestedNav;

public interface IViewModelFactory
{
    Task<IScreenViewModel> CreateAndLoadAsync(
        Screen screen, 
        INavigator navigator, 
        CancellationToken cancellationToken = default
    );
}