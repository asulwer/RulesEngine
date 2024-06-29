// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace DemoApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            DateTime start = DateTime.Now;

            using (var cts = new CancellationTokenSource())
            {
                await new CancellationToken.Basic().Run(cts.Token);
                await new CancellationToken.BasicWithCustomTypes().Run(cts.Token);
                await new CancellationToken.JSON().Run(cts.Token);
                await new CancellationToken.NestedInput().Run(cts.Token);
                await new CancellationToken.EF().Run(cts.Token);
                await new CancellationToken.UseFastExpressionCompiler().Run(cts.Token);
                await new CancellationToken.MultipleWorkflows().Run(cts.Token);
                await new CancellationToken.CustomParameterName().Run(cts.Token);
            }

            new BasicDemo().Run();
            new EFDemo().Run();
            new JSONDemo().Run();
            new NestedInputDemo().Run();
            
            DateTime end = DateTime.Now;

            Console.WriteLine($"running time: {end-start}");
        }
    }
}
