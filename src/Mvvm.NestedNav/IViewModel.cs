using System.ComponentModel;

namespace Mvvm.NestedNav;

public interface IViewModel
{
    void OnBecomeVisible();
    void OnMoveToBackground();
    void OnDestroy();
}