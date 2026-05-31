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

    [Header("Distance & Heartbeat Settings")]
    public float maxDetectionRadius = 15f;
    public float minPanicRadius = 4.5f;
    public float slamDistance = 3.5f;

    private bool isAnomalyActive = false;
    private bool hasSlammed = false;

    // —крипт повн≥стю спить, поки AnomalyLogic не виконаЇ цей метод:
    public void EnableScreamerAnomaly()
    {
        if (isAnomalyActive) return;

        isAnomalyActive = true;
        hasSlammed = false;

        // ЅлокуЇмо звичайний контролер дверей гравц€
        if (normalDoorController != null) normalDoorController.enabled = false;

        // ¬микаЇмо монстра в щ≥лин≥
        if (monsterObject != null) monsterObject.SetActive(true);

        // «апускаЇмо звук серц€
        if (heartbeatAudio != null)
        {
            heartbeatAudio.volume = 0f;
            heartbeatAudio.pitch = 0.8f;
            heartbeatAudio.Play();
        }

        // ѕереводимо ан≥матор у стан прив≥дчинених дверей
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("StartAnomaly");
            doorAnimator.Play("Door_HalfOpen");
        }

        Debug.Log("<color=magenta>[ToiletDoorScreamer]: јномал≥ю усп≥шно ≥н≥ц≥ал≥зовано менеджером круг≥в!</color>");
    }

    void Update()
    {
        // якщо аномал≥€ не вибрана (чисте коло або шанс 0) Ч Update повн≥стю ≥гноруЇтьс€!
        if (!isAnomalyActive || playerTransform == null) return;

        Vector3 targetDoorPosition = doorTransform != null ? doorTransform.position : transform.position;
        float distance = Vector3.Distance(playerTransform.position, targetDoorPosition);

        // 1. Ћог≥ка серцебитт€
        if (!hasSlammed && distance <= maxDetectionRadius)
        {
            float t = Mathf.InverseLerp(maxDetectionRadius, minPanicRadius, distance);
            if (heartbeatAudio != null)
            {
                if (!heartbeatAudio.isPlaying) heartbeatAudio.Play();
                heartbeatAudio.volume = Mathf.Lerp(0f, 1f, t);
                heartbeatAudio.pitch = Mathf.Lerp(0.8f, 1.6f, t);
            }
        }

        // 2. Ћог≥ка захлопуванн€
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

        Debug.Log("<color=red>[ToiletDoorScreamer]: ƒвер≥ туалету гучно захлопнулис€!</color>");
    }
}