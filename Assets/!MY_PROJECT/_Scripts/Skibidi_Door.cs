using UnityEngine;
using System.Collections;

public class ToiletDoorScreamer : MonoBehaviour
{
    public Transform player;
    public float interactDist = 4f;
    public float openAngle = 90f;
    public float smooth = 6f;

    [Header("Звуки самой двери")]
    public AudioSource openSound;
    public AudioSource closeSound;

    [Header("Настройки скримера")]
    public GameObject zombie;
    public AudioSource screamAudio;
    public float destroyDelay = 1.5f;

    // Теперь это свойство можно включить из другого скрипта
    [HideInInspector]
    public bool isAnomalyActive = false;

    private bool isOpen = false;
    private bool hasTriggered = false;
    private Quaternion defaultRotation;
    private Quaternion openRotation;

    void Start()
    {
        defaultRotation = transform.localRotation;
        openRotation = defaultRotation * Quaternion.Euler(0, openAngle, 0);

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // УБРАЛИ СТРОЧКУ isAnomalyActive = true; чтобы дверь зря не скримила на чистых кругах

        if (zombie != null) zombie.SetActive(false);
    }

    void Update()
    {
        if (player != null)
        {
            float dist = Vector3.Distance(player.position, transform.position);

            if (dist <= interactDist)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Vector3 dirToDoor = (transform.position - player.position).normalized;
                    Vector3 playerLook = player.forward;

                    float dot = Vector3.Dot(playerLook, dirToDoor);

                    if (dot > 0.6f)
                    {
                        isOpen = !isOpen;

                        if (isOpen)
                        {
                            if (openSound != null) openSound.Play();

                            // Скример сработает только если аномалия активна
                            if (isAnomalyActive && !hasTriggered)
                            {
                                hasTriggered = true;
                                StartCoroutine(StartScreamRoutine());
                            }
                        }
                        else
                        {
                            if (closeSound != null) closeSound.Play();
                        }
                    }
                }
            }
        }

        Quaternion targetRotation = isOpen ? openRotation : defaultRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }

    // Этот метод теперь будет вызывать AnomalyLogic
    public void EnableScreamerAnomaly()
    {
        isAnomalyActive = true;
    }

    private IEnumerator StartScreamRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        if (zombie != null) zombie.SetActive(true);
        if (screamAudio != null) screamAudio.Play();
        yield return new WaitForSeconds(destroyDelay);
        if (zombie != null) Destroy(zombie);
    }
}