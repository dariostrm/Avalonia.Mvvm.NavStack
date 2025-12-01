using CommunityToolkit.Mvvm.ComponentModel;

namespace Mvvm.NestedNav;

public abstract class ViewModelBase() : ObservableValidator, IViewModel
{
    private INavigator? _navigator;
    
    protected internal INavigator Navigator
    {
        get => _navigator 
               ?? throw new InvalidOperationException("The Navigator is not available in the constructor. " +
                                                      "Use it only after the ViewModel has been created.");
        internal set => _navigator = value;
    }

    public virtual void OnBecomeVisible() {}

    public virtual void OnMoveToBackground() {}
    
    public virtual void OnDestroy() {}
}