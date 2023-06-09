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
    public Button[] characterButtons;
    public RawImage image;
    public TMP_Text coinsText;
    public Button buyButton;
    public Button selectButton;
    public string saveFilePath = "/CharacterData.json";
    private string fullSavePath;
    public List<CharacterData> characterList = new List<CharacterData>();
    private int selectedCharacterIndex = 0;
    public EXP expScript;

    private void Start()
    {
        fullSavePath = Application.persistentDataPath + saveFilePath;
        Debug.Log(fullSavePath);
        LoadData();
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

    private void LoadData()
    {
        UnityEngine.TextAsset characterDataAsset = Resources.Load<UnityEngine.TextAsset>("CharacterData");
        if (characterDataAsset != null)
        {
            string json = characterDataAsset.text;
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
        else
        {
            Debug.LogError("Unable to find CharacterData file in Resources");
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
}
