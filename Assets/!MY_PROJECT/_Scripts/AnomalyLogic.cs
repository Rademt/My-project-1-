using UnityEngine;
using System.Collections.Generic;

public class AnomalyLogic : MonoBehaviour
{
    public enum AnomalyType { SwapObjects, JustSpawn, ScaleObject, TriggerScreamer }

    [System.Serializable]
    public class AdvancedAnomaly
    {
        public string name;
        public AnomalyType type;

        [Header("For Swapping / Spawning")]
        public GameObject normalObject;
        public GameObject anomalyObject;

        [Header("For Scaling")]
        public GameObject objectToScale;
        public Vector3 targetScale = new Vector3(2f, 2f, 2f);

        [Header("For Custom Scripts (like Screamers)")]
        public GameObject screamerTriggerObject;
    }

    [Tooltip("Шанс появи аномалії (0-100)")]
    public int AnomalyChance = 50;

    [Header("🛠️ РЕЖИМ ТЕСТУВАННЯ (ДЛЯ РОЗРОБНИКА)")]
    [Tooltip("Якщо включено, рандом відключається, і завжди спавниться аномалія, вибрана нижче")]
    public bool debugMode = false;
    [Tooltip("Індекс аномалії зі списку нижче, котру требя примусово увімкнути (0, 1, 2...)")]
    public int debugAnomalyIndex = 0;

    [Header("Список усіх аномалій")]
    public List<AdvancedAnomaly> allAnomalies;

    [HideInInspector]
    public bool wasAnomalySpawned = false;

    private static List<int> anomalyPool = new List<int>();

    void Start()
    {
        Debug.Log($"[DEBUG] Головний трігер '{gameObject.name}' успішно активовано на scenic й готовий ловити гравця!");

#if UNITY_EDITOR
        if (Time.realtimeSinceStartup < 2f)
        {
            PlayerPrefs.SetInt("CurrentLoop", debugMode ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log("<color=orange>[РЕДАКТОР]: Налаштування поверхів оптимізовано для тестування!</color>");
        }
#endif

        wasAnomalySpawned = false;
        if (allAnomalies == null || allAnomalies.Count == 0) return;

        foreach (var anomaly in allAnomalies)
        {
            if (anomaly.anomalyObject != null) anomaly.anomalyObject.SetActive(false);
            if (anomaly.normalObject != null) anomaly.normalObject.SetActive(true);

            if (anomaly.screamerTriggerObject != null)
            {
                var wheelScript = anomaly.screamerTriggerObject.GetComponentInChildren<TriggerScreamerObject>();

                if (wheelScript != null)
                {
                    anomaly.screamerTriggerObject.SetActive(false);
                }
                else
                {
                    var trapScript = anomaly.screamerTriggerObject.GetComponentInChildren<ToiletTrapAnomaly>();

                    if (trapScript != null)
                    {
                        anomaly.screamerTriggerObject.SetActive(false);
                    }
                }
            }
        }

        Random.InitState(System.DateTime.Now.Millisecond + System.DateTime.Now.Second);
        int currentLoop = PlayerPrefs.GetInt("CurrentLoop", 0);

        if (debugMode)
        {
            if (debugAnomalyIndex >= 0 && debugAnomalyIndex < allAnomalies.Count)
            {
                wasAnomalySpawned = true;
                ActivateAnomaly(allAnomalies[debugAnomalyIndex]);
                Debug.Log($"<color=magenta>[DEBUG РЕЖИМ]: Аномалія була запущена примусово №{debugAnomalyIndex} — {allAnomalies[debugAnomalyIndex].name}!</color>");
            }
            else
            {
                Debug.LogError($"[DEBUG ПОМИЛКА]: Індекс {debugAnomalyIndex} немає у списку аномалій!");
            }
            return;
        }

        if (currentLoop == 0)
        {
            Debug.Log("<color=yellow>[ЭТАЖ 0]:</color> Ознайомлювальне коло. Абсолютна чистота.");
            return;
        }

        int globalRoll = Random.Range(0, 101);

        if (globalRoll <= AnomalyChance)
        {
            wasAnomalySpawned = true;

            if (anomalyPool.Count == 0 || anomalyPool.Count > allAnomalies.Count)
            {
                GenerateAnomalyPool();
            }

            int chosenIndex = anomalyPool[0];
            anomalyPool.RemoveAt(0);

            if (chosenIndex >= 0 && chosenIndex < allAnomalies.Count)
            {
                ActivateAnomaly(allAnomalies[chosenIndex]);
            }
        }
        else
        {
            Debug.Log($"<color=green>[ЧИСТЕ КОЛО]:</color> Рол не пройшов. Колісниця спить. Поверх: {currentLoop}");
        }
    }

    private void GenerateAnomalyPool()
    {
        anomalyPool.Clear();

        for (int i = 0; i < allAnomalies.Count; i++)
        {
            anomalyPool.Add(i);
        }

        for (int i = anomalyPool.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            int temp = anomalyPool[i];
            anomalyPool[i] = anomalyPool[rnd];
            anomalyPool[rnd] = temp;
        }

        Debug.Log($"<color=orange>[УМНА СУМКА]: Колоду аномалій успішно перемішано! Доступно унікальних подій: {anomalyPool.Count}</color>");
    }

    private void ActivateAnomaly(AdvancedAnomaly anomaly)
    {
        switch (anomaly.type)
        {
            case AnomalyType.SwapObjects:
                if (anomaly.normalObject != null) anomaly.normalObject.SetActive(false);
                if (anomaly.anomalyObject != null) anomaly.anomalyObject.SetActive(true);
                break;

            case AnomalyType.JustSpawn:
                if (anomaly.anomalyObject != null) anomaly.anomalyObject.SetActive(true);
                break;

            case AnomalyType.ScaleObject:
                if (anomaly.objectToScale != null) anomaly.objectToScale.transform.localScale = anomaly.targetScale;
                break;

            case AnomalyType.TriggerScreamer:
                if (anomaly.screamerTriggerObject != null)
                {
                    anomaly.screamerTriggerObject.SetActive(true);

                    ToiletDoorScreamer toiletScript = anomaly.screamerTriggerObject.GetComponentInChildren<ToiletDoorScreamer>();
                    if (toiletScript != null)
                    {
                        toiletScript.enabled = true;
                        toiletScript.EnableScreamerAnomaly();
                    }

                    ToiletTrapAnomaly trapScript = anomaly.screamerTriggerObject.GetComponentInChildren<ToiletTrapAnomaly>();
                    if (trapScript != null)
                    {
                        trapScript.EnableScreamerAnomaly();
                    }

                    TriggerScreamerObject wheelScript = anomaly.screamerTriggerObject.GetComponentInChildren<TriggerScreamerObject>();
                    if (wheelScript != null)
                    {
                        Debug.Log($"[AnomalyLogic] Колісниця '{anomaly.screamerTriggerObject.name}' активована як АНОМАЛІЯ!");
                    }
                }
                break;
        }

        Debug.Log($"<color=cyan>[ВИБРАНО АНОМАЛІЮ]:</color> {anomaly.name}");
    }
}