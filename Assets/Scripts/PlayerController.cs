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

    [SerializeField]
    private float trustForce = 1000f;

    [Header("Spring Sertings:")]

    [SerializeField]
    private float jointSpring = 20f;

    [SerializeField]
    private float jointMaxForece = 40f;

    //Component Caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<PlayerMotor>();

        joint = GetComponent<ConfigurableJoint>();

        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    // Update is called once per frame
    void Update()
    {
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
        animator.SetFloat("ForwardVelocity", _vInput);

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

        if (Input.GetButton("Jump"))
        {
            _trusterForce = Vector3.up * trustForce;
            SetJointSettings(0f);
        }
        else
        {
            SetJointSettings(jointSpring);
        }

        //Apply Trust force
        motor.ApplyTruster(_trusterForce);
    }

    void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive { 
            positionSpring = _jointSpring, 
            maximumForce = jointMaxForece
        }; 
    }
}
