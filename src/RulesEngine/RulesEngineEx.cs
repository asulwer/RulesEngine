// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using Newtonsoft.Json;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RulesEngine
{
    public partial class RulesEngine : IRulesEngineEx
    {
        public async IAsyncEnumerable<List<RuleResultTree>> ExecuteAllWorkflows(RuleParameter[] inputs, [EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (var wf_nam in _rulesCache.GetAllWorkflowNames())
            {
                if (!ct.IsCancellationRequested)
                    yield return await ExecuteWorkflow(wf_nam, inputs, ct);
            }
        }
        public async Task<List<RuleResultTree>> ExecuteWorkflow(string workflowName, RuleParameter[] inputs, CancellationToken ct = default)
        {
            Array.Sort(inputs, (RuleParameter a, RuleParameter b) => string.Compare(a.Name, b.Name));

            var result = new List<RuleResultTree>();

            foreach (var rule in _rulesCache.GetWorkflow(workflowName).Rules)
            {
                if (ct.IsCancellationRequested)
                    break;

                var resultTree = await ExecuteRule(workflowName, rule.RuleName, inputs, ct);
                result.Add(resultTree);
            }

            return result;
        }
        public async Task<RuleResultTree> ExecuteRule(string workflowName, string ruleName, RuleParameter[] ruleParams, CancellationToken ct = default)
        {
            RuleResultTree ruleResultTree = null;

            if (!ct.IsCancellationRequested)
            {
                if (RegisterRule(workflowName, ruleParams))
                {
                    var compiledRulesCacheKey = GetCompiledRulesKey(workflowName, ruleParams);

                    var compiledRule = _rulesCache.GetCompiledRules(compiledRulesCacheKey)[ruleName];
                    ruleResultTree = compiledRule(ruleParams);

                    FormatErrorMessages(ruleResultTree);

                    if (ruleResultTree.ChildResults != null)
                        await ExecuteActionAsync(ruleResultTree.ChildResults, ct);

                    var actionResult = await ExecuteActionForRuleResult(ruleResultTree, true, ct);

                    ruleResultTree.ActionResult = new ActionResult
                    {
                        Output = actionResult.Output,
                        Exception = actionResult.Exception
                    };
                }
                else
                {
                    // if rules are not registered with Rules Engine
                    throw new ArgumentException($"Rule config file is not present for the {workflowName} workflow");
                }
            }

            return ruleResultTree;
        }
        public async Task<ActionRuleResult> ExecuteRuleActions(string workflowName, string ruleName, RuleParameter[] inputs, CancellationToken ct = default)
        {
            var compiledRule = CompileRule(workflowName, ruleName, inputs);
            var resultTree = compiledRule(inputs);

            return await ExecuteActionForRuleResult(resultTree, true, ct);
        }

        private RuleResultTree FormatErrorMessages(RuleResultTree ruleResult)
        {
            if (_reSettings.EnableFormattedErrorMessage && !ruleResult.IsSuccess)
            {
                var errorMessage = ruleResult?.Rule?.ErrorMessage;
                if (string.IsNullOrWhiteSpace(ruleResult.ExceptionMessage) && errorMessage != null)
                {
                    var errorParameters = Regex.Matches(errorMessage, ParamParseRegex);

                    var inputs = ruleResult.Inputs;
                    foreach (var param in errorParameters)
                    {
                        var paramVal = param?.ToString();
                        var property = paramVal?.Substring(2, paramVal.Length - 3);
                        if (property?.Split('.')?.Count() > 1)
                        {
                            var typeName = property?.Split('.')?[0];
                            var propertyName = property?.Split('.')?[1];
                            errorMessage = UpdateErrorMessage(errorMessage, inputs, property, typeName, propertyName);
                        }
                        else
                        {
                            var arrParams = inputs?.Select(c => new { Name = c.Key, c.Value });
                            var model = arrParams?.Where(a => string.Equals(a.Name, property))?.FirstOrDefault();
                            var value = model?.Value != null ? JsonConvert.SerializeObject(model?.Value) : null;
                            errorMessage = errorMessage?.Replace($"$({property})", value ?? $"$({property})");
                        }
                    }
                    ruleResult.ExceptionMessage = errorMessage;
                }
            }

            return ruleResult;
        }
    }
}
