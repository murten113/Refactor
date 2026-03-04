using UnityEngine;

/// <summary>
/// Interface for player sub-components. Each component handles a specific behavior
/// and can work independently where possible.
/// </summary>
public interface IPlayerComponent
{
    void Process(PlayerScript player);
}
