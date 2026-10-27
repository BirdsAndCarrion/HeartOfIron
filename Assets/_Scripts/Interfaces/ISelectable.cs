/// <summary>
/// Defines an interface for objects that can be selected.
/// Implementing classes should handle selection logic and return a relevant identifier or description.
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// Called when the object is selected.
    /// </summary>
    /// <returns>A string representing the identifier or description of the selected object.</returns>
    /// <summary>
    string OnSelected();
}
