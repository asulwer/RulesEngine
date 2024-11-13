// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace RulesEngine.Models
{
    public class ImplicitObject
    {
        #region Implicit Operators

        public static implicit operator ImplicitObject(bool value)
        {
            return default;
        }

        public static implicit operator ImplicitObject(bool? value)
        {
            return default;
        }

        public static implicit operator ImplicitObject(char value)
        {
            return default;
        }

        public static implicit operator ImplicitObject(char? value)
        {
            return default;
        }

        public static implicit operator ImplicitObject(string value)
        {
            return default;
        }

        public static implicit operator ImplicitObject(int value)
        {
            return default;
        }

        public static implicit operator ImplicitObject(int? value)
        {
            return default;
        }

        public static implicit operator ImplicitObject(decimal value)
        {
            return default;
        }

        public static implicit operator ImplicitObject(decimal? value)
        {
            return default;
        }

        #endregion

    }
}
