using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Directory.Test.Helpers {
    [ExcludeFromCodeCoverage]
    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T> {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable) { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression) { }

        public IAsyncEnumerator<T> GetEnumerator() => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }
}
