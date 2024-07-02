using System.Diagnostics.CodeAnalysis;

namespace Llamas.Models;

/// <summary>
/// Size of the model in terms of the number of model parameters
/// </summary>
public sealed record ModelSize
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Llamas.Models.ModelSize" /> class.
    /// </summary>
    public ModelSize() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Llamas.Models.ModelSize" /> class.
    /// </summary>
    [SetsRequiredMembers]
    public ModelSize(decimal parameterSize, char parameterMagnitude, int multiplier = 1)
    {
        Multiplier = multiplier;
        ParameterSize = parameterSize;
        ParameterMagnitude = parameterMagnitude;
    }

    /// <summary>
    /// Optional multiplier for the model size, e.g. 8x
    /// </summary>
    public int Multiplier { get; init; } = 1;

    /// <summary>
    /// Number of model parameters
    /// </summary>
    public required decimal ParameterSize { get; init; }

    /// <summary>
    /// Character representing the magnitude of the parameter size, e.g. M, B
    /// </summary>
    public required char ParameterMagnitude { get; init; }

    /// <inheritdoc />
    public override string ToString() =>
        Multiplier != 1
            ? $"{Multiplier}x{ParameterSize}{ParameterMagnitude}"
            : $"{ParameterSize}{ParameterMagnitude}";
}
