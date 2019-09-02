using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Directory.Test.Helpers {
    public static class EnumerableExtensions {
        public static DbSet<T> AsMockedDbSet<T>(this IEnumerable<T> @this) where T : class {
            TestAsyncEnumerable<T> underlyingData = new TestAsyncEnumerable<T>(@this);

            Mock<DbSet<T>> mockedSet = new Mock<DbSet<T>>();
            mockedSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(((IQueryable<T>)underlyingData).Provider);
            mockedSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(((IQueryable<T>)underlyingData).Expression);
            mockedSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(((IQueryable<T>)underlyingData).ElementType);

            mockedSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetEnumerator()).Returns(((IAsyncEnumerable<T>)underlyingData).GetEnumerator());
            mockedSet.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(((IEnumerable)underlyingData).GetEnumerator());
            mockedSet.As<IEnumerable<T>>().Setup(m => m.GetEnumerator()).Returns(((IEnumerable<T>)underlyingData).GetEnumerator());

            return mockedSet.Object;
        }
    }
}
