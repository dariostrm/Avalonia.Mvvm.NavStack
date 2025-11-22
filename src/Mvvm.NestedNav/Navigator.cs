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
    private readonly IViewModelFactory _viewModelFactory;
    
    private readonly BehaviorSubject<IImmutableStack<Screen>> _stackSubject;
    private readonly BehaviorSubject<IScreenViewModel?> _currentViewModelSubject = new(null);
    
    public IObservable<IImmutableStack<Screen>> Stack => _stackSubject.AsObservable();
    public IImmutableStack<Screen> StackValue => _stackSubject.Value;
        
    public IObservable<Screen> CurrentScreen => _stackSubject.Select(stack => stack.Peek());
    public Screen CurrentScreenValue => _stackSubject.Value.Peek();

    public IObservable<IScreenViewModel> CurrentViewModel => _currentViewModelSubject.AsObservable().Where(vm => vm != null)!;
    public IScreenViewModel CurrentViewModelValue => _currentViewModelSubject.Value 
                                               ?? throw new InvalidOperationException("The current ViewModel has not been loaded yet.");

    public INavigator? ParentNavigator { get; }
    public bool CanGoBack => StackValue.Count() > 1;
    
    private readonly Subject<NavigatingEventArgs> _navigatingSubject = new();
    private readonly Subject<NavigatedEventArgs> _navigatedSubject = new();
    private readonly Subject<NavigatingBackEventArgs> _navigatingBackSubject = new();
    private readonly Subject<Screen> _navigatedBackSubject = new();
    
    public IObservable<NavigatingEventArgs> Navigating => _navigatingSubject.AsObservable();
    public IObservable<NavigatedEventArgs> Navigated => _navigatedSubject.AsObservable();
    public IObservable<NavigatingBackEventArgs> NavigatingBack => _navigatingBackSubject.AsObservable();
    public IObservable<Screen> NavigatedBack => _navigatedBackSubject.AsObservable();
    
    public Navigator(Screen initialScreen, IViewModelFactory viewModelFactory, INavigator? parentNavigator = null)
    {
        _viewModelFactory = viewModelFactory;
        ParentNavigator = parentNavigator;
        var initialStack = ImmutableStack<Screen>.Empty.Push(initialScreen);
        _stackSubject = new BehaviorSubject<IImmutableStack<Screen>>(initialStack);
        CurrentScreen
            .Subscribe(OnNewScreen)
            .DisposeWith(_disposables);
    }

    private void OnNewScreen(Screen newScreen)
    {
        _ = LoadNewViewModelAsync(newScreen)
            .ContinueWith(t     =>       
            {
                if (t is { IsFaulted: true, Exception: not null })
                    Console.WriteLine("Error during LoadNewViewModelAsync: " + t.Exception);
            });
    }
    
    private async Task LoadNewViewModelAsync(Screen screen)
    {
        var newCts = new CancellationTokenSource();

        // Atomically replace the CTS and cancel+dispose the previous one
        var previous = Interlocked.Exchange(ref _currentViewModelLoadingCts, newCts);
        previous?.Cancel();
        previous?.Dispose();
        var token = newCts.Token;
        
        try
        {
            var viewModel = await _viewModelFactory.CreateAndLoadAsync(screen, this, token);

            if (Interlocked.CompareExchange(ref _currentViewModelLoadingCts, null, newCts) == newCts)
            {
                _currentViewModelSubject.OnNext(viewModel);
            }
        }
        catch (OperationCanceledException)
        {
            //expected
        }
    }
    
    public async Task SetStackAsync(IImmutableStack<Screen> newStack)
    {
        if (newStack == StackValue)
            return;
        var tcs = new TaskCompletionSource();
        var subscription = CurrentViewModel
            .Skip(1)
            .Take(1)
            .Subscribe(_ => tcs.SetResult());

        try
        {
            var oldScreen = CurrentScreenValue;
            _navigatingSubject.OnNext(new NavigatingEventArgs(oldScreen, CurrentViewModelValue, newStack.Peek()));
            _stackSubject.OnNext(newStack);
            await tcs.Task;
            _navigatedSubject.OnNext(new NavigatedEventArgs(oldScreen, CurrentScreenValue, CurrentViewModelValue));
        }
        finally
        {
            subscription.Dispose();
        }
    }
    
    public void SetStack(IImmutableStack<Screen> newStack)
    {
        _ = SetStackAsync(newStack)
            .ContinueWith(t =>
            {
                if (t is { IsFaulted: true, Exception: not null })
                    Console.WriteLine("Error during SetStackAsync: " + t.Exception);
            });
    }

    public void Navigate(Screen screen)
    {
        _ = NavigateAsync(screen)
            .ContinueWith(t =>
            {
                if (t is { IsFaulted: true, Exception: not null })
                    Console.WriteLine("Error during NavigateAsync: " + t.Exception);
            });
    }

    public Task NavigateAsync(Screen screen) => SetStackAsync(StackValue.Push(screen));

    public void GoBack()
    {
        _ = GoBackAsync()
            .ContinueWith(t =>
            {
                if (t is { IsFaulted: true, Exception: not null })
                    Console.WriteLine("Error during GoBackAsync: " + t.Exception);
            });
    }

    public async Task GoBackAsync()
    {
        if (!CanGoBack) return;
        
        var oldScreen = CurrentScreenValue;
        var oldViewModel = CurrentViewModelValue;
        _navigatingBackSubject.OnNext(new NavigatingBackEventArgs(oldScreen, oldViewModel));
        await SetStackAsync(StackValue.Pop());
        _navigatedBackSubject.OnNext(oldScreen);
    }

    public void Dispose()
    {
        _currentViewModelLoadingCts?.Cancel();
        _currentViewModelLoadingCts?.Dispose();
        _disposables.Dispose();
    }
}