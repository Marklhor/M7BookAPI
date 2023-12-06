using BLL.Implementations;
using DAL;
using DAL.Repositories.Interfaces;
using Domain.Entities;
using Moq;

namespace TestUnitaire;

public class BookStoreServiceUnitTest
{
    [Fact]
    public async void AddBookAsync_With_BookAndAuthor_Should_Be_ReturnBook()
    {

        //Arrange (Arrange les donn�es d'entr�e et isole la m�thode � tester grace aux simulacres.)
        Book book = new Book
        {
            Title = "Test",
            Author = new Author
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Test"
            }
        };

        IBookRepository bookRepository = Mock.Of<IBookRepository>();
        Mock.Get(bookRepository)
            .Setup(bookRepository => bookRepository.AddAsync(book))
            .ReturnsAsync(() =>
            {
                return new Book
                {
                    Id = 1, // Id g�n�r� par la base de donn�es
                    Title = "Test",
                    AuthorId = 1
                };
            });

        //Simuler AuthorRepository
        IAuthorRepository authorRepository = Mock.Of<IAuthorRepository>();
        Mock.Get(authorRepository)
            .Setup(authorRepository => authorRepository.GetByIdAsync(book.Author.Id))
            .ReturnsAsync(book.Author);
        Mock.Get(authorRepository)
            .Setup(authorRepository => authorRepository.AddAsync(book.Author))
            .ReturnsAsync(book.Author);


        IUOW uow = Mock.Of<IUOW>();
        Mock.Get(uow)
            .Setup(uow => uow.Books)
            .Returns(bookRepository);
        Mock.Get(uow)
            .Setup(uow => uow.Authors)
            .Returns(authorRepository);

        BookStoreService bookStoreService = new BookStoreService(uow);

        //Act (Ex�cute la m�thode � tester avec les donn�es d'entr�e.)

        var result = await bookStoreService.AddBookAsync(book);

        //Assert (V�rifie que la m�thode � tester a produit les r�sultats attendus.)

        Assert.True(result.AuthorId == book.Author.Id);
        Assert.True(result.Title == book.Title);

    }
}