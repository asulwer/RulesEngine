using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RulesEngine.Models;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;

namespace DemoApp
{
    public class ActionWorkflow : IDemo
    {
        public async Task Run(CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Running {nameof(ActionWorkflow)}....");

            var workflows = new Workflow[] {
                new() {
                    WorkflowName = "Test Workflow3",
                    Rules = new List<Rule> {
                        new() {
                            RuleName = "Test Rule",
                            Expression = "1==1", //not used when Actions are being used
                            Actions = new RuleActions {
                                OnSuccess = new ActionInfo {
                                    Name = "OutputExpression",
                                    Context = new Dictionary<string, object> {{"expression", "count"}}
                                }
                            }
                        }
                    }
                },
                new() {
                    WorkflowName = "Test Workflow4",
                    Rules = new List<Rule> {
                        new() {
                            RuleName = "Test Rule",
                            Expression = "1==1", //not used when Actions are being used
                            Actions = new RuleActions {
                                OnSuccess = new ActionInfo {
                                    Name = "OutputExpression",
                                    Context = new Dictionary<string, object> {{"expression", "counts"}}
                                }
                            }
                        }
                    }
                }
            };

            var settings = new ReSettings() {
                IgnoreException = true,
                EnableExceptionAsErrorMessage = false
            };

            var bre = new RulesEngine.RulesEngine(workflows, settings);

            var inputs = new RuleParameter[] { new("input1", new { count = 1 }) };

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
}
