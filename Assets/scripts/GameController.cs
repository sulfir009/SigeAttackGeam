using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CharacterSelector;

public class GameController : MonoBehaviour
{
    public Transform playerSpawnPoint;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (GlobalContext.SelectedCharacter == null)
        {
            Debug.LogWarning("No character selected, default character Fox will be spawned");
            CharacterSelector characterSelector = GameObject.FindObjectOfType<CharacterSelector>();
            if (characterSelector != null)
            {
                characterSelector.SelectCharacter(0);
            }
            else
            {
                Debug.LogError("Failed to find CharacterSelector");
                return;
            }
        }

        Debug.Log("Spawning player with character " + GlobalContext.SelectedCharacter.Name);


        GameObject characterPrefab = Resources.Load<GameObject>(GlobalContext.SelectedCharacter.Name);
        if (characterPrefab == null)
        {
            Debug.LogError("Failed to load character prefab");
            return;
        }

        GameObject player = Instantiate(characterPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        PlayerController playerController = player.AddComponent<PlayerController>();
        playerController.playerPower = GlobalContext.SelectedCharacter.Strength;
        playerController.speed = GlobalContext.SelectedCharacter.Speed;

        playerController.joystick = GameObject.Find("Dynamic Joystick").GetComponent<DynamicJoystick>();
        playerController.deathMessage = GameObject.Find("DethMesh").GetComponent<TextMeshProUGUI>();
        playerController.buttonRoll = GameObject.Find("Roll").GetComponent<Button>();
        playerController.characterController = player.GetComponent<CharacterController>();
        playerController.playerAnimator = player.GetComponent<Animator>();
        playerController.runButton = GameObject.Find("Speed").GetComponent<Button>();
        playerController.attackButton = GameObject.Find("Attak").GetComponent<Button>();
        playerController.playerTransform = player.transform;

        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null)
        {
            ControllCamera controlCamera = mainCamera.GetComponent<ControllCamera>();
            if (controlCamera != null)
            {
                controlCamera.playerTransform = player.transform;
                playerController.controllCamera = controlCamera;
            }
            else
            {
                Debug.LogError("Failed to find ControllCamera on the Main Camera");
            }
        }
        else
        {
            Debug.LogError("Failed to find Main Camera in the scene");
        }
    }
}
