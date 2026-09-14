using UnityEngine;

public class Tracer : MonoBehaviour
{
    public LineRenderer tracer;

    [Header("Tracer Settings")]
    [SerializeField] private float _dissolveDuration = 1.0f; // Duration for the tracer to dissolve

    private float _elapsedTime = 0.0f; // Track the elapsed time for the dissolve effect

    private void Awake()
    {
        _elapsedTime = 0.0f; // Initialize the elapsed time when the tracer is created
        tracer.material.SetVector("_Noise_speed_RG", new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
        EventRouter.RegisterWithHub(gameObject); // Register the tracer with the EventRouter
    }

    private void Update()
    {
        if (_elapsedTime < _dissolveDuration)
        {
            _elapsedTime += Time.deltaTime;
            float dissolveAmount = Mathf.Clamp01(_elapsedTime / _dissolveDuration);
            tracer.material.SetFloat("_Dissolve", dissolveAmount);
        }
        else
        {
            EventRouter.OnTracerDeactivation?.Invoke(gameObject); // Notify the EventRouter about the tracer deactivation
            EventRouter.UnregisterFromHub(gameObject);
            Destroy(gameObject); // Destroy the tracer object once it has fully dissolved
        }
    }

}
