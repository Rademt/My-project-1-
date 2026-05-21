using UnityEngine;
using System.Collections;

public class SimpleDoor : MonoBehaviour
{
    public Transform player;
    public float interactDist = 4f;
    public float openAngle = 90f;
    public float smooth = 6f;

    private bool isOpen = false;
    private Quaternion defaultRotation;
    private Quaternion openRotation;

    void Start()
    {
        defaultRotation = transform.localRotation;
        openRotation = defaultRotation * Quaternion.Euler(0, openAngle, 0);
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
                    isOpen = !isOpen;
                    Debug.Log(isOpen ? "Открываем дверь" : "Закрываем дверь");
                }
            }
        }

        Quaternion targetRotation = isOpen ? openRotation : defaultRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }
}