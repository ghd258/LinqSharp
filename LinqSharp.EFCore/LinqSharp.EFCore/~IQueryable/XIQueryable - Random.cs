﻿// Copyright 2020 zmjack
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// See the LICENSE file in the project root for more information.

using System.Linq;

namespace LinqSharp.EFCore
{
    public static partial class XIQueryable
    {
        /// <summary>
        /// Select the specified number of random record from a source set.
        /// <para>[Warning] Before calling this function, you need to open the provider functions.</para>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="this"></param>
        /// <param name="takeCount"></param>
        /// <returns></returns>
        public static IQueryable<TSource> Random<TSource>(this IQueryable<TSource> @this, int takeCount)
            where TSource : class
        {
            return @this.OrderBy(x => DbFunc.Random()).Take(takeCount);
        }

    }

}
