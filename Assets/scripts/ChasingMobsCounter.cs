using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChasingMobsCounter : MonoBehaviour
{
    public PlayerController playerController;
    public TextMesh chasingMobsText;
    

    private List<MobChaseController> chasingMobs = new List<MobChaseController>();

    private void Update()
    {
        UpdateChasingMobsText();
    }

    public void AddChasingMob(MobChaseController mobChaseController)
    {
        chasingMobs.Add(mobChaseController);
      //  mobChaseController.SetAlliedState(true);
        UpdateChasingMobsText();
    }

    public void RemoveChasingMob(MobChaseController mobChaseController)
    {
        chasingMobs.Remove(mobChaseController);
        UpdateChasingMobsText();
    }

    private void UpdateChasingMobsText()
    {
        chasingMobsText.text = $"{playerController.powerRet().ToString()}";
    }
}
