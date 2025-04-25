using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Filtering;
using Project.Utils;

[AddComponentMenu("Project/Player/Player Interaction Filter")]
public class PlayerInteractionFilter : MonoBehaviour, IEnable, IXRHoverFilter, IXRSelectFilter
{
    private bool isEnabled = false;
    public bool canProcess => isEnabled;

    public void Enable() => isEnabled = true;
    public void Disable() => isEnabled = false;

    public bool Process(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRHoverInteractor interactor, UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable interactable) => false;
    public bool Process(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor, UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable) => false;
}