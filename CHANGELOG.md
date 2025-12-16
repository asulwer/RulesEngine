# CHANGELOG

All notable changes to this project will be documented in this file.

## [6.0.13]

## New Feature
* use the result of the previous rule in the next rules expression by @asulwer in https://github.com/asulwer/RulesEngine/pull/136

## What's Changed
* Bump Microsoft.EntityFrameworkCore and 3 others by @dependabot[bot] in https://github.com/asulwer/RulesEngine/pull/129
* solves issue https://github.com/asulwer/RulesEngine/issues/130 and all past topics on it by @asulwer in https://github.com/asulwer/RulesEngine/pull/131
* Fix AmbiguousMatchException  by @SJ-narbutas in https://github.com/asulwer/RulesEngine/pull/132
* Bump dotnet-reportgenerator-globaltool and xunit.runner.visualstudio by @dependabot[bot] in https://github.com/asulwer/RulesEngine/pull/134
* Bump dotnet-reportgenerator-globaltool and System.Linq.Dynamic.Core by @dependabot[bot] in https://github.com/asulwer/RulesEngine/pull/135

## New Contributors
* @SJ-narbutas made their first contribution in https://github.com/asulwer/RulesEngine/pull/132

**Full Changelog**: https://github.com/asulwer/RulesEngine/compare/v6.0.12...v6.0.13

## [6.0.12]

