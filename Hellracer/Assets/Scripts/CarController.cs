using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
using TMPro;

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}
     
public class CarController : MonoBehaviour {
    public List<AxleInfo> axleInfos; 
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public ParticleSystem driftParticleLeft;
    public ParticleSystem driftParticleRight;

    public ParticleSystem accelParticleLeft;
    public ParticleSystem accelParticleRight;
    public TextMeshProUGUI SpeedDisplay;
    private CarInputActions inputActions;
    private PlayerInput playerInput;
    private float motorInput;
    private float steering;

    [SerializeField]
    CinemachineVirtualCamera VCam;
    public bool ShouldAffectFOV;
    public AnimationCurve CameraFOVCurve;
    public float minSpeed = 0f;
    public float maxSpeed = 10f;
    private float playerSpeed;
    private float FieldOfView;

    public float sideDriftCutoff; //this and tirespin cutoff affects when smoke particles are spawned
    public float tireSpinCutoff; 

    private void Start() 
    {
         foreach (AxleInfo axleInfo in axleInfos) 
         {
            axleInfo.leftWheel.ConfigureVehicleSubsteps(0.1f, 9, 15);
            axleInfo.rightWheel.ConfigureVehicleSubsteps(0.1f, 9, 15);
         }
         playerInput = GetComponent<PlayerInput>();
         inputActions = new CarInputActions();
         inputActions.CarController.Enable();

        FieldOfView = 60;

    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) {
            return;
        }
     
        Transform visualWheel = collider.transform.GetChild(0);
     
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

      
     
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
     
    public void FixedUpdate()
    {
        steering = maxSteeringAngle * inputActions.CarController.Steering.ReadValue<float>();
        motorInput = maxMotorTorque * inputActions.CarController.Accelerate.ReadValue<float>();

        if (ShouldAffectFOV)
        {
        playerSpeed = GetComponent<Rigidbody>().velocity.magnitude;
      
        float parameter = Mathf.InverseLerp(minSpeed, maxSpeed, playerSpeed);
        Debug.Log(parameter);
        parameter = CameraFOVCurve.Evaluate(parameter);
        FieldOfView = Mathf.Lerp(60, 100, parameter);
        VCam.m_Lens.FieldOfView = FieldOfView;
        }
        foreach (AxleInfo axleInfo in axleInfos) 
        {
            if (axleInfo.steering) 
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
             {
                if (motorInput >= 0)
                {
                    axleInfo.leftWheel.motorTorque = motorInput;
                    axleInfo.rightWheel.motorTorque = motorInput;
                }
                else if (motorInput < 0)
                {
                    axleInfo.leftWheel.motorTorque = motorInput / 8;
                    axleInfo.rightWheel.motorTorque = motorInput / 8;
                }
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);

            WheelHit hit = new WheelHit();
            WheelCollider leftWheel = axleInfo.leftWheel;
            WheelCollider rightWheel = axleInfo.rightWheel;


     
            if (leftWheel.GetGroundHit(out hit))
            {
                if (hit.sidewaysSlip > sideDriftCutoff || hit.sidewaysSlip < -sideDriftCutoff)
                   driftParticleLeft.gameObject.SetActive(true);
                   else
                   driftParticleLeft.gameObject.SetActive(false);
            }

            if (rightWheel.GetGroundHit(out hit))
            {
                if (hit.sidewaysSlip > sideDriftCutoff || hit.sidewaysSlip < -sideDriftCutoff)
                   driftParticleRight.gameObject.SetActive(true);
                   else
                   driftParticleRight.gameObject.SetActive(false);
            }

             if (leftWheel.GetGroundHit(out hit))
            {
               if (hit.forwardSlip > tireSpinCutoff || hit.forwardSlip < -tireSpinCutoff)
                   accelParticleLeft.gameObject.SetActive(true);
                   else
                   accelParticleLeft.gameObject.SetActive(false);
            }

            if (rightWheel.GetGroundHit(out hit))
            {
                if (hit.forwardSlip > tireSpinCutoff || hit.forwardSlip < -tireSpinCutoff)
                   accelParticleRight.gameObject.SetActive(true);
                   else
                   accelParticleRight.gameObject.SetActive(false);
            }
        }
    }
}