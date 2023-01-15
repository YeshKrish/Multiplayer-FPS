using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float lookSentivity = 3f;

    [Header("Thruster Settings")]
    [SerializeField]
    private float trustForce = 1000f;
    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 1.5f;
    //Create Regentime for thrusters
    private float thrusterFuelAmount = 1f;

    [SerializeField]
    private LayerMask environmentMask;

    [Header("Spring Sertings:")]

    [SerializeField]
    private float jointSpring = 20f;

    [SerializeField]
    private float jointMaxForece = 40f;

    //Animation variables
    float velocity = 0.0f;
    float minVelocity = 0.0f;
    float maxVelocity = 1.0f;

    public float acceleration = 5.0f;
    public float deceleration = 5.0f;

    int velocityHash;

    //Component Caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    // Start is called before the first frame update
    void Start()
    {
        velocityHash = Animator.StringToHash("ForwardVelocity");

        motor = GetComponent<PlayerMotor>();

        joint = GetComponent<ConfigurableJoint>();

        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    // Update is called once per frame
    void Update()
    {
        //Setting target position for spring
        //this makes the physics acts right when it come to
        //applying gravity when flying over objects
        RaycastHit _hit;
        if(Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool backwardPressed = Input.GetKey(KeyCode.S);
        //  if (!IsOwner) return;

        //Calculate movement velocity as a 3D vector
        float _hInput = Input.GetAxis("Horizontal");
        float _vInput = Input.GetAxis("Vertical");

        Vector3 _movHorizontal = transform.right * _hInput;
        Vector3 _movVertical = transform.forward * _vInput;

        //Final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

        motor.Move(_velocity);

        //Animate movement
        animator.SetFloat(velocityHash, velocity);
        ChangeVelocity(forwardPressed, backwardPressed);
        LockAndResetVelocity(forwardPressed, backwardPressed);

        //calculate rotation as a 3D vector (turning around)
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSentivity;

        //Apply rotation
        motor.Rotate(_rotation);

        //calculate camera rotation as a 3D vector (turning around)
        float _xRot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX = _xRot * lookSentivity;

        //Apply rotation
        motor.RotateCamera(_cameraRotationX);

        //Calculater trust force based on player input
        Vector3 _trusterForce = Vector3.zero;

        if (Input.GetButton("Jump") && thrusterFuelAmount > 0)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if(thrusterFuelAmount > 0.01f)
            {
                _trusterForce = Vector3.up * trustForce;
                SetJointSettings(0f);
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        //Apply Trust force
        motor.ApplyTruster(_trusterForce);
    }

    void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring,
            maximumForce = jointMaxForece
        };
    }

    void ChangeVelocity(bool forwardPressed, bool backwardPressed)
    {
        if (forwardPressed && velocity < maxVelocity && !backwardPressed)
        {
            velocity += Time.deltaTime * acceleration;
        }
        if (!forwardPressed && velocity > minVelocity)
        {
            velocity -= Time.deltaTime * deceleration;
        }
        if (backwardPressed && velocity > -maxVelocity)
        {
            velocity -= Time.deltaTime * acceleration;
        }
        if (!backwardPressed && velocity < minVelocity)
        {
            velocity += Time.deltaTime * deceleration;
        }
    }

    void LockAndResetVelocity(bool forwardPressed, bool backwardPressed)
    {
        if (forwardPressed && velocity >= maxVelocity && !backwardPressed)
        {
            velocity = maxVelocity;
        }
        if (backwardPressed && velocity <= -maxVelocity && !forwardPressed)
        {
            velocity = -maxVelocity;
        }
        if (!forwardPressed && velocity != 0.0f && !backwardPressed && (velocity < 0.05 && velocity > -0.05))
        {
            velocity = 0.0f;
        }
    }
}
