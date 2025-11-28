using System.ComponentModel;

namespace Mvvm.NestedNav;

public interface IViewModel
{
    void Initialize(INavigator navigator, Route route);
    
    void OnNavigatedTo();
    void OnNavigatedFrom();
    void OnDestroy();
}