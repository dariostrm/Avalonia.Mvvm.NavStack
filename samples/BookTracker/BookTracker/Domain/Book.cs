using System;

namespace BookTracker.Domain;

public record Book(
    Guid Id,
    string Title,
    string Author,
    int Pages,
    bool IsRead
)
{
    public static Book Create(string title, string author, int pages) =>
        new(Guid.NewGuid(), title, author, pages, false);
}