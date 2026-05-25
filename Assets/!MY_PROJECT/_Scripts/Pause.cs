using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [Header("Папка BUTTONS из Канваса (вкладки слева)")]
    public GameObject buttonsFolder;

    [Header("Папка PANELS из Канваса (контент настроек)")]
    public GameObject panelsFolder;

    [Header("Скрипт контроллера игрока (PlayerMovement или т.п.)")]
    public MonoBehaviour playerController;

    [HideInInspector]
    public bool isPaused = false;

    void Start()
    {
        ResumeGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Стопим время

        // Включаем папки с кнопками и всеми панелями разом
        if (buttonsFolder != null) buttonsFolder.SetActive(true);
        if (panelsFolder != null) panelsFolder.SetActive(true);

        // Вырубаем контроллер игрока, чтобы разлочить мышь
        if (playerController != null) playerController.enabled = false;

        // Включаем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Время пошло

        // Прячем весь интерфейс паузы
        if (buttonsFolder != null) buttonsFolder.SetActive(false);
        if (panelsFolder != null) panelsFolder.SetActive(false);

        // Возвращаем управление игроку
        if (playerController != null) playerController.enabled = true;

        // Прячем курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Demo1");
    }
}