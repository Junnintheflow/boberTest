using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [Header("Параметры")]
    [SerializeField] private string requiredKeyName; 
    [SerializeField] private AudioClip openDoorSound; 
    [SerializeField] private AudioClip backgroundMusic; 
    [SerializeField] private GameObject endUI; 
    [SerializeField] private Button endButton; 

    private Camera playerCamera;
    [SerializeField] private float intRange = 3f; 
    
    private bool isOpen = false;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            TryOpenDoor();
        }
    }

    private void TryOpenDoor()
    {
        Item heldItem = GetHeldItem();
        
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, intRange))
        {
            if (hit.collider.CompareTag("Door")) 
            {
                if (heldItem != null && heldItem.itemName == requiredKeyName)
                {
                    OpenDoor();
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            return;
        }
    }

    private Item GetHeldItem()
    {
        if (Inventory.Instance != null && Inventory.Instance.selectedItemIndex >= 0)
        {
            return Inventory.Instance.items[Inventory.Instance.selectedItemIndex];
        }
        return null;
    }

    private void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            if (openDoorSound && audioSource)
            {
                audioSource.PlayOneShot(openDoorSound);
            }

            if (endUI != null)
                endUI.SetActive(true);

            if (backgroundMusic && audioSource)
            {
                audioSource.clip = backgroundMusic;
                audioSource.loop = true; 
                audioSource.Play();
            }
        }
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
