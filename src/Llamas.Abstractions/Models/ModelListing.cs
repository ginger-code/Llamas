using System;
using System.Diagnostics.CodeAnalysis;

namespace Llamas.Models;

/// <summary>
/// Information brief about a model available to pull from https://ollama.com/library
/// </summary>
public sealed record ModelListing
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Llamas.Models.ModelListing" /> class.
    /// </summary>
    public ModelListing() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Llamas.Models.ModelListing" /> class.
    /// </summary>
    [SetsRequiredMembers]
    public ModelListing(
        string name,
        string description,
        DateTimeOffset updated,
        ModelSize[] modelSizes
    )
    {
        Name = name;
        Description = description;
        Updated = updated;
        ModelSizes = modelSizes;
    }

    /// <summary>
    /// Name of the model
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Description of the model
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Approximate time model was last updated
    /// </summary>
    public required DateTimeOffset Updated { get; init; }

    /// <summary>
    /// Array of available sizes for the model
    /// </summary>
    public required ModelSize[] ModelSizes { get; init; }
}
