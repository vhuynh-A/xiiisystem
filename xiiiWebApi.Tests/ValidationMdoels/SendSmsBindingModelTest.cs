using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xiiiWebApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace xiiiWebApi.Tests.ValidationMdoels
{
    [TestClass]
    public class SendSmsBindingModelTest
    {
        [TestMethod]
        public void SendSmsBindingModel_WhenValid_ShouldBeValid()
        {
            // Arrange
            var model = new SendSmsBindingModel { To = Guid.NewGuid().ToString(), Message = Guid.NewGuid().ToString() };
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void SendSmsBindingModel_WhenMissingOrEmptyTo_ShouldBeInvalid()
        {
            // Arrange
            var modelNull = new SendSmsBindingModel { Message = Guid.NewGuid().ToString() };
            var contextNull = new ValidationContext(modelNull, serviceProvider: null, items: null);
            var resultsNull = new List<ValidationResult>();

            var modelEmpty = new SendSmsBindingModel { Message = Guid.NewGuid().ToString(), To = "" };
            var contextEmpty = new ValidationContext(modelEmpty, serviceProvider: null, items: null);
            var resultsEmpty = new List<ValidationResult>();

            // Act
            var isValidNull = Validator.TryValidateObject(modelNull, contextNull, resultsNull, true);
            var isValidEmpty = Validator.TryValidateObject(modelEmpty, contextEmpty, resultsEmpty, true);

            // Assert
            Assert.IsFalse(isValidNull);
            Assert.IsFalse(isValidEmpty);
        }

        [TestMethod]
        public void SendSmsBindingModel_WhenMissingOrEmptyMessage_ShouldBeInvalid()
        {
            // Arrange
            var modelNull = new SendSmsBindingModel { To = Guid.NewGuid().ToString() };
            var contextNull = new ValidationContext(modelNull, serviceProvider: null, items: null);
            var resultsNull = new List<ValidationResult>();

            var modelEmpty = new SendSmsBindingModel { To = Guid.NewGuid().ToString(), Message = "" };
            var contextEmpty = new ValidationContext(modelEmpty, serviceProvider: null, items: null);
            var resultsEmpty = new List<ValidationResult>();

            // Act
            var isValidNull = Validator.TryValidateObject(modelNull, contextNull, resultsNull, true);
            var isValidEmpty = Validator.TryValidateObject(modelEmpty, contextEmpty, resultsEmpty, true);

            // Assert
            Assert.IsFalse(isValidNull);
            Assert.IsFalse(isValidEmpty);
        }
    }
}
