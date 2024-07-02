using System.Diagnostics.CodeAnalysis;

namespace Llamas.Models;

/// <summary>
/// Size of a file in terms of bytes
/// </summary>
public sealed record FileSize
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Llamas.Models.FileSize" /> class.
    /// </summary>
    public FileSize() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Llamas.Models.FileSize" /> class.
    /// </summary>
    [SetsRequiredMembers]
    public FileSize(decimal size, string unit)
    {
        Size = size;
        Unit = unit;
    }

    /// <summary>
    /// Size of the file
    /// </summary>
    public required decimal Size { get; init; }

    /// <summary>
    /// Unit of size
    /// </summary>
    public required string Unit { get; init; }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"{Size}{Unit}";
}