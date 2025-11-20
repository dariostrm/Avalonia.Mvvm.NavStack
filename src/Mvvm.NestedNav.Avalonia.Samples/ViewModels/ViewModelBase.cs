using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Mvvm.NestedNav.Avalonia.Samples.ViewModels;

public abstract class ViewModelBase : ObservableObject, IViewModel
{
    private INavigator? _navigator;
    public virtual INavigator Navigator
    {
        get => _navigator ?? throw new System.InvalidOperationException("The ViewModel has not been loaded yet.");
        private set => _navigator = value;
    }
    
    private Screen? _screen;
    public virtual Screen Screen 
    {
        get => _screen ?? throw new System.InvalidOperationException("The ViewModel has not been loaded yet.");
        private set => _screen = value;
    }
    
    public virtual Task LoadAsync(INavigator navigator, Screen screen, CancellationToken cancellationToken = default)
    {
        Navigator = navigator;
        Screen = screen;
        return Task.CompletedTask;
    }
}