// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using RulesEngine.Models;
using RulesEngine.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RulesEngine.UnitTest
{
    [ExcludeFromCodeCoverage]
    public class EmptyArrayTest
    {
        [Fact]
        private async Task EmptyArray_ReturnsExepectedResults()
        {
            Workflow[] workflow = [
                new() {
                    WorkflowName = "Workflow",
                    Rules = [
                        new() {
                            RuleName = "equal string",
                            Expression = "not things.Any(a == \"widget\")"
                        },
                        new() {
                            RuleName = "equal integer",
                            Expression = "not things.Any(a == 3)"
                        },
                        new() {
                            RuleName = "not equal integer",
                            Expression = "not things.Any(a != 3)"
                        },
                        new() {
                            RuleName = "greater than integer",
                            Expression = "not things.Any(a > 3)"
                        },
                        new() {
                            RuleName = "less than integer",
                            Expression = "not things.Any(a < 3)"
                        },
                        new() {
                            RuleName = "greater than equal to integer",
                            Expression = "not things.Any(a >= 3)"
                        },
                        new() {
                            RuleName = "less than equal to integer",
                            Expression = "not things.Any(a <= 3)"
                        }
                    ]
                }
            ];
            var rulesEngine = new RulesEngine(workflow, new() {
                IsExpressionCaseSensitive = false,
                CustomTypes = [
                    typeof(IEnumerable)
                ]
            });

            var options = new JsonSerializerOptions {
                Converters = { new ObjectAsPrimitiveConverter() },
                WriteIndented = true,
            };
            string payload = @"{""things"":[]}";

            var target = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(payload, options);

            CancellationTokenSource cancellationTokenSource = new();
            List<RuleResultTree> results = await rulesEngine.ExecuteAllRulesAsync("Workflow", cancellationTokenSource.Token, target);

            Assert.True(results.TrueForAll(r => r.IsSuccess));

            foreach (var result in results)
            {
                Assert.Equal(result.ExceptionMessage, string.Empty);
                Assert.True(result.IsSuccess);
            }


        }

    }
}
