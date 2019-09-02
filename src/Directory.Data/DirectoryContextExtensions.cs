using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Directory.Data {
    public static class DirectoryContextExtensions {
        public static Task<Brother> FindBrotherByIdAsync(this DbSet<Brother> @this, int id)
            => @this.FirstOrDefaultAsync(b => b.Id == id);
    }
}
