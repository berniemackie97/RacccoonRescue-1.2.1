using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LinqUtils
{
	public static bool AllNull<T>(this IEnumerable<T> seq)
	{
		var result = seq.All(i => i == null || i.Equals(null));
		return result;
	}

	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> seq)
	{
		var result = seq.Where(i => i != null && !i.Equals(null));
		return result;
	}
}