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

    // �������� ����� �� �������� ������ ����� �����
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
            SaveData();  // ��������� ������ ��� ����� ���������� ���������� �����
        }
    }
    public void BuyCharacter(int price)
    {
        // ���������, ��� � ������ ���������� �����
        if (Coins >= price)
        {
            // �������� ��������� ��������� �� ����� ������ � ��������� ������
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
        // �������� ����������� ������ ��� ������ ���� (���� ����)
        LoadData();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        // ���������� UI ������ ����
        UpdateUI();
    }

    // ���������� ����� ����� �������� ����
    public void AddPoints(string mobType)
    {
        // ���������� ����� � ����� � ����������� �� ���� ����
        coins += mobCoins[mobType];
        points += 1;

        // �������� �� ���������� ������
        CheckLevelUp();
    }

    // ���������� ����� ����� ��������� �����
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

    // ���������� UI
    private void UpdateUI()
    {
        pointsText.text = "" + points.ToString();
        levelText.text = "" + level.ToString();
        coinsText.text = "" + coins.ToString();
        gemsText.text = "" + gems.ToString();
        slider.value = 0.674f + ((float)points / 100) * (1 - 0.674f); // calculate slider value based on points
    }

    // ���������� ������ � JSON ��� ������ �� ����
    private void OnApplicationQuit()
    {
        SaveData();
    }

    // ���������� ������ � JSON
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

    // �������� ������ �� JSON
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
