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
    public AudioSource elevatorSound; // Звук движения/закрытия лифта

    [Header("Логика аномалий")]
    public AnomalyLogic anomalyLogic; // Перетащи сюда наш менеджер аномалий

    private bool playerInside = false;

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
        if (elevatorAnimator != null)
        {
            elevatorAnimator.Play(closeAnimationName);
        }

        if (elevatorSound != null)
        {
            elevatorSound.Play();
        }

        Debug.Log("Двери лифта закрылись. Поездка началась...");

        Invoke("CompleteLoopTransition", 5f);
    }

    void CompleteLoopTransition()
    {
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