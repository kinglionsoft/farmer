using System;
using Xunit;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.EF;
using System.Threading.Tasks;
using Test.Helper;

namespace Test
{
    public class SampleTest : IDisposable
    {
        public void Dispose()
        {
            // TODO: Clean your test data here!
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Multiplte(int p)
        {
            p.Should().BePositive();
        }

        [Fact]
        public void MockInterface()
        {
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(u => u.Create(1))
                 .Callback(() => Console.WriteLine("creatation started."))
                .Returns(new User { Id = 1 })
                 .Callback(() => Console.WriteLine("creatation done."));

            userServiceMock.Object.Create(1).Should().Equals(1);

            userServiceMock.As<IDisposable>()
                .Setup(u => u.Dispose());
        }

        [Fact]
        public void MockException()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(f => f.Create(Match.Create<int>(i => i < 0)))
             .Throws<ArgumentException>();

            mock.Object.Create(0);
        }

        [Fact]
        public void MockVerify()
        {
            var mock = new Mock<IUserService>();
            mock.Object.Create(0);
            mock.Verify(f => f.Create(It.IsAny<int>()));
        }

        [Fact]
        public void MockGet()
        {
            var mock = new Mock<User>();
            var name = mock.Object.Name;
            mock.VerifyGet(x => x.Name); // only available for virtural properties.
        }

        [Fact]
        public void MockDbContext()
        {
            var users = new[]
            {
                new User(1, "A"),
                new User(2, "B")
            };

            var userSetMock = TestHelper.MockDbSet(users);

            var contextMock = new Mock<TestContext>();
            contextMock.Setup(m=>m.Users).Returns(userSetMock.Object);

            IUserService service = new UserService(contextMock.Object);
            var user = service.Get(1);
            user.Should().NotBeNull();
            user.Id.Should().Equals(1);
        }

        [Fact]
        public async Task MockAsyncQuery()
        {
             var users = new[]
            {
                new User(1, "A"),
                new User(2, "B")
            };

            var userSetMock = TestHelper.MockAsyncDbSet(users);

            var contextMock = new Mock<TestContext>();
            contextMock.Setup(m=>m.Users).Returns(userSetMock.Object);

            IUserService service = new UserService(contextMock.Object);
            var user = await service.GetAsync(1);
            user.Should().NotBeNull();
            user.Id.Should().Equals(1);
        }
    }
}
