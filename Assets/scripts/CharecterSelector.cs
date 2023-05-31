using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelector : MonoBehaviour
{
    public Button selectButton;
    public Image characterImage;
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