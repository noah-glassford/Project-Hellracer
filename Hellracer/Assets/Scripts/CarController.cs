using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public ParticleSystem driftParticle;
     

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
        visualWheel.transform.localRotation = rotation;
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

            if (axleInfo.leftWheel.GetGroundHit(out WheelHit hit))
            {
            if (hit.sidewaysSlip > .15)
            {
                driftParticle.gameObject.SetActive(true);
            }
            else
            {
                driftParticle.gameObject.SetActive(false);
            }
            }
          
        }
    }
}