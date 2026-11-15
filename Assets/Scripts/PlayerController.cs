using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;

    [SerializeField] private float speed = 30f;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(h,0f,v);
        characterController.Move(dir * speed * Time.deltaTime);
    }
}
