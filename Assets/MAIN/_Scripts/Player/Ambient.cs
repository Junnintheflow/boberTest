using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    [SerializeField] private AudioClip ambientClip; 
    [SerializeField] private GameObject endUI; 
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (ambientClip)
        {
            audioSource.clip = ambientClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (endUI != null && endUI.activeSelf)
        {
            audioSource.Stop(); 
        }
    }
}