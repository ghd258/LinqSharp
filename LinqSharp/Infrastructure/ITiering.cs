﻿// Copyright 2020 zmjack
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace LinqSharp.Infrastructure
{
    public interface ITiering<TElement> : IEnumerable<TElement>
    {
        object Key { get; }
        int Span { get; }
        bool Final { get; }
        IEnumerable<ITiering<TElement>> SubTiers { get; }
    }
}