using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static class InputFactory
{
    private static Dictionary<string, Func<BaseInput>> inputs;

    static InputFactory()
    {
        inputs = new Dictionary<string, Func<BaseInput>>()
        {
            {nameof(Accelerometer), () => new AccelerometerInput() },
            {nameof(AmbientTemperatureSensor), () => new AmbientTemperatureInput() },
            {nameof(AttitudeSensor), () => new AttitudeInput() },
            {nameof(Gamepad), () => new GamepadInput() },
            {nameof(GravitySensor), () => new GravityInput() },
            {nameof(Gyroscope), () => new GyroscopeInput() },
            {nameof(HumiditySensor), () => new HumidityInput() },
            {nameof(Keyboard), () => new KeyboardInput() },
            {nameof(LightSensor), () => new LightInput() },
            {nameof(LinearAccelerationSensor), () => new LinearAccelerationInput() },
            {nameof(MagneticFieldSensor), () => new MagneticFieldInput() },
            {nameof(Mouse), () => new MouseInput() },
            {nameof(PressureSensor), () => new PressureInput() },
            {nameof(ProximitySensor), () => new ProximityInput() },
            {nameof(StepCounter), () => new StepCounterInput() },
            {nameof(Touchscreen), () => new TouchscreenInput() },
            {nameof(TrackedDevice), () => new TrackedDeviceInput() },
        };
    }

    public static BaseInput CreateInput(string layout)
    {
        if (!inputs.TryGetValue(layout, out Func<BaseInput> creator))
            return new NoInput();
        return creator.Invoke();
    }
    public static BaseInput CreateInput(InputDevice device, string layout, InputEventPtr inputPtr)
    {
        var result = CreateInput(layout);
        if (result == null)
            return null;
        result.SetUp(device, inputPtr);
        return result;
    }
}