## What's Changed
* Bump xunit.runner.visualstudio from 3.1.0 to 3.1.1 by @dependabot in https://github.com/asulwer/RulesEngine/pull/125
* Bump BenchmarkDotNet and 5 others by @dependabot in https://github.com/asulwer/RulesEngine/pull/126
* Bump BenchmarkDotNet from 0.15.1 to 0.15.2 by @dependabot in https://github.com/asulwer/RulesEngine/pull/127
* Fixed [Issue 128](https://github.com/asulwer/RulesEngine/issues/128)

**Full Changelog**: https://github.com/asulwer/RulesEngine/compare/v6.0.11...v6.0.12

## [6.0.11]

## What's Changed
* Update xunit.runner.visualstudio to 3.1.0 by @dependabot in https://github.com/asulwer/RulesEngine/pull/122
* Update System.Linq.Dynamic.Core to 1.6.3 by @dependabot in https://github.com/asulwer/RulesEngine/pull/123
* updated nuget packages by @asulwer in https://github.com/asulwer/RulesEngine/pull/124


**Full Changelog**: https://github.com/asulwer/RulesEngine/compare/v6.0.10...v6.0.11

## [6.0.10]

## What's Changed
* Bump dotnet-reportgenerator-globaltool from 5.4.4 to 5.4.5 by @dependabot in #114
* Bump FastExpressionCompiler from 5.0.2 to 5.0.3 by @dependabot in #115
* Bump FastExpressionCompiler from 5.0.3 to 5.1.1 by @dependabot in #117
* Bump Microsoft.EntityFrameworkCore from 9.0.3 to 9.0.4 by @dependabot in #118
* Bump System.Text.Json from 9.0.3 to 9.0.4 by @dependabot in #119
* Fix duplication of inputs during EvaluateRule execution by @RenanCarlosPereira in #120
* Bump System.Linq.Dynamic.Core from 1.6.0.2 to 1.6.2 by @dependabot in #121

**Full Changelog**: https://github.com/asulwer/RulesEngine/compare/v6.0.9...v6.0.10

## [6.0.9]

## What's Changed
* Bump xunit.runner.visualstudio from 3.0.1 to 3.0.2 by @dependabot in https://github.com/asulwer/RulesEngine/pull/105
* Bump dotnet-reportgenerator-globaltool from 5.4.3 to 5.4.4 by @dependabot in https://github.com/asulwer/RulesEngine/pull/106
* Bump Microsoft.NET.Test.Sdk from 17.12.0 to 17.13.0 by @dependabot in https://github.com/asulwer/RulesEngine/pull/107
* Bump Microsoft.EntityFrameworkCore and Microsoft.EntityFrameworkCore.Sqlite by @dependabot in https://github.com/asulwer/RulesEngine/pull/108
* Bump System.Linq.Dynamic.Core from 1.6.0.1 to 1.6.0.2 by @dependabot in https://github.com/asulwer/RulesEngine/pull/110
* Bump Microsoft.EntityFrameworkCore from 9.0.2 to 9.0.3 by @dependabot in https://github.com/asulwer/RulesEngine/pull/111
* Exception handling in Actions by @asulwer in https://github.com/asulwer/RulesEngine/pull/112

**Full Changelog**: https://github.com/asulwer/RulesEngine/compare/v6.0.8...v6.0.9
 
## [6.0.8]

## What's Changed
* Bump dotnet-reportgenerator-globaltool from 5.4.1 to 5.4.3 by @dependabot in https://github.com/asulwer/RulesEngine/pull/98
* Bump System.Text.Json from 9.0.0 to 9.0.1 by @dependabot in https://github.com/asulwer/RulesEngine/pull/100
* Bump Microsoft.EntityFrameworkCore from 9.0.0 to 9.0.1 by @dependabot in https://github.com/asulwer/RulesEngine/pull/102
* Bump coverallsapp/github-action from 2.3.4 to 2.3.6 by @dependabot in https://github.com/asulwer/RulesEngine/pull/101
* manually updated nuget packages by @asulwer in https://github.com/asulwer/RulesEngine/pull/104

**Full Changelog**: https://github.com/asulwer/RulesEngine/compare/v6.0.7...v6.0.8

## [6.0.7]

- Updated dependencies to latest

## [6.0.6]

- Updated dependencies to latest
- Resolves the following issues [75](https://github.com/asulwer/RulesEngine/issues/75) & [90](https://github.com/asulwer/RulesEngine/issues/90)
  - [Deserializing](https://learn.microsoft.com/en-us/dotnet/api/system.text.json) Json (includes an empty array) to ExpandoObject
- targeting `netstandard2.0` & `netstandard2.1`
 
## [6.0.2] [6.0.4] [6.0.5]

- Updated dependencies to latest

## [6.0.1]

### Added
- **Preserve Original Object**: Introduced `OriginalValue` property in `RuleParameter` to preserve the original `Expando` without converting its type.
- **TryGetPropertyValue Method**: Implemented `TryGetPropertyValue` to safely access properties, returning a boolean indicating success and setting the out parameter to the property value or `null` if not found.

### Fixed
- **Data Integrity Issue**: Ensured both the original dynamic object and the typed version are accessible, maintaining data integrity throughout the workflow execution.

For more details, refer to the [GitHub issue](https://github.com/asulwer/RulesEngine/issues/53).

## [6.0.0]

- **GitHub Actions**
  - Updated `codeql` task.
  - Updated `coverallsapp/github-action` from `v2.2.1` to `v2.3.0`.

### Changed
- Switched JSON serialization library from `Newtonsoft.Json` to `System.Text.Json`.

### Added
- Added more demo examples in the demo project.
- Action context now has access to cancellation tokens.
- New overloads for `ExecuteAllRulesAsync` to support cancellation tokens:
  ```csharp
  ValueTask<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, CancellationToken cancellationToken, params object[] inputs);
  ValueTask<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, RuleParameter[] ruleParams, CancellationToken cancellationToken);
  ```
- New overload for `ExecuteActionWorkflowAsync` to support cancellation tokens:
  ```csharp
  ValueTask<ActionRuleResult> ExecuteActionWorkflowAsync(string workflowName, string ruleName, RuleParameter[] ruleParameters, CancellationToken cancellationToken);
  ```
- Added a `GetCancellationToken` method in `ActionContext` to get the cancellation token:
  ```csharp
  public CancellationToken GetCancellationToken();
  ```

### Fixed
- All issues in the master fork have been resolved, usually with a demo app supporting the solution.

### Target Framework
- Only targeting `netstandard2.1`.

## [5.0.3]
- Updated dependencies to latest
- Fixed RulesEngine throwing exception when type name is same as input name
- Added config to disable FastCompile for expressions
- Added RuleParameter.Create method for better handling on types when value is null

## [5.0.2]
- Fixed Scoped Params returning incorrect results in some corner case scenarios

## [5.0.1]
- Added option to disable automatic type registry for input parameters in reSettings
- Added option to make expression case sensitive in reSettings

## [5.0.0]
- Fixed security bug related to System.Dynamic.Linq.Core

### Breaking Changes
- As a part of security bug fix, method call for only registered types via reSettings will be allowed. This only impacts strongly typed inputs and nested types

## [4.0.0]
- RulesEngine is now available in both dotnet 6 and netstandard 2.0
- Dependency on ILogger, MemoryCache have been removed 
- Obsolete Properties and Methods have been removed
- Fixed name of RuleParameter is ignored if the type is recognized (by @peeveen)
### Breaking Changes
- ILogger has been removed from RulesEngine and all its constructors
```diff
- RulesEngine(string[] jsonConfig, ILogger logger = null, ReSettings reSettings = null)
+ RulesEngine(string[] jsonConfig, ReSettings reSettings = null)

- RulesEngine(Workflow[] Workflows, ILogger logger = null, ReSettings reSettings = null)
+ RulesEngine(Workflow[] Workflows, ReSettings reSettings = null)

- RulesEngine(ILogger logger = null, ReSettings reSettings = null)
+ RulesEngine(ReSettings reSettings = null)
```
- Obsolete methods and properties have been removed, from the follow models:-
	- RuleResultTree
		- `ToResultTreeMessages()` has been removed from `RuleResultTree` model
		- `GetMessages()` has been removed from `RuleResultTree` model
		- `RuleEvaluatedParams` has been removed from `RuleResultTree` model, Please use `Inputs` instead

	- Workflow
		- `WorkflowRulesToInject` has been removed, Please use `WorkflowsToInject` instead
		- `ErrorType` has been removed from `Rule`

	- Resettings
		- `EnableLocalParams` has been removed from `ReSettings`, Please use `EnableScopedParams` instead
	

## [3.5.0]
- `EvaluateRule` action now support custom inputs and filtered inputs
- Added `ContainsWorkflow` method in RulesEngine (by @okolobaxa)
- Fixed minor bugs (#258 & #259)

## [3.4.0]
- Made RulesEngine Strong Name and Authenticode signed
- Renamed few models to streamline names (by @alexrich)
	- `WorkflowRules` is renamed to `Workflow`
	- `WorkflowRulesToInject` is renamed to `WorkflowsToInject`
	- `RuleAction` is renamed to `RuleActions`
	
	**Note**: The old models are still supported but will be removed with version 4.0.0


## [3.3.0]
- Added support for actions in nested rules
- Improved serialization support for System.Text.Json for workflow model
  
Breaking Change:
  - Type of Action has been changed from `Dictionary<ActionTriggerType, ActionInfo>` to `RuleActions`
    - No impact if you are serializing workflow from json
    - For workflow objects created in code, refer - [link](https://github.com/microsoft/RulesEngine/pull/182/files#diff-a5093dda2dcc1e4958ce3533edb607bb61406e1f0a9071eca4e317bdd987c0d3)

## [3.2.0]
- Added AddOrUpdateWorkflow method to update workflows atomically (by @AshishPrasad)
- Updated dependencies to latest

Breaking Change:
  - `AddWorkflow` now throws exception if you try to add a workflow which already exists.
  Use `AddOrUpdateWorkflow` to update existing workflow

## [3.1.0]
- Added globalParams feature which can be applied to all rules
- Enabled localParams support for nested Rules
- Made certain fields in Rule model optional allowing users to define workflow with minimal fields
- Added option to disable Rule in workflow json
- Added `GetAllRegisteredWorkflow` to RulesEngine to return all registered workflows
- Runtime errors for expressions will now be logged as errorMessage instead of throwing Exceptions by default
- Fixed RuleParameter passed as null

## [3.0.2]
- Fixed LocalParams cache not getting cleaned up when RemoveWorkflows and ClearWorkflows are called

## [3.0.1]
- Moved ActionResult and ActionRuleResult under RulesEngine.Models namespace


## [3.0.0]
### Major Enhancements
- Added support for Actions. More details on [actions wiki](https://github.com/microsoft/RulesEngine/wiki/Actions)
- Major performance improvement
	- 25% improvement from previous version
	- Upto 35% improvement by disabling optional features
- RulesEngine now virtually supports unlimited inputs (Previous limitation was 16 inputs)
- RuleExpressionParser is now available to use expression evaluation outside RulesEngine

### Breaking Changes
- `ExecuteRule` method has been renamed to `ExecuteAllRulesAsync`
- `Input` field in RuleResultTree has been changed to `Inputs` which returns all the the inputs as Dictionary of name and value pair

## [2.1.5] - 02-11-2020
- Added `Properties` field to Rule to allow custom fields to Rule

## [2.1.4] - 15-10-2020
- Added exception data properties to identify RuleName.

## [2.1.3] - 12-10-2020
- Optional parameter for rethrow exception on failure of expression compilation.

## [2.1.2] - 02-10-2020
- Fixed binary expression requirement. Now any expression will work as long as it evalutes to boolean.

## [2.1.1] - 01-09-2020
- Fixed exception thrown when errormessage field is null
- Added better messaging when identifier is not found in expression
- Fixed other minor bugs

## [2.1.0] - 18-05-2020
- Adding local param support to make expression authroing more intuitive.

## [2.0.0] - 18-05-2020
### Changed
- Interface simplified by removing redundant parameters in the IRulesEngine.
- Custom Logger replaced with Microsoft Logger.

## [1.0.2] - 16-01-2020
### Added
- Cache system added so that rules compilation is stored and thus made more efficient.

### Fix
- Concurrency issue which arose by dictionary was resolved.

## [1.0.1] - 24-09-2019
### Added
- Exceptions handling scenario in the case a rule execution throws an exception 

## [1.0.0] - 20-08-2019

### Added
- The first version of the NuGet
