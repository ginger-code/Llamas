using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Llamas.Models;

/// <summary>
/// Details about a model available to pull from https://ollama.com/library
/// </summary>
public sealed record ModelListingDetails
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Llamas.Models.ModelListingDetails" /> class.
    /// </summary>
    public ModelListingDetails() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Llamas.Models.ModelListingDetails" /> class.
    /// </summary>
    [SetsRequiredMembers]
    public ModelListingDetails(
        string name,
        string description,
        string version,
        DateTimeOffset updated,
        ModelSize[] modelSizes,
        IReadOnlyDictionary<ModelSize, FileSize> fileSizes,
        string readmeMarkup
    )
    {
        Name = name;
        Description = description;
        Version = version;
        Updated = updated;
        ModelSizes = modelSizes;
        FileSizes = fileSizes;
        ReadmeMarkup = readmeMarkup;
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
    /// Hash for this version of the model
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// Approximate time model was last updated
    /// </summary>
    public required DateTimeOffset Updated { get; init; }

    /// <summary>
    /// Array of available sizes for the model
    /// </summary>
    public required ModelSize[] ModelSizes { get; init; }

    /// <summary>
    /// Dictionary of available file sizes by their model size
    /// </summary>
    public required IReadOnlyDictionary<ModelSize, FileSize> FileSizes { get; init; }

    /// <summary>
    /// HTML markup of the readme for the model
    /// </summary>
    public required string ReadmeMarkup { get; init; }
}
