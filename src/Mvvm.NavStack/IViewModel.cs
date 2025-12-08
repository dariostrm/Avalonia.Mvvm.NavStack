namespace Mvvm.NavStack;

public interface IViewModel
{
    void OnBecomeVisible();
    void OnMoveToBackground();
    void OnDestroy();
}