using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
    public ModelListing(string name, string description, DateOnly updated, string[]? modelTags)
    {
        Name = name;
        Description = description;
        Updated = updated;
        ModelTags = modelTags ?? ["latest"];
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
    /// Approximate date model was last updated
    /// </summary>
    public required DateOnly Updated { get; init; }

    /// <summary>
    /// Array of tags for the model
    /// </summary>
    public required string[] ModelTags { get; init; }
}
