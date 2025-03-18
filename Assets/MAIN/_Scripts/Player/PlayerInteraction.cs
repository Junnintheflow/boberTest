using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Параметры")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    private Outline currentOutline; 

    private void Update()
    {
        CheckForInteractable();
    }

        private void CheckForInteractable()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            Outline outline = hit.collider.GetComponent<Outline>();
            
            if (outline)
            {
                if (currentOutline != outline)
                {
                    RemoveOutline(); // fix upd. очистка предыдущего выделенного объекта
                    currentOutline = outline;
                    currentOutline.enabled = true; 
                }
            }
        }
        else
        {
            RemoveOutline(); 
        }
    }

    private void RemoveOutline()
    {
        if (currentOutline)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }
}