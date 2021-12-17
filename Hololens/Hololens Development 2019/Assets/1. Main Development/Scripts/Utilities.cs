using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Utilities
{
    /*Unfortunately, I haven't found any other way to pass the correct arbitrary device
    If you were simply returning the name of the device, then you could get i.e AndroidRotationVector
    instead of an Accelerometer. Despite that the latter is a generalization of the former, you have to
    send Accelerometer to the server and not AndroidRotationVector, because the server will most likely
    be missing the Android SDK, if it isn't built on Android;*/

    /*We can probably hack it with reflection, but since it's a one time job, there is no need
     * to meddle around with it. If Unity updates to other sensors, simply adding them here should work.*/

    //Should probably search for another approach to addnig multiple buttons without needing to bind them to a device (?)
    public static string GetSupportedDeviceLayout(InputDevice device)
    {
        if (device is Mouse)
            return nameof(Mouse);
        else if (device is Keyboard)
            return nameof(Keyboard);
        else if (device is Touchscreen)
            return nameof(Touchscreen);
        else if (device is Gamepad)
            return nameof(Gamepad);
        else if (device is Accelerometer)
            return nameof(Accelerometer);
        else if (device is UnityEngine.InputSystem.Gyroscope)
            return nameof(UnityEngine.InputSystem.Gyroscope);
        else if (device is GravitySensor)
            return nameof(GravitySensor);
        else if (device is AttitudeSensor)
            return nameof(AttitudeSensor);
        else if (device is LinearAccelerationSensor)
            return nameof(LinearAccelerationSensor);
        else if (device is MagneticFieldSensor)
            return nameof(MagneticFieldSensor);
        else if (device is LightSensor)
            return nameof(LightSensor);
        else if (device is PressureSensor)
            return nameof(PressureSensor);
        else if (device is ProximitySensor)
            return nameof(ProximitySensor);
        else if (device is HumiditySensor)
            return nameof(HumiditySensor);
        else if (device is AmbientTemperatureSensor)
            return nameof(AmbientTemperatureSensor);
        else if (device is StepCounter)
            return nameof(StepCounter);
        return "";
    }

}