namespace Llamas.Enums;

/// <summary>
/// Result of deleting a model
/// </summary>
public enum DeleteModelResult
{
    /// Model was deleted
    Deleted,

    /// Model with matching name not found
    NotFound,

    /// Model was not deleted
    NotDeleted
}
