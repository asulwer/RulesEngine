// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using RulesEngine.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RulesEngine.Interfaces
{
    public interface IRulesEngineEx : IBaseRulesEngine
    {
        IAsyncEnumerable<List<RuleResultTree>> ExecuteAllWorkflows(RuleParameter[] inputs, CancellationToken ct = default);
        Task<List<RuleResultTree>> ExecuteWorkflow(string workflowName, RuleParameter[] inputs, CancellationToken ct = default);
        Task<RuleResultTree> ExecuteRule(string workflowName, string ruleName, RuleParameter[] ruleParams, CancellationToken ct = default);
        Task<ActionRuleResult> ExecuteRuleActions(string workflowName, string ruleName, RuleParameter[] inputs, CancellationToken ct = default);        
    }
}
