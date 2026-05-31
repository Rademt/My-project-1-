using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class lightning : MonoBehaviour
{
    [Header("Налаштування світла")]
    [Tooltip("Батьківський об'єкт, в якому лежать усі твої Spot Light")]
    public GameObject lightningLightsGroup;

    [Tooltip("Яскравість спалаху блискавки")]
    public float flashIntensity = 5f;

    [Header("Налаштування часу (в секундах)")]
    public float minTimeBetweenThunders = 20f;
    public float maxTimeBetweenThunders = 45f;

    [Header("Звук грому (Опціонально)")]
    public AudioSource thunderAudioSource;
    public List<AudioClip> thunderSounds;

    private float timer;
    private float nextThunderTime;
    private List<Light> lightsList = new List<Light>();

    void Start()
    {
        // Знаходимо всі компоненти Light всередині нашої групи ламп
        if (lightningLightsGroup != null)
        {
            // На випадок, якщо група вимкнена в інспекторі — вмикаємо її один раз назавжди
            lightningLightsGroup.SetActive(true);

            Light[] foundLights = lightningLightsGroup.GetComponentsInChildren<Light>(true);
            lightsList.AddRange(foundLights);
        }

        // Гасимо всі лампи до нуля на старті
        SetLightsIntensity(0f);
        ResetTimer();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextThunderTime)
        {
            StartCoroutine(TriggerThunderstorm());
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        timer = 0f;
        nextThunderTime = Random.Range(minTimeBetweenThunders, maxTimeBetweenThunders);
    }

    private void SetLightsIntensity(float intensity)
    {
        foreach (Light light in lightsList)
        {
            if (light != null)
            {
                light.intensity = intensity;
            }
        }
    }

    private IEnumerator TriggerThunderstorm()
    {
        if (lightsList.Count == 0) yield break;

        // --- Перший спалах ---
        SetLightsIntensity(flashIntensity);
        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        SetLightsIntensity(0f);

        // --- Маленька пауза між подвійним ударом ---
        yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));

        // --- Другий спалах (більш затяжний) ---
        SetLightsIntensity(flashIntensity);

        if (thunderAudioSource != null && thunderSounds != null && thunderSounds.Count > 0)
        {
            AudioClip randomThunder = thunderSounds[Random.Range(0, thunderSounds.Count)];
            thunderAudioSource.clip = randomThunder;
            thunderAudioSource.Play();
        }

        yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));
        SetLightsIntensity(0f);
    }
}