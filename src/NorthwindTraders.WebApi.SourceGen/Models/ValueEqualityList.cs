using System.Collections.Generic;
using System.Linq;

namespace NorthwindTraders.WebApi.SourceGen.Models;

/// <summary>
///     Simple list that compares values for equality. Intended to be used when contained in a record type.
/// </summary>
/// <remarks>
///     Taken from: https://stackoverflow.com/a/69366347.
/// </remarks>
public sealed class ValueEqualityList<T>(bool requireMatchingOrder = false) : List<T>
{
    public override bool Equals(object? obj)
    {
        if (!(obj is IEnumerable<T> enumerable)) return false;
        if (!requireMatchingOrder) return ScrambledEquals(enumerable, this);
        return enumerable.SequenceEqual(this);
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        foreach (var item in this)
        {
            if (item is null)
                continue;

            hashCode ^= item.GetHashCode();
        }

        return hashCode;
    }

    /// <summary>
    /// Returns true if both enumerables contain the same items, regardless of order. O(N*Log(N))
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    private static bool ScrambledEquals(IEnumerable<T> first, IEnumerable<T> second)
    {
        var counts = GetCounts(first);

        foreach (var item in second)
        {
            if (!counts.TryGetValue(item, out var count)) return false;
            count -= 1;
            counts[item] = count;
            if (count < 0) return false;
        }

        return counts.Values.All(c => c == 0);
    }

    private static Dictionary<T, int> GetCounts(IEnumerable<T> enumerable)
    {
        var counts = new Dictionary<T, int>();
        foreach (var item in enumerable)
        {
            if (!counts.TryGetValue(item, out var count))
            {
                count = 0;
            }

            count++;
            counts[item] = count;
        }

        return counts;
    }
}