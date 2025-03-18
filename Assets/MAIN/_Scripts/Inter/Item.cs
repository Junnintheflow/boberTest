using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Параметры")]
    public string itemName; 
    public Sprite itemIcon; 
    public GameObject prefab;
    public AudioClip pickupSound;
}
