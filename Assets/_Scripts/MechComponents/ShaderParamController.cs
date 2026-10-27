using UnityEngine;

public class ShaderParamController : MonoBehaviour
{
    private Material material;

    private readonly string edgeColorParam = "_EdgeColor";
    private readonly string edgeWidthParam = "_EdgeWidth";
    private readonly string noiseScaleParam = "_NoiseScale";
    private readonly string dissolveSpeedParam = "_DissolveSpeed";
    private readonly string dissolveValueParam = "_DissolveValue";
    private readonly string dissolveActiveParam = "_DissolveActive";
    private readonly string hitTimerParam = "_HitTimer";
    private readonly string damageTypeParam = "_DamageType";
    private readonly string isHoveredParam = "_IsHovered";
    private readonly string hoveredColorParam = "_HoveredColor";

    private readonly float ENABLED = 1.0f; // Example value, adjust as needed
    private readonly float DISABLED = 0.0f; // Example value, adjust as needed

    [SerializeField] private float dissolveSpeed = 1.0f; // Example value, adjust as needed
    [SerializeField] private float flashDuration = 0.5f; // Example value, adjust as needed
    private int fadingIn = 0;

    public void Init(Material material)
    {
        this.material = material;
    }

    void Update()
    {
        if (fadingIn == 1)
        {
            float dissolveValue = material.GetFloat(dissolveValueParam);
            dissolveValue += Time.deltaTime;
            if (dissolveValue >= 1.0f)
            {
                dissolveValue = 1.0f;
                DisableDissolveEffect(); // Stop fading in
            }
            material.SetFloat(dissolveValueParam, dissolveValue);
        }
        else if (fadingIn == 2)
        {
            float dissolveValue = material.GetFloat(dissolveValueParam);
            dissolveValue -= Time.deltaTime;
            if (dissolveValue <= 0.0f)
            {
                dissolveValue = 0.0f;
                DisableDissolveEffect(); // Stop fading out
            }
            material.SetFloat(dissolveValueParam, dissolveValue);
        }

    }

    public void EnableDissolveEffect(Color color, bool fadingIn)
    {
        material.SetColor(edgeColorParam, color);
        material.SetFloat(dissolveSpeedParam, dissolveSpeed); // Example value, adjust as needed
        if (fadingIn)
        {
            this.fadingIn = 1;
            material.SetFloat(dissolveValueParam, 0.0f); // Start from 0 when fading in
        }
        else
        {
            this.fadingIn = 2;
            material.SetFloat(dissolveValueParam, 1.0f); // Start from 1 when fading out
        }
        material.SetFloat(dissolveActiveParam, ENABLED);
    }
    
    private void DisableDissolveEffect()
    {
        material.SetFloat(dissolveActiveParam, DISABLED);
        fadingIn = 0; // Ensure fading is stopped when disabling the effect
    }

    public void TakeDamage(float damageType)
    {
        material.SetFloat(hitTimerParam, flashDuration); // Example value, adjust as needed
        material.SetFloat(damageTypeParam, damageType);
    }

    public void SetHovered(bool isHovered)
    {
        material.SetFloat(isHoveredParam, isHovered ? ENABLED : DISABLED);
    }

    public void SetHoveredColor(Color color)
    {
        material.SetColor(hoveredColorParam, color);
    }
}
