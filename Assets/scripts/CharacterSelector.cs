using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class CharacterData
{
    public string name;
    public bool isBought;
    public string image;
    public int price;
    public int strength;
    public int speed;
    public bool hasSkill;
}
public class Character
{
    public string Name { get; set; }
    public bool IsBought { get; set; }
    public string Image { get; set; }
    public int Price { get; set; }
    public int Strength { get; set; }
    public int Speed { get; set; }
    public bool HasSkill { get; set; }

    public Character(CharacterData characterData)
    {
        Name = characterData.name;
        IsBought = characterData.isBought;
        Image = characterData.image;
        Price = characterData.price;
        Strength = characterData.strength;
        Speed = characterData.speed;
        HasSkill = characterData.hasSkill;
    }
}
[System.Serializable]
public class CharacterDataArray
{
    public CharacterData[] characters;
    public string selectedCharacterName;
}

public class CharacterSelector : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text powerText;
    public TMP_Text speedText;
    public TMP_Text skillText;
    public RawImage image;
    public TMP_Text coinsText;
    public Button buyButton;
    public Button selectButton;
    public string saveFilePath = "/CharacterData.json";
    private string fullSavePath;
    private List<CharacterData> characterList = new List<CharacterData>();
    private int selectedCharacterIndex = -1;
    private string selectedCharacterName = null;
    public EXP expScript;

    private void Start()
    {
        fullSavePath = Application.persistentDataPath + saveFilePath;
        LoadData();
        UpdateCoinsText();
    }

    private void LoadData()
    {
        if (File.Exists(fullSavePath))
        {
            string json = File.ReadAllText(fullSavePath);
            CharacterDataArray characterDataArray = JsonUtility.FromJson<CharacterDataArray>(json);

            if (characterDataArray.characters != null)
            {
                characterList = new List<CharacterData>(characterDataArray.characters);
                if (characterList.Count > 0)
                {
                    selectedCharacterIndex = 0;
                    UpdateUI();
                }
                selectedCharacterName = characterDataArray.selectedCharacterName;
            }
        }
        else
        {
            Debug.LogError("Unable to find CharacterData file in persistent data path");
        }
    }

    public void SaveData()
    {
        CharacterDataArray characterDataArray = new CharacterDataArray
        {
            characters = characterList.ToArray(),
            selectedCharacterName = selectedCharacterName
        };

        string json = JsonUtility.ToJson(characterDataArray, true);
        File.WriteAllText(fullSavePath, json);
    }

    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= characterList.Count)
        {
            Debug.LogError("Invalid character index");
            return;
        }

        selectedCharacterIndex = index;
        UpdateUI();
    }

    public void BuyCharacter()
    {
        if (selectedCharacterIndex < 0 || selectedCharacterIndex >= characterList.Count)
        {
            Debug.LogError("Invalid character index");
            return;
        }

        if (characterList[selectedCharacterIndex].isBought)
        {
            Debug.LogError("Character already bought");
            return;
        }

        if (expScript.Coins >= characterList[selectedCharacterIndex].price)
        {
            expScript.BuyCharacter(characterList[selectedCharacterIndex].price);
            characterList[selectedCharacterIndex].isBought = true;
            UpdateUI();
            SaveData();
        }
        else
        {
            Debug.LogError("Not enough coins to buy character");
        }
    }

    private void UpdateUI()
    {
        if (selectedCharacterIndex < 0 || selectedCharacterIndex >= characterList.Count)
        {
            Debug.LogError("Invalid character index");
            return;
        }

        CharacterData character = characterList[selectedCharacterIndex];
        nameText.text = character.name;
        powerText.text = $"{character.strength}";
        speedText.text = $"{character.speed}";
        skillText.text = $"{(character.hasSkill ? "Yes" : "No")}";

        Texture2D loadedTexture = Resources.Load<Texture2D>(character.image);
        if (loadedTexture == null)
        {
            Debug.LogError("Failed to load image at path " + character.image);
        }
        else
        {
            image.texture = loadedTexture;
        }

        if (character.isBought)
        {
            buyButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);
        }
    }

    private void UpdateCoinsText()
    {
        coinsText.text = $"{expScript.coins}";
    }
    public static class GlobalContext
    {
        public static Character SelectedCharacter { get; set; }
    }
    public void SelectCharacterAsCurrent()
    {
        if (selectedCharacterIndex < 0 || selectedCharacterIndex >= characterList.Count)
        {
            Debug.LogError("Invalid character index");
            return;
        }

        GlobalContext.SelectedCharacter = new Character(characterList[selectedCharacterIndex]);
    }

    public void SelectCharacterByName(string characterName)
    {
        int index = characterList.FindIndex(character => character.name == characterName);
        Debug.Log(Application.persistentDataPath);
        if (index != -1)
        {
            SelectCharacter(index);
        }
        else
        {
            Debug.LogError("Character with name " + characterName + " not found");
        }
    }
}
