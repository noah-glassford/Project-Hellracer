using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

    private void Start() 
    {
         foreach (AxleInfo axleInfo in axleInfos) 
         {
            axleInfo.leftWheel.ConfigureVehicleSubsteps(0.1f, 9, 15);
            axleInfo.rightWheel.ConfigureVehicleSubsteps(0.1f, 9, 15);
         }
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
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
     
        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);

            //SpeedDisplay.text = transform.GetChild(0).GetComponent<Rigidbody>().velocity.magnitude.ToString();

            WheelHit hit = new WheelHit();
     
            WheelCollider leftWheel = axleInfo.leftWheel;
            WheelCollider rightWheel = axleInfo.rightWheel;
     
            if (leftWheel.GetGroundHit(out hit))
            {
                if (hit.sidewaysSlip > .05 || hit.sidewaysSlip < -0.05)
                   driftParticleLeft.gameObject.SetActive(true);
                   else
                   driftParticleLeft.gameObject.SetActive(false);
            }

            if (rightWheel.GetGroundHit(out hit))
            {
                if (hit.sidewaysSlip > .05 || hit.sidewaysSlip < -0.05)
                   driftParticleRight.gameObject.SetActive(true);
                   else
                   driftParticleRight.gameObject.SetActive(false);
            }

          
        }
    }
}