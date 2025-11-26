using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mvvm.NestedNav;

public class Navigator : INavigator
{
    private CancellationTokenSource? _currentViewModelLoadingCts;
    
    private readonly CompositeDisposable _disposables = new();
    private readonly IViewModelResolver _viewModelResolver;

    private readonly BehaviorSubject<NavBackStack> _stackSubject = new(NavBackStack.Empty);
    
    public IObservable<NavBackStack> BackStack => _stackSubject.AsObservable();
    public NavBackStack BackStackValue => _stackSubject.Value;

    public INavigator? ParentNavigator { get; }
    public bool CanGoBack => BackStackValue.Entries.Count() > 1;

    private readonly Subject<NavigatingEventArgs> _navigatingSubject = new();
    private readonly Subject<NavigatedEventArgs> _navigatedSubject = new();
    
    public IObservable<NavigatingEventArgs> Navigating => _navigatingSubject.AsObservable();
    public IObservable<NavigatedEventArgs> Navigated => _navigatedSubject.AsObservable();
    
    public Navigator(IViewModelResolver viewModelResolver, Screen? initialScreen = null, INavigator? parentNavigator = null)
    {
        _viewModelResolver = viewModelResolver;
        ParentNavigator = parentNavigator;
        if (initialScreen != null)
        {
            Navigate(initialScreen);
        }
    }
    
    public void OverrideBackStack(IEnumerable<Screen> screens)
    {
        var newBackStack = screens
            .Select(screen => NavEntry.Create(screen, _viewModelResolver))
            .Aggregate(NavBackStack.Empty, (stack, entry) => stack.Push(entry));
        SetBackStack(newBackStack);
    }
    
    private void SetBackStack(NavBackStack newBackStack)
    {
        var oldEntry = BackStackValue.CurrentEntry;
        var newEntry = newBackStack.CurrentEntry;
        _navigatingSubject.OnNext(new NavigatingEventArgs(oldEntry?.Screen, oldEntry?.ViewModel, newEntry?.Screen));
        _stackSubject.OnNext(newBackStack);
        if (newEntry == null)
            return;
        LoadNewViewModel(newEntry.Screen, newEntry.ViewModel, onLoaded: () =>
        {
            _navigatedSubject.OnNext(new NavigatedEventArgs(oldEntry?.Screen, newEntry.Screen, newEntry.ViewModel));
        });
    }

    public void Navigate(Screen screen)
    {
        var newBackStack = BackStackValue.Push(NavEntry.Create(screen, _viewModelResolver));
        SetBackStack(newBackStack);
    }

    public void GoBack()
    {
        var newBackStack = BackStackValue.TryPop(out var poppedEntry);
        if (poppedEntry == null)
            return;
        SetBackStack(newBackStack);
    }

    public void GoBackOrClear()
    {
        if (CanGoBack)
            GoBack();
        else
            Clear();
    }

    public void GoBackTo(Screen screen)
    {
        var newBackStack = BackStackValue;
        while (!newBackStack.IsEmpty && newBackStack.CurrentScreen?.Equals(screen) == false)
        {
            newBackStack = newBackStack.TryPop(out _);
        }
        SetBackStack(newBackStack);
    }

    public void ClearAndSet(Screen screen) => OverrideBackStack([screen]);

    public void Clear() => OverrideBackStack([]);

    public void ReplaceCurrent(Screen screen)
    {
        var newBackStack = BackStackValue.TryPop(out _);
        newBackStack = newBackStack.Push(NavEntry.Create(screen, _viewModelResolver));
        SetBackStack(newBackStack);
    }

    private void LoadNewViewModel(Screen screen, IScreenViewModel vm, Action? onLoaded = null)
    {
        _ = LoadNewViewModelAsyncWithCallback(screen, vm, onLoaded);
    }
    
    private async Task LoadNewViewModelAsyncWithCallback(Screen screen, IScreenViewModel vm, Action? onLoaded)
    {
        await LoadNewViewModelAsync(screen, vm);
        onLoaded?.Invoke();
    }
    
    private async Task LoadNewViewModelAsync(Screen screen, IScreenViewModel vm)
    {
        var newCts = new CancellationTokenSource();
        
        Interlocked.Exchange(ref _currentViewModelLoadingCts, newCts);
        var token = newCts.Token;
        
        try
        {
            vm.Initialize(this, screen);
            await vm.LoadAsync(token);
        }
        catch (OperationCanceledException)
        {
            //expected
        }
    }
    

    public void Dispose()
    {
        _currentViewModelLoadingCts?.Cancel();
        _currentViewModelLoadingCts?.Dispose();
        _disposables.Dispose();
        _stackSubject.Dispose();
        _navigatingSubject.Dispose();
        _navigatedSubject.Dispose();
    }
}