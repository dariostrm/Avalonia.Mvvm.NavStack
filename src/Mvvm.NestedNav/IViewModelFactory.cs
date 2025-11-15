using Mvvm.NestedNav;

namespace Mvvm.NestedNav;

public interface IViewModelFactory
{
    Task<IViewModel> CreateAndLoadAsync(
        Screen screen, 
        INavigator navigator, 
        CancellationToken cancellationToken = default
    );
    
    
}