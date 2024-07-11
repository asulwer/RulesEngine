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
                        RuleName = "Test Rule",
                        Expression = "1 == 1",
                        Actions = new RuleActions {
                            OnSuccess = new ActionInfo {
                                Name = "OutputExpression",
                                Context = new Dictionary<string, object> {{"expression", "count > 3"}}
                            }
                        }
                    }
                }
            },
            new() {
                WorkflowName = "Test Workflow2",
                Rules = new List<Rule> {
                    new() {
                        RuleName = "Test Rule",
                        Expression = "1 == 1",
                        Actions = new RuleActions {
                            OnSuccess = new ActionInfo {
                                Name = "OutputExpression",
                                Context = new Dictionary<string, object> {{"expression", "count < 3"}}
                            }
                        }
                    }
                }
            }
        };

        var bre = new RulesEngine.RulesEngine(workflows);

        dynamic input1 = new ExpandoObject();
        input1.count = 1;
        var inputs = new RuleParameter[] { new ("input1", input1) };
        
        //var inputs = new RuleParameter[] { new ("input1", new {count = 1})};
        //DynamicClass dc = inputs[0].Value as DynamicClass;
        //var names = dc.GetDynamicMemberNames();
        //var value = dc.GetDynamicPropertyValue<int>("count");

        foreach (var workflow in workflows)
        {
            var ret = await bre.ExecuteAllRulesAsync(workflow.WorkflowName, inputs, cancellationToken);

            //Different ways to show test results:
            var outcome = ret.TrueForAll(r => r.IsSuccess);

            ret.OnSuccess(eventName => {
                Console.WriteLine($"Result '{eventName}' is as expected.");
                outcome = true;
            });

            ret.OnFail(() => {
                outcome = false;
            });

            Console.WriteLine($"Test outcome: {outcome}.");
        }
    }
}