using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [Header("Настройки дистанции и взгляда")]
    public Transform player;
    [Tooltip("Дистанция, на которой игрок может подергать дверь")]
    public float interactDist = 2f;
    [Tooltip("Насколько точно нужно смотреть на дверь (0.6f — это хороший угол взгляда)")]
    public float lookAngleThreshold = 0.6f;

    [Header("Звук запертой двери")]
    [Tooltip("Сюда закинь звук дерганья ручки или глухого стука закрытой двери")]
    public AudioSource lockedDoorSound;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player != null)
        {
            float dist = Vector3.Distance(player.position, transform.position);

            if (dist <= interactDist)
            {
                Vector3 dirToDoor = (transform.position - player.position).normalized;
                Vector3 playerLook = player.forward;

                float dot = Vector3.Dot(playerLook, dirToDoor);

                if (dot > lookAngleThreshold)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Debug.Log($"[LockedDoor]: Дверь заперта. Нажато Е строго со взглядом на дверь! Dot: {dot}");

                        if (lockedDoorSound != null)
                        {
                            if (lockedDoorSound.isPlaying) lockedDoorSound.Stop();

                            lockedDoorSound.Play();
                        }
                    }
                }
            }
        }
    }
}