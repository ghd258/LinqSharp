﻿// Copyright 2020 zmjack
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// See the LICENSE file in the project root for more information.

using LinqSharp.Query;
using NStandard;
using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace LinqSharp;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ExpressionExtensions
{
    /// <summary>
    /// Rebind parameter of expression, then return itself.
    /// </summary>
    /// <typeparam name="TExpression"></typeparam>
    /// <param name="this"></param>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static TExpression RebindParameter<TExpression>(this TExpression @this, Expression origin, Expression target)
        where TExpression : Expression
    {
        if (origin == target) return @this;
        else
        {
            var visitor = new ExpressionRebindVisitor(origin, target);
            return (visitor.Visit(@this) as TExpression)!;
        }
    }

    /// <summary>
    /// Rebind all parameters of the specified expressions, combine them, then return the final expression.
    /// </summary>
    /// <typeparam name="TLambdaExpression"></typeparam>
    /// <param name="this"></param>
    /// <param name="binary"></param>
    /// <returns></returns>
    public static TLambdaExpression? LambdaJoin<TLambdaExpression>(this TLambdaExpression[] @this, Func<Expression, Expression, BinaryExpression> binary)
        where TLambdaExpression : LambdaExpression
    {
        if (@this.AllSame(x => x.Parameters.Count))
        {
            var parameters = @this.First().Parameters;
            var lambda = Expression.Lambda(@this.Aggregate(null as Expression, (acc, exp) =>
            {
                if (acc is null) return exp.Body;
                else
                {
                    TLambdaExpression rebindExp = exp;
                    foreach (var (item1, item2) in Any.Zip(exp.Parameters, parameters))
                    {
                        rebindExp = RebindParameter(rebindExp, item1, item2);
                    }
                    return binary(acc, rebindExp.Body);
                }
            })!, parameters) as TLambdaExpression;
            return lambda;
        }
        else return null;
    }

}
