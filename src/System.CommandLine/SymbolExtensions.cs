﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Linq;

namespace System.CommandLine
{
    /// <summary>
    /// Provides extension methods for symbols.
    /// </summary>
    internal static class SymbolExtensions
    {
        internal static IReadOnlyList<Argument> Arguments(this Symbol symbol)
        {
            switch (symbol)
            {
                case Option option:
                    return new[]
                    {
                        option.Argument
                    };
                case Command command:
                    return command.Arguments;
                case Argument argument:
                    return new[]
                    {
                        argument
                    };
                default:
                    throw new NotSupportedException();
            }
        }

        internal static Parser GetOrCreateDefaultParser(this Symbol symbol)
        {
            var root = GetOrCreateRootCommand(symbol);

            if (root.ImplicitParser is not { } parser)
            {
                parser = new Parser(new CommandLineConfiguration(root));
                root.ImplicitParser = parser;
            }

            return parser;
        }

        internal static Command GetOrCreateRootCommand(Symbol symbol)
        {
            if (symbol is Command cmd)
            {
                return cmd;
            }

            if (symbol.FirstParent is null)
            {
                return new RootCommand { symbol };
            }

            ParentNode? current = symbol.FirstParent;
            while (current is not null)
            {
                if (current.Symbol is RootCommand root)
                {
                    return root;
                }

                current = current.Next;
            }
            
            return new RootCommand { symbol };
        }
    }
}