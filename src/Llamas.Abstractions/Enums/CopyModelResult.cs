namespace Llamas.Enums;

/// <summary>
/// Result of copying a model
/// </summary>
public enum CopyModelResult
{
    /// Model was copied
    Copied,

    /// Model with matching name not found
    NotFound,

    /// Model was not copied
    NotCopied
}
