using Microsoft.EntityFrameworkCore;
namespace UMServer.Tests.Common;

public static class ModelUtils
{
    public static IQueryable<T> MockDBSet<T>(List<T> values) where T : class
    {
        var result = new Mock<DbSet<T>>();
        var source = values.AsQueryable();

        result
            .As<IQueryable<T>>()
            .Setup(_ => _.Provider)
            .Returns(source.Provider);
        result
            .As<IQueryable<T>>()
            .Setup(_ => _.Expression)
            .Returns(source.Expression);
        result
            .As<IQueryable<T>>()
            .Setup(_ => _.ElementType)
            .Returns(source.ElementType);
        result
            .As<IQueryable<T>>()
            .Setup(_ => _.GetEnumerator())
            .Returns(source.GetEnumerator());

        return result.Object;
    }
}

// public class DbSetList<T> : List<T>, IAsyncEnumerable<T>
// {
//     public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
//     {
//         return GetAsyncEn
//         throw new NotImplementedException();
//     }
// }