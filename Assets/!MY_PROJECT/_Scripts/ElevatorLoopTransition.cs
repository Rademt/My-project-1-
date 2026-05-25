using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ElevatorLoopTransition : MonoBehaviour
{
    [Header("Настройки дверей")]
    public bool isCorrectChoice; // Это правильный путь (вперед)?

    [Header("Компоненты лифта")]
    public Animator elevatorAnimator; // Аниматор дверей лифта
    public string closeAnimationName = "Elevator_Close";

    [Header("Звуковое сопровождение")]
    public AudioSource doorOpenSound;   // Звук ОТКРЫТИЯ дверей (играет сам при старте этажа)
    public AudioSource doorCloseSound;  // Звук ЗАКРЫТИЯ дверей (играет при входе игрока)
    public AudioSource elevatorMusic;   // Хоррор-музыка поездки лифта

    [Header("Логика аномалий")]
    public AnomalyLogic anomalyLogic; // Перетащи сюда наш менеджер аномалий

    private bool playerInside = false;

    void Start()
    {
        // 1. Сразу при загрузке сцены/этажа включаем звук открытия дверей
        if (doorOpenSound != null)
        {
            doorOpenSound.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Если зашел игрок и процесс еще не запущен
        if (other.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            StartElevatorProcess();
        }
    }

    void StartElevatorProcess()
    {
        // 2. Запускаем анимацию закрытия дверей
        if (elevatorAnimator != null)
        {
            elevatorAnimator.Play(closeAnimationName);
        }

        // 3. Включаем звук закрывания дверей
        if (doorCloseSound != null)
        {
            doorCloseSound.Play();
        }

        Debug.Log("Двери лифта закрываются...");

        // 4. Через 1.5 секунды (когда двери почти захлопнулись) включаем музыку поездки
        Invoke("PlayElevatorMusic", 2.5f);

        // 5. Через 5 секунд завершаем поездку и переключаем этаж
        Invoke("CompleteLoopTransition", 7f);
    }

    void PlayElevatorMusic()
    {
        if (elevatorMusic != null)
        {
            elevatorMusic.Play();
            Debug.Log("Лифт поехал. Играет музыка поездки.");
        }
    }

    void CompleteLoopTransition()
    {
        // Перед перезагрузкой сцены глушим музыку, если она играет
        if (elevatorMusic != null) elevatorMusic.Stop();

        int currentLoop = PlayerPrefs.GetInt("CurrentLoop", 0);

        bool thereWasAnomaly = false;
        if (anomalyLogic != null)
        {
            thereWasAnomaly = anomalyLogic.wasAnomalySpawned;
        }

        bool playerGuessedRight = false;

        if (thereWasAnomaly)
        {
            playerGuessedRight = !isCorrectChoice;
        }
        else
        {
            playerGuessedRight = isCorrectChoice;
        }

        if (playerGuessedRight)
        {
            currentLoop++;
            PlayerPrefs.SetInt("CurrentLoop", currentLoop);
            PlayerPrefs.Save();
            Debug.Log($"Правильно! Лифт приехал на этаж: {currentLoop}");

            if (currentLoop >= 8)
            {
                Debug.Log("ПОБЕДА!");
                PlayerPrefs.SetInt("CurrentLoop", 0);
                PlayerPrefs.Save();
            }
        }
        else
        {
            Debug.Log("Ошибка! Лифт возвращает вас в самое начало.");
            PlayerPrefs.SetInt("CurrentLoop", 0);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}