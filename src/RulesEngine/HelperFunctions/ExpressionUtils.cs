﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;

namespace RulesEngine.HelperFunctions
{
    public static class ExpressionUtils
    {
        //TODO: expand upon this to include more builtin
        public static bool CheckContains(string check, string valList)
        {
            if (string.IsNullOrEmpty(check) || string.IsNullOrEmpty(valList))
                return false;

            var list = valList.Split(',').ToList();
            return list.Contains(check);
        }
    }
}
