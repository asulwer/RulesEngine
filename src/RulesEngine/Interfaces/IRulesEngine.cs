// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using RulesEngine.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RulesEngine.Interfaces
{
    public interface IRulesEngine : IBaseRulesEngine
    {
        /// <summary>
        /// This will execute all the rules of the specified workflow
        /// </summary>
        /// <param name="workflowName">The name of the workflow with rules to execute against the inputs</param>
        /// <param name="inputs">A variable number of inputs</param>
        /// <returns>List of rule results</returns>
        ValueTask<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, params object[] inputs);
        ValueTask<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, object[] inputs, CancellationToken ct = default);

        /// <summary>
        /// This will execute all the rules of the specified workflow
        /// </summary>
        /// <param name="workflowName">The name of the workflow with rules to execute against the inputs</param>
        /// <param name="ruleParams">A variable number of rule parameters</param>
        /// <returns>List of rule results</returns>
        ValueTask<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, params RuleParameter[] ruleParams);
        ValueTask<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, RuleParameter[] ruleParams, CancellationToken ct = default);

        ValueTask<ActionRuleResult> ExecuteActionWorkflowAsync(string workflowName, string ruleName, RuleParameter[] ruleParameters);

        ValueTask<ActionRuleResult> ExecuteActionWorkflowAsync(string workflowName, string ruleName, RuleParameter[] ruleParameters, CancellationToken cancellationToken);
    }
}
