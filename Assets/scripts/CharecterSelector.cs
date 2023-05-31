using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharecterSelector : MonoBehaviour
{
    public Button selectButton;
    public RawImage characterImage;
    public TextMeshProUGUI characterDescription;

    public Sprite[] characterSprites;
    public string[] characterDescriptions;

    private int currentCharacterIndex;

    private void Start()
    {
        selectButton.onClick.AddListener(ChangeCharacter);
        currentCharacterIndex = 0;
        UpdateCharacter();
    }

    private void ChangeCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex + 1) % characterSprites.Length;
        UpdateCharacter();
    }

    private void UpdateCharacter()
    {
        characterImage.sprite = characterSprites[currentCharacterIndex];
        characterDescription.text = characterDescriptions[currentCharacterIndex];
    }
}
