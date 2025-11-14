using Mvvm.NestedNav;

namespace Mvvm.NestedNav;

public interface IViewModelFactory
{
    Task<IScreenViewModel> CreateAndLoadAsync(
        Route route, 
        IScreenNavigator screenNavigator, 
        CancellationToken cancellationToken = default
    );
    
    
}