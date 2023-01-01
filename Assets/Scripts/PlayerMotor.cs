using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;

    private Vector3 rotation = Vector3.zero;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Gets the movement vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    //Gets the rotation vector
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }


    //Perform movement based on velocity variable
    void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
        }
    }

    void PerformRotation()
    { 
        rb.MoveRotation(transform.rotation * Quaternion.Euler(rotation));
    }

}
