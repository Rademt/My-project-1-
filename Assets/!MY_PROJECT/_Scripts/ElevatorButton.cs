using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    [Header("Какую дверь открывать?")]
    public Animator doorAnimator;
    public string openAnimationName = "Elevator_Door_Open";

    [Header("Звук кнопки/лифта")]
    public AudioSource buttonSound;

    private bool isPlayerNearby = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Игрок подошел к кнопке. Нажмите E.");
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
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TriggerElevatorCall();
        }
    }

    void TriggerElevatorCall()
    {
        if (doorAnimator != null)
        {
            doorAnimator.Play(openAnimationName);
            Debug.Log("Лифт вызван, двери открываются!");
        }

        if (buttonSound != null)
        {
            buttonSound.Play();
        }
    }
}