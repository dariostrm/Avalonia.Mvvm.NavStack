using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mvvm.NestedNav.Exceptions;
using NestedNav.Dialogs;

namespace Mvvm.NestedNav.Dialogs;

public partial class DialogViewModel : ScreenViewModel, IDialogViewModel
{
    [ObservableProperty] private string _title = "Dialog";

    [ObservableProperty] private string _primaryButtonText = "OK";
    [ObservableProperty] private ICommand _primaryCommand;

    [ObservableProperty] private string _secondaryButtonText = string.Empty;
    [ObservableProperty] private bool _isSecondaryButtonVisible;
    [ObservableProperty] private ICommand _secondaryCommand = new RelayCommand(() => { });

    [ObservableProperty] private string _closeButtonText = "Cancel";
    [ObservableProperty] private bool _isCloseButtonVisible = true;

    public DialogViewModel()
    {
        PrimaryCommand = new RelayCommand(Close);
    }

    protected virtual void Close() => Navigator.GoBack();
    
    [RelayCommand]
    public virtual void RequestClose() => Close();
    
    protected void StateChanged()
    {
        ValidateAllProperties();
        ((IRelayCommand)PrimaryCommand).NotifyCanExecuteChanged();
        ((IRelayCommand)SecondaryCommand).NotifyCanExecuteChanged();
    }
}

public abstract partial class DialogViewModel<TResult> : DialogViewModel, IDialogViewModel<TResult> 
    where TResult : class
{
    public TResult? Result { get; protected set; }
    
    protected Action PrimaryAction { get; private set; } = null!;
    protected Func<bool> CanExecutePrimary { get; private set; } = null!;

    protected Action SecondaryAction { get; private set; } = null!;
    protected Func<bool> CanExecuteSecondary { get; private set; } = null!;

    protected DialogViewModel()
    {
        SetPrimaryAction(() => RequestClose(Result));
        SetSecondaryAction(() => RequestClose(Result));
    }

    public override void Initialize(INavigator navigator, Screen screen)
    {
        base.Initialize(navigator, screen);
        if (screen is not DialogScreen<TResult> dialogScreen)
            throw new InvalidScreenException(nameof(DialogViewModel<TResult>));
        Title = dialogScreen.Title;
    }

    protected void RequestClose(TResult? result)
    {
        Result = result;
        RequestClose();
    }

    [RelayCommand]
    public void Cancel() => RequestClose(null);
    
    protected void SetPrimaryAction(Action action, Func<bool>? canExecute = null)
    {
        PrimaryAction = action;
        CanExecutePrimary = canExecute ?? (() => true);
        PrimaryCommand = new RelayCommand(PrimaryAction, CanExecutePrimary);
    }
    protected void SetSecondaryAction(Action action, Func<bool>? canExecute = null)
    {
        SecondaryAction = action;
        CanExecuteSecondary = canExecute ?? (() => true);
        SecondaryCommand = new RelayCommand(SecondaryAction, CanExecuteSecondary);
    }
    protected void SetPrimaryCanExecute(Func<bool> canExecute)
    {
        CanExecutePrimary = canExecute;
        PrimaryCommand = new RelayCommand(PrimaryAction, CanExecutePrimary);
    }
    protected void SetSecondaryCanExecute(Func<bool> canExecute) 
    {
        CanExecuteSecondary = canExecute;
        SecondaryCommand = new RelayCommand(SecondaryAction, CanExecuteSecondary);
    }
}