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

    [Header("📊 БАЛАНС АНОМАЛИЙ (ДИНАМИЧЕСКИЙ)")]
    [Tooltip("Базовый шанс аномалии на этаже (0-100)")]
    public int baseAnomalyChance = 65;
    [Tooltip("На сколько процентов повышать шанс, если прошлый этаж был чистым")]
    public int chanceStepUp = 15;
    [Tooltip("На сколько процентов урезать шанс, если на прошлом этаже БЫЛА аномалия")]
    public int chanceStepDown = 20;

    [Header("🎯 НАЛАШТУВАННЯ СКРИМЕРІВ")]
    [Tooltip("Максимальна кількість УНІКАЛЬНИХ скримерів за всю гру (проходження 8 поверхів)")]
    [Range(1, 4)] public int maxScreamersPerGame = 3;
    [Tooltip("Базовый шанс, що аномалія виявиться скримером")]
    [Range(0, 100)] public int screamerChance = 50;

    [Header("🛠️ РЕЖИМ ТЕСТУВАННЯ (ДЛЯ РОЗРОБНИКА)")]
    public bool debugMode = false;
    public int debugAnomalyIndex = 0;

    [Header("Список усіх аномалий (15 штук)")]
    public List<AdvancedAnomaly> allAnomalies;

    [HideInInspector]
    public bool wasAnomalySpawned = false;

    // Статические переменные (сохраняются при перезагрузке сцены между этажами)
    private static List<int> regularAnomalyPool = new List<int>();
    private static List<int> screamerPool = new List<int>();
    private static int screamersSpawnedInThisGame = 0;

    // Переменные "Игрового Режиссера" для контроля темпа
    private static int currentDynamicChance = 65;
    private static bool wasLastFloorScreamer = false;
    private static int consecutiveAnomaliesCount = 0;

    void Start()
    {
        Debug.Log($"[DEBUG] Головний трігер '{gameObject.name}' успішно активовано!");

#if UNITY_EDITOR
        if (Time.realtimeSinceStartup < 2f)
        {
            PlayerPrefs.SetInt("CurrentLoop", debugMode ? 1 : 0);
            PlayerPrefs.Save();
            ResetDirectorState();
            Debug.Log("<color=orange>[РЕДАКТОР]: Все системні счетчики режиссера сброшены!</color>");
        }
#endif

        wasAnomalySpawned = false;
        if (allAnomalies == null || allAnomalies.Count == 0) return;

        // Оригинальная чистка сцены (двери не пропадают)
        foreach (var anomaly in allAnomalies)
        {
            if (anomaly.anomalyObject != null) anomaly.anomalyObject.SetActive(false);
            if (anomaly.normalObject != null) anomaly.normalObject.SetActive(true);

            if (anomaly.screamerTriggerObject != null)
            {
                var wheelScript = anomaly.screamerTriggerObject.GetComponentInChildren<TriggerScreamerObject>();
                if (wheelScript != null) anomaly.screamerTriggerObject.SetActive(false);
                else
                {
                    var trapScript = anomaly.screamerTriggerObject.GetComponentInChildren<ToiletTrapAnomaly>();
                    if (trapScript != null) anomaly.screamerTriggerObject.SetActive(false);
                }
            }
        }

        Random.InitState(System.DateTime.Now.Millisecond + System.DateTime.Now.Second);
        int currentLoop = PlayerPrefs.GetInt("CurrentLoop", 0);

        if (currentLoop == 0)
        {
            ResetDirectorState();
            Debug.Log("<color=yellow>[ЭТАЖ 0]:</color> Ознакомление. Все счетчики сброшены.");
            return;
        }

        // Режим тестирования
        if (debugMode)
        {
            if (debugAnomalyIndex >= 0 && debugAnomalyIndex < allAnomalies.Count)
            {
                wasAnomalySpawned = true;
                ActivateAnomaly(allAnomalies[debugAnomalyIndex]);
            }
            return;
        }

        // Инициализация пулов, если пусты
        if (regularAnomalyPool.Count == 0 || screamerPool.Count == 0)
        {
            GenerateSeparatedPools();
        }

        // БРОСОК КУБИКА С УЧЕТОМ ДИНАМИЧЕСКОГО ШАНСА
        int globalRoll = Random.Range(0, 101);
        Debug.Log($"[РЕЖИССЕР]: Текущий шанс аномалии: {currentDynamicChance}%. Выпало кубиком: {globalRoll}");

        // Жесткое ограничение: не давать более 2 обычных аномалий подряд, чтобы игрок отдыхал
        if (consecutiveAnomaliesCount >= 2)
        {
            globalRoll = 999; // Принудительно делаем круг чистым
            Debug.Log("<color=cyan>[РЕЖИССЕР]: Принудительный чистый этаж для передышки игрока.</color>");
        }

        if (globalRoll <= currentDynamicChance)
        {
            wasAnomalySpawned = true;
            consecutiveAnomaliesCount++;
            int chosenIndex = -1;

            int screamerRoll = Random.Range(0, 101);

            // ПРОВЕРКА: Был ли скример на прошлом этаже? Если да, то сейчас принудительно спавним обычную аномалию
            if (wasLastFloorScreamer)
            {
                screamerRoll = 999; // Отключаем скример на этот ход
                Debug.Log("<color=yellow>[РЕЖИССЕР]: Скример заблокирован, так как на прошлом этаже уже был скример!</color>");
            }

            if (screamerRoll <= screamerChance && screamerPool.Count > 0 && screamersSpawnedInThisGame < maxScreamersPerGame)
            {
                chosenIndex = screamerPool[0];
                screamerPool.RemoveAt(0);
                screamersSpawnedInThisGame++;
                wasLastFloorScreamer = true; // Запоминаем, что этот этаж забрал скример
                Debug.Log($"<color=red>[РЕЖИССЕР]: Будет скример! Всего: {screamersSpawnedInThisGame}/{maxScreamersPerGame}</color>");
            }
            else if (regularAnomalyPool.Count > 0)
            {
                chosenIndex = regularAnomalyPool[0];
                regularAnomalyPool.RemoveAt(0);
                wasLastFloorScreamer = false; // Обычная аномалия — скример "спит"
            }

            if (chosenIndex >= 0 && chosenIndex < allAnomalies.Count)
            {
                ActivateAnomaly(allAnomalies[chosenIndex]);
            }

            // Корректируем динамический шанс вниз на следующий этаж (чтобы не было спама)
            currentDynamicChance = Mathf.Clamp(currentDynamicChance - chanceStepDown, 20, 90);
        }
        else
        {
            // ЭТАЖ ЧИСТЫЙ
            wasLastFloorScreamer = false;
            consecutiveAnomaliesCount = 0; // Сбрасываем счетчик аномалий подряд

            // Корректируем динамический шанс вверх (если игроку скучно, поднимаем шанс встретить аномалию дальше)
            currentDynamicChance = Mathf.Clamp(currentDynamicChance + chanceStepUp, 20, 90);
            Debug.Log($"<color=green>[ЧИСТЕ КОЛО]:</color> Следующий этаж будет иметь шанс аномалии: {currentDynamicChance}%");
        }
    }

    private void ResetDirectorState()
    {
        screamersSpawnedInThisGame = 0;
        currentDynamicChance = baseAnomalyChance;
        wasLastFloorScreamer = false;
        consecutiveAnomaliesCount = 0;
        regularAnomalyPool.Clear();
        screamerPool.Clear();
    }

    private void GenerateSeparatedPools()
    {
        regularAnomalyPool.Clear();
        screamerPool.Clear();

        for (int i = 0; i < allAnomalies.Count; i++)
        {
            if (allAnomalies[i].type == AnomalyType.TriggerScreamer)
                screamerPool.Add(i);
            else
                regularAnomalyPool.Add(i);
        }

        // Перемешивание обычных
        for (int i = regularAnomalyPool.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            int temp = regularAnomalyPool[i];
            regularAnomalyPool[i] = regularAnomalyPool[rnd];
            regularAnomalyPool[rnd] = temp;
        }

        // Перемешивание скримеров
        for (int i = 0; i < screamerPool.Count - 1; i++)
        {
            int rnd = Random.Range(0, screamerPool.Count);
            int temp = screamerPool[i];
            screamerPool[i] = screamerPool[rnd];
            screamerPool[rnd] = temp;
        }
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
                    if (toiletScript != null) { toiletScript.enabled = true; toiletScript.EnableScreamerAnomaly(); }

                    ToiletTrapAnomaly trapScript = anomaly.screamerTriggerObject.GetComponentInChildren<ToiletTrapAnomaly>();
                    if (trapScript != null) trapScript.EnableScreamerAnomaly();
                }
                break;
        }
        Debug.Log($"<color=cyan>[АКТИВНА АНОМАЛИЯ]:</color> {anomaly.name}");
    }
}