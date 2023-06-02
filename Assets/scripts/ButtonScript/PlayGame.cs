using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayGame : MonoBehaviour
{
    private CharacterSelector characterSelector;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Load()
    {
        characterSelector.SelectCharacterAsCurrent();
        SceneManager.LoadScene("SampleScene");
    }
    public void LoadShop()
    {
        SceneManager.LoadScene("Shop");
    }
    public void LoadPlayers()
    {
        SceneManager.LoadScene("Player");
    }
    public void LoadHome()
    {
        SceneManager.LoadScene("Menu");
    }
    public void EventsLoad()
    {
        SceneManager.LoadScene("Eventusing UnityEngine;\r\nusing UnityEngine.UI;\r\nusing System.IO;\r\nusing TMPro;\r\nusing System.Collections.Generic;\r\n\r\n[System.Serializable]\r\npublic class CharacterData\r\n{\r\n    public string name;\r\n    public bool isBought;\r\n    public string image;\r\n    public int price;  // Цена персонажа\r\n    public string stats;\r\n}\r\npublic static class JsonHelper\r\n{\r\n    public static T[] FromJson<T>(string json)\r\n    {\r\n        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);\r\n        return wrapper.items;\r\n    }\r\n\r\n    public static string ToJson<T>(T[] array, bool prettyPrint)\r\n    {\r\n        Wrapper<T> wrapper = new Wrapper<T>();\r\n        wrapper.items = array;\r\n        return JsonUtility.ToJson(wrapper, prettyPrint);\r\n    }\r\n\r\n    [System.Serializable]\r\n    private class Wrapper<T>\r\n    {\r\n        public T[] items;\r\n    }\r\n}\r\n\r\npublic class CharacterSelector : MonoBehaviour\r\n{\r\n    public TMP_Text nameText;\r\n    public TMP_Text statsText;\r\n    public Image image;\r\n    public TMP_Text coinsText;\r\n    public Button buyButton;\r\n    public Button selectButton;\r\n    public string saveFilePath = \"/characterData.json\";\r\n    private string fullSavePath;\r\n    private List<CharacterData> characterList = new List<CharacterData>();\r\n    private int selectedCharacterIndex = -1;\r\n    private int coins = 1000;  // Предположим, что игрок начинает с 1000 монет\r\n\r\n    private void Start()\r\n    {\r\n        fullSavePath = Application.persistentDataPath + saveFilePath;\r\n        LoadData();\r\n        UpdateCoinsText();\r\n    }\r\n    public static class JsonHelper\r\n{\r\n    public static T[] FromJson<T>(string json)\r\n    {\r\n        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);\r\n        return wrapper.items;\r\n    }\r\n\r\n    public static string ToJson<T>(T[] array, bool prettyPrint)\r\n    {\r\n        Wrapper<T> wrapper = new Wrapper<T>();\r\n        wrapper.items = array;\r\n        return JsonUtility.ToJson(wrapper, prettyPrint);\r\n    }\r\n\r\n    [System.Serializable]\r\n    private class Wrapper<T>\r\n    {\r\n        public T[] items;\r\n    }\r\n}\r\n\r\n    private void LoadData()\r\n    {\r\n        // Загрузка данных из файла, если он существует\r\n        if (File.Exists(fullSavePath))\r\n        {\r\n            string json = File.ReadAllText(fullSavePath);\r\n            CharacterData[] characters = JsonHelper.FromJson<CharacterData>(json);\r\n\r\n            if (characters != null)\r\n            {\r\n                characterList = new List<CharacterData>(characters);\r\n                if (characterList.Count > 0)\r\n                {\r\n                    selectedCharacterIndex = 0;\r\n                    UpdateUI();\r\n                }\r\n            }\r\n        }\r\n    }\r\n\r\n    public void SaveData()\r\n    {\r\n        // Сохранение данных в файл\r\n        string json = JsonHelper.ToJson(characterList.ToArray(), true);\r\n        File.WriteAllText(fullSavePath, json);\r\n    }\r\n\r\n    public void SelectCharacter(int index)\r\n    {\r\n        if (index < 0 || index >= characterList.Count)\r\n        {\r\n            Debug.LogError(\"Invalid character index\");\r\n            return;\r\n        }\r\n\r\n        selectedCharacterIndex = index;\r\n        UpdateUI();\r\n    }\r\n\r\n    public void BuyCharacter()\r\n    {\r\n        if (selectedCharacterIndex < 0 || selectedCharacterIndex >= characterList.Count)\r\n        {\r\n            Debug.LogError(\"Invalid character index\");\r\n            return;\r\n        }\r\n\r\n        if (characterList[selectedCharacterIndex].isBought)\r\n        {\r\n            Debug.LogError(\"Character already bought\");\r\n            return;\r\n        }\r\n\r\n        if (coins < characterList[selectedCharacterIndex].price)\r\n        {\r\n            Debug.LogError(\"Not enough coins to buy character\");\r\n            return;\r\n        }\r\n\r\n        coins -= characterList[selectedCharacterIndex].price;\r\n        characterList[selectedCharacterIndex].isBought = true;\r\n\r\n        UpdateCoinsText();\r\n        UpdateUI();\r\n        SaveData();\r\n    }\r\n\r\n    private void UpdateUI()\r\n    {\r\n        if (selectedCharacterIndex < 0 || selectedCharacterIndex >= characterList.Count)\r\n        {\r\n            Debug.LogError(\"Invalid character index\");\r\n            return;\r\n        }\r\n\r\n        CharacterData character = characterList[selectedCharacterIndex];\r\n        nameText.text = character.name;\r\n        statsText.text = character.stats;\r\n        // Для этого вы должны загрузить изображение из ресурсов\r\n        // image.sprite = Resources.Load<Sprite>(character.image);\r\n        buyButton.gameObject.SetActive(!character.isBought);\r\n        selectButton.gameObject.SetActive(character.isBought);\r\n    }\r\n\r\n    private void UpdateCoinsText()\r\n    {\r\n        coinsText.text = $\"{coins}\";\r\n    }\r\n}\r\nusing UnityEngine;");
    }
}
