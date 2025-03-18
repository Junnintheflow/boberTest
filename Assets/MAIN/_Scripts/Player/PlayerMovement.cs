using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Параметры")]
    [SerializeField] private float moveSpeed = 5f; 
    [SerializeField] private float gravity = -9.81f; // дефолт значение

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded; // флажок для земли

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // fix upd обнуляю ск падения из-за бага с набором скорости
        }

        MovePlayer();

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical"); 

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
}