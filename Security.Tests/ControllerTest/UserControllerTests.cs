using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Security.API.Controllers;
using Security.DataServices.interfaces;
using Security.Shared;
using Security.Shared.Dtos;
using Security.Shared.Enums;
using Security.Shared.Requests;
using Security.Tests.Extensions;
using Security.Tests.ServicesTest;

namespace Security.Tests.ControllerTest
{

    [TestClass]
    public class UserControllerTests : UnitTestBase
    {
        protected override void AddDependencies(ServiceCollection services)
        {
            // Controller under test
            services.AddServiceUnderTest<UserController, UserController>();

            // Mocked dependencies
            services.AddMocked<IUserService>();
        }


        #region Create User

        [TestMethod]
        public async Task Create_Success()
        {
            // Arrange
            var request = new UserRequest
            {
                Email = "test@domain.com",
                UserName = "testuser",
                FullName = "Test User",
                MobileNumber = "123",
                Language = "en",
                Culture = "en-US",
                Password = "Pass123!"
            };

            var expectedResponse = new CreateUserDto()
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
            };

            var serviceMock = ServiceProvider.GetService<IUserService>() as IMocked<IUserService>;
            serviceMock!.Mock
                .Setup(s => s.Create(It.IsAny<UserRequest>()))
                .ReturnsAsync(expectedResponse);

            var controller = ServiceProvider.GetService<UserController>();

            // Act
            var result = await controller!.Create(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            Assert.AreEqual(200, obj.StatusCode);
            var dataResponse = obj.Value as DataResponse<CreateUserDto>;
            Assert.IsNotNull(dataResponse);
            Assert.AreEqual(EDataResponseCode.Success, dataResponse.ResponseCode);
            Assert.AreEqual(request.UserName, dataResponse.Data!.UserName);
            serviceMock.Mock.Verify(s => s.Create(It.IsAny<UserRequest>()), Times.Once);
        }

        [TestMethod]
        public async Task Create_Should_Return_BadRequest_When_Invalid_Model()
        {
            // Arrange
            var controller = ServiceProvider.GetService<UserController>()!;
            var invalidRequest = new UserRequest
            {
                // missing required fields
                Password = "weak"
            };
            invalidRequest.AddErrorMessage(new ValidationResult("Invalid input param"));
            // Act
            var result = await controller.Create(invalidRequest);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(400, objectResult.StatusCode);

            var response = objectResult.Value as DataResponse<object>;
            Assert.IsNotNull(response);
            Assert.AreEqual(EDataResponseCode.InvalidInputParameter, response.ResponseCode);
        }

        #endregion


        #region Get User

        [TestMethod]
        public async Task Get_Success()
        {
            var id = Guid.NewGuid();
            var expected = new ReturnUserDto { Id = id, UserName = "Test" };

            var serviceMock = ServiceProvider.GetService<IUserService>() as IMocked<IUserService>;
            serviceMock!.Mock.Setup(s => s.GetById(id)).ReturnsAsync(expected);

            var controller = ServiceProvider.GetService<UserController>();

            var result = await controller!.Get(id);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            Assert.AreEqual(200, obj.StatusCode);

            var data = obj.Value as DataResponse<ReturnUserDto>;
            Assert.IsNotNull(data);
            Assert.AreEqual(expected.Id, data.Data!.Id);
            serviceMock.Mock.Verify(s => s.GetById(id), Times.Once);
        }

        [TestMethod]
        public async Task Get_Should_Return_BadRequest_When_Id_Empty()
        {
            var controller = ServiceProvider.GetService<UserController>();

            var result = await controller!.Get(Guid.Empty);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            Assert.AreEqual(400, obj.StatusCode);
        }

        #endregion

        #region Update User

        [TestMethod]
        public async Task Update_Success()
        {
            var id = Guid.NewGuid();
            var request = new UserRequest
            {
                Email = "updated@domain.com",
                UserName = "updated",
                FullName = "Updated User",
                MobileNumber = "999",
                Language = "en",
                Culture = "en-US",
                Password = "123"
            };

            var serviceMock = ServiceProvider.GetService<IUserService>() as IMocked<IUserService>;
            serviceMock!.Mock.Setup(s => s.Update(id, request)).ReturnsAsync(true);

            var controller = ServiceProvider.GetService<UserController>();

            var result = await controller!.Update(id, request);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            Assert.AreEqual(200, obj.StatusCode);

            var data = obj.Value as DataResponse<bool>;
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Data);
            serviceMock.Mock.Verify(s => s.Update(id, request), Times.Once);
        }

        [TestMethod]
        public async Task Update_Return_BadRequest_When_Invalid_Model()
        {
            var id = Guid.NewGuid();
            var request = new UserRequest
            {
                UserName = "updated",
                FullName = "Updated User",
            };
            request.AddErrorMessage(new ValidationResult("Invalid input param"));


            var controller = ServiceProvider.GetService<UserController>();
            var result = await controller!.Update(id, request);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(400, objectResult.StatusCode);

            var response = objectResult.Value as DataResponse<object>;
            Assert.IsNotNull(response);
            Assert.AreEqual(EDataResponseCode.InvalidInputParameter, response.ResponseCode);
        }


        [TestMethod]
        public async Task Update_Should_Return_BadRequest_When_Id_Empty()
        {
            var request = new UserRequest();
            var controller = ServiceProvider.GetService<UserController>();

            var result = await controller!.Update(Guid.Empty, request);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            Assert.AreEqual(400, obj.StatusCode);
        }

        #endregion

        #region Delete User

        [TestMethod]
        public async Task Delete_Success()
        {
            var id = Guid.NewGuid();
            var serviceMock = ServiceProvider.GetService<IUserService>() as IMocked<IUserService>;
            serviceMock!.Mock.Setup(s => s.Delete(id)).ReturnsAsync(true);

            var controller = ServiceProvider.GetService<UserController>();

            var result = await controller!.Delete(id);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            Assert.AreEqual(200, obj.StatusCode);

            var data = obj.Value as DataResponse<bool>;
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Data);
            serviceMock.Mock.Verify(s => s.Delete(id), Times.Once);
        }

        [TestMethod]
        public async Task Delete_Should_Return_BadRequest_When_Id_Empty()
        {
            var controller = ServiceProvider.GetService<UserController>();
            var result = await controller!.Delete(Guid.Empty);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            Assert.AreEqual(400, obj.StatusCode);
        }

        #endregion

        #region Validate Password

        [TestMethod]
        public async Task ValidatePassword_Success()
        {
            var id = Guid.NewGuid();
            var request = new ValidatePasswordRequest { Password = "123" };
            var serviceMock = ServiceProvider.GetService<IUserService>() as IMocked<IUserService>;
            serviceMock!.Mock.Setup(s => s.ValidatePassword(id, request)).ReturnsAsync(true);

            var controller = ServiceProvider.GetService<UserController>();

            var result = await controller!.ValidatePassword(id, request);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            Assert.AreEqual(200, obj.StatusCode);

            var data = obj.Value as DataResponse<bool>;
            Assert.IsTrue(data!.Data);
        }

        [TestMethod]
        public async Task ValidatePassword_Should_Return_BadRequest_When_Invalid()
        {
            var controller = ServiceProvider.GetService<UserController>();
            var result = await controller!.ValidatePassword(Guid.Empty, new ValidatePasswordRequest());
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            Assert.AreEqual(400, obj.StatusCode);
        }

        #endregion

    }

}