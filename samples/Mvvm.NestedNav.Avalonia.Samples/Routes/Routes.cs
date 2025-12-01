namespace Mvvm.NestedNav.Avalonia.Samples.Routes;

public record HomeRoute : Route;
public record ProfileRoute : Route;
public record SettingsRoute : Route;
public record DetailsRoute(string Message) : Route;