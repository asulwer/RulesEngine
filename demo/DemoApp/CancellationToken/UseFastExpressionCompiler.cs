﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoApp.CancellationToken
{
    public class UseFastExpressionCompiler
    {
        public async Task Run(System.Threading.CancellationToken ct = default)
        {
            var worflow = new Workflow[] {
                new Workflow {
                    WorkflowName = "UseFastExpressionCompilerTest",
                    Rules = new List<Rule> {
                        new Rule {
                            RuleName = "check local param with plus operator",
                            Expression = "Total > 0",
                            LocalParams = [
                                new ScopedParam() { Name = "Field1", Expression = "AppData.Details.Sum(l => l.Amount.Value)" },
                                new ScopedParam() { Name = "Field2", Expression = "AppData.Details.Sum(l => l.Amount.Value)" },
                                new ScopedParam() { Name = "Field3", Expression = "AppData.Details.Sum(l => l.Amount.Value)" },
                                new ScopedParam() { Name = "Total", Expression = "Field1 + Field2 + Field3" }
                            ]
                        }
                    }
                }
            };

            var appData = new RuleParameter[] {
                new RuleParameter("AppData", new AppData() {
                    Details = new List<Detail>
                    {
                        new Detail { Amount = 1.0m },
                        new Detail { Amount = 2.0m },
                        new Detail { Amount = 3.0m }
                    }
                })
            };

            var reSettingsWithCustomTypes = new ReSettings {
                UseFastExpressionCompiler = true //default setting is true
            };

            var bre = new RulesEngine.RulesEngine(worflow, reSettingsWithCustomTypes);

            var ret = await bre.ExecuteAllRulesAsync("UseFastExpressionCompilerTest", appData, ct);

            if (ret is { Count: > 0 })
            {
                Console.WriteLine(ret[0].IsSuccess);
            }
        }

        internal class AppData
        {
            public List<Detail> Details { get; set; } = new List<Detail>();
        }

        internal class Detail
        {
            public decimal? Amount { get; set; }
        }
    }
}