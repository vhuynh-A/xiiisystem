using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Http.Results;
using xiiiCommonModels;
using xiiiWebApi.Controllers;
using xiiiWebApi.Models;
using xiiiWebApi.Tests.Dal;

namespace xiiiWebApi.Tests.Controllers
{
    [TestClass]
    public class SmsContollerTest
    {
        [TestMethod]
        public void SmsController_EmptyPostMessage_ShouldReturnBadRequest()
        {
            // Arrange
            var repository = new MockSmsRepository();
            var controller = new SmsController(repository);

            // Act
            var actionResult = controller.Post(null);

            // Assert
            Assert.IsTrue(actionResult is BadRequestErrorMessageResult);
        }

        [TestMethod]
        public void SmsController_ValidPostMessage_ShouldReturnCreated()
        {
            // Arrange
            var repository = new MockSmsRepository();
            var controller = new SmsController(repository);
            var model = new SendSmsBindingModel { To = Guid.NewGuid().ToString(), Message = Guid.NewGuid().ToString() };

            // Act
            var actionResult = controller.Post(model);

            // Assert
            Assert.IsTrue(actionResult is CreatedNegotiatedContentResult<SmsMessage>);
        }

        [TestMethod]
        public void SmsController_ValidPostMessage_ShouldReturnCompleteResponse()
        {
            // Arrange
            var repository = new MockSmsRepository();
            var controller = new SmsController(repository);
            var model = new SendSmsBindingModel { To = Guid.NewGuid().ToString(), Message = Guid.NewGuid().ToString() };

            // Act
            var actionResult = controller.Post(model);
            var result = (actionResult as CreatedNegotiatedContentResult<SmsMessage>).Content;

            // Assert
            Assert.IsTrue(result.Created > DateTime.MinValue, "created too late");
            Assert.IsTrue(Guid.Empty != result.Guid, "no guid generated");
        }

        [TestMethod]
        public void SmsController_ValidPostMessage_ShouldSaveToTableAndQueue()
        {
            // Arrange
            var repository = new MockSmsRepository();
            var controller = new SmsController(repository);
            var model = new SendSmsBindingModel { To = Guid.NewGuid().ToString(), Message = Guid.NewGuid().ToString() };

            // Act
            var actionResult = controller.Post(model);

            // Assert
            Assert.AreEqual(1, repository.QueueLength, "failed to save to queue");
            Assert.AreEqual(1, repository.TableSize, "failed to save to table");
        }

        [TestMethod]
        public void SmsController_IfRecordExists_ShouldReturnMessage()
        {
            // Arrange
            var repository = new MockSmsRepository();
            var guid = Guid.NewGuid();
            var to = Guid.NewGuid().ToString();
            var model = new SmsMessage
            {
                Created = DateTime.UtcNow,
                From = Guid.NewGuid().ToString(),
                Guid = guid,
                Message = Guid.NewGuid().ToString(),
                To = to
            };
            repository.Insert(model);
            var controller = new SmsController(repository);

            // Act
            var result1 = controller.Get(guid);
            var result2 = controller.Get(to, guid);

            // Assert
            Assert.IsTrue(result1 is OkNegotiatedContentResult<SmsMessage>);
            Assert.IsTrue(result2 is OkNegotiatedContentResult<SmsMessage>);

            var content1 = (result1 as OkNegotiatedContentResult<SmsMessage>).Content;
            var content2 = (result2 as OkNegotiatedContentResult<SmsMessage>).Content;

            Assert.IsTrue(content1.Guid == guid);
            Assert.IsTrue(content2.Guid == guid);
        }

        [TestMethod]
        public void SmsController_IfRecordDoesNotExists_ShouldReturnNotFound()
        {
            // Arrange
            var repository = new MockSmsRepository();
            var model = new SmsMessage
            {
                Created = DateTime.UtcNow,
                From = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid(),
                Message = Guid.NewGuid().ToString(),
                To = Guid.NewGuid().ToString()
            };
            repository.Insert(model);
            var controller = new SmsController(repository);

            // Act
            var result1 = controller.Get(Guid.NewGuid());
            var result2 = controller.Get(Guid.NewGuid().ToString(), Guid.NewGuid());

            // Assert
            Assert.IsTrue(result1 is NotFoundResult);
            Assert.IsTrue(result2 is NotFoundResult);
        }
    }
}
