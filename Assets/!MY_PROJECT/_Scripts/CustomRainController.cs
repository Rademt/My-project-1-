using UnityEngine;

public class CustomRainController : MonoBehaviour
{
    [Header("Follow & Rotation Settings")]
    public Transform cameraTransform;
    public float rainHeight = 25.0f;
    public float rainForwardOffset = -7.0f;

    [Header("Particle Settings")]
    public ParticleSystem rainParticleSystem;
    [Range(0f, 1f)]
    public float rainIntensity = 1f;
    public float maxRateOverTime = 500f;

    [Header("3D Audio Clips for Windows")]
    public AudioClip rainLightClip;
    public AudioClip rainMediumClip;
    public AudioClip rainHeavyClip;

    [Header("Windows Audio Group")]
    public GameObject windows3DAudioGroup;

    private float currentIntensity = -1f;
    private AudioClip currentActiveClip;

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (rainParticleSystem == null)
        {
            rainParticleSystem = GetComponent<ParticleSystem>();
        }

        if (rainParticleSystem != null)
        {
            var mainModule = rainParticleSystem.main;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        transform.position = cameraTransform.position;
        transform.Translate(0.0f, rainHeight, rainForwardOffset);
        transform.rotation = Quaternion.Euler(0.0f, cameraTransform.rotation.eulerAngles.y, 0.0f);

        if (Mathf.Abs(currentIntensity - rainIntensity) > 0.001f)
        {
            currentIntensity = rainIntensity;
            UpdateRainParameters();
        }
    }

    private void UpdateRainParameters()
    {
        if (rainParticleSystem != null)
        {
            var emission = rainParticleSystem.emission;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(currentIntensity * maxRateOverTime);
        }

        AudioClip targetClip = null;
        if (currentIntensity > 0f && currentIntensity <= 0.33f) targetClip = rainLightClip;
        else if (currentIntensity > 0.33f && currentIntensity <= 0.66f) targetClip = rainMediumClip;
        else if (currentIntensity > 0.66f) targetClip = rainHeavyClip;

        if (windows3DAudioGroup != null)
        {
            AudioSource[] windowSources = windows3DAudioGroup.GetComponentsInChildren<AudioSource>();

            foreach (AudioSource source in windowSources)
            {
                if (currentIntensity <= 0f)
                {
                    if (source.isPlaying) source.Stop();
                    continue;
                }

                if (source.clip != targetClip)
                {
                    source.gameObject.SetActive(false);
                    source.clip = targetClip;
                    source.gameObject.SetActive(true);

                    if (targetClip != null && !source.isPlaying)
                    {
                        source.loop = true;
                        source.Play();
                    }
                }

                source.volume = currentIntensity;
            }
        }
    }
}