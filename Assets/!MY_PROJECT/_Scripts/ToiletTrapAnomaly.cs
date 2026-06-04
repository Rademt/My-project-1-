using UnityEngine;

public class ToiletTrapAnomaly : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public Transform doorTransform;
    public AudioSource heartbeatAudio;
    public AudioSource slamAudio;
    public Animator doorAnimator;
    public GameObject monsterObject;

    [Header("Door Control Integration")]
    public MonoBehaviour normalDoorController;

    [Header("Light Control (NEW)")]
    [Tooltip("Перетяни сюда Light (источник света) внутри этого туалета")]
    public Light toiletLight;
    [Tooltip("Если включено, свет полностью выключится при активации аномалии. Если выключено — начнет мигать.")]
    public bool turnOffCompletely = true;

    [Header("Distance & Heartbeat Settings")]
    public float maxDetectionRadius = 15f;
    public float minPanicRadius = 4.5f;
    public float slamDistance = 3.5f;

    private bool isAnomalyActive = false;
    private bool hasSlammed = false;
    private float flickerTimer = 0f;

    public void EnableScreamerAnomaly()
    {
        if (isAnomalyActive) return;

        isAnomalyActive = true;
        hasSlammed = false;

        if (normalDoorController != null) normalDoorController.enabled = false;

        if (monsterObject != null) monsterObject.SetActive(true);

        if (toiletLight != null && turnOffCompletely)
        {
            toiletLight.enabled = false;
        }

        if (heartbeatAudio != null)
        {
            heartbeatAudio.volume = 0f;
            heartbeatAudio.pitch = 0.8f;
            heartbeatAudio.Play();
        }

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("StartAnomaly");
            doorAnimator.Play("Door_HalfOpen");
        }

        Debug.Log("<color=magenta>[ToiletTrapAnomaly]: Аномалия со светом успешно запущена!</color>");
    }

    void Update()
    {
        if (!isAnomalyActive || playerTransform == null) return;

        Vector3 targetDoorPosition = doorTransform != null ? doorTransform.position : transform.position;
        float distance = Vector3.Distance(playerTransform.position, targetDoorPosition);

        if (!hasSlammed)
        {
            if (distance <= maxDetectionRadius)
            {
                float t = Mathf.InverseLerp(maxDetectionRadius, minPanicRadius, distance);
                if (heartbeatAudio != null)
                {
                    if (!heartbeatAudio.isPlaying) heartbeatAudio.Play();
                    heartbeatAudio.volume = Mathf.Lerp(0f, 1f, t);
                    heartbeatAudio.pitch = Mathf.Lerp(0.8f, 1.6f, t);
                }
            }

            if (toiletLight != null && !turnOffCompletely && distance <= maxDetectionRadius)
            {
                flickerTimer += Time.deltaTime;
                float flickerSpeed = Mathf.Lerp(0.2f, 0.05f, Mathf.InverseLerp(maxDetectionRadius, minPanicRadius, distance));

                if (flickerTimer >= flickerSpeed)
                {
                    toiletLight.enabled = !toiletLight.enabled;
                    flickerTimer = 0f;
                }
            }
        }

        if (!hasSlammed && distance <= slamDistance)
        {
            ExecuteDoorSlam();
        }
    }

    private void ExecuteDoorSlam()
    {
        hasSlammed = true;

        if (heartbeatAudio != null) heartbeatAudio.Stop();
        if (monsterObject != null) monsterObject.SetActive(false);

        if (doorAnimator != null)
        {
            doorAnimator.Play("Door_SlamShut");
        }

        if (slamAudio != null) slamAudio.Play();

        if (toiletLight != null)
        {
            toiletLight.enabled = false;
        }

        if (normalDoorController != null) normalDoorController.enabled = true;

        Debug.Log("<color=red>[ToiletTrapAnomaly]: Двери захлопнулись, свет вырубился окончательно!</color>");
    }
}