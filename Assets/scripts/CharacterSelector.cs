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
    public Button uppButton;
    public Button selectButton;
    public string saveFilePath = "/CharacterData.json";
    private string fullSavePath;
    public List<CharacterData> characterList = new List<CharacterData>();
    public int selectedCharacterIndex = 0;
    public EXP expScript;

    private void Start()
    {
        uppButton.onClick.AddListener(UpgradeCharacter);
        fullSavePath = Application.persistentDataPath + saveFilePath;
        Debug.Log(fullSavePath);
        LoadData();

        // выбираем и покупаем персонажа "Fox" по умолчанию, если он еще не куплен
        CharacterData foxCharacter = characterList.Find(c => c.name == "Fox");
        if (foxCharacter != null && !foxCharacter.isBought)
        {
            foxCharacter.isBought = true;
            SelectCharacter(characterList.IndexOf(foxCharacter));
        }

        UpdateCoinsText();
        buyButton.onClick.AddListener(() =>
        {
            if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characterList.Count)
            {
                BuyCharacter();
            }
        });
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() =>
            {
                SelectCharacter(index);
            });
        }
    }
    public void UpgradeCharacter()
    {
        if (selectedCharacterIndex < 0 || selectedCharacterIndex >= characterList.Count)
        {
            Debug.LogError("Invalid character index");
            return;
        }

        CharacterData selectedCharacter = characterList[selectedCharacterIndex];
        if (!selectedCharacter.isBought)
        {
            Debug.LogError("Character not bought");
            return;
        }

        if (expScript.Coins >= selectedCharacter.price)
        {
            expScript.BuyCharacter(selectedCharacter.price); // уменьшаем количество монет
            selectedCharacter.strength += 2; // улучшаем силу персонажа
            selectedCharacter.price = (int)(selectedCharacter.price + 120f); // увеличиваем цену следующего улучшения
            UpdateUI();
            SaveData();
        }
        else
        {
            Debug.LogError("Not enough coins to upgrade character");
        }
    }

    private void LoadData()
    {
        string json = null;
        if (File.Exists(fullSavePath))
        {
            json = File.ReadAllText(fullSavePath);
        }
        else
        {
            UnityEngine.TextAsset characterDataAsset = Resources.Load<UnityEngine.TextAsset>("CharacterData");
            if (characterDataAsset != null)
            {
                json = characterDataAsset.text;
            }
            else
            {
                Debug.LogError("Unable to find CharacterData file in Resources");
                return;
            }
        }

        CharacterDataArray characterDataArray = JsonUtility.FromJson<CharacterDataArray>(json);
        if (characterDataArray != null)
        {
            if (characterDataArray.characters != null)
            {
                characterList = new List<CharacterData>(characterDataArray.characters);
                if (characterList.Count > 0)
                {
                    selectedCharacterIndex = characterDataArray.selectedCharacterIndex;
                    if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characterList.Count)
                    {
                        UpdateUI();
                    }
                    else
                    {
                        Debug.LogError("Invalid selectedCharacterIndex in the saved data");
                    }
                }
                else
                {
                    Debug.LogError("No characters found in the saved data");
                }
            }
            else
            {
                Debug.LogError("No characters found in the saved data");
            }
        }
        else
        {
            Debug.LogError("Failed to parse CharacterDataArray from JSON");
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
        GlobalContext.SelectedCharacter = characterList[index]; // Добавляем эту строку, чтобы обновить выбранный персонаж в глобальном контексте
        UpdateUI();
        SaveData();
    }

    public class GlobalContext
    {
        public static CharacterData SelectedCharacter { get; set; }
    }

    public void BuyCharacter()
    {
        if (selectedCharacterIndex < 0 || selectedCharacterIndex >= characterList.Count)
        {
            Debug.LogError("Invalid character index");
            return;
        }

        CharacterData selectedCharacter = characterList[selectedCharacterIndex];
        if (selectedCharacter.isBought)
        {
            Debug.LogError("Character already bought");
            return;
        }

        if (expScript.Coins >= selectedCharacter.price)
        {
            expScript.BuyCharacter(selectedCharacter.price);
            selectedCharacter.isBought = true;
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
        uppButton.gameObject.SetActive(character.isBought);
        nameText.text = character.name;
        powerText.text = $"{character.strength}";
        speedText.text = $"{character.speed}";
        skillText.text = $"{(character.hasSkill ? "Yes" : "No")}";
        price.text = character.price.ToString();

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
}
