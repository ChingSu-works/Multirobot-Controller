using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] public FixedJoystick _joystick;
    [SerializeField] public float _speed = 100f;

    float joyConvert;
    bool _Direction;

    private void Update()
    {
        Forward_Backward();
        Left_Right(_Direction);
    }

    public void Left_Right(bool direction)
    {
        this.transform.Rotate(Vector3.forward * _joystick.Horizontal * _speed);
    }

    public void Forward_Backward()
    {
        GameObject.FindWithTag("Left Wheel").transform.Rotate(Vector3.down * _joystick.Vertical, Space.Self);
        GameObject.FindWithTag("Right Wheel").transform.Rotate(Vector3.up * _joystick.Vertical, Space.Self);
    }
}
