using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace XUnitTestProject1.Controllers
{
    public class BookBorrowsInitTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;


        public BookBorrowsInitTests()
        {
            _server = ServerFactory.GetServerInstance();
            _client = _server.CreateClient();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                _db.BookBorrow.Add(new BookBorrow
                {
                    IdBookBorrow = 1,
                    IdUser = 1,
                    IdBook = 1,
                    //DateFrom = new DateTime(),
                    //DateTo = new DateTime(),
                    BorrowDate = new DateTime(),
                    ReturnDate = new DateTime(),
                    Comments = "GSDHJKfilu2gh89432 4u 2389-fhsjk h"
                });

                _db.SaveChanges();

            }
        }


        [Fact]
        public async Task AddBookBorrow_201Created()
        {
            var newBookBorrow = new BookBorrow
            {
                IdUser = 2,
                IdBook = 2,
                BorrowDate = DateTime.Today,
                Comments = "GSD2389-fhsjk h"

            };
            

            var borrow = JsonConvert.SerializeObject(newBookBorrow);
            StringContent borrowStringContent = new StringContent(borrow, Encoding.UTF8, "application/json");
            var httpResponse = await _client.PostAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows", borrowStringContent);

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();


            Assert.True(httpResponse.StatusCode == HttpStatusCode.Created);

        }

        [Fact]
        public async Task UpdateBookBorrow_200Ok()
        {
            var updatedBookBorrow = new BookBorrow
            {
                IdBookBorrow = 1,
                IdUser = 1,
                IdBook = 1,
                BorrowDate = new DateTime(),
                ReturnDate = new DateTime(),
                Comments = "jednak ma byc taki komentarz"
            };

            var content = JsonConvert.SerializeObject(updatedBookBorrow);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows/{updatedBookBorrow.IdBookBorrow}", stringContent);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
                Assert.True(_db.BookBorrow.Any(info => (
                    info.IdBookBorrow == updatedBookBorrow.IdBookBorrow
                    && info.IdUser == updatedBookBorrow.IdUser
                    && info.IdBook == updatedBookBorrow.IdBook
                    && info.BorrowDate == updatedBookBorrow.BorrowDate
                    && info.ReturnDate == updatedBookBorrow.ReturnDate
                    && info.Comments == updatedBookBorrow.Comments
                    )
                ));
            }

            /* NIEPRAWIDŁOWE BO UŻYWA METODY GET WIĘC JEŻELI ONA MA BŁĘDY TO AUTOMATYCZNIE BŁĄD POJAWI SIĘ I TUTAJ
            
            var GetResponse = await _client.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows/{updatedBookBorrow.IdBookBorrow}");
            GetResponse.EnsureSuccessStatusCode();
            var GetResponseContent = await GetResponse.Content.ReadAsStringAsync();
            var BookBorrowAfterUpdate = JsonConvert.DeserializeObject<BookBorrow>(content);
            Assert.Equal(updatedBookBorrow.BorrowDate, BookBorrowAfterUpdate.BorrowDate);
            */
        }

    }
}
