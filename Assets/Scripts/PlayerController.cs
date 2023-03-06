using UnityEngine;

// [RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f; 
    [SerializeField]
    private float lookSensitivity = 3f; 

    [SerializeField]
    private float thrusterForce = 1300f; 

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f; 
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f; 
    private float thrusterFuelAmount = 1f; 

    public float GetThrusterFuelAmount ()
    {
        return thrusterFuelAmount; 
    }

    [SerializeField]
    private LayerMask environmentMask; 

    [Header("Spring Settings")]
    [SerializeField]
    private float jointSpring = 20f; 
    [SerializeField]
    private float jointMaxForce = 40f; 

    private PlayerMotor motor; 
    private ConfigurableJoint joint;
    // private Animator animator; 

    void Start()
    {
        motor = GetComponent<PlayerMotor>(); 
        joint = GetComponent<ConfigurableJoint>(); 
        // animator = GetComponent<Animator>(); 

        SetJointSettings(jointSpring); 
    }

    void Update()
    {
        if (PauseMenu.isOn)
        {
            // if (Cursor.lockState != CursorLockMode.None)
            // {
            //     Cursor.lockState = CursorLockMode.None; 
            //     Cursor.visible = true; 
            // }

            motor.Move (Vector3.zero); 
            motor.Rotate (Vector3.zero); 
            motor.RotateCamera (0f); 
            motor.ApplyThruster (Vector3.zero); 

            return; 
        }

        // if (Cursor.lockState != CursorLockMode.Locked)
        // {
        //     Cursor.lockState = CursorLockMode.Locked;
        //     Cursor.visible = false;
        // }

        RaycastHit _hit; 
        if (Physics.Raycast (transform.position, Vector3.down, out _hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3 (0f, -_hit.point.y, 0f); 
        }
        else
        {
            joint.targetPosition = new Vector3 (0f, 0f, 0f); 
        }

        float _xMovement = Input.GetAxis("Horizontal"); 
        float _zMovement = Input.GetAxis("Vertical"); 

        Vector3 _moveHorizontal = transform.right * _xMovement; 
        Vector3 _moveVertical = transform.forward * _zMovement; 

        Vector3 _velocity = (_moveHorizontal + _moveVertical) * speed; 

        // animator.SetFloat("ForwardVelocity", _zMovement); 

        motor.Move(_velocity); 

        float _yRotation = Input.GetAxisRaw("Mouse X"); 

        Vector3 _rotation = new Vector3(0f, _yRotation, 0f) * lookSensitivity; 

        motor.Rotate(_rotation);

        float _xRotation = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX = _xRotation * lookSensitivity;

        motor.RotateCamera(_cameraRotationX);

        Vector3 _thrusterForce = Vector3.zero; 
        if (Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime; 

            if (thrusterFuelAmount >= 0.01)
            {
                _thrusterForce = Vector3.up * thrusterForce; 
                SetJointSettings(0f); 
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime; 
            SetJointSettings(jointSpring); 
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f); 

        motor.ApplyThruster(_thrusterForce);

        // if (!GetComponent<Player>().isDead)
        // {
        //     if (Input.GetKeyDown(KeyCode.Alpha1))
        //     {
        //         GetComponent<WeaponManager>().CmdSwitchWeapon(0);
        //     }
        //     if (Input.GetKeyDown(KeyCode.Alpha2))
        //     {
        //         GetComponent<WeaponManager>().CmdSwitchWeapon(1);
        //     }
        //     if (Input.GetKeyDown(KeyCode.Alpha3))
        //     {
        //         GetComponent<WeaponManager>().CmdSwitchWeapon(2);
        //     }
        // }
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring, 
            maximumForce = jointMaxForce
        };
    }
}
