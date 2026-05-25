using UnityEngine;

public class CustomRainController : MonoBehaviour
{
    [Header("Follow & Rotation Settings")]
    [Tooltip("Ссылка на главную камеру игрока. Если пусто, скрипт найдет её сам.")]
    public Transform cameraTransform;
    [Tooltip("Высота, на которой туча летит над игроком")]
    public float rainHeight = 25.0f;
    [Tooltip("На сколько метров перед игроком смещена туча (обычно отрицательное, например -7)")]
    public float rainForwardOffset = -7.0f;

    [Header("Particle Settings")]
    [Tooltip("Твоя рабочая система частиц дождя")]
    public ParticleSystem rainParticleSystem;
    [Range(0f, 1f)]
    [Tooltip("Интенсивность дождя от 0 до 1")]
    public float rainIntensity = 1f;
    [Tooltip("Максимальное количество капель в секунду при интенсивности = 1")]
    public float maxRateOverTime = 500f;

    [Header("3D Audio Clips for Windows")]
    [Tooltip("Звук для слабого дождя (интенсивность до 0.3)")]
    public AudioClip rainLightClip;
    [Tooltip("Звук для среднего дождя (интенсивность от 0.3 до 0.7)")]
    public AudioClip rainMediumClip;
    [Tooltip("Звук для сильного ливня (интенсивность выше 0.7)")]
    public AudioClip rainHeavyClip;

    [Header("Windows Audio Group")]
    [Tooltip("Родительский пустой объект, в котором лежат все 3D источники звука у окон")]
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

        // Гарантируем правильные настройки мирового пространства для капель
        if (rainParticleSystem != null)
        {
            var mainModule = rainParticleSystem.main;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // 1. Логика следования и вращения за камерой
        transform.position = cameraTransform.position;
        transform.Translate(0.0f, rainHeight, rainForwardOffset);
        transform.rotation = Quaternion.Euler(0.0f, cameraTransform.rotation.eulerAngles.y, 0.0f);

        // 2. Проверка изменения ползунка интенсивности
        if (Mathf.Abs(currentIntensity - rainIntensity) > 0.001f)
        {
            currentIntensity = rainIntensity;
            UpdateRainParameters();
        }
    }

    private void UpdateRainParameters()
    {
        // ИСПРАВЛЕНО: Правильное изменение интенсивности капель через MinMaxCurve
        if (rainParticleSystem != null)
        {
            var emission = rainParticleSystem.emission;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(currentIntensity * maxRateOverTime);
        }

        // Выбираем правильный аудиоклип в зависимости от ползунка
        AudioClip targetClip = null;
        if (currentIntensity > 0f && currentIntensity <= 0.33f) targetClip = rainLightClip;
        else if (currentIntensity > 0.33f && currentIntensity <= 0.66f) targetClip = rainMediumClip;
        else if (currentIntensity > 0.66f) targetClip = rainHeavyClip;

        // Управляем всеми 3D источниками звука у окон
        if (windows3DAudioGroup != null)
        {
            AudioSource[] windowSources = windows3DAudioGroup.GetComponentsInChildren<AudioSource>();

            foreach (AudioSource source in windowSources)
            {
                // Если интенсивность = 0, полностью тушим звук у окон
                if (currentIntensity <= 0f)
                {
                    if (source.isPlaying) source.Stop();
                    continue;
                }

                // Если клип сменился (например, перешли с легкого на средний)
                if (source.clip != targetClip)
                {
                    source.gameObject.SetActive(false); // Быстрый сброс источника, чтобы Unity применила клип на лету
                    source.clip = targetClip;
                    source.gameObject.SetActive(true);

                    if (targetClip != null && !source.isPlaying)
                    {
                        source.loop = true;
                        source.Play();
                    }
                }

                // Динамически подстраиваем громкость 3D звука под ползунок внутри выбранного режима
                source.volume = currentIntensity;
            }
        }
    }
}