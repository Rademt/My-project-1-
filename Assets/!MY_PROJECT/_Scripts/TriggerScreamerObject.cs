using UnityEngine;

public class TriggerScreamerObject : MonoBehaviour
{
    [Header("Кому запускати анімацію?")]
    public Animator wheelchairAnimator;

    [Header("Назва анімації в Аніматорі")]
    public string animationName = "Wheelchair_Roll";

    [Header("Звук для анімації")]
    public AudioSource soundSource;
    public AudioSource hit;

    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;

            if (wheelchairAnimator != null)
            {
                wheelchairAnimator.Play(animationName);
                Debug.Log("Крісло покатилось!");
            }

            if (soundSource != null)
            {
                soundSource.Play();
                Debug.Log("Звук увімкнено!");
                hit.Play();
                Debug.Log("БАМ");
            }
        }
    }
}