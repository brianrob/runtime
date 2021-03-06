// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace System.Linq.Parallel.Tests
{
    internal static class KeyValuePair
    {
        internal static KeyValuePair<T, U> Create<T, U>(T first, U second)
        {
            return new KeyValuePair<T, U>(first, second);
        }
    }
}
