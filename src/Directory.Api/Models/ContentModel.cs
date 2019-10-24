using System.Collections.Generic;

namespace Directory.Api.Models {
    public class ContentModel<T> {
        public ContentModel(IEnumerable<T> content) => Content = content;

        public IEnumerable<T> Content { get; }
    }
}
