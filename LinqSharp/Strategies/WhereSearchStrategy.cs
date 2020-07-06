﻿// Copyright 2020 zmjack
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// See the LICENSE file in the project root for more information.

using NStandard;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqSharp.Strategies
{
    public class WhereSearchStrategy<TEntity> : WhereStringStrategy<TEntity>
    {
        private static readonly MethodInfo _Method_Enumerable_op_Any = typeof(Enumerable)
            .GetMethodViaQualifiedName("Boolean Any[TSource](System.Collections.Generic.IEnumerable`1[TSource], System.Func`2[TSource,System.Boolean])")
            .MakeGenericMethod(typeof(string));
        private static readonly MethodInfo _Method_String_op_Contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
        private static readonly MethodInfo _Method_String_op_Equals = typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string) });

        public WhereSearchStrategy(string searchString, Expression<Func<TEntity, object>> searchMembers, SearchOption option)
        {
            Func<Expression, Expression, Expression> compareExp;
            MethodInfo stringMethod;

            switch (option)
            {
                case SearchOption.Contains:
                case SearchOption.NotContains: stringMethod = _Method_String_op_Contains; break;

                case SearchOption.Equals:
                case SearchOption.NotEquals: stringMethod = _Method_String_op_Equals; break;

                default: throw new NotSupportedException();
            }

            compareExp = (singlePartOfInExp, secharStringExp) => singlePartOfInExp.For(exp =>
            {
                if (exp.Type == typeof(string))
                    return Expression.Call(singlePartOfInExp, stringMethod, secharStringExp);
                else if (exp.Type.GetInterface(typeof(IEnumerable).FullName) != null)
                {
                    var parameter = Expression.Parameter(typeof(string));
                    Expression<Func<string, bool>> lambda;

                    switch (option)
                    {
                        case SearchOption.Contains:
                        case SearchOption.Equals:
                            lambda = Expression.Lambda<Func<string, bool>>(
                                Expression.Call(parameter, stringMethod, secharStringExp),
                                parameter);
                            break;

                        case SearchOption.NotContains:
                        case SearchOption.NotEquals:
                            lambda = Expression.Lambda<Func<string, bool>>(
                                Expression.Not(Expression.Call(parameter, stringMethod, secharStringExp)),
                                parameter);
                            break;

                        default: throw new NotSupportedException();
                    }

                    return Expression.Call(_Method_Enumerable_op_Any, singlePartOfInExp, lambda);
                }
                else throw new NotSupportedException();
            });

            Init(searchMembers, compareExp, searchString ?? "");
        }

    }
}