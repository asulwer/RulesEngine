// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using RulesEngine.Extensions;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace DemoApp;

public class Actions : IDemo
{
    public async Task Run(CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Running {nameof(Actions)}....");

        var workflows = new Workflow[] {
            new() {
                WorkflowName = "Test Workflow1",
                Rules = new List<Rule> {
                    new() {
                        RuleName = "GiveDiscount10Percent",
                        Expression = "input1.Total <= 1.5",
                        Actions = new RuleActions {
                            OnSuccess = new ActionInfo {
                                Name = "OutputExpression",
                                Context = new Dictionary<string, object> {{"Expression", "input1.Total * 1.1"}}
                            }
                        }
                    }
                }
            },
            new() {
                WorkflowName = "Test Workflow2",
                Rules = new List<Rule> {
                    new() {
                        RuleName = "GiveDiscount20Percent",
                        Expression = "input1.Total > 1.5",
                        Actions = new RuleActions {
                            OnSuccess = new ActionInfo {
                                Name = "OutputExpression",
                                Context = new Dictionary<string, object> {{"Expression", "input1.Total * 1.2" } }
                            }
                        }
                    }
                }
            }
        };

        var bre = new RulesEngine.RulesEngine(workflows);

        dynamic input1 = new ExpandoObject();
        input1.Total = 1.5m;
        var inputs = new RuleParameter[] { new RuleParameter("input1", input1) };

        ExpandoObject data = inputs[0].Value.ToExpando<RuleParameter>();

        //var inputs = new RuleParameter[] { new ("input1", new {count = 1})};
        //DynamicClass dc = inputs[0].Value as DynamicClass;
        //var names = dc.GetDynamicMemberNames();
        //var value = dc.GetDynamicPropertyValue<int>("count");

        foreach (var workflow in workflows)
        {
            var rrList = await bre.ExecuteAllRulesAsync(workflow.WorkflowName, inputs, cancellationToken);

            foreach (var rr in rrList)
            {
                if (rr.IsSuccess && rr.ActionResult != null)
                    Console.WriteLine($"{rr.Rule.RuleName} : {rr.ActionResult.Output}");
            }
        }
    }
}