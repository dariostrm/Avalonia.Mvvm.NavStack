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

    public void Navigate(Screen screen)
    {
        _ = NavigateAsync(screen)
            .ContinueWith(t =>
            {
                if (t is { IsFaulted: true, Exception: not null })
                    Console.WriteLine("Error during NavigateAsync: " + t.Exception);
            });
    }

    public async Task NavigateAsync(Screen screen)
    {
        var tcs = new TaskCompletionSource();
        var subscription = CurrentViewModel
            .Skip(1)
            .Take(1)
            .Subscribe(_ => tcs.SetResult());

        try
        {
            var newStack = StackValue.Push(screen);
            _stackSubject.OnNext(newStack);
            await tcs.Task;
        }
        finally
        {
            subscription.Dispose();
        }
    }

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
        if (!CanGoBack)
            throw new InvalidOperationException("Cannot go back, stack has only one screen.");
        
        var tcs = new TaskCompletionSource();
        var subscription = CurrentViewModel
            .Skip(1)
            .Take(1)
            .Subscribe(_ => tcs.SetResult());
        
        try
        {
            var newStack = StackValue.Pop();
            _stackSubject.OnNext(newStack);
            await tcs.Task;
        }
        finally
        {
            subscription.Dispose();
        }
    }
    
    public void Dispose()
    {
        _currentViewModelLoadingCts?.Cancel();
        _currentViewModelLoadingCts?.Dispose();
        _disposables.Dispose();
    }
}