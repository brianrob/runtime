// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using ILLink.Shared.DataFlow;
using ILLink.Shared.TrimAnalysis;
using Microsoft.CodeAnalysis;

namespace ILLink.RoslynAnalyzer.TrimAnalysis
{
    internal static class GenericArgumentDataFlow
    {
        public static void ProcessGenericArgumentDataFlow(Location location, INamedTypeSymbol type, Action<Diagnostic> reportDiagnostic)
        {
            while (type is { IsGenericType: true })
            {
                ProcessGenericArgumentDataFlow(location, type.TypeArguments, type.TypeParameters, reportDiagnostic);
                type = type.ContainingType;
            }
        }

        public static void ProcessGenericArgumentDataFlow(Location location, IMethodSymbol method, Action<Diagnostic> reportDiagnostic)
        {
            ProcessGenericArgumentDataFlow(location, method.TypeArguments, method.TypeParameters, reportDiagnostic);

            ProcessGenericArgumentDataFlow(location, method.ContainingType, reportDiagnostic);
        }

        public static void ProcessGenericArgumentDataFlow(Location location, IFieldSymbol field, Action<Diagnostic> reportDiagnostic)
        {
            ProcessGenericArgumentDataFlow(location, field.ContainingType, reportDiagnostic);
        }

        public static void ProcessGenericArgumentDataFlow(Location location, IPropertySymbol property, Action<Diagnostic> reportDiagnostic)
        {
            ProcessGenericArgumentDataFlow(location, property.ContainingType, reportDiagnostic);
        }

        private static void ProcessGenericArgumentDataFlow(
            Location location,
            ImmutableArray<ITypeSymbol> typeArguments,
            ImmutableArray<ITypeParameterSymbol> typeParameters,
            Action<Diagnostic> reportDiagnostic)
        {
            var diagnosticContext = new DiagnosticContext(location, reportDiagnostic);
            for (int i = 0; i < typeArguments.Length; i++)
            {
                var typeArgument = typeArguments[i];
                // Apply annotations to the generic argument
                var genericParameterValue = new GenericParameterValue(typeParameters[i]);
                if (genericParameterValue.DynamicallyAccessedMemberTypes != DynamicallyAccessedMemberTypes.None)
                {
                    SingleValue genericArgumentValue = SingleValueExtensions.FromTypeSymbol(typeArgument)!;
                    var reflectionAccessAnalyzer = new ReflectionAccessAnalyzer(reportDiagnostic, typeHierarchyType: null);
                    var requireDynamicallyAccessedMembersAction = new RequireDynamicallyAccessedMembersAction(diagnosticContext, reflectionAccessAnalyzer);
                    requireDynamicallyAccessedMembersAction.Invoke(genericArgumentValue, genericParameterValue);
                }

                // Recursively process generic argument data flow on the generic argument if it itself is generic
                if (typeArgument is INamedTypeSymbol namedTypeArgument && namedTypeArgument.IsGenericType)
                    ProcessGenericArgumentDataFlow(location, namedTypeArgument, reportDiagnostic);
            }
        }

        public static bool RequiresGenericArgumentDataFlow(INamedTypeSymbol type)
        {
            while (type is { IsGenericType: true })
            {
                if (RequiresGenericArgumentDataFlow(type.TypeParameters))
                    return true;

                foreach (var typeArgument in type.TypeArguments)
                {
                    if (typeArgument is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType
                        && RequiresGenericArgumentDataFlow(namedTypeSymbol))
                        return true;
                }

                type = type.ContainingType;
            }

            return false;
        }

        public static bool RequiresGenericArgumentDataFlow(IMethodSymbol method)
        {
            if (method.IsGenericMethod)
            {
                if (RequiresGenericArgumentDataFlow(method.TypeParameters))
                    return true;

                foreach (var typeArgument in method.TypeArguments)
                {
                    if (typeArgument is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType
                        && RequiresGenericArgumentDataFlow(namedTypeSymbol))
                        return true;
                }
            }

            return RequiresGenericArgumentDataFlow(method.ContainingType);
        }

        public static bool RequiresGenericArgumentDataFlow(IFieldSymbol field)
        {
            return RequiresGenericArgumentDataFlow(field.ContainingType);
        }

        public static bool RequiresGenericArgumentDataFlow(IPropertySymbol property)
        {
            return RequiresGenericArgumentDataFlow(property.ContainingType);
        }

        private static bool RequiresGenericArgumentDataFlow(ImmutableArray<ITypeParameterSymbol> typeParameters)
        {
            foreach (var typeParameter in typeParameters)
            {
                var genericParameterValue = new GenericParameterValue(typeParameter);
                if (genericParameterValue.DynamicallyAccessedMemberTypes != DynamicallyAccessedMemberTypes.None)
                    return true;
            }

            return false;
        }
    }
}
