﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RulesEngine.Models
{
    /// <summary>
    /// Workflow rules class for deserialization  the json config file
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Workflow
    {
        /// <summary>
        /// Gets the workflow name.
        /// </summary>
        public string WorkflowName { get; set; }

        public IEnumerable<string> WorkflowsToInject { get; set; }

        public RuleExpressionType RuleExpressionType { get; set; } = RuleExpressionType.LambdaExpression;

        /// <summary>
        /// Gets or Sets the global params which will be applicable to all rules
        /// </summary>
        public IEnumerable<ScopedParam> GlobalParams { get; set; }

        /// <summary>
        /// list of rules.
        /// </summary>
        public IEnumerable<Rule> Rules { get; set; }
    }
}
