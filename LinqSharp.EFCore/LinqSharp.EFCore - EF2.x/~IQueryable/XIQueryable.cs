﻿// Copyright 2020 zmjack
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// See the LICENSE file in the project root for more information.

using LinqSharp.EFCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using NStandard;
using Remotion.Linq.Parsing.Structure;
using System;
using System.Linq;

namespace LinqSharp
{
    public static partial class XIQueryable
    {
        /// <summary>
        /// Gets the generated Sql string.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string ToSql<TEntity>(this IQueryable<TEntity> @this)
        {
            if (EFVersion.AtLeast(3, 0)) throw EFVersion.NeedNewerVersionException;
            if (EFVersion.AtLeast(2, 1))
            {
                var queryCompiler = @this.Provider.GetReflector<EntityQueryProvider>().DeclaredField<QueryCompiler>("_queryCompiler").Value;
                var queryCompilerReflector = queryCompiler.GetReflector();

                var queryModel = queryCompilerReflector.DeclaredField<QueryModelGenerator>("_queryModelGenerator").Value.ParseQuery(@this.Expression);

                var dependencies = queryCompilerReflector.DeclaredProperty<Database>("Database").DeclaredProperty<DatabaseDependencies>("Dependencies").Value;
                var modelVisitor = dependencies.QueryCompilationContextFactory.Create(false).CreateQueryModelVisitor() as RelationalQueryModelVisitor;
                modelVisitor.CreateQueryExecutor<TEntity>(queryModel);

                var sql = modelVisitor.Queries.Select(x => $"{x.ToString().TrimEnd(';')};{Environment.NewLine}").Join("");
                return sql;
            }
            else if (EFVersion.AtLeast(2, 0))
            {
                var queryCompiler = @this.Provider.GetReflector<EntityQueryProvider>().DeclaredField<QueryCompiler>("_queryCompiler").Value;
                var queryCompilerReflector = queryCompiler.GetReflector();

                var nodeTypeProvider = queryCompilerReflector.DeclaredProperty<INodeTypeProvider>("NodeTypeProvider").Value;
                var parser = queryCompilerReflector.Method("CreateQueryParser").Call(nodeTypeProvider) as QueryParser;
                var queryModel = parser.GetParsedQuery(@this.Expression);

                var dependencies = queryCompilerReflector.DeclaredProperty<Database>("Database").DeclaredProperty<DatabaseDependencies>("Dependencies").Value;
                var modelVisitor = dependencies.QueryCompilationContextFactory.Create(false).CreateQueryModelVisitor() as RelationalQueryModelVisitor;
                modelVisitor.CreateQueryExecutor<TEntity>(queryModel);

                var sql = modelVisitor.Queries.Select(x => $"{x.ToString().TrimEnd(';')};{Environment.NewLine}").Join("");
                return sql;
            }
            else throw EFVersion.NotSupportedException;
        }

    }
}
