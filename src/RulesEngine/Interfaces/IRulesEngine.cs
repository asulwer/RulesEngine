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
        ValueTask<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, params object[] inputs);
        ValueTask<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, object[] inputs, CancellationToken ct = default);
        ValueTask<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, params RuleParameter[] ruleParams);
        ValueTask<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, RuleParameter[] ruleParams, CancellationToken ct = default);
        ValueTask<ActionRuleResult> ExecuteActionWorkflowAsync(string workflowName, string ruleName, RuleParameter[] ruleParameters);
        ValueTask<ActionRuleResult> ExecuteActionWorkflowAsync(string workflowName, string ruleName, RuleParameter[] ruleParameters, CancellationToken ct = default);
    }
}
