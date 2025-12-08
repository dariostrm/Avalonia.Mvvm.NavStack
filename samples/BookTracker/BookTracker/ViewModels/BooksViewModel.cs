using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BookTracker.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mvvm.NavStack;

namespace BookTracker.ViewModels;

public partial class BooksViewModel : ViewModelBase
{
    private readonly IBookRepository _bookRepository;

    public BooksViewModel(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
        _ = LoadBooksAsync();
    }
    
    [ObservableProperty] private ObservableCollection<Book> _books = [];
    
    [ObservableProperty] private bool _loaded;
    
    private async Task LoadBooksAsync()
    {
        var books = await _bookRepository.GetBooksAsync();
        Books = new ObservableCollection<Book>(books);
        _bookRepository.BooksChanged += OnBooksChanged;
        Loaded = true;
    }

    private void OnBooksChanged(IList<Book> newBooks)
    {
        Books = new ObservableCollection<Book>(newBooks);
    }
    
    [RelayCommand]
    private void SelectBook(Guid bookId)
    {
        // In this case passing the whole Book object through the route would also be fine
        Navigator.Navigate(new BookRoute(bookId));
    }
}