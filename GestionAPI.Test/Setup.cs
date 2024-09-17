using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionAPI.Test
{
    public static class Setup
    {
        public static GestionDbContext GetInMemoryDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<GestionDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new GestionDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
