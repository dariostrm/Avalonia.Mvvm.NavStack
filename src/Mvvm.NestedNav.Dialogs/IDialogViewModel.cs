using System.Windows.Input;

namespace Mvvm.NestedNav.Dialogs;

public interface IDialogViewModel : IViewModel
{
    string Title { get; }
    
    string PrimaryButtonText { get; }
    ICommand PrimaryCommand { get; }
    
    string SecondaryButtonText { get; }
    bool IsSecondaryButtonVisible { get; }
    ICommand SecondaryCommand { get; }
    
    bool IsCloseButtonVisible { get; }
    string CloseButtonText { get; }
    ICommand RequestCloseCommand { get; }
    void RequestClose();
    
    event EventHandler? Closed;
}

public interface IDialogViewModel<out TResult> : IDialogViewModel
    where TResult : class
{
    TResult? Result { get; }
}