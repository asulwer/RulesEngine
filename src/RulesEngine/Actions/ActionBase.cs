﻿// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RulesEngine.Actions
{
    public abstract class ActionBase
    {
        internal async virtual ValueTask<ActionRuleResult> ExecuteAndReturnResultAsync(ActionContext context, RuleParameter[] ruleParameters, ReSettings reSettings, bool includeRuleResults = false)
        {
            var result = new ActionRuleResult();
            try
            {
                result.Output = await Run(context, ruleParameters);
            }
            catch (Exception ex)
            {
                if (!reSettings.IgnoreException && reSettings.EnableExceptionAsErrorMessage)
                    result.Exception = new Exception($"Exception while executing {GetType().Name}: {ex.Message}", ex);
                else if(!reSettings.IgnoreException && !reSettings.EnableExceptionAsErrorMessage)
                    throw ex;
            }
            finally
            {
                if (includeRuleResults)
                {
                    result.Results = new List<RuleResultTree>()
                    {
                        context.GetParentRuleResult()
                    };
                }
            }
            return result;
        }
        public abstract ValueTask<object> Run(ActionContext context, RuleParameter[] ruleParameters);
    }
}
