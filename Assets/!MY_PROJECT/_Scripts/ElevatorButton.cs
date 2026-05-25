using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    [Header("Какую дверь открывать?")]
    public Animator doorAnimator;
    public string openAnimationName = "Elevator_Door_Open";

    [Header("Настройки задержки")]
    [Tooltip("Задержка перед открытием дверей в секундах")]
    public float openDelay = 1.5f;

    [Header("Звук кнопки/лифта")]
    public AudioSource buttonSound;   // Звук клика самой кнопки (играет СРАЗУ)
    public AudioSource doorOpenSound; // Звук открытия дверей лифта (играет С ЗАДЕРЖКОЙ)

    private bool isPlayerNearby = false;
    private bool isCalled = false; // Чтобы игрок не нажимал кнопку по сто раз, пока лифт "едет"

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCalled)
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
        // Проверяем, что игрок рядом, нажал E и лифт еще не вызван
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isCalled)
        {
            TriggerElevatorCall();
        }
    }

    void TriggerElevatorCall()
    {
        isCalled = true; // Блокируем повторные нажатия

        // 1. СРАЗУ воспроизводим звук нажатия кнопки
        if (buttonSound != null)
        {
            buttonSound.Play();
        }

        Debug.Log($"Кнопка нажата. Лифт приедет через {openDelay} сек...");

        // 2. Запускаем открытие дверей с задержкой через Invoke
        Invoke("OpenElevatorDoors", openDelay);
    }

    // Этот метод вызовется автоматически через указанное в openDelay время
    void OpenElevatorDoors()
    {
        // 3. Запускаем анимацию дверей
        if (doorAnimator != null)
        {
            doorAnimator.Play(openAnimationName);
            Debug.Log("Лифт приехал, двери открываются!");
        }

        // 4. Включаем звук открытия дверей
        if (doorOpenSound != null)
        {
            doorOpenSound.Play();
        }
    }
}