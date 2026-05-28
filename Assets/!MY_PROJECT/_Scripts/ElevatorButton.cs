using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    [Header("Які двері відкривати?")]
    public Animator doorAnimator;
    public string openAnimationName = "Elevator_Door_Open";

    [Header("Налаштування затримки")]
    public float openDelay = 1.5f;

    [Header("Звук кнопки/ліфта")]
    public AudioSource buttonSound;
    public AudioSource doorOpenSound;

    private bool isPlayerNearby = false;
    private bool isCalled = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCalled)
        {
            isPlayerNearby = true;
            Debug.Log("Гравець підійшов до кнопки. Натисніть E.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isCalled)
        {
            TriggerElevatorCall();
        }
    }

    void TriggerElevatorCall()
    {
        isCalled = true;

        if (buttonSound != null)
        {
            buttonSound.Play();
        }

        Debug.Log($"Кнопка натиснута. Ліфт прибуде через {openDelay} сек...");

        Invoke("OpenElevatorDoors", openDelay);
    }

    void OpenElevatorDoors()
    {
        if (doorAnimator != null)
        {
            doorAnimator.Play(openAnimationName);
            Debug.Log("Ліфт приїхав, двері відчиняються!");
        }

        if (doorOpenSound != null)
        {
            doorOpenSound.Play();
        }
    }
}