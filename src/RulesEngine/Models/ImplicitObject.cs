// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// https://github.com/asulwer/RulesEngine/issues/75

using System;
using System.Diagnostics.CodeAnalysis;

namespace RulesEngine.Models
{
    [ExcludeFromCodeCoverage]
    public class ImplicitObject : IEquatable<ImplicitObject>
    {
        public static implicit operator ImplicitObject(bool _) => default;

        public static implicit operator ImplicitObject(bool? _) => default;

        public static implicit operator ImplicitObject(char _) => default;

        public static implicit operator ImplicitObject(char? _) => default;

        public static implicit operator ImplicitObject(string _) => default;

        public static implicit operator ImplicitObject(int _) => default;

        public static implicit operator ImplicitObject(int? _) => default;

        public static implicit operator ImplicitObject(decimal _) => default;

        public static implicit operator ImplicitObject(decimal? _) => default;

        public static implicit operator ImplicitObject(float _) => default;

        public static implicit operator ImplicitObject(float? _) => default;

        public static bool operator <(ImplicitObject l, ImplicitObject r) => default;

        public static bool operator >(ImplicitObject l, ImplicitObject r) => default;

        public static bool operator <=(ImplicitObject l, ImplicitObject r) => default;

        public static bool operator >=(ImplicitObject l, ImplicitObject r) => default;

        public static bool operator ==(ImplicitObject l, ImplicitObject r) => default;

        public static bool operator !=(ImplicitObject l, ImplicitObject r) => default;

        public bool Equals(ImplicitObject other)
        {
            return default;
        }

        public override bool Equals(object obj)
        {
            return default;
        }

        public override int GetHashCode()
        {
            return default;
        }
    }
}