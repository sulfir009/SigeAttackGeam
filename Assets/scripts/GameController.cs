using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

        GameObject characterPrefab = Resources.Load<GameObject>(GlobalContext.SelectedCharacter.name);
        if (characterPrefab == null)
        {
            Debug.LogError("Failed to load character prefab");
            return;
        }

        GameObject player = Instantiate(characterPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        PlayerController playerController = player.AddComponent<PlayerController>();
        playerController.playerPower = GlobalContext.SelectedCharacter.strength; // Пример присвоения силы персонажу
        playerController.speed = GlobalContext.SelectedCharacter.speed; // Пример присвоения speed персонажу

        playerController.joystick = GameObject.Find("Dynamic Joystick").GetComponent<DynamicJoystick>();
        playerController.deathMessage = GameObject.Find("DethMesh").GetComponent<TextMeshProUGUI>();
        playerController.buttonRoll = GameObject.Find("Roll").GetComponent<Button>();
        playerController.characterController = player.GetComponent<CharacterController>();
        playerController.playerAnimator = player.GetComponent<Animator>();
        playerController.runButton = GameObject.Find("Speed").GetComponent<Button>();
        playerController.attackButton = GameObject.Find("Attak").GetComponent<Button>();

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
