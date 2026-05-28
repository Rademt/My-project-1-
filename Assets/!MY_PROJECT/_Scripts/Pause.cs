using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [Header("Папка BUTTONS з канвасу (вкладки ліворуч)")]
    public GameObject buttonsFolder;

    [Header("Папка PANELS з Канваса (контент налаштувань)")]
    public GameObject panelsFolder;

    [Header("Скрипт контролера гравця (PlayerMovement тощо)")]
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
        Time.timeScale = 0f;

        if (buttonsFolder != null) buttonsFolder.SetActive(true);
        if (panelsFolder != null) panelsFolder.SetActive(true);

        if (playerController != null) playerController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (buttonsFolder != null) buttonsFolder.SetActive(false);
        if (panelsFolder != null) panelsFolder.SetActive(false);

        if (playerController != null) playerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Demo1");
    }
}