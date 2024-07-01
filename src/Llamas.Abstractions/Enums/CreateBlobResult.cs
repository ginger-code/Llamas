namespace Llamas.Enums;

/// <summary>
/// Result of creating a blob
/// </summary>
public enum CreateBlobResult
{
    /// Blob was created
    Created,

    /// Blob digest mismatched data
    DigestMismatch,

    /// Blob was not created
    NotCreated
}
