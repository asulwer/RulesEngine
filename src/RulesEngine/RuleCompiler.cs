﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using RulesEngine.Exceptions;
using RulesEngine.ExpressionBuilders;
using RulesEngine.Extensions;
using RulesEngine.HelperFunctions;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RulesEngine
{
    /// <summary>
    /// Rule compilers
    /// </summary>
    internal class RuleCompiler
    {
        /// <summary>
        /// The nested operators
        /// </summary>
        private readonly ExpressionType[] nestedOperators = new ExpressionType[] { ExpressionType.And, ExpressionType.AndAlso, ExpressionType.Or, ExpressionType.OrElse };

        /// <summary>
        /// The expression builder factory
        /// </summary>
        private readonly RuleExpressionBuilderFactory _expressionBuilderFactory;
        private readonly ReSettings _reSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleCompiler"/> class.
        /// </summary>
        /// <param name="expressionBuilderFactory">The expression builder factory.</param>
        /// <exception cref="ArgumentNullException">expressionBuilderFactory</exception>
        internal RuleCompiler(RuleExpressionBuilderFactory expressionBuilderFactory, ReSettings reSettings)
        {
            _expressionBuilderFactory = expressionBuilderFactory ?? throw new ArgumentNullException($"{nameof(expressionBuilderFactory)} can't be null.");
            _reSettings = reSettings;
        }

        /// <summary>
        /// Compiles the rule
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rule"></param>
        /// <param name="input"></param>
        /// <param name="ruleParam"></param>
        /// <returns>Compiled func delegate</returns>
        internal RuleFunc<RuleResultTree> CompileRule(Rule rule, RuleExpressionType ruleExpressionType, RuleParameter[] ruleParams, Lazy<RuleExpressionParameter[]> globalParams)
        {
            if (rule == null)
            {
                var ex =  new ArgumentNullException(nameof(rule));
                throw ex;
            }
            try
            {
                var globalParamExp = globalParams.Value;
                var extendedRuleParams = ruleParams.Concat(globalParamExp.Select(c => new RuleParameter(c.ParameterExpression.Name,c.ParameterExpression.Type)))
                                                   .DistinctBy(x => x.Name).ToArray();
                var ruleExpression = GetDelegateForRule(rule, extendedRuleParams);
                

                return GetWrappedRuleFunc(rule,ruleExpression,ruleParams,globalParamExp);
            }
            catch (Exception ex)
            {
                var message = $"Error while compiling rule `{rule.RuleName}`: {ex.Message}";
                return Helpers.ToRuleExceptionResult(_reSettings, rule, new RuleException(message, ex));
            }
        }



        /// <summary>
        /// Gets the expression for rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="typeParameterExpressions">The type parameter expressions.</param>
        /// <param name="ruleInputExp">The rule input exp.</param>
        /// <returns></returns>
        private RuleFunc<RuleResultTree> GetDelegateForRule(Rule rule, RuleParameter[] ruleParams)
        {
            var scopedParamList = GetRuleExpressionParameters(rule.RuleExpressionType, rule?.LocalParams, ruleParams);

            var extendedRuleParams = ruleParams.Concat(scopedParamList.Select(c => new RuleParameter(c.ParameterExpression.Name, c.ParameterExpression.Type)))
                                               .ToArray();

            RuleFunc<RuleResultTree> ruleFn;
            
            if (Enum.TryParse(rule.Operator, out ExpressionType nestedOperator) && nestedOperators.Contains(nestedOperator) &&
                rule.Rules != null && rule.Rules.Any())
            {
                ruleFn = BuildNestedRuleFunc(rule, nestedOperator, extendedRuleParams);
            }
            else
            {
                ruleFn = BuildRuleFunc(rule, extendedRuleParams);
            }

            return GetWrappedRuleFunc(rule, ruleFn, ruleParams, scopedParamList);
        }

        internal RuleExpressionParameter[] GetRuleExpressionParameters(RuleExpressionType ruleExpressionType,IEnumerable<ScopedParam> localParams, RuleParameter[] ruleParams)
        {
            if(!_reSettings.EnableScopedParams)
            {
                return new RuleExpressionParameter[] { };
            }
            var ruleExpParams = new List<RuleExpressionParameter>();

            if (localParams?.Any() == true)
            {
                var parameters = ruleParams.Select(c => c.ParameterExpression).ToList();
                var parameterNames = new HashSet<string>(parameters.Select(p => p.Name));

                var expressionBuilder = GetExpressionBuilder(ruleExpressionType);
                var parametersArray = parameters.ToArray();

                foreach (var lp in localParams)
                {
                    try
                    {
                        if (!parameterNames.Add(lp.Name)) continue;

                        var lpExpression = expressionBuilder.Parse(lp.Expression, parametersArray, null);

                        var ruleExpParam = new RuleExpressionParameter {
                            ParameterExpression = Expression.Parameter(lpExpression.Type, lp.Name),
                            ValueExpression = lpExpression
                        };
                        parameters.Add(ruleExpParam.ParameterExpression);
                        ruleExpParams.Add(ruleExpParam);

                        parametersArray = parameters.ToArray();
                    }
                    catch(Exception ex)
                    {
                        var message = $"{ex.Message}, in ScopedParam: {lp.Name}";
                        throw new RuleException(message);
                    }
                }
            }
            return ruleExpParams.ToArray();

        }

        /// <summary>
        /// Builds the expression.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="typeParameterExpressions">The type parameter expressions.</param>
        /// <param name="ruleInputExp">The rule input exp.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private RuleFunc<RuleResultTree> BuildRuleFunc(Rule rule, RuleParameter[] ruleParams)
        {
            var ruleExpressionBuilder = GetExpressionBuilder(rule.RuleExpressionType);

            var ruleFunc = ruleExpressionBuilder.BuildDelegateForRule(rule, ruleParams);

            return ruleFunc;
        }

        /// <summary>
        /// Builds the nested expression.
        /// </summary>
        /// <param name="parentRule">The parent rule.</param>
        /// <param name="childRules">The child rules.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="typeParameterExpressions">The type parameter expressions.</param>
        /// <param name="ruleInputExp">The rule input exp.</param>
        /// <returns>Expression of func delegate</returns>
        /// <exception cref="InvalidCastException"></exception>
        private RuleFunc<RuleResultTree> BuildNestedRuleFunc(Rule parentRule, ExpressionType operation, RuleParameter[] ruleParams)
        {
            var ruleFuncList = new List<RuleFunc<RuleResultTree>>();
            foreach (var r in parentRule.Rules.Where(c => c.Enabled))
            {
                ruleFuncList.Add(GetDelegateForRule(r, ruleParams));
            }

            return (paramArray) => {
                var (isSuccess, resultList) = ApplyOperation(paramArray, ruleFuncList, operation);
                bool isSuccessFn(object[] p) => isSuccess;
                var result = Helpers.ToResultTree(_reSettings, parentRule, resultList, isSuccessFn);
                return result(paramArray);
            };
        }


        private (bool isSuccess ,IEnumerable<RuleResultTree> result) ApplyOperation(RuleParameter[] paramArray,IEnumerable<RuleFunc<RuleResultTree>> ruleFuncList, ExpressionType operation)
        {
            if (ruleFuncList?.Any() != true)
            {
                return (false,new List<RuleResultTree>());
            }

            var resultList = new List<RuleResultTree>();
            var isSuccess = false;

            if(operation == ExpressionType.And || operation == ExpressionType.AndAlso)
            {
                isSuccess = true;
            }

            foreach(var ruleFunc in ruleFuncList)
            {
                var ruleResult = ruleFunc(paramArray);
                resultList.Add(ruleResult);
                switch (operation)
                {
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        isSuccess = isSuccess && ruleResult.IsSuccess;
                        if(_reSettings.NestedRuleExecutionMode ==  NestedRuleExecutionMode.Performance && isSuccess == false)
                        {
                            return (isSuccess, resultList);
                        }
                        break;

                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        isSuccess = isSuccess || ruleResult.IsSuccess;
                        if (_reSettings.NestedRuleExecutionMode == NestedRuleExecutionMode.Performance && isSuccess == true)
                        {
                            return (isSuccess, resultList);
                        }
                        break;
                }
                
            }
            return (isSuccess, resultList);
        }

        internal Func<object[],Dictionary<string,object>> CompileScopedParams(RuleExpressionType ruleExpressionType, RuleParameter[] ruleParameters,RuleExpressionParameter[] ruleExpParams)
        {
            return GetExpressionBuilder(ruleExpressionType).CompileScopedParams(ruleParameters, ruleExpParams);

        }

        private RuleFunc<RuleResultTree> GetWrappedRuleFunc(Rule rule, RuleFunc<RuleResultTree> ruleFunc,RuleParameter[] ruleParameters,RuleExpressionParameter[] ruleExpParams)
        {
            if(ruleExpParams.Length == 0)
            {
                return ruleFunc;
            }
            var paramDelegate = CompileScopedParams(rule.RuleExpressionType,ruleParameters, ruleExpParams);

            return (ruleParams) => {
                var inputs = ruleParams.Select(c => c.Value).ToArray();
                IEnumerable<RuleParameter> scopedParams;
                try
                {
                    var scopedParamsDict = paramDelegate(inputs);
                    scopedParams = scopedParamsDict.Select(c => new RuleParameter(c.Key, c.Value));
                }
                catch(Exception ex)
                {
                    var message = $"Error while executing scoped params for rule `{rule.RuleName}` - {ex}";
                    var resultFn = Helpers.ToRuleExceptionResult(_reSettings, rule, new RuleException(message, ex));
                    return resultFn(ruleParams);
                }
               
                var extendedInputs = ruleParams.Concat(scopedParams).DistinctBy(x => x.Name);
                var result = ruleFunc(extendedInputs.ToArray());
                return result;
            };
        }

        private RuleExpressionBuilderBase GetExpressionBuilder(RuleExpressionType expressionType)
        {
            return _expressionBuilderFactory.RuleGetExpressionBuilder(expressionType);
        }
    }
}
