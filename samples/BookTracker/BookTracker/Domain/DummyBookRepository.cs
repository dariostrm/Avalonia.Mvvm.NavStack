using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookTracker.Domain;

public class DummyBookRepository : IBookRepository
{
    private readonly List<Book> _books = [];
    
    public event Action<IList<Book>>? BooksChanged;

    public DummyBookRepository()
    {
        //Dummy data
        _books.Add(Book.Create(title: "The Great Gatsby", author: "F. Scott Fitzgerald", pages: 180));
        _books.Add(Book.Create(title: "To Kill a Mockingbird", author: "Harper Lee", pages: 281));
        _books.Add(Book.Create(title: "1984", author: "George Orwell", pages: 328));
        _books.Add(Book.Create(title: "Pride and Prejudice", author: "Jane Austen", pages: 279));
    }
    
    public Task<IList<Book>> GetBooksAsync()
    {
        return Task.FromResult<IList<Book>>(_books);
    }

    public async Task<Book?> GetBookAsync(Guid id)
    {
        // Simulate async operation
        await Task.Delay(100);
        
        return _books.Find(book => book.Id == id);
    }

    public async Task AddBookAsync(Book book)
    {
        // Simulate async operation
        await Task.Delay(100);
        
        _books.Add(book);
        BooksChanged?.Invoke(_books);
    }

    public async Task UpdateBookAsync(Book book)
    {
        // Simulate async operation
        await Task.Delay(100);
        
        var index = _books.FindIndex(b => b.Id == book.Id);
        if (index != -1)
        {
            _books[index] = book;
            BooksChanged?.Invoke(_books);
        }
    }

    public async Task DeleteBookAsync(Guid bookId)
    {
        // Simulate async operation
        await Task.Delay(100);
        
        var book = _books.Find(b => b.Id == bookId);
        if (book != null)
        {
            _books.Remove(book);
            BooksChanged?.Invoke(_books);
        }
    }

}