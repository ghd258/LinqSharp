﻿// Copyright 2020 zmjack
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqSharp.Dev
{
    public static partial class XIEnumerable
    {
        public static IEnumerable<TSource> WhereNot<TSource>(this IEnumerable<TSource> @this, Expression<Func<TSource, bool>> predicate)
        {
            return @this.Where(Expression.Lambda<Func<TSource, bool>>(Expression.Not(predicate.Body), predicate.Parameters).Compile());
        }
    }

}
