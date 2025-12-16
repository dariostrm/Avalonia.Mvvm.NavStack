using NSubstitute;

namespace Avalonia.LiteRoute.Tests;

public class NavigatorTests
{
    private readonly IViewModelFactory _factory = Substitute.For<IViewModelFactory>();
    
    [Fact]
    public void Constructor_Should_CreateNavEntryForInitialRoute()
    {
        // Arrange
        var initialRoute = new TestRoute();
        var testViewModel = Substitute.For<IViewModel>();
        _factory.CreateViewModel(initialRoute, Arg.Any<INavigator>()).Returns(testViewModel);

        // Act
        var navigator = new Navigator(_factory, initialRoute);

        // Assert
        Assert.Single(navigator.BackStack);
        Assert.Equal(initialRoute, navigator.CurrentEntry.Route);
        Assert.Same(testViewModel, navigator.CurrentEntry.ViewModel);
        Assert.Same(navigator.CurrentEntry, navigator.BackStack.Peek());
        _factory.CreateViewModel(initialRoute, navigator).Received(1);
    }
    
    [Fact]
    public void CanGoBack_Should_Be_False_Initially()
    {
        // Arrange
        var navigator = new Navigator(_factory, new TestRoute());

        // Act (None needed, checking initial state)

        // Assert
        Assert.False(navigator.CanGoBack());
    }
    
    [Fact]
    public void CanGoBack_Should_Be_True_After_Navigation()
    {
        // Arrange
        var navigator = new Navigator(_factory, new TestRoute());
        _factory.CreateViewModel(Arg.Any<Route>(), Arg.Any<INavigator>())
            .Returns(Substitute.For<IViewModel>());
    
        // Act
        navigator.Navigate(new TestRoute());

        // Assert
        Assert.True(navigator.CanGoBack());
    }
}