using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotJoint : MonoBehaviour {
    public Vector3 Axis;
    public Vector3 StartOffset;

    public float MinAngle;
    public float MaxAngle;

    void Awake()
    {
        StartOffset = transform.localPosition;
    }

    public float MoveArm(float angle)
    {
        return SetAngle(angle);
    }

    private float SetAngle(float angle)
    {
        if(Axis.x == 1)
        {
            transform.eulerAngles = new Vector3(angle, 0, 0);
        }
        else if(Axis.y == 1)
        {
            transform.eulerAngles = new Vector3(0, angle, 0);
        }
        else if (Axis.z == 1)
        {
            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        return angle;
    }
}
