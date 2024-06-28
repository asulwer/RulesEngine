﻿// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RulesEngineBenchmark
{
    [MemoryDiagnoser]
    public class REBenchmark
    {
        private readonly RulesEngine.RulesEngine rulesEngine;
        private readonly RuleParameter[] ruleInputs;
        private readonly List<Workflow> workflow;

        private class ListItem
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }

        public REBenchmark()
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "NestedInputDemo.json", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                throw new Exception("Rules not found.");
            
            var fileData = File.ReadAllText(files[0]);
            workflow = JsonConvert.DeserializeObject<List<Workflow>>(fileData);

            rulesEngine = new RulesEngine.RulesEngine(workflow.ToArray(), new ReSettings {
                EnableFormattedErrorMessage = false,
                EnableScopedParams = false
            });

            ruleInputs = new RuleParameter[] {
                new RuleParameter("input1", new {
                    SimpleProp = "simpleProp",
                    NestedProp = new {
                        SimpleProp = "nestedSimpleProp",
                        ListProp = new List<ListItem>
                        {
                            new ListItem
                            {
                                Id = 1,
                                Value = "first"
                            },
                            new ListItem
                            {
                                Id = 2,
                                Value = "second"
                            }
                        }
                    }
                })
            };
        }

        [Params(1000, 10000)]
        public int N;

        [Benchmark]
        public void RuleExecutionDefault()
        {
            _ = Task.Run(async () => {
                await foreach (var async_rrt in rulesEngine.ExecuteAllWorkflows(ruleInputs, CancellationToken.None))
                {
                    _ = async_rrt;
                }
            });
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            _ = BenchmarkRunner.Run<REBenchmark>();
        }
    }
}