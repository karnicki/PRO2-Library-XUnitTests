using System;
using System.Collections.Generic;
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
                    IdUser = 1,
                    IdBook = 1,
                    Comments = "GSDHJKfilu2gh89432 4u 2389-fhsjk h"
                });

                _db.SaveChanges();

            }
        }


        [Fact]
        public async Task AddBookBorrow_200Ok()
        {
            var httpResponse = await _client.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows");

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var borrows = JsonConvert.DeserializeObject<IEnumerable<BookBorrow>>(content);
            // using (var scope = _server.Host.Services.CreateScope())
            // {
            //     var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
            //     Assert.True(_db.User.Any());
            // }

            Assert.True(borrows.Count() == 1);
            Assert.True(borrows.ElementAt(0).IdBook == 1);
        }

    }
}
