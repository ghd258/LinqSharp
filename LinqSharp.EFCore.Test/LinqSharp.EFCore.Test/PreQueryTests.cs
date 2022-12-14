﻿using LinqSharp.EFCore.Data.Test;
using Northwnd;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace LinqSharp.EFCore.Test
{
    public class PreQueryTests
    {
        [Fact]
        public void Test1()
        {
            using var mysql = ApplicationDbContext.UseMySql();
            (string CategoryName, int Year)[] queryParams = new[]
            {
                ("Beverages", 1996),
                ("Seafood", 1997),
            };

            var preQueries = queryParams.Select(p =>
            {
                return new PreQuery<ApplicationDbContext, OrderDetail>(x => x.OrderDetails)
                    .Include(x => x.ProductLink.CategoryLink)
                    .Include(x => x.OrderLink)
                    .Where(x =>
                        x.ProductLink.CategoryLink.CategoryName == p.CategoryName
                        && x.OrderLink.OrderDate.Value.Year == p.Year);
            }).ToArray();
            var query = preQueries.Feed(mysql);

            Assert.Equal(240, query.Length);
            Assert.Equal(78, preQueries[0].Result.Length);
            Assert.Equal(162, preQueries[1].Result.Length);
        }

        [Fact]
        public void Test3()
        {
            using var mysql = ApplicationDbContext.UseMySql();
            /// TODO: Use interface creation to optimize calls.
            var preQueries = new[]
            {
                new PreQuery<ApplicationDbContext, Order>(x => x.Orders)
                    .Include(x => x.OrderDetails).ThenInclude<ICollection<OrderDetail>, OrderDetail, Product>(x => x.ProductLink)
                    .Include(x => x.OrderDetails)
                    .Where(x => true),
            };
            var query = preQueries.Feed(mysql);
            Assert.Equal("x => True", preQueries[0].ToString());
        }
    }

}
