using Xunit;
using LapStore.Test.Helpers;
using LapStore.DAL;
using LapStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using LapStore.DAL.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.Test.UnitOfWorkTest
{
    public class GenericRepositoryTest
    {
        [Fact]
        public void GenericRepository_ReturnsRepositoryOfCorrectType()
        {
            // Arrange: Set up in-memory database
            var options = new DbContextOptionsBuilder<LapStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "RepoTestDB")
                .Options;

            // Act: Create context and unit of work
            using var context = new LapStoreDbContext(options);
            var unitOfWork = new UnitOfWork(context);

            // Act: Get repository from unit of work
            var repo = unitOfWork.GenericRepository<TestEntity>();

            // Assert: Check if the returned repository is of the correct type
            Assert.IsType<GenericRepository<TestEntity>>(repo);
        }
    }
} 