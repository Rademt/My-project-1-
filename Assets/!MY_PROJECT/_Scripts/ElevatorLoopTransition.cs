using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ElevatorLoopTransition : MonoBehaviour
{
    [Header("Налаштування дверей")]
    public bool isCorrectChoice;

    [Header("Компоненти ліфта")]
    public Animator elevatorAnimator;
    public string closeAnimationName = "Elevator_Close";

    [Header("Звуковий супровід")]
    public AudioSource doorOpenSound;
    public AudioSource doorCloseSound;
    public AudioSource elevatorMusic;

    [Header("Логіка аномалій")]
    public AnomalyLogic anomalyLogic;

    [Header("Екран Перемоги (UI)")]
    public GameObject victoryMenuPanel;

    [Header("Скрипти для вимкнення після перемоги")]
    public GameObject pauseControllerObject;
    public MonoBehaviour playerMovementScript;

    private bool playerInside = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (doorOpenSound != null)
        {
            doorOpenSound.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
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

        if (doorCloseSound != null)
        {
            doorCloseSound.Play();
        }

        Debug.Log("Двері ліфта зачиняються...");

        Invoke("PlayElevatorMusic", 2.5f);

        Invoke("CompleteLoopTransition", 7f);
    }

    void PlayElevatorMusic()
    {
        if (elevatorMusic != null)
        {
            elevatorMusic.Play();
            Debug.Log("Ліфт рушив. Лунає музика супроводу.");
        }
    }

    void CompleteLoopTransition()
    {
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
            Debug.Log($"Правильно! Ліфт приїхав на поверх: {currentLoop}");

            if (currentLoop >= 1)
            {
                Debug.Log("ПЕРЕМОГА!");
                PlayerPrefs.SetInt("CurrentLoop", 0);
                PlayerPrefs.Save();

                TriggerVictoryWindow();
                return;
            }
        }
        else
        {
            Debug.Log("Помилка! Ліфт повертає вас на самий початок.");
            PlayerPrefs.SetInt("CurrentLoop", 0);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void TriggerVictoryWindow()
    {
        if (pauseControllerObject != null)
        {
            pauseControllerObject.SetActive(false);
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (victoryMenuPanel != null)
        {
            victoryMenuPanel.SetActive(true);
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMainMenu(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}