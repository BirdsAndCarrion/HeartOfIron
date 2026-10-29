using UnityEngine;

public class MechCooldownProgressIndicator : MonoBehaviour
{
    [SerializeField] private GameObject cooldownProgressIndicator;
    [SerializeField] private ResourceBar cooldownResourceBar;

    public void UpdateCooldownProgress(float progress, float maxProgress)
    {
        cooldownResourceBar.SetResourceMax(maxProgress);
        float calcProgress = maxProgress - progress;
        cooldownResourceBar.SetResourceCurrent(calcProgress);
    }

    public void SetActive(bool isActive)
    {
        cooldownProgressIndicator.SetActive(isActive);
    }
}
