﻿// Copyright 2020 zmjack
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using NStandard;
using System;

#pragma warning disable IDE0060 // Remove unused parameter

namespace LinqSharp.EFCore
{
    internal static class PostgreSQLFuncProvider
    {
        public static void Register(ModelBuilder modelBuilder)
        {
            Random(modelBuilder);
            Concat(modelBuilder);
        }

        private static void Random(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDbFunction(typeof(DbFunc).GetMethod(nameof(DbFunc.Random)))
                .HasTranslation(args => Translator.Function<double>("RANDOM", args));
        }

        private static void Concat(ModelBuilder modelBuilder)
        {
            for (int n = DbFunc.MinConcat; n <= DbFunc.MaxConcat; n++)
            {
                var types = new Type[n].Let(() => typeof(string));
                modelBuilder
                    .HasDbFunction(typeof(DbFunc).GetMethod(nameof(DbFunc.Concat), types))
                    .HasTranslation(args => Translator.Function<string>("CONCAT", args));
            }
        }

    }
}

#pragma warning restore IDE0060 // Remove unused parameter
