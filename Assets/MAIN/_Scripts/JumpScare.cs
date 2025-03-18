using UnityEngine;

public class JumpScare : MonoBehaviour
{
    [Header("Параметры")]
    [SerializeField] private Transform player; 
    [SerializeField] private float triggerDistance = 15f; 
    [SerializeField] private float moveSpeed = 4f; 
    [SerializeField] private float acceleration = 5f; 

    [Header("Звук")]
    [SerializeField] private AudioClip jumpscareSound; 
    [SerializeField] private AudioSource playerAudioSource; 

    private bool isTriggered = false;
    private float currentSpeed;

    private void Update()
    {
        if (isTriggered) return; 
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= triggerDistance)
        {
            TriggerJumpscare();
        }
    }

    private void TriggerJumpscare()
    {
        isTriggered = true;
        currentSpeed = moveSpeed; 
        StartCoroutine(MoveAndDelete());

        if (playerAudioSource && jumpscareSound)
        {
            playerAudioSource.PlayOneShot(jumpscareSound);
        }
    }

    private System.Collections.IEnumerator MoveAndDelete()
    {
        while (true)
        {
            currentSpeed += acceleration * Time.deltaTime; 
            transform.position += Vector3.forward * currentSpeed * Time.deltaTime; 

            if (transform.position.x > 20f) 
            {
                Destroy(gameObject); 
                yield break;
            }

            yield return null;
        }
    }
}