// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RulesEngine.UnitTest.ActionTests
{
    [ExcludeFromCodeCoverage]
    public class ActionsWithExceptions
    {
        [Fact]
        public async Task Exception_Ignore()
        {
            var settings = new ReSettings() {
                IgnoreException = true,
                EnableExceptionAsErrorMessage = false
            };

            var engine = new RulesEngine(GetWorkflow(), settings);

            var inputs = new RuleParameter[] { new("input1", new { count = 1 }) };

            var result = await engine.ExecuteActionWorkflowAsync("WorkflowAction_Exception", "Action", inputs, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Null(result.Output);
            Assert.Null(result.Exception);
        }

        [Fact]
        public async Task Exception_AsMessage()
        {
            var settings = new ReSettings() {
                IgnoreException = false,
                EnableExceptionAsErrorMessage = true
            };

            var engine = new RulesEngine(GetWorkflow(), settings);

            var inputs = new RuleParameter[] { new("input1", new { count = 1 }) };

            var result = await engine.ExecuteActionWorkflowAsync("WorkflowAction_Exception", "Action", inputs, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Null(result.Output);
            Assert.NotNull(result.Exception);
        }

        [Fact]
        public async Task Exception_AsException()
        {
            var settings = new ReSettings() {
                IgnoreException = false,
                EnableExceptionAsErrorMessage = false
            };

            var engine = new RulesEngine(GetWorkflow(), settings);

            var inputs = new RuleParameter[] { new("input1", new { count = 1 }) };

            await Assert.ThrowsAsync<System.Linq.Dynamic.Core.Exceptions.ParseException>(async () => await engine.ExecuteActionWorkflowAsync("WorkflowAction_Exception", "Action", inputs, CancellationToken.None));            
        }

        private Workflow[] GetWorkflow()
        {
            return new Workflow[] {
                new() {
                    WorkflowName = "WorkflowAction_Exception",
                    Rules = new List<Rule> {
                        new() {
                            RuleName = "Action",
                            Expression = "1==1", //not used when Actions are being used
                            Actions = new RuleActions {
                                OnSuccess = new ActionInfo {
                                    Name = "OutputExpression",
                                    Context = new Dictionary<string, object> {{"expression", "counts"}} //fails because it should be count
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
