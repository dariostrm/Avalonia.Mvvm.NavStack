using CommunityToolkit.Mvvm.ComponentModel;

namespace Mvvm.NestedNav;

public abstract class ViewModelBase<TRoute> : ObservableValidator, IViewModel
    where TRoute : Route
{
    private INavigator? _navigator;
    public virtual INavigator Navigator
    {
        get => _navigator ?? throw new InvalidOperationException("The ViewModel has not been initialized yet.");
        private set => _navigator = value;
    }
    
    private TRoute? _route;
    public virtual TRoute Route 
    {
        get => _route ?? throw new InvalidOperationException("The ViewModel has not been initialized yet.");
        private set => _route = value;
    }
    
    public virtual void Initialize(INavigator navigator, TRoute route)
    {
        Navigator = navigator;
        Route = route;
    }

    public void Initialize(INavigator navigator, Route route)
    {
        if (route is not TRoute typedRoute)
        {
            throw new ArgumentException($"Invalid route type. Expected {typeof(TRoute).FullName}, but got {route.GetType().FullName}.");
        }
        
        Initialize(navigator, typedRoute);
    }

    public virtual void OnNavigatedTo() {}

    public virtual void OnNavigatingFrom() {}

    public virtual void OnNavigatedFrom() {}
    
    public virtual void OnClosing() {}
}