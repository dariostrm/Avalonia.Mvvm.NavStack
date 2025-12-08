using System;
using System.Threading.Tasks;
using BookTracker.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mvvm.NavStack;

namespace BookTracker.ViewModels;

public partial class BookViewModel : ViewModelBase
{
    private readonly IBookRepository _bookRepository;
    
    public BookViewModel(Guid bookId, IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
        _ = LoadBookAsync(bookId);
    }
    
    [ObservableProperty] private Book? _book;
    
    [ObservableProperty] private bool _loaded;
    
    private async Task LoadBookAsync(Guid bookId)
    {
        var book = await _bookRepository.GetBookAsync(bookId);
        Book = book;
        Loaded = true;
    }
    
    [RelayCommand]
    private void GoBack()
    {
        Navigator.GoBack();
    }
}