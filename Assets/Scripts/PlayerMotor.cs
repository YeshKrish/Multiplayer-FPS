using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;

    private Vector3 rotation = Vector3.zero;

    private Vector3 camerRotation = Vector3.zero;

    private Vector3 trustForce = Vector3.zero;

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

    //Get a force vector for trusters
    public void ApplyTruster(Vector3 _trusterForce)
    {
        trustForce = _trusterForce;
    }

    //Gets the rotation vector for the camera
    public void RotateCamera(Vector3 _cameraRotation)
    {
        camerRotation = _cameraRotation;
    }

    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
        PerformCameraRotation();
    }


    //Perform movement based on velocity variable
    void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
        }

        if(trustForce != Vector3.zero)
        {
            rb.AddForce(trustForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    //Perform rotation based on rotation variable
    void PerformRotation()
    { 
        rb.MoveRotation(transform.rotation * Quaternion.Euler(rotation));
    }

    //Perform rotation based on camerarotation variable
    void PerformCameraRotation()
    {
        if(cam != null)
        {
            cam.transform.Rotate(-camerRotation);
        }
        
    }

}
