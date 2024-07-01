using System;

namespace Llamas.Enums;

/// <summary>
/// A unite of time to use for keep-alive parameters
/// </summary>
public enum UnitOfTime
{
    /// "s"
    Second,

    /// "m"
    Minute,

    /// "h"
    Hour
}

/// <summary>
/// Static extensions for <see cref="UnitOfTime"/>
/// </summary>
public static class UnitOfTimeExtensions
{
    /// <summary>
    /// Converts a <see cref="UnitOfTime"/> to the equivalent number of seconds.
    /// </summary>
    /// <param name="unitOfTime"></param>
    /// <param name="units"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static int ToSeconds(this UnitOfTime unitOfTime, int units) =>
        unitOfTime switch
        {
            UnitOfTime.Second => units,
            UnitOfTime.Minute => units * 60,
            UnitOfTime.Hour => units * 3600,
            _ => throw new ArgumentOutOfRangeException(nameof(unitOfTime), unitOfTime, null)
        };
}
