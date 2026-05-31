using UnityEngine;

public class HeartbeatAnomaly : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform; // Ссылка на Player (First Person Controller)
    public AudioSource heartbeatAudio; // Наш источник звука сердца

    [Header("Distance Settings")]
    public float maxDetectionRadius = 15f; // Дистанция, на которой сердце начинает стучать
    public float minPanicRadius = 2f;      // Дистанция максимальной паники (у самой двери)

    [Header("Audio Tweak Settings")]
    public float maxVolume = 1f;
    public float minPitch = 0.8f;  // Медленный стук издалека
    public float maxPitch = 1.6f;  // Сумасшедший стук вплотную

    private bool isAnomalyActive = false;

    public void InitializeHeartbeatAnomaly(bool active)
    {
        isAnomalyActive = active;
        if (isAnomalyActive && heartbeatAudio != null)
        {
            heartbeatAudio.volume = 0f;
            heartbeatAudio.pitch = minPitch;
            heartbeatAudio.Play();
        }
        else if (heartbeatAudio != null)
        {
            heartbeatAudio.Stop();
        }
    }

    void Update()
    {
        if (!isAnomalyActive || playerTransform == null || heartbeatAudio == null) return;

        // Считаем расстояние между игроком и дверью
        float distance = Vector3.Distance(playerTransform.position, transform.position);

        if (distance <= maxDetectionRadius)
        {
            // Нормализуем значение от 0.0 (далеко) до 1.0 (вплотную)
            float t = Mathf.InverseLerp(maxDetectionRadius, minPanicRadius, distance);

            // Динамически меняем громкость и скорость звука
            heartbeatAudio.volume = Mathf.Lerp(0f, maxVolume, t);
            heartbeatAudio.pitch = Mathf.Lerp(minPitch, maxPitch, t);
        }
        else
        {
            heartbeatAudio.volume = 0f;
        }
    }
}