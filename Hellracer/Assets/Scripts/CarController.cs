using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
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
    public TextMeshProUGUI SpeedDisplay;
    private CarInputActions inputActions;
    private PlayerInput playerInput;
    private float motorInput;
    private float steering;

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
         inputActions.CarController.Accelerate.performed += Accelerate;
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

    private void Accelerate(InputAction.CallbackContext obj)
    {
        motorInput = maxMotorTorque * obj.ReadValue<float>();
    }
     
    private void Update() 
    {
         steering = maxSteeringAngle * inputActions.CarController.Steering.ReadValue<float>();
    }
    public void FixedUpdate()
    {
       
     
        foreach (AxleInfo axleInfo in axleInfos) 
        {
            if (axleInfo.steering) 
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
             {
                Debug.Log(motorInput);
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

            //SpeedDisplay.text = transform.GetChild(0).GetComponent<Rigidbody>().velocity.magnitude.ToString();

            WheelHit hit = new WheelHit();
     
            WheelCollider leftWheel = axleInfo.leftWheel;
            WheelCollider rightWheel = axleInfo.rightWheel;
     
            if (leftWheel.GetGroundHit(out hit))
            {
                if (hit.sidewaysSlip > .15 || hit.sidewaysSlip < -0.15)
                   driftParticleLeft.gameObject.SetActive(true);
                   else
                   driftParticleLeft.gameObject.SetActive(false);
            }

            if (rightWheel.GetGroundHit(out hit))
            {
                if (hit.sidewaysSlip > .15 || hit.sidewaysSlip < -0.15)
                   driftParticleRight.gameObject.SetActive(true);
                   else
                   driftParticleRight.gameObject.SetActive(false);
            }
        }
    }
}