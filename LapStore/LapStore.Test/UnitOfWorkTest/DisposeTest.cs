using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LapStore.Test.Helpers;
using LapStore.DAL;
using LapStore.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LapStore.Test.UnitOfWorkTest
{
    public class DisposeTest
    {
        [Fact]
        public void Dispose_DoesNotThrow()
        {
            // Arrange: Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<LapStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "DisposeTestDB")
                .Options;

            using var context = new LapStoreDbContext(options);
            var unitOfWork = new UnitOfWork(context);

            // Act: Record any exception thrown during Dispose
            var exception = Record.Exception(() => unitOfWork.Dispose());

            // Assert: Ensure no exception is thrown during Dispose
            Assert.Null(exception);
        }
    }
} 