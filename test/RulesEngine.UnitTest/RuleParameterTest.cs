// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using RulesEngine.Models;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;

namespace RulesEngine.Tests
{
    public class RuleParameterTests
    {
        [Fact]
        public void TryGetPropertyValue_WithNullPropertyName_ReturnsFalseAndNull()
        {
            // Arrange
            dynamic input = new ExpandoObject();
            input.count = 1;
            var ruleParameter = new RuleParameter("data", input);

            // Act
            var result = ruleParameter.TryGetPropertyValue<int>(null, out var value);

            // Assert
            Assert.False(result);
            Assert.Equal(default, value);
        }

        [Fact]
        public void TryGetPropertyValue_WithNonExistentPropertyName_ReturnsFalseAndNull()
        {
            // Arrange
            dynamic input = new ExpandoObject();
            input.count = 1;
            var ruleParameter = new RuleParameter("data", input);

            // Act
            var result = ruleParameter.TryGetPropertyValue<int?>("nonExistentProperty", out var value);

            // Assert
            Assert.False(result);
            Assert.Equal(default, value);
        }

        [Fact]
        public void TryGetPropertyValue_WithValidPropertyName_ReturnsTrueAndCorrectValue()
        {
            // Arrange
            dynamic input = new ExpandoObject();
            input.count = 1;
            var ruleParameter = new RuleParameter("data", input);

            // Act
            var result = ruleParameter.TryGetPropertyValue<int>("count", out var value);

            // Assert
            Assert.True(result);
            Assert.Equal(input.count, value);
        }

        [Fact]
        public void Constructor_SetsOriginalValueCorrectly_WithExpandoObject()
        {
            // Arrange
            dynamic input = new ExpandoObject();
            input.count = 1;

            // Act
            var ruleParameter = new RuleParameter("data", input);

            // Assert
            Assert.Equal(input, ruleParameter.OriginalValue);
        }

        [Fact]
        public void Constructor_SetsOriginalValueCorrectly_WithPrimitiveType()
        {
            // Arrange
            int input = 5;

            // Act
            var ruleParameter = new RuleParameter("data", input);

            // Assert
            Assert.Equal(input, ruleParameter.OriginalValue);
        }

        [Fact]
        public void Constructor_SetsOriginalValueCorrectly_WithCustomObject()
        {
            // Arrange
            var input = new { Name = "Test", Value = 10 };

            // Act
            var ruleParameter = new RuleParameter("data", input);

            // Assert
            Assert.Equal(input, ruleParameter.OriginalValue);
        }

        [Fact]
        public void Constructor_SetsOriginalValueCorrectly_WithExpandoObject_WithItemKey()
        {
            // Arrange
            var input = new ExpandoObject();
            ((IDictionary<string, object>)input)["Item"] = "hello";
            ((IDictionary<string, object>)input)["L"] = 1000;
            ((IDictionary<string, object>)input)["W"] = 500;

            // Act
            var ruleParameter = new RuleParameter("data", input);

            // Assert
            Assert.Equal(input, ruleParameter.OriginalValue);
        }
    }
}