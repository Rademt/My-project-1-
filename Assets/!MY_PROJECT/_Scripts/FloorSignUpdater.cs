using UnityEngine;
using TMPro;

public class FloorSignUpdater : MonoBehaviour
{
    private TextMeshPro textMesh3D;
    private TextMeshProUGUI textMeshUI;

    void Start()
    {
        UpdateFloorText();
    }

    public void UpdateFloorText()
    {
        int currentLoop = PlayerPrefs.GetInt("CurrentLoop", 0);
        Debug.Log($"[FloorSignUpdater] Вывожу на стену уровень: {currentLoop}");

        textMesh3D = GetComponent<TextMeshPro>();
        if (textMesh3D != null)
        {
            textMesh3D.text = currentLoop.ToString();
            return;
        }

        textMeshUI = GetComponent<TextMeshProUGUI>();
        if (textMeshUI != null)
        {
            textMeshUI.text = currentLoop.ToString();
        }
    }

    [ContextMenu("Сбросить игру на 0 этаж")]
    public void ResetPrefsForDebug()
    {
        PlayerPrefs.SetInt("CurrentLoop", 0);
        PlayerPrefs.Save();
        UpdateFloorText();
        Debug.Log("<color=red>[DEBUG]: Все сохранения стерты вручную. Вы на 0-м этаже!</color>");
    }
}