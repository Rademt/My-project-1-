using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public Transform player;
    public float interactDist = 4f;
    public float openAngle = 90f;
    public float smooth = 6f;

    [Header("Звуки двери")]
    public AudioSource openSound;
    public AudioSource closeSound;

    private bool isOpen = false;
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
                        Debug.Log(isOpen ? "Открываем одиночную дверь" : "Закрываем одиночную дверь");

                        if (isOpen)
                        {
                            if (openSound != null) openSound.Play();
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
}