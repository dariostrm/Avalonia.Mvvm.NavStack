using System;
using Mvvm.NavStack;

namespace BookTracker;

//Main tabs
public record BooksTab() : Route;
public record SettingsTab() : Route;
public record AboutTab() : Route;

//BooksTab routes
public record BookRoute(Guid BookId) : Route;