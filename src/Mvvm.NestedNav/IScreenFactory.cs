using Mvvm.NestedNav.TabNavigation;
using NestedNav;

namespace Mvvm.NestedNav;

public interface IScreenFactory
{
    Screen CreateScreen(Route route, IScreenNavigator navigator);
    
    Screen CreateTabScreen(Route route, ITabNavigator navigator);
}