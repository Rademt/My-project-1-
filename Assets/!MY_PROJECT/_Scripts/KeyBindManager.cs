using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Для работы с обычным Text
using TMPro;        // Для работы с TextMeshPro

public class KeyBindManager : MonoBehaviour
{
    // Словарь, где хранятся действия и связанные с ними кнопки
    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    [System.Serializable]
    public struct KeyBindUI
    {
        public string actionName; // Например: "Forward", "Backward"

        [Header("Перетащи сюда объект notassigned")]
        public GameObject textObject; // Берем GameObject, чтобы юнити точно дала его перетащить!
    }

    [Header("Список кнопок интерфейса")]
    public List<KeyBindUI> uiBinds;

    private string currentActionToRebind = "";
    private GameObject currentTextObjectToUpdate = null;

    void Awake()
    {
        // Инициализируем кнопки по умолчанию
        InitKey("Forward", KeyCode.W);
        InitKey("Backward", KeyCode.S);
        InitKey("Left", KeyCode.A);
        InitKey("Right", KeyCode.D);
        InitKey("Jump", KeyCode.Space);
        InitKey("Sprint", KeyCode.LeftShift);
        InitKey("Interact", KeyCode.E);

        // Обновляем текст на всех кнопках при старте
        UpdateAllUI();
    }

    void InitKey(string action, KeyCode defaultKey)
    {
        string savedKey = PlayerPrefs.GetString(action, defaultKey.ToString());
        KeyCode loadedKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), savedKey);

        if (keys.ContainsKey(action))
            keys[action] = loadedKey;
        else
            keys.Add(action, loadedKey);
    }

    void UpdateAllUI()
    {
        foreach (var bind in uiBinds)
        {
            if (keys.ContainsKey(bind.actionName) && bind.textObject != null)
            {
                SetTextOnObject(bind.textObject, keys[bind.actionName].ToString());
            }
        }
    }

    public void StartRebind(string actionName)
    {
        currentActionToRebind = actionName;

        foreach (var bind in uiBinds)
        {
            if (bind.actionName == actionName)
            {
                currentTextObjectToUpdate = bind.textObject;
                SetTextOnObject(currentTextObjectToUpdate, "PRESS ANY KEY...");
                break;
            }
        }
    }

    void OnGUI()
    {
        if (currentActionToRebind != "" && Event.current.isKey)
        {
            KeyCode newKey = Event.current.keyCode;

            if (newKey == KeyCode.Escape) return;

            keys[currentActionToRebind] = newKey;
            PlayerPrefs.SetString(currentActionToRebind, newKey.ToString());
            PlayerPrefs.Save();

            if (currentTextObjectToUpdate != null)
            {
                SetTextOnObject(currentTextObjectToUpdate, newKey.ToString());
            }

            currentActionToRebind = "";
            currentTextObjectToUpdate = null;
        }
    }

    // Вспомогательный метод, который сам разберется, Text там или TextMeshPro
    private void SetTextOnObject(GameObject obj, string text)
    {
        // Проверяем на обычный Text
        Text normalText = obj.GetComponent<Text>();
        if (normalText != null)
        {
            normalText.text = text;
            return;
        }

        // Проверяем на TextMeshPro
        TextMeshProUGUI tmproText = obj.GetComponent<TextMeshProUGUI>();
        if (tmproText != null)
        {
            tmproText.text = text;
            return;
        }
    }
}