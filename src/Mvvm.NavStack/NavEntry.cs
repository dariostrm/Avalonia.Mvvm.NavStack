namespace Mvvm.NavStack;

public record NavEntry(Route Route, IViewModel ViewModel)
{
    public NavEntryLifecycleState LifecycleState { get; private set; }
    public event EventHandler<NavEntryLifecycleState>? StateChanged;
    public event EventHandler? Destroyed;

    public void OnNavigatedTo()
    {
        ViewModel.OnBecomeVisible();
        SetLifecycleState(NavEntryLifecycleState.Active);
    }
    
    public void OnNavigatedFrom()
    {
        ViewModel.OnMoveToBackground();
        SetLifecycleState(NavEntryLifecycleState.Inactive);
    }
    
    public void OnDestroy()
    {
        ViewModel.OnDestroy();
        SetLifecycleState(NavEntryLifecycleState.Destroyed);
    }
    
    private void SetLifecycleState(NavEntryLifecycleState newState)
    {
        if (LifecycleState != newState)
        {
            LifecycleState = newState;
            StateChanged?.Invoke(this, newState);
            if (newState == NavEntryLifecycleState.Destroyed)
            {
                Destroyed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}