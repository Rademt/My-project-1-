using UnityEngine;
using System.Collections;
using TMPro; // Якщо використовуєш TextMeshPro

public class Simple_Loading : MonoBehaviour
{
    [Header("Час показу плашки")]
    public float waitTime = 3f; // Скільки секунд триматиметься чорний екран

    private CanvasGroup canvasGroup;

    void Awake()
    {
        // Додаємо або знаходимо компонент для плавного зникнення
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Робимо панель повністю видимою та блокуючою кліки на старті
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    void Start()
    {
        // Запускаємо таймер зникнення
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        // Просто чекаємо, поки гра повністю "прокинеться" у фоні
        yield return new WaitForSeconds(waitTime);

        // Плавно зменшуємо видимість до нуля
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * 1.5f; // Швидкість зникнення
            yield return null;
        }

        // Повністю вимикаємо об'єкт, щоб він не заважав грати
        gameObject.SetActive(false);
    }
}