using UnityEngine;

public class DoubleDoor : MonoBehaviour
{
    public Transform leftHinge;
    public Transform rightHinge;
    public float openAngle = 90f;
    public float smooth = 6f;

    [Header("Звуки двері")]
    public AudioSource openSound;
    public AudioSource closeSound;

    private bool isOpen = false;
    private bool playerIsNear = false;
    private Quaternion leftDefaultRot;
    private Quaternion rightDefaultRot;

    void Start()
    {
        leftDefaultRot = leftHinge.localRotation;
        rightDefaultRot = rightHinge.localRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerIsNear)
        {
            isOpen = !isOpen;

            if (isOpen)
            {
                if (openSound != null) openSound.Play();
            }
            else
            {
                if (closeSound != null) closeSound.Play();
            }
        }



        Quaternion leftTarget = leftDefaultRot * Quaternion.Euler(0, -openAngle, 0);
        Quaternion rightTarget = rightDefaultRot * Quaternion.Euler(0, openAngle, 0);

        Quaternion targetL = isOpen ? leftTarget : leftDefaultRot;
        Quaternion targetR = isOpen ? rightTarget : rightDefaultRot;

        leftHinge.localRotation = Quaternion.Slerp(leftHinge.localRotation, targetL, Time.deltaTime * smooth);
        rightHinge.localRotation = Quaternion.Slerp(rightHinge.localRotation, targetR, Time.deltaTime * smooth);
    }
}