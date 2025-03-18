using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class ItemPickup : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float pickupRange = 3f; 
    [SerializeField] public Transform holdPoint;
    [SerializeField] private AudioSource audioSource;

    private Camera playerCamera;
    private Inventory inventory; 

    private bool isPickingUp = false; //fix upd флажок для анимации

    public Item item; 

    private void Start()
    {
        playerCamera = Camera.main;
        inventory = FindFirstObjectByType<Inventory>();

        // если я слишком сонный и забыл добавить audio соус, поэтому прифаером напишу
        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>(); 
        } 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            TryPickupItem();
        }
    }

    private void TryPickupItem()
    {
        if (isPickingUp || inventory.IsThrowing) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                isPickingUp = true;

                item.GetComponent<Collider>().enabled = false;
                item.GetComponent<Rigidbody>().useGravity = false;

                item.transform.DOMove(holdPoint.position, 0.5f).OnComplete(() =>
                {
                    item.gameObject.SetActive(false);
                    Inventory.Instance.AddItem(item);
                    isPickingUp = false; 

                    if (item.pickupSound)
                    {
                        audioSource.PlayOneShot(item.pickupSound);
                    }
                });
            }
        }
    }
}