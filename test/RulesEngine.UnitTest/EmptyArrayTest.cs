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
                            RuleName = "empty array",
                            Expression = "not things.Any(a == 3)",
                            RuleExpressionType = RuleExpressionType.LambdaExpression
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

            Assert.Single(results);

            var result = results[0];

            Assert.Equal(result.ExceptionMessage, string.Empty);
            Assert.True(result.IsSuccess);

        }

    }
}
