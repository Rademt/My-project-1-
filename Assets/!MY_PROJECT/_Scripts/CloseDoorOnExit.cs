using UnityEngine;

public class CloseDoorOnExit : MonoBehaviour
{
    [Header("Какую дверь закрыть?")]
    public Animator doorAnimator;
    public string closeAnimationName = "Elevator_Door_Close";

    [Header("Звук закрытия")]
    public AudioSource closeSound;

    private bool isClosed = false;

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isClosed)
        {
            isClosed = true;

            if (doorAnimator != null)
            {
                doorAnimator.Play(closeAnimationName);
                Debug.Log("Игрок вышел. Двери лифта закрылись.");
            }

            if (closeSound != null)
            {
                closeSound.Play();
            }
        }
    }
}