using CommunityToolkit.Mvvm.ComponentModel;

namespace Mvvm.NestedNav;

public abstract class ScreenViewModel : ObservableValidator, IScreenViewModel
{
    private INavigator? _navigator;
    public virtual INavigator Navigator
    {
        get => _navigator ?? throw new InvalidOperationException("The ViewModel has not been initialized yet.");
        private set => _navigator = value;
    }
    
    private Screen? _screen;
    public virtual Screen Screen 
    {
        get => _screen ?? throw new InvalidOperationException("The ViewModel has not been initialized yet.");
        private set => _screen = value;
    }
    
    public virtual void Initialize(INavigator navigator, Screen screen)
    {
        Navigator = navigator;
        Screen = screen;
    }

    public virtual Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}