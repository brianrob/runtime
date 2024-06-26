// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace ILVerify
{
    public interface IResolver
    {
        /// <summary>
        /// Resolve assembly to PEReader. This method should return the same instance when queried multiple times.
        /// </summary>
        PEReader ResolveAssembly(AssemblyNameInfo assemblyName);

        /// <summary>
        /// Resolve module to PEReader. This method should return the same instance when queried multiple times.
        /// </summary>
        PEReader ResolveModule(AssemblyNameInfo referencingAssembly, string fileName);
    }
}
