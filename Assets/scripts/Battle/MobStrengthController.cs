using UnityEngine;
using TMPro;

public class MobStrengthController : MonoBehaviour
{
    public int strength;
    public TextMesh strengthText;
    private MobChaseController chaseController;

    private void Start()
    {
        UpdateStrengthText();
    }

    public void UpdateStrengthText()
    {
        strengthText.text = strength.ToString();
    }

    public void SetStrength(int newStrength)
    {
        strength = newStrength;
        UpdateStrengthText();
    }

    public void DecreaseStrength(int amount)
    {
        strength -= amount;
        UpdateStrengthText();
        
    }
}
