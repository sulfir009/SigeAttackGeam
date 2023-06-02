using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class EXP : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI gemsText;
    public int points;
    public int level;
    public int coins;
    public int gems;

    // «начени€ монет за убийство разных типов мобов
    private Dictionary<string, int> mobCoins = new Dictionary<string, int>() {
        { "weakSkeleton", 2 },
        { "mediumSkeleton", 3 },
        { "dragon", 5 },
        { "fox", 4 }
    };
    public int Coins
    {
        get { return coins; }
        set
        {
            coins = value;
            SaveData();  // сохран€ем данные при любых изменени€х количества монет
        }
    }
    public void BuyCharacter(int price)
    {
        // ѕровер€ем, что у игрока достаточно монет
        if (Coins >= price)
        {
            // ¬ычитаем стоимость персонажа из монет игрока и сохран€ем данные
            Coins -= price;
        }
        else
        {
            Debug.LogError("Not enough coins to buy character");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // «агрузка сохраненных данных при старте игры (если есть)
        LoadData();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        // ќбновление UI каждый кадр
        UpdateUI();
    }

    // ƒобавление очков после убийства моба
    public void AddPoints(string mobType)
    {
        // ”величение монет и очков в зависимости от типа моба
        coins += mobCoins[mobType];
        points += 1;

        // ѕроверка на увеличение уровн€
        CheckLevelUp();
    }

    // ƒобавление очков после выживани€ волны
    public void SurviveWave()
    {
        coins += 5;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (points >= 100)
        {
            level += 1;
            points -= 100;
            coins += 2000;
            gems += 10;
            slider.value = 0.674f; // reset slider when level up
        }
    }

    // ќбновление UI
    private void UpdateUI()
    {
        pointsText.text = "" + points.ToString();
        levelText.text = "" + level.ToString();
        coinsText.text = "" + coins.ToString();
        gemsText.text = "" + gems.ToString();
        slider.value = 0.674f + ((float)points / 100) * (1 - 0.674f); // calculate slider value based on points
    }

    // —охранение данных в JSON при выходе из игры
    private void OnApplicationQuit()
    {
        SaveData();
    }

    // —охранение данных в JSON
    private void SaveData()
    {
        PlayerData data = new PlayerData()
        {
            points = points,
            level = level,
            coins = coins,
            gems = gems
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    // «агрузка данных из JSON
    private void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            points = data.points;
            level = data.level;
            coins = data.coins;
            gems = data.gems;
        }
    }

    [System.Serializable]
    class PlayerData
    {
        public int points;
        public int level;
        public int coins;
        public int gems;
    }
}
