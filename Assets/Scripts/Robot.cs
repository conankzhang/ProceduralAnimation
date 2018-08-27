using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour {

    public RobotJoint[] Joints;
    public float SamplingDistance;
    public float LearningRate;
    public float DistanceThreshold;
    public GameObject target;
    public float[] angles;

    public Vector3 ForwardKinematics(float[] angles)
    {
        Vector3 prevPoint = Joints[0].transform.position;
        Quaternion rotation = transform.rotation;

        for (int i = 1; i < Joints.Length; i++)
        {
            rotation *= Quaternion.AngleAxis(angles[i - 1], Joints[i - 1].Axis);
            Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;

            prevPoint = nextPoint;
        }

        return prevPoint;
    }

    public float DistanceFromTarget(Vector3 target, float[] angles)
    {
        Vector3 point = ForwardKinematics(angles);
        return Vector3.Distance(point, target);
    }

    public float PartialGradient(Vector3 target, float[] angles, int i)
    {
        float angle = angles[i];

        // Gradient: [F(x + SamplingDistance) - F(x)] / h
        float f_x = DistanceFromTarget(target, angles);

        angles[i] += SamplingDistance;
        float f_x_plus_d = DistanceFromTarget(target, angles);

        float gradient = (f_x_plus_d - f_x) / SamplingDistance;

        angles[i] = angle;

        return gradient;
    }

    public void InverseKinematics(Vector3 target, float[] angles)
    {
        if(DistanceFromTarget(target, angles) < DistanceThreshold)
        {
            return;
        }

        for (int i = Joints.Length - 1; i >= 0; i--)
        {
            float gradient = PartialGradient(target, angles, i);
            angles[i] -= LearningRate * gradient;

            angles[i] = Mathf.Clamp(angles[i], Joints[i].MinAngle, Joints[i].MaxAngle);

            if(DistanceFromTarget(target, angles) < DistanceThreshold)
            {
                return;
            }
        }

        for (int i = 0; i < Joints.Length - 1; i++)
        {
            Joints[i].MoveArm(angles[i]);
        }
    }

	// Use this for initialization
	void Start () {
        angles = new float[Joints.Length];
	}
	
	// Update is called once per frame
	void Update () {
        InverseKinematics(target.transform.position, angles);
	}
}
