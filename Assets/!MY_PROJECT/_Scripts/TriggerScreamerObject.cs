using UnityEngine;

public class TriggerScreamerObject : MonoBehaviour
{
    [Header("Кому запускать анимацию?")]
    public Animator wheelchairAnimator;

    [Header("Имя анимации в Аниматоре")]
    public string animationName = "Wheelchair_Roll";

    [Header("Звук для анимации")]
    public AudioSource soundSource;

    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;

            if (wheelchairAnimator != null)
            {
                wheelchairAnimator.Play(animationName);
                Debug.Log("Кресло покатилось!");
            }

            if (soundSource != null)
            {
                soundSource.Play();
                Debug.Log("Звук запущен!");
            }
        }
    }
}