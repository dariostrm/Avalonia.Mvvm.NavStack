using System.ComponentModel;

namespace Mvvm.NestedNav;

public interface IScreenViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
    void Initialize(INavigator navigator, Screen screen);
    Task LoadAsync(CancellationToken cancellationToken = default);
}