﻿// Copyright 2020 zmjack
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
#if EFCORE3_0_OR_GREATER
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
#endif
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqSharp.EFCore.Functions
{
    public class DbFuncRegister
    {
#if EFCORE3_0_OR_GREATER
        public delegate SqlExpression TranslatorDelegate(MethodInfo method, SqlExpression[] args);
#else
        public delegate Expression TranslatorDelegate(MethodInfo method, Expression[] args);
#endif
        public ModelBuilder ModelBuilder { get; }

        public DbFuncRegister(ModelBuilder modelBuilder)
        {
            ModelBuilder = modelBuilder;
        }

        public void Register(Expression<Func<object>> dbFunc, TranslatorDelegate register)
        {
            MethodInfo method = null;

            if (dbFunc.Body is UnaryExpression unary)
            {
                if (unary.NodeType == ExpressionType.Convert && unary.Type == typeof(object))
                {
                    if (unary.Operand is MethodCallExpression call) method = call.Method;
                }
            }
            else if (dbFunc.Body is MethodCallExpression call) method = call.Method;
            else throw new ArgumentException("Invalid expression.", nameof(dbFunc));

            ModelBuilder.HasDbFunction(method).HasTranslation(args => register(method, args.ToArray()));
        }

    }
}