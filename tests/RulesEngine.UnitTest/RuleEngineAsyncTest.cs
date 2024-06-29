// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RulesEngine.Exceptions;
using RulesEngine.HelperFunctions;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RulesEngine.UnitTest
{
    [Trait("Category", "Unit")]
    [ExcludeFromCodeCoverage]
    public class RulesEngineAsyncTest
    {
        [Theory]
        [InlineData("rules1.json")]
        public void RulesEngine_New_ReturnsNotNull(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);
            Assert.NotNull(re);
        }

        [Theory]
        [InlineData("rules2.json")]
        public async Task RulesEngine_InjectedRules_ContainsInjectedRules(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", GetInput1()),
                new RuleParameter("input2", GetInput2()),
                new RuleParameter("input3", GetInput3())
            };

            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Theory]
        [InlineData("rules2.json")]
        public async Task ExecuteRule_ReturnsListOfRuleResultTree(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", GetInput1()),
                new RuleParameter("input2", GetInput2()),
                new RuleParameter("input3", GetInput3())
            };

            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.IsType<List<RuleResultTree>>(result);
            Assert.Contains(result, c => c.IsSuccess);
        }

        [Theory]
        [InlineData("rules1.json", "rules6.json")]
        public async Task ExecuteRule_AddWorkflowWithSameName_ThrowsValidationException(string previousWorkflowFile, string newWorkflowFile)
        {
            var re = GetRulesEngine(previousWorkflowFile);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", GetInput1()),
                new RuleParameter("input2", GetInput2()),
                new RuleParameter("input3", GetInput3())
            };

            // Run previous rules.
            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.IsType<List<RuleResultTree>>(result);
            Assert.Contains(result, c => c.IsSuccess);

            // Fetch and add new rules.
            var newWorkflow = ParseAsWorkflow(newWorkflowFile);

            Assert.Throws<RuleValidationException>(() => re.AddWorkflow(newWorkflow));
        }

        [Theory]
        [InlineData("rules1.json", "rules6.json")]
        public async Task ExecuteRule_AddOrUpdateWorkflow_ExecutesUpdatedRules(string previousWorkflowFile, string newWorkflowFile)
        {
            var re = GetRulesEngine(previousWorkflowFile);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", GetInput1()),
                new RuleParameter("input2", GetInput2()),
                new RuleParameter("input3", GetInput3())
            };

            // Run previous rules.
            List<RuleResultTree> result1 = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result1);
            Assert.IsType<List<RuleResultTree>>(result1);
            Assert.Contains(result1, c => c.IsSuccess);

            // Fetch and update new rules.
            Workflow newWorkflow = ParseAsWorkflow(newWorkflowFile);
            re.AddOrUpdateWorkflow(newWorkflow);

            // Run new rules.
            List<RuleResultTree> result2 = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result2);
            Assert.IsType<List<RuleResultTree>>(result2);
            Assert.DoesNotContain(result2, c => c.IsSuccess);

            // New execution should have different result than previous execution.
            var previousResults = result1.Select(c => new { c.Rule.RuleName, c.IsSuccess });
            var newResults = result2.Select(c => new { c.Rule.RuleName, c.IsSuccess });
            Assert.NotEqual(previousResults, newResults);
        }

        [Theory]
        [InlineData("rules2.json")]
        public void GetAllRegisteredWorkflows_ReturnsListOfAllWorkflows(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);
            var workflow = re.GetAllRegisteredWorkflowNames();

            Assert.NotNull(workflow);
            Assert.Equal(2, workflow.Count);
            Assert.Contains("inputWorkflow", workflow);
        }

        [Fact]
        public void GetAllRegisteredWorkflows_NoWorkflow_ReturnsEmptyList()
        {
            var re = new RulesEngine();
            var workflow = re.GetAllRegisteredWorkflowNames();

            Assert.NotNull(workflow);
            Assert.Empty(workflow);
        }

        [Theory]
        [InlineData("rules2.json")]
        public async Task ExecuteRule_ManyInputs_ReturnsListOfRuleResultTree(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", GetInput1()),
                new RuleParameter("input2", GetInput2()),
                new RuleParameter("input3", GetInput3()),

                new RuleParameter("input4", GetInput1()),
                new RuleParameter("input5", GetInput2()),
                new RuleParameter("input6", GetInput3()),

                new RuleParameter("input7", GetInput1()),
                new RuleParameter("input8", GetInput2()),
                new RuleParameter("input9", GetInput3()),

                new RuleParameter("input10", GetInput1()),
                new RuleParameter("input11", GetInput2()),
                new RuleParameter("input12", GetInput3()),

                new RuleParameter("input13", GetInput1()),
                new RuleParameter("input14", GetInput2()),
                new RuleParameter("input15", GetInput3()),

                new RuleParameter("input16", GetInput1()),
                new RuleParameter("input17", GetInput2()),
                new RuleParameter("input18", GetInput3())
            };

            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.IsType<List<RuleResultTree>>(result);
            Assert.Contains(result, c => c.IsSuccess);
        }

        [Theory]
        [InlineData("rules2.json")]
        public async Task ExecuteRule_CalledMultipleTimes_ReturnsSameResult(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", GetInput1()),
                new RuleParameter("input2", GetInput2()),
                new RuleParameter("input3", GetInput3())
            };

            List<RuleResultTree> result1 = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result1);
            Assert.IsType<List<RuleResultTree>>(result1);
            Assert.Contains(result1, c => c.IsSuccess);

            List<RuleResultTree> result2 = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result2);
            Assert.IsType<List<RuleResultTree>>(result2);
            Assert.Contains(result2, c => c.IsSuccess);

            var expected = result1.Select(c => new { c.Rule.RuleName, c.IsSuccess });
            var actual = result2.Select(c => new { c.Rule.RuleName, c.IsSuccess });
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("rules2.json")]
        public async Task ExecuteRule_SingleObject_ReturnsListOfRuleResultTree(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", GetInput1())
            };

            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.IsType<List<RuleResultTree>>(result);
            Assert.DoesNotContain(result, c => c.IsSuccess);
        }

        [Theory]
        [InlineData("rules3.json")]
        public async Task ExecuteRule_ExceptionScenario_RulesInvalid(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", GetInput1()),
                new RuleParameter("input2", GetInput2()),
                new RuleParameter("input3", GetInput3())
            };

            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result[0].ExceptionMessage) || string.IsNullOrWhiteSpace(result[0].ExceptionMessage));
        }

        [Fact]
        public void RulesEngine_New_IncorrectJSON_ThrowsException()
        {
            Assert.Throws<RuleValidationException>(() => {
                var workflow = new Workflow();
                var re = CreateRulesEngine(workflow);
            });

            Assert.Throws<RuleValidationException>(() => {
                var workflow = new Workflow() { WorkflowName = "test" };
                var re = CreateRulesEngine(workflow);
            });
        }

        [Fact]
        public void RulesEngine_AddOrUpdate_IncorrectJSON_ThrowsException()
        {
            Assert.Throws<RuleValidationException>(() => {
                var workflow = new Workflow();
                var re = new RulesEngine();
                re.AddOrUpdateWorkflow(workflow);
            });

            Assert.Throws<RuleValidationException>(() => {
                var workflow = new Workflow() { WorkflowName = "test" };
                var re = new RulesEngine();
                re.AddOrUpdateWorkflow(workflow);
            });
        }

        [Theory]
        [InlineData("rules1.json")]
        public async Task ExecuteRule_InvalidWorkFlow_ThrowsException(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", GetInput1())
            };

            await Assert.ThrowsAsync<NullReferenceException>(async () => { await re.ExecuteWorkflow("inputWorkflow1", rp, CancellationToken.None); });
        }

        [Theory]
        [InlineData("rules1.json")]
        public async Task RemoveWorkflow_RemovesWorkflow(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);
            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", GetInput1()),
                new RuleParameter("input2", GetInput2()),
                new RuleParameter("input3", GetInput3())
            };

            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            
            Assert.NotNull(result);
            re.RemoveWorkflow("inputWorkflow");

            await Assert.ThrowsAsync<NullReferenceException>(async () => await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None));
        }

        [Theory]
        [InlineData("rules1.json")]
        public async Task ClearWorkflow_RemovesAllWorkflow(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);
            re.ClearWorkflows();

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", GetInput1()),
                new RuleParameter("input2", GetInput2()),
                new RuleParameter("input3", GetInput3())
            };

            await Assert.ThrowsAsync<NullReferenceException>(async () => await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None));
            await Assert.ThrowsAsync<NullReferenceException>(async () => await re.ExecuteWorkflow("inputWorkflowReference", rp, CancellationToken.None));
        }

        [Theory]
        [InlineData("rules1.json")]
        [InlineData("rules2.json")]
        public async Task ExecuteRule_InputWithVariableProps_ReturnsResult(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", GetInput1()),
                new RuleParameter("input2", GetInput2()),
                new RuleParameter("input3", GetInput3())
            };

            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.IsType<List<RuleResultTree>>(result);
            Assert.Contains(result, c => c.IsSuccess);

            //rp[2].Value = "world";

            result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.IsType<List<RuleResultTree>>(result);
            Assert.Contains(result, c => c.IsSuccess);
        }

        [Theory]
        [InlineData("rules2.json")]
        public async Task ExecuteRule_ReturnsProperErrorOnMissingRuleParameter(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);

            var rp = new RuleParameter[]
            {
                new RuleParameter("customName", GetInput1()),
                new RuleParameter("input2", GetInput2()),
                new RuleParameter("input3", GetInput3())
            };

            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.IsType<List<RuleResultTree>>(result);
            Assert.Contains(result.First().ChildResults, c => c.ExceptionMessage.Contains("Unknown identifier 'input1'"));
        }

        [Theory]
        [InlineData("rules5.json", "hello", true)]
        [InlineData("rules5.json", null, false)]
        public async Task ExecuteRule_WithInjectedUtils_ReturnsListOfRuleResultTree(string ruleFileName, string propValue, bool expectedResult)
        {
            var re = GetRulesEngine(ruleFileName);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new { Property1 = propValue }),
                new RuleParameter("utils", new TestInstanceUtils())
            };
            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);

            //dynamic input1 = new ExpandoObject();

            //input1.Property1 = propValue;

            //var utils = new TestInstanceUtils();

            //var result = await re.ExecuteAllRulesAsync("inputWorkflow", [new RuleParameter("input1", input1), new RuleParameter("utils", utils)]);
            Assert.NotNull(result);
            Assert.IsType<List<RuleResultTree>>(result);
            Assert.All(result, c => Assert.Equal(expectedResult, c.IsSuccess));
        }

        [Theory]
        [InlineData("rules6.json")]
        public async Task ExecuteRule_RuleWithMethodExpression_ReturnsSucess(string ruleFileName)
        {
            Func<bool> func = () => true;

            var re = GetRulesEngine(ruleFileName, new ReSettings {
               CustomTypes = new[] { typeof(Func<bool>) }
            });

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new { Property1 = "hello", Boolean = false, Method = func }),
                new RuleParameter("utils", new TestInstanceUtils())
            };
            
            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.IsType<List<RuleResultTree>>(result);
            Assert.All(result, c => Assert.True(c.IsSuccess));
        }

        [Theory]
        [InlineData("rules7.json")]
        public async Task ExecuteRule_RuleWithUnaryExpression_ReturnsSucess(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new { Boolean = false })
            };

            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.IsType<List<RuleResultTree>>(result);
            Assert.All(result, c => Assert.True(c.IsSuccess));
        }

        [Theory]
        [InlineData("rules8.json")]
        public async Task ExecuteRule_RuleWithMemberAccessExpression_ReturnsSucess(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new { Boolean = false })
            };

            List<RuleResultTree> result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.IsType<List<RuleResultTree>>(result);
            Assert.All(result, c => Assert.False(c.IsSuccess));
        }

        [Theory]
        [InlineData("rules9.json")]
        public async Task ExecuteRule_MissingMethodInExpression_ReturnsException(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName, new ReSettings() { EnableExceptionAsErrorMessage = false });

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new { Boolean = false, Data = new {TestProperty = "" } })
            };

            await Assert.ThrowsAsync<RuleException>(async () => {
                var result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            });
        }

        [Theory]
        [InlineData("rules9.json")]
        public async Task ExecuteRule_CompilationException_ReturnsAsErrorMessage(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName, new ReSettings() { EnableExceptionAsErrorMessage = true });

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new { Boolean = false, Data = new {TestProperty = "" } })
            };
            var result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);

            Assert.NotNull(result);
            Assert.StartsWith("Exception while parsing expression", result[1].ExceptionMessage);
        }

        [Theory]
        [InlineData("rules9.json")]
        public async Task ExecuteRuleWithIgnoreException_CompilationException_DoesNotReturnsAsErrorMessage(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName, new ReSettings() { EnableExceptionAsErrorMessage = true, IgnoreException = true });

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new { Boolean = false, Data = new {TestProperty = "" } })
            };
            var result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result[1].ExceptionMessage.StartsWith("Exception while parsing expression"));
        }

        [Theory]
        [InlineData("rules10.json")]
        public async Task ExecuteRuleWithJsonElement(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName, new ReSettings() {
                EnableExceptionAsErrorMessage = true,
                CustomTypes = new Type[] { typeof(System.Text.Json.JsonElement) }
            });

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new {
                    Data = System.Text.Json.JsonSerializer.SerializeToElement(new { category= "abc" })
                })
            };
            
            var result = await re.ExecuteWorkflow("inputWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.All(result, c => Assert.True(c.IsSuccess));
        }

        [Theory]
        [InlineData("rules11.json")]
        public async Task RulesEngineWithGlobalParam_RunsSuccessfully(string ruleFileName)
        {
            var re = GetRulesEngine(ruleFileName, new ReSettings() { });

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new[] {
                    new {
                        Value= 0.13259286,
                        ChangeDateTime= "2023-07-28T19:57:07.432339Z"
                    },
                    new {
                        Value= 0.09435427,
                        ChangeDateTime= "2023-07-28T19:58:04.536459Z"
                    },
                    new {
                        Value= 0.14896593,
                        ChangeDateTime= "2023-07-28T19:59:08.682072Z"
                    },
                    new {
                        Value= 0.12852388,
                        ChangeDateTime= "2023-07-28T20:00:06.78036Z"
                    },
                    new {
                        Value= 0.17011189,
                        ChangeDateTime= "2023-07-28T20:00:54.873615Z"
                    },
                    new {
                        Value= 0.0532116,
                        ChangeDateTime= "2023-07-28T20:02:52.04049Z"
                    },
                    new {
                        Value= 0.04064374,
                        ChangeDateTime= "2023-07-28T20:03:54.168499Z"
                    },
                    new {
                        Value= 0.03748944,
                        ChangeDateTime= "2023-07-28T20:03:54.194786Z"
                    },
                    new {
                        Value= 0.07752395,
                        ChangeDateTime= "2023-07-28T20:06:32.451464Z"
                    },
                    new {
                        Value= 0.07294922,
                        ChangeDateTime= "2023-07-28T20:07:38.691755Z"
                    },
                    new {
                        Value= 0.09892442,
                        ChangeDateTime= "2023-07-28T20:08:37.98802Z"
                    },
                    new {
                        Value= 0.06370641,
                        ChangeDateTime= "2023-07-28T20:05:41.358461Z"
                    },
                    new {
                        Value= 0.07550429,
                        ChangeDateTime= "2023-07-28T20:09:48.129748Z"
                    },
                    new {
                        Value= 0.0653021,
                        ChangeDateTime= "2023-07-28T20:10:48.274482Z"
                    },
                    new {
                        Value= 0.09304246,
                        ChangeDateTime= "2023-07-28T20:11:49.436983Z"
                    },
                    new {
                        Value= 0.0797422,
                        ChangeDateTime= "2023-07-28T20:12:53.609118Z"
                    },
                    new {
                        Value= 0.08211832,
                        ChangeDateTime= "2023-07-28T20:13:52.699728Z"
                    },
                    new {
                        Value= 0.06955433,
                        ChangeDateTime= "2023-07-28T20:15:03.843289Z"
                    },
                    new {
                        Value= 0.07626661,
                        ChangeDateTime= "2023-07-28T20:15:03.870057Z"
                    },
                    new {
                        Value= 0.05033984,
                        ChangeDateTime= "2023-07-28T20:16:17.032262Z"
                    },
                    new {
                        Value= 0.05202596,
                        ChangeDateTime= "2023-07-28T20:17:20.172669Z"
                    },
                    new {
                        Value= 0.06861198,
                        ChangeDateTime= "2023-07-28T20:18:32.303309Z"
                    },
                    new {
                        Value= 0.04935532,
                        ChangeDateTime= "2023-07-28T20:19:33.451426Z"
                    },
                    new {
                        Value= 0.04073699,
                        ChangeDateTime= "2023-07-28T20:20:37.737395Z"
                    },
                    new {
                        Value= 0.02164916,
                        ChangeDateTime= "2023-07-28T20:21:38.883635Z"
                    },
                    new {
                        Value= 0.01334031,
                        ChangeDateTime= "2023-07-28T20:22:40.053193Z"
                    },
                    new {
                        Value= 0.0336915,
                        ChangeDateTime= "2023-07-28T20:23:44.240297Z"
                    },
                    new {
                        Value= 0.04870055,
                        ChangeDateTime= "2023-07-28T20:26:33.584756Z"
                    },
                    new {
                        Value= 0.07125243,
                        ChangeDateTime= "2023-07-28T20:28:11.7889Z"
                    },
                    new {
                        Value= 0.04904275,
                        ChangeDateTime= "2023-07-28T20:24:40.346216Z"
                    },
                    new {
                        Value= 0.03625701,
                        ChangeDateTime= "2023-07-28T20:27:20.707478Z"
                    },
                    new {
                        Value= 0.05703328,
                        ChangeDateTime= "2023-07-28T20:28:57.876436Z"
                    },
                    new {
                        Value= 0.04364996,
                        ChangeDateTime= "2023-07-28T20:25:43.496357Z"
                    },
                    new {
                        Value= 0.07558272,
                        ChangeDateTime= "2023-07-28T20:30:11.023295Z"
                    },
                    new {
                        Value= 0.03073958,
                        ChangeDateTime= "2023-07-28T20:33:00.347672Z"
                    },
                    new {
                        Value= 0.0341309,
                        ChangeDateTime= "2023-07-28T20:33:59.790621Z"
                    },
                    new {
                        Value= 0.05270871,
                        ChangeDateTime= "2023-07-28T20:31:15.166193Z"
                    },
                    new {
                        Value= 0.09138862,
                        ChangeDateTime= "2023-07-28T20:32:08.259273Z"
                    },
                    new {
                        Value= 0.15922104,
                        ChangeDateTime= "2023-07-28T20:35:12.963809Z"
                    },
                    new {
                        Value= 0.11383641,
                        ChangeDateTime= "2023-07-28T20:36:26.120815Z"
                    },
                    new {
                        Value= 0.12404025,
                        ChangeDateTime= "2023-07-28T20:37:37.27212Z"
                    },
                    new {
                        Value= 0.06010197,
                        ChangeDateTime= "2023-07-28T20:38:47.409412Z"
                    },
                    new {
                        Value= 0.08396237,
                        ChangeDateTime= "2023-07-28T20:39:37.504217Z"
                    },
                    new {
                        Value= 0.06731881,
                        ChangeDateTime= "2023-07-28T20:40:27.588895Z"
                    },
                    new {
                        Value= 0.05617253,
                        ChangeDateTime= "2023-07-28T20:41:33.760373Z"
                    },
                    new {
                        Value= 0.0585155,
                        ChangeDateTime= "2023-07-28T20:42:26.847144Z"
                    },
                    new {
                        Value= 0.06793098,
                        ChangeDateTime= "2023-07-28T20:43:36.988904Z"
                    },
                    new {
                        Value= 0.06879344,
                        ChangeDateTime= "2023-07-28T20:44:46.133926Z"
                    },
                    new {
                        Value= 0.06931814,
                        ChangeDateTime= "2023-07-28T20:45:50.275932Z"
                    },
                    new {
                        Value= 0.04802603,
                        ChangeDateTime= "2023-07-28T20:46:36.367289Z"
                    },
                    new {
                        Value= 0.04036225,
                        ChangeDateTime= "2023-07-28T20:47:27.484188Z"
                    },
                    new {
                        Value= 0.04968483,
                        ChangeDateTime= "2023-07-28T20:48:13.582228Z"
                    },
                    new {
                        Value= 0.0773483,
                        ChangeDateTime= "2023-07-28T19:49:16.354277Z"
                    },
                    new {
                        Value= 0.08710921,
                        ChangeDateTime= "2023-07-28T19:48:25.253743Z"
                    },
                    new {
                        Value= 0.07665287,
                        ChangeDateTime= "2023-07-28T19:50:25.496642Z"
                    },
                    new {
                        Value= 0.06121748,
                        ChangeDateTime= "2023-07-28T19:51:20.644955Z"
                    },
                    new {
                        Value= 0.04179136,
                        ChangeDateTime= "2023-07-28T19:52:26.793369Z"
                    },
                    new {
                        Value= 0.13522345,
                        ChangeDateTime= "2023-07-28T19:54:19.051669Z"
                    },
                    new {
                        Value= 0.08536856,
                        ChangeDateTime= "2023-07-28T19:56:04.287806Z"
                    },
                    new {
                        Value= 0.05041369,
                        ChangeDateTime= "2023-07-28T19:53:18.901696Z"
                    },
                    new {
                        Value= 0.1627249,
                        ChangeDateTime= "2023-07-28T19:55:13.160235Z"
                    },
                    new {
                        Value= 0.05,
                        ChangeDateTime= "2023-07-28T19:54:03.2197Z"
                    },
                    new {
                        Value= 0.05,
                        ChangeDateTime= "2023-07-28T19:56:00.802023Z"
                    },
                    new {
                        Value= 0.02792705297470093,
                        ChangeDateTime= "2023-07-28T20:49:03.6825337Z"
                    }
                }.ToList())
            };

            var result = await re.ExecuteWorkflow("MyWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.False(result[0].IsSuccess);
            Assert.True(result[1].IsSuccess);
        }

        [Fact]
        public async Task ExecuteRule_RuntimeError_ShouldReturnAsErrorMessage()
        {
            var workflows = new Workflow[] {
                new Workflow {
                    WorkflowName = "TestWorkflow",
                    Rules = new[] {
                        new Rule {
                            RuleName = "ruleWithRuntimeError",
                            Expression = "input1.Country.ToLower() == \"india\""
                        }
                    }
                } 
            };

            var re = new RulesEngine(workflows, null);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new RuleTestClass { Country = null })
            };

            var result = await re.ExecuteWorkflow("TestWorkflow", rp, CancellationToken.None);
            Assert.NotNull(result);
            Assert.All(result, rule => Assert.False(rule.IsSuccess));
            Assert.All(result, rule => Assert.StartsWith("Error while executing rule :", rule.ExceptionMessage));
        }

        [Fact]
        public async Task ExecuteRule_RuntimeErrorInPreviousRun_ShouldReturnEmptyErrorMessage()
        {
            var workflow = new Workflow
            {
                WorkflowName = "TestWorkflow",
                Rules = new[] {
                    new Rule {
                        RuleName = "ruleWithRuntimeError",
                        Expression = "input1.Country.ToLower() == \"india\""
                    }
                }
            };

            var re = new RulesEngine(new[] { workflow }, null);

            var rp1 = new RuleParameter[]
            {
                new RuleParameter("input1", new RuleTestClass { Country = null })
            };

            var result = await re.ExecuteWorkflow("TestWorkflow", rp1, CancellationToken.None);
            Assert.NotNull(result);
            Assert.All(result, rule => Assert.False(rule.IsSuccess));
            Assert.All(result, rule => Assert.StartsWith("Error while executing rule :", rule.ExceptionMessage));

            var rp2 = new RuleParameter[]
            {
                new RuleParameter("input1", new RuleTestClass { Country = "india" })
            };

            result = await re.ExecuteWorkflow("TestWorkflow", rp2, CancellationToken.None);
            Assert.NotNull(result);
            Assert.All(result, rule => Assert.True(rule.IsSuccess));
            Assert.All(result, rule => Assert.Empty(rule.ExceptionMessage));
        }

        [Fact]
        public async Task ExecuteRule_RuntimeError_ThrowsException()
        {
            var workflow = new Workflow {
                WorkflowName = "TestWorkflow",
                Rules = new[] {
                    new Rule {
                        RuleName = "ruleWithRuntimeError",
                        Expression = "input1.Country.ToLower() == \"india\""
                    }
                }
            };

            var re = new RulesEngine(new[] { workflow }, new ReSettings {
                EnableExceptionAsErrorMessage = false
            });

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new RuleTestClass { Country = null })
            };

            _ = await Assert.ThrowsAsync<RuleException>(async () => await re.ExecuteWorkflow("TestWorkflow", rp, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteRule_RuntimeError_IgnoreException_DoesNotReturnException()
        {
            var workflow = new Workflow {
                WorkflowName = "TestWorkflow",
                Rules = new[] {
                    new Rule {
                        RuleName = "ruleWithRuntimeError",
                        Expression = "input1.Country.ToLower() == \"india\""
                    }
                }
            };

            var re = new RulesEngine(new[] { workflow }, new ReSettings {
                IgnoreException = true
            });

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new RuleTestClass { Country = null })
            };

            var result = await re.ExecuteWorkflow("TestWorkflow", rp, CancellationToken.None);

            Assert.NotNull(result);
            Assert.All(result, rule => Assert.False(rule.IsSuccess));
            Assert.All(result, rule => Assert.Empty(rule.ExceptionMessage));
        }

        [Fact]
        public async Task RemoveWorkFlow_ShouldRemoveAllCompiledCache()
        {
            var workflow = new Workflow {
                WorkflowName = "Test",
                Rules = new Rule[]{
                    new Rule {
                        RuleName = "RuleWithLocalParam",
                        LocalParams = new List<LocalParam> {
                            new LocalParam {
                                Name = "lp1",
                                Expression = "true"
                            }
                        },
                        RuleExpressionType = RuleExpressionType.LambdaExpression,
                        Expression = "lp1 ==  true"
                    }
                }
            };

            var re = new RulesEngine();
            re.AddWorkflow(workflow);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", "hello")
            };

            var result1 = await re.ExecuteWorkflow("Test", rp, CancellationToken.None);
            Assert.True(result1.All(c => c.IsSuccess));

            re.RemoveWorkflow("Test");
            workflow.Rules.First().LocalParams.First().Expression = "false";
            re.AddWorkflow(workflow);

            var result2 = await re.ExecuteWorkflow("Test", rp, CancellationToken.None);
            Assert.True(result2.All(c => c.IsSuccess == false));
        }

        [Fact]
        public async Task ClearWorkFlow_ShouldRemoveAllCompiledCache()
        {
            var workflow = new Workflow {
                WorkflowName = "Test",
                Rules = new Rule[]{
                    new Rule {
                        RuleName = "RuleWithLocalParam",
                        LocalParams = new LocalParam[] {
                            new LocalParam {
                                Name = "lp1",
                                Expression = "true"
                            }
                        },
                        RuleExpressionType = RuleExpressionType.LambdaExpression,
                        Expression = "lp1 ==  true"
                    }
                }
            };

            var re = new RulesEngine();
            re.AddWorkflow(workflow);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", "hello")
            };

            var result1 = await re.ExecuteWorkflow("Test", rp, CancellationToken.None);
            Assert.True(result1.All(c => c.IsSuccess));

            re.ClearWorkflows();
            workflow.Rules.First().LocalParams.First().Expression = "false";

            re.AddWorkflow(workflow);
            var result2 = await re.ExecuteWorkflow("Test", rp, CancellationToken.None);
            Assert.True(result2.All(c => c.IsSuccess == false));
        }

        [Fact]
        public async Task ExecuteRule_WithNullInput_ShouldNotThrowException()
        {
            var workflow = new Workflow {
                WorkflowName = "Test",
                Rules = new Rule[]{
                    new Rule {
                        RuleName = "RuleWithLocalParam",

                        RuleExpressionType = RuleExpressionType.LambdaExpression,
                        Expression = "input1 == null || input1.hello.world = \"wow\""
                    }
                }
            };

            var re = new RulesEngine();
            re.AddWorkflow(workflow);

            var result1 = await re.ExecuteWorkflow("Test", new RuleParameter[] { new RuleParameter("input1", null) }, CancellationToken.None);
            Assert.True(result1.All(c => c.IsSuccess));

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new { hello = new { world = "wow" } })
            };

            var result2 = await re.ExecuteWorkflow("Test", rp, CancellationToken.None);
            Assert.True(result2.All(c => c.IsSuccess));
        }

        [Fact]
        public async Task ExecuteRule_SpecialCharInWorkflowName_RunsSuccessfully()
        {
            var workflow = new Workflow {
                WorkflowName = "Exámple",
                Rules = new Rule[]{
                    new Rule {
                        RuleName = "RuleWithLocalParam",

                        RuleExpressionType = RuleExpressionType.LambdaExpression,
                        Expression = "input1 == null || input1.hello.world = \"wow\""
                    }
                }
            };

            var workflowStr = "{\"WorkflowName\":\"Exámple\",\"WorkflowsToInject\":null,\"GlobalParams\":null,\"Rules\":[{\"RuleName\":\"RuleWithLocalParam\",\"Properties\":null,\"Operator\":null,\"ErrorMessage\":null,\"Enabled\":true,\"ErrorType\":\"Warning\",\"RuleExpressionType\":\"LambdaExpression\",\"WorkflowsToInject\":null,\"Rules\":null,\"LocalParams\":null,\"Expression\":\"input1 == null || input1.hello.world = \\\"wow\\\"\",\"Actions\":null,\"SuccessMessage\":null}]}";

            var re = new RulesEngine(new string[] { workflowStr }, null);

            var rp = new RuleParameter[]
            {
                new RuleParameter("input1", new { hello = new { world = "wow" } })
            };

            var result3 = await re.ExecuteWorkflow("Exámple", rp, CancellationToken.None);
            Assert.True(result3.All(c => c.IsSuccess));
        }

        [Fact]
        public void ContainsWorkFlowName_ShouldReturn()
        {
            const string ExistedWorkflowName = "ExistedWorkflowName";
            const string NotExistedWorkflowName = "NotExistedWorkflowName";

            var workflow = new Workflow {
                WorkflowName = ExistedWorkflowName,
                Rules = new Rule[]{
                    new Rule {
                        RuleName = "Rule",
                        RuleExpressionType = RuleExpressionType.LambdaExpression,
                        Expression = "1==1"
                    }
                }
            };

            var re = new RulesEngine();
            re.AddWorkflow(workflow);

            Assert.True(re.ContainsWorkflow(ExistedWorkflowName));
            Assert.False(re.ContainsWorkflow(NotExistedWorkflowName));
        }

        [Theory]
        [InlineData(typeof(RulesEngine), new[] { typeof(IBaseRulesEngine), typeof(IRulesEngine), typeof(IRulesEngineEx) })]
        public void Class_PublicMethods_ArePartOfInterface(Type classType, Type[] interfaceType)
        {
            var classMethods = classType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Count();
            var interfaceMethods = (from Type t in interfaceType select t.GetMethods().Count()).Sum();

            Assert.Equal(interfaceMethods, classMethods);
        }

        private RulesEngine CreateRulesEngine(Workflow workflow)
        {
            var json = JsonConvert.SerializeObject(workflow);
            return new RulesEngine(new string[] { json }, null);
        }

        private RulesEngine GetRulesEngine(string filename, ReSettings reSettings = null)
        {
            var data = GetFileContent(filename);

            var injectWorkflow = new Workflow {
                WorkflowName = "inputWorkflowReference",
                WorkflowsToInject = new List<string> { "inputWorkflow" }
            };

            var injectWorkflowStr = JsonConvert.SerializeObject(injectWorkflow);
            return new RulesEngine(new string[] { data, injectWorkflowStr }, reSettings);
        }

        private string GetFileContent(string filename)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory() as string, "TestData", filename);
            return File.ReadAllText(filePath);
        }

        private Workflow ParseAsWorkflow(string WorkflowsFileName)
        {
            string content = GetFileContent(WorkflowsFileName);
            return JsonConvert.DeserializeObject<Workflow>(content);
        }

        private dynamic GetInput1()
        {
            var converter = new ExpandoObjectConverter();
            var basicInfo = "{\"name\": \"Dishant\",\"email\": \"abc@xyz.com\",\"creditHistory\": \"good\",\"country\": \"canada\",\"loyaltyFactor\": 3,\"totalPurchasesToDate\": 10000}";
            return JsonConvert.DeserializeObject<ExpandoObject>(basicInfo, converter);
        }

        private dynamic GetInput2()
        {
            var converter = new ExpandoObjectConverter();
            var orderInfo = "{\"totalOrders\": 5,\"recurringItems\": 2}";
            return JsonConvert.DeserializeObject<ExpandoObject>(orderInfo, converter);
        }

        private dynamic GetInput3()
        {
            var converter = new ExpandoObjectConverter();
            var telemetryInfo = "{\"noOfVisitsPerMonth\": 10,\"percentageOfBuyingToVisit\": 15}";
            return JsonConvert.DeserializeObject<ExpandoObject>(telemetryInfo, converter);
        }
                
        [ExcludeFromCodeCoverage]
        public class TestInstanceUtils
        {
            public bool CheckExists(string str)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    return true;
                }

                return false;
            }
        }
    }
}