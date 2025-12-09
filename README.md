# Avalonia.Mvvm.NavStack
A lightweight C#/Avalonia navigation library built to work with the CommunityToolkit.Mvvm framework.

## Features

- Navigation between ViewModels (stack-based)
- Easy parameterized navigation
- First-class support for Dependency Injection (easy to mix navigation parameters with services in constructors)
- Handles the physical back button
- Supports animations via the `PageTransition` property on the `NavigationHost`
- Leverages the built-in TabControl for tab navigation, as well as the ViewLocator for View/ViewModel resolution
- Lifecycle hooks for navigation entries
- Simple and intuitive API

## Getting Started

For the whole example, check out the [BookTracker sample app](./samples/BookTracker)

Install the NuGet package:
```bash
# The package has not been published yet
```

Define a route for the ViewModels you want to navigate to (for example in a `Routes.cs` file)
Routes identify different ViewModels in your application and can contain any parameters needed for navigation.
```csharp
//Main tabs
public record BooksTab() : Route;
public record SettingsTab() : Route;
public record AboutTab() : Route;

//BooksTab routes (when a book is selected in the BooksTab)
public record BookRoute(Guid BookId) : Route;
```

Now you need to define the factory for the ViewModels based on the routes.
This is where you can also inject any services needed by the ViewModels.
In the App.axaml.cs:
```csharp
serviceCollection.AddSingleton<Func<Route, IViewModel>>(serviceProvider => route => route switch
{
    AboutTab => new AboutViewModel(),
    BooksTab => new BooksViewModel(
        bookRepository: serviceProvider.GetRequiredService<IBookRepository>()
    ),
    SettingsTab => new SettingsViewModel(),
    BookRoute r => new BookViewModel(
        bookId: r.BookId, 
        bookRepository: serviceProvider.GetRequiredService<IBookRepository>()
    ),
    _ => throw new ArgumentOutOfRangeException(nameof(route))
});
serviceCollection.AddSingleton<IViewModelFactory, ViewModelFactory>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var viewModelFactory = serviceProvider.GetRequiredService<IViewModelFactory>();
Resources.Add("ViewModelFactory", viewModelFactory);
```

<details>
  <summary>If you're not using dependency injection:</summary>

```csharp
var viewModelFactory = new ViewModelFactory(viewModelResolver: route => route switch
{
    AboutTab => new AboutViewModel(),
    ...
    _ => throw new ArgumentOutOfRangeException(nameof(route))
});
Resources.Add("ViewModelFactory", viewModelFactory);
```
</details>

---

You can now use the `NavigationHost` control wherever you want to have navigation capabilities.
Each `NavigationHost` has its own navigation stack and an initial route. 
For basic usage, you can just define a single `NavigationHost` in your `MainView.axaml`:
```xml
 <navStack:NavigationHost>
    <navStack:NavigationHost.InitialRoute>
        <bookTracker:BooksTab/>
    </navStack:NavigationHost.InitialRoute>
</navStack:NavigationHost>
```

> [!NOTE]
> The `NavigationHost` needs access to the `IViewModelFactory`. 
> By default, it will look for it in the application resources, which is why you need the `Resources.Add("ViewModelFactory", viewModelFactory);` line in the previous step.
> but you can also inject or create a ViewModelFactory in your ViewModel and set it explicitly like this:
> ```xml
> <navStack:NavigationHost ViewModelFactory="{Binding ViewModelFactory}">
> ```

---

Finally, just inherit from `ViewModelBase` in your ViewModels to get access to the `Navigator` and its methods:
`ViewModelBase` already inherits from `ObservableValidator` from the CommunityToolkit.Mvvm framework, so you can also use all its features in your ViewModels.
```csharp
//BooksViewModel.cs
public partial class BooksViewModel : ViewModelBase
{
    ...
    [RelayCommand]
    private void SelectBook(Guid bookId)
    {
        // In this case passing the whole Book object through the route would also be fine
        Navigator.Navigate(new BookRoute(bookId));
    }
}

//BookViewModel.cs
public partial class BookViewModel : ViewModelBase
{
    [RelayCommand]
    private void GoBack()
    {
        Navigator.GoBack();
    }
}
```
For other navigation methods, check out the `INavigator` interface [here](./src/Mvvm.NavStack/INavigator.cs).

---
## Tab Navigation
For tab navigation, simply use the `TabControl` provided by Avalonia and define a `NavigationHost` for each tab.
```xml
<TabControl>
    <TabItem Header="Books">
        <navStack:NavigationHost>
            <navStack:NavigationHost.InitialRoute>
                <bookTracker:BooksTab/>
            </navStack:NavigationHost.InitialRoute>
        </navStack:NavigationHost>
    </TabItem>
    <TabItem Header="Settings">
        <navStack:NavigationHost>
            <navStack:NavigationHost.InitialRoute>
                <bookTracker:SettingsTab/>
            </navStack:NavigationHost.InitialRoute>
        </navStack:NavigationHost>
    </TabItem>
    <TabItem Header="About">
        <navStack:NavigationHost>
            <navStack:NavigationHost.InitialRoute>
                <bookTracker:AboutTab/>
            </navStack:NavigationHost.InitialRoute>
        </navStack:NavigationHost>
    </TabItem>
</TabControl>
```

## Lifecycle Hooks
You can override the following methods in your ViewModels to hook into the navigation lifecycle:
- `OnBecomeVisible()` - Called when the ViewModel becomes visible (navigated to or returned to)
- `OnMoveToBackground()` - Called when the ViewModel becomes invisible (navigated away but still in the stack)
- `OnDestroy()` - Called when the ViewModel is removed from the navigation stack and therefore destroyed