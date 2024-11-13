// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// https://github.com/asulwer/RulesEngine/issues/75
// https://stackoverflow.com/questions/65972825/c-sharp-deserializing-nested-json-to-nested-dictionarystring-object

using RulesEngine.HelperFunctions;
using RulesEngine.Models;
using RulesEngine.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;

namespace DemoApp
{
    public class JsonSerializerConverter : IDemo
    {
        public async Task Run(CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Running {nameof(JsonSerializerConverter)}....");

            string payload = "{\"prop\":\"someString\",\"someInt\":3,\"nest\":{\"code\":\"bar\",\"foo\":true},\"emptyArray\":[],\"populatedArray\":[{\"a\":2,\"subArray\":[{\"c\":4}]}]}";

            var options = new JsonSerializerOptions {
                Converters = { new ObjectAsPrimitiveConverter(floatFormat: FloatFormat.Double, unknownNumberFormat: UnknownNumberFormat.Error, objectFormat: ObjectFormat.Expando) },
                WriteIndented = true,
            };
            var target = JsonSerializer.Deserialize<ExpandoObject>(payload, options)!;

            Workflow[] workflow = [
                new() {
                WorkflowName = "Workflow",
                Rules = [
                    new() {
                        RuleName = "someInt check",
                        Expression = "someInt > 1",
                        RuleExpressionType = RuleExpressionType.LambdaExpression
                    },
                    new() {
                        RuleName = "empty array",
                        Expression = "not emptyArray.Any(a == 'a')",
                        RuleExpressionType = RuleExpressionType.LambdaExpression
                    },
                    new() {
                        RuleName = "populatedArray with subArray not match",
                        Expression = "populatedArray.Any(subArray.Any(c == 4))",
                        RuleExpressionType = RuleExpressionType.LambdaExpression
                    },
                    new() {
                        RuleName = "check prop",
                        Expression = "prop = \"someString\"",
                        RuleExpressionType = RuleExpressionType.LambdaExpression
                    },
                    new() {
                        RuleName = "check nested code",
                        Expression = "nest.code eq \"bar\" and nest.foo == true",
                        RuleExpressionType = RuleExpressionType.LambdaExpression
                    }
                ]
            }
            ];

            var rulesEngine = new RulesEngine.RulesEngine(workflow);

            List<RuleResultTree> results = await rulesEngine.ExecuteAllRulesAsync("Workflow", cancellationToken, target);

            //Different ways to show test results:
            var outcome = results.TrueForAll(r => r.IsSuccess);

            results.OnSuccess(eventName => {
                Console.WriteLine($"Result '{eventName}' is as expected.");
                outcome = true;
            });

            results.OnFail(() => {
                outcome = false;
            });

            Console.WriteLine($"Test outcome: {outcome}.");
        }
    }
}
