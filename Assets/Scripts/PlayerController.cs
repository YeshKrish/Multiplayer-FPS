using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    private float mouseSentivity = 3f;

    private PlayerMotor motor;
    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate movement velocity as a 3D vector
        float _hInput = Input.GetAxisRaw("Horizontal");
        float _vInput = Input.GetAxisRaw("Vertical");

        Vector3 _movHorizontal = transform.right * _hInput;
        Vector3 _movVertical = transform.forward * _vInput;

        //Final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical).normalized * speed;

        motor.Move(_velocity);

        //calculate rotation as a 3D vector (turning around)
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * mouseSentivity;

        //Apply rotation
        motor.Rotate(_rotation);

    }
}
