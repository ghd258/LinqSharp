﻿using LinqSharp.EFCore.Data.Test;
using Microsoft.EntityFrameworkCore.Design;

namespace DbCreator.MySql
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            return ApplicationDbContext.UseMySql(b => b.MigrationsAssembly("DbCreator.MySql"));
        }
    }
}
