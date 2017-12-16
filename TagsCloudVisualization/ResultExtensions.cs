using System.Collections.Generic;

namespace TagsCloudVisualization
{
    public static class ResultExtensions
    {
        public static Result<T[]> CheckForErrors<T>(this IReadOnlyList<Result<T>> results)
        {
            var values = new List<T>();
            for (var i = 0; i < results.Count; i++)
            {
                if(!results[i].IsSuccess)
                    return Result.Fail<T[]>($"Failed on {typeof(T)} at index {i} with error: {results[i].Error}");
                values.Add(results[i].Value);
            }
            return Result.Ok(values.ToArray());
        }
    }
}