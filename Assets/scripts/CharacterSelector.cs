using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Collections.Generic;

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
    public int index;
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
    public int Index { get; set; }

    public Character(CharacterData characterData)
    {
        Name = characterData.name;
        IsBought = characterData.isBought;
        Image = characterData.image;
        Price = characterData.price;
        Strength = characterData.strength;
        Speed = characterData.speed;
        HasSkill = characterData.hasSkill;
        Index = characterData.index;
    }
}

[System.Serializable]
public class CharacterDataArray
{
    public CharacterData[] characters;
    public int selectedCharacterIndex;
}

public class CharacterSelector : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text powerText;
    public TMP_Text speedText;
    public TMP_Text skillText;
    public TMP_Text price;
    public Button[] characterButtons;
    public RawImage image;
    public TMP_Text coinsText;
    public Button buyButton;
    public Button selectButton;
    public string saveFilePath = "/CharacterData.json";
    private string fullSavePath;
    static public List<CharacterData> characterList = new List<CharacterData>();
    public int selectedCharacterIndex = -1;
    public EXP expScript;

    private void Start()
    {
        

        fullSavePath = Application.persistentDataPath + saveFilePath;
        Debug.Log(fullSavePath);
        LoadData();
        
        UpdateCoinsText();

        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            characterButtons[i].onClick.RemoveAllListeners();
            characterButtons[i].onClick.AddListener(() =>
            {
                SelectCharacter(index);
            });
        }
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
                    UpdateUI();
                }
                selectedCharacterIndex = characterDataArray.selectedCharacterIndex;
                
            }
        }
        else
        {
            Debug.LogError("Unable to find CharacterData file");
        }
    }

    public void SaveData()
    {
        CharacterDataArray characterDataArray = new CharacterDataArray
        {
            characters = characterList.ToArray(),
            selectedCharacterIndex = selectedCharacterIndex
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
        GlobalContext.SelectedCharacter = new Character(characterList[selectedCharacterIndex]);
        
        SaveData();
        UpdateUI();
    }

    public void BuyCharacter()
    {
        Debug.Log("Buying Character with index: " + selectedCharacterIndex);

        if (GlobalContext.SelectedCharacter.IsBought)
        {
            Debug.LogError("Character already bought");
            return;
        }

        if (expScript.Coins >= GlobalContext.SelectedCharacter.Price)
        {
            expScript.BuyCharacter(GlobalContext.SelectedCharacter.Price);
            characterList[selectedCharacterIndex].isBought = true;
            GlobalContext.SelectedCharacter = new Character(characterList[selectedCharacterIndex]);
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
        if (GlobalContext.SelectedCharacter == null)
        {
            Debug.LogError("No character selected");
            return;
        }

        Character character = GlobalContext.SelectedCharacter;
        nameText.text = character.Name;
        powerText.text = $"{character.Strength}";
        speedText.text = $"{character.Speed}";
        skillText.text = $"{(character.HasSkill ? "Yes" : "No")}";
        price.text = character.Price.ToString();

        Texture2D loadedTexture = Resources.Load<Texture2D>(character.Image);
        if (loadedTexture == null)
        {
            Debug.LogError("Failed to load image at path " + character.Image);
        }
        else
        {
            image.texture = loadedTexture;
        }

        if (character.IsBought)
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
    public int SelectedCharacterIndex
    {
        get { return selectedCharacterIndex; }
    }
}
