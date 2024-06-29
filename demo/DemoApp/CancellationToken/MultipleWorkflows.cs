﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using RulesEngine.Extensions;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoApp.CancellationToken
{
    public class MultipleWorkflows
    {
        public async Task Run(System.Threading.CancellationToken ct = default)
        {
            Console.WriteLine($"Running {nameof(MultipleWorkflows)}....");

            var workflows = new Workflow[] {
                new Workflow {
                    WorkflowName = "Test Workflow1",
                    Rules = new List<Rule> {
                        new Rule {
                            RuleName = "Test Rule1",
                            SuccessEvent = "Count is less",
                            ErrorMessage = "Over Expected",
                            Expression = "count < 3",
                        },
                        new Rule {
                            RuleName = "Test Rule2",
                            SuccessEvent = "Count is more",
                            ErrorMessage = "Under Expected",
                            Expression = "count > 3",
                        }
                    }
                },
                new Workflow {
                    WorkflowName = "Test Workflow2",
                    Rules = new List<Rule> {
                        new Rule {
                            RuleName = "Test Rule",
                            SuccessEvent = "Count is greater",
                            ErrorMessage = "Under Expected",
                            Expression = "count > 3",
                        }
                    }
                },
                new Workflow {
                    WorkflowName = "Test Workflow3",
                    Rules = new List<Rule> {
                        new Rule {
                            RuleName = "Test Rule",
                            Expression = "1 == 1",
                            Actions = new RuleActions() {
                                OnSuccess = new ActionInfo {
                                    Name = "OutputExpression",
                                    Context =  new Dictionary<string, object> {
                                        {"expression", "2*2"}
                                    }
                                }
                            }
                        }
                    }
                },
                new Workflow {
                    WorkflowName = "Test Workflow4",
                    Rules = new List<Rule> {
                        new Rule {
                            RuleName = "Test Rule",
                            Expression = "1 == 1",
                            Actions = new RuleActions() {
                                OnSuccess = new ActionInfo {
                                    Name = "OutputExpression",
                                    Context =  new Dictionary<string, object> {
                                        {"expression", "4*4"}
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var bre = new RulesEngine.RulesEngine(workflows, null);

            var inputs = new RuleParameter[] {
                new RuleParameter("input1", new { count = 1 })
            };

            foreach (var workflow in workflows)
            {
                var ret = await bre.ExecuteAllRulesAsync(workflow.WorkflowName, inputs);

                var outcome = false;

                //Different ways to show test results:
                outcome = ret.TrueForAll(r => r.IsSuccess);

                ret.OnSuccess((eventName) => {
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
}
