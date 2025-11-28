namespace Mvvm.NestedNav.Dialogs;

public interface IDialogHost
{
    bool IsDialogOpen { get; }
    
    void ShowDialog(DialogRoute dialogRoute, Action onClosed);
    
    Task ShowDialogAsync(DialogRoute dialogRoute);
    
    void ShowDialog<TDialogResult>(
        DialogRoute dialogRoute,
        Action<TDialogResult?> onClosed
    ) where TDialogResult : class;
    
    Task<TDialogResult?> ShowDialogAsync<TDialogResult>(DialogRoute dialogRoute) 
        where TDialogResult : class;
}