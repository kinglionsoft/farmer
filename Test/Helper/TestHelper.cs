using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using Test.Helper.Async;

namespace Test.Helper
{
    public static class TestHelper
    {
        public static Mock<DbSet<T>> MockDbSet<T>(IEnumerable<T> fakeData) where T : class, new()
        {
            var setMock = new Mock<DbSet<T>>();
            var qeuryMock = setMock.As<IQueryable<T>>();
            var fakeDataQueryable = fakeData.AsQueryable();
            qeuryMock.Setup(m => m.Provider).Returns(fakeDataQueryable.Provider);
            qeuryMock.Setup(m => m.ElementType).Returns(fakeDataQueryable.ElementType);
            qeuryMock.Setup(m => m.Expression).Returns(fakeDataQueryable.Expression);
            qeuryMock.Setup(m => m.GetEnumerator()).Returns(() => fakeDataQueryable.GetEnumerator());
            return setMock;
        }

        public static Mock<DbSet<T>> MockAsyncDbSet<T>(IEnumerable<T> fakeData) where T : class, new()
        {
            var setMock = new Mock<DbSet<T>>();

            setMock.As<IAsyncEnumerable<T>>()
                .Setup(d => d.GetEnumerator())
                .Returns(new AsyncEnumerator<T>(fakeData.GetEnumerator()));

            var qeuryMock = setMock.As<IQueryable<T>>();
            var fakeDataQueryable = fakeData.AsQueryable();
            qeuryMock.Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(fakeDataQueryable.Provider)); 
            qeuryMock.Setup(m => m.ElementType).Returns(fakeDataQueryable.ElementType);
            qeuryMock.Setup(m => m.Expression).Returns(fakeDataQueryable.Expression);
            qeuryMock.Setup(m => m.GetEnumerator()).Returns(() => fakeDataQueryable.GetEnumerator());
            return setMock;
        }
    }
}