﻿using LinqSharp.EFCore.Data;
using LinqSharp.EFCore.Data.Test;
using System.Linq;
using Xunit;

namespace LinqSharp.EFCore.Test
{
    public class CacheableTests
    {
        public class NameContainer : ICacheable<NameContainer.PreQueries>
        {
            public PreQueries Source { get; }

            public class PreQueries
            {
                public PreQuery<ApplicationDbContext, LS_Name> LS_Names { get; set; }
            }

            public NameContainer(string name)
            {
                Source = new PreQueries
                {
                    LS_Names = new PreQuery<ApplicationDbContext, LS_Name>(x => x.LS_Names).Where(x => x.Name == name),
                };
            }
        }

        [Fact]
        public void Test1()
        {
            using var mysql = ApplicationDbContext.UseMySql();
            using (var trans = mysql.Database.BeginTransaction())
            {
                mysql.LS_Names.Add(new LS_Name { Name = "A" });
                mysql.LS_Names.Add(new LS_Name { Name = "B" });
                mysql.LS_Names.Add(new LS_Name { Name = "C" });
                mysql.SaveChanges();

                var containers = new[] { "A", "C" }.Select(n => new NameContainer(n)).ToArray();
                mysql.ApplyCache(containers);

                mysql.ApplyCache(containers);

                Assert.Equal(2, containers.SelectMany(x => x.Source.LS_Names.Result).Count());
            }

        }

    }
}
