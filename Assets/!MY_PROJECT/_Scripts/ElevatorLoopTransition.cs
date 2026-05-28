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

    [Header("Экран Победы (UI)")]
    [Tooltip("Перетащи сюда скопированную и переделанную плашку паузы")]
    public GameObject victoryMenuPanel;

    [Header("Скрипты для отключения при победе")]
    [Tooltip("Перетащи сюда объект Паузы (на котором висит скрипт паузы)")]
    public GameObject pauseControllerObject;
    [Tooltip("Перетащи сюда твоего Персонажа/Камеру (где висит управление мышью)")]
    public MonoBehaviour playerMovementScript;

    private bool playerInside = false;

    void Start()
    {
        // Убедимся, что время идет нормально (на случай, если вышли из меню или перезапустили)
        Time.timeScale = 1f;

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

        // 4. Через 2.5 секунды (когда двери почти захлопнулись) включаем музыку поездки
        Invoke("PlayElevatorMusic", 2.5f);

        // 5. Через 7 секунд завершаем поездку и переключаем этаж
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
        // Перед действиями глушим музыку, если она играет
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

            if (currentLoop >= 3)
            {
                Debug.Log("ПОБЕДА!");
                // Сбрасываем сохранения для следующего раза
                PlayerPrefs.SetInt("CurrentLoop", 0);
                PlayerPrefs.Save();

                // Вызываем наше окно победы вместо перезагрузки сцены
                TriggerVictoryWindow();
                return; // Выходим из метода, чтобы сцена НЕ перезагружалась!
            }
        }
        else
        {
            Debug.Log("Ошибка! Лифт возвращает вас в самое начало.");
            PlayerPrefs.SetInt("CurrentLoop", 0);
            PlayerPrefs.Save();
        }

        // Перезагружаем сцену, только если игра продолжается
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void TriggerVictoryWindow()
    {
        // 1. ПРИНУДИТЕЛЬНО ВЫКЛЮЧАЕМ СКРИПТ ПАУЗЫ, чтобы Escape больше не работал
        if (pauseControllerObject != null)
        {
            pauseControllerObject.SetActive(false);
        }

        // 2. ВЫКЛЮЧАЕМ УПРАВЛЕНИЕ ИГРОКОМ, чтобы его скрипт не прятал курсор каждый кадр
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        // 3. Показываем плашку выигрыша
        if (victoryMenuPanel != null)
        {
            victoryMenuPanel.SetActive(true);
        }

        // 4. Ставим игру на тотальную паузу
        Time.timeScale = 0f;

        // 5. Жестко освобождаем и показываем курсор мыши
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Этот метод привяжем на OnClick() кнопки «Главное меню» на экране победы
    public void LoadMainMenu(string sceneName)
    {
        Time.timeScale = 1f; // ОБЯЗАТЕЛЬНО возвращаем время в норму перед сменой сцены!
        SceneManager.LoadScene(sceneName);
    }
}