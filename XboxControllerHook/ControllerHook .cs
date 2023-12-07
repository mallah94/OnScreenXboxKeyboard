// ControllerHook class that implements the IControllerHook interface
using SharpDX.XInput;
using System;
using System.Reflection;
using XboxControllerHook.Enums;
using XboxControllerHook.Helpers;
using XboxControllerHook.Interfaces;

public class ControllerHook : IControllerHook
{
    private Controller[] _controllers;
    private List<Tuple<XboxControllerButton, bool, DateTime>>[] _buttonStates;
    private XboxControllerButton _lastKeyPressed;
    private bool _stopActiveController = false;
    private bool _stopListening = false;
    // Event to notify when a button is pressed
    public event EventHandler<KeyPressedEventArgs>? KeyPressed;
    public event EventHandler<SecretActivatedEventArgs>? SecretActivated;
    public event EventHandler? ActiveControllerLost;

    public ControllerHook()
    {
        _controllers = new Controller[4]; // 4 possible controllers

        _buttonStates = new List<Tuple<XboxControllerButton, bool, DateTime>>[4];

        for (int i = 0; i < 4; i++)
        {
            _controllers[i] = new Controller((UserIndex)i);
            _buttonStates[i] = new List<Tuple<XboxControllerButton, bool, DateTime>>();
        }
        _lastKeyPressed = XboxControllerButton.Unknown;
    }

    // Method to get the last key press
    public XboxControllerButton GetLastKeyPress()
    {
        return _lastKeyPressed;
    }

    // Method to get a list of all currently clicked buttons
    public List<XboxControllerButton> GetClickedButtons()
    {
        var clickedButtons = new List<XboxControllerButton>();

        foreach (var buttonState in _buttonStates)
        {
            foreach (var kvp in buttonState)
            {
                if (kvp.Item2)
                {
                    clickedButtons.Add(kvp.Item1);
                }
            }
        }

        return clickedButtons;
    }

    // Method to get the values of the left and right joysticks
    public JoystickValues GetJoystickValues()
    {
        var joystickValues = new JoystickValues();

        // Assuming only one controller is connected
        var state = _controllers[0].GetState().Gamepad;

        // Normalize joystick values to the range [-1, 1]
        joystickValues.LeftX = state.LeftThumbX / 32767.0f;
        joystickValues.LeftY = state.LeftThumbY / 32767.0f;
        joystickValues.RightX = state.RightThumbX / 32767.0f;
        joystickValues.RightY = state.RightThumbY / 32767.0f;

        return joystickValues;
    }

    // Event handler for button presses
    protected virtual void OnKeyPressed(XboxControllerButton button, bool isButtonHeld)
    {
        KeyPressed?.Invoke(this, new KeyPressedEventArgs { PressedButton = button, IsButtonHeld = isButtonHeld });
    }

    // Periodically check for button presses
    public async Task StartChecking()
    {
        InitializeButtons();
        var button = XboxControllerButton.Back;

        while (!_stopListening)
        {
            for (int i = 0; i < 4; i++  )
            {
                var foundButton = _buttonStates[i].FirstOrDefault(x => x.Item1 == button);
                if (!_controllers[i].IsConnected)
                { continue; }
                var gamepad = _controllers[i].GetState().Gamepad;
                var backButtonPressed = gamepad.Buttons.HasFlag(GamepadButtonFlags.Back);
                if (foundButton != null)
                {
                    if (foundButton.Item2 != backButtonPressed)
                    {
                        _buttonStates[i].Remove(foundButton);
                        _buttonStates[i].Add(new Tuple<XboxControllerButton, bool, DateTime>(button, backButtonPressed, DateTime.Now));
                    }
                    else if (backButtonPressed && foundButton.Item2 == backButtonPressed && DateTime.Now - foundButton.Item3 >= TimeSpan.FromSeconds(2))
                    {
                        // Button is being held
                        SecretActivated?.Invoke(this, new SecretActivatedEventArgs() { Index = (UserIndex)i });
                            await StartCheckingTypingController((UserIndex)i);
                    }
                }
            }

            await Task.Delay(50);
        }

        _stopListening = false;
        _buttonStates = new List<Tuple<XboxControllerButton, bool, DateTime>>[4];
    }

    private async Task StartCheckingTypingController(UserIndex index)
    {
        var controller = _controllers.FirstOrDefault(x => x.UserIndex == index);
        if (controller is null)
            return;
        var firstButtonPressed = false;
        while (controller.IsConnected && !_stopActiveController)
        {
            foreach (XboxControllerButton button in Enum.GetValues(typeof(XboxControllerButton)))
            {
                bool isButtonPressed = IsButtonPressed(index, button);
                var foundButton = _buttonStates[(int)index].FirstOrDefault(x => x.Item1 == button);
                if (foundButton != null)
                {   
                    if (foundButton.Item2 != isButtonPressed)
                    {
                        if (isButtonPressed && firstButtonPressed)
                        {
                            OnKeyPressed(button, false);
                        }
                        firstButtonPressed = true;
                        _buttonStates[(int)index].Remove(foundButton);
                        _buttonStates[(int)index].Add(new Tuple<XboxControllerButton, bool, DateTime>(button, isButtonPressed, DateTime.Now));
                    }
                    else if (isButtonPressed && foundButton.Item2 == isButtonPressed && DateTime.Now - foundButton.Item3 >= TimeSpan.FromSeconds(2))
                    {
                        if (firstButtonPressed)
                        {
                            OnKeyPressed(button, true);
                        }
                    }                 
                }
            }

            await Task.Delay(50);
        }

        if (!controller.IsConnected)
        {
            ActiveControllerLost?.Invoke(this, new EventArgs());
        }

        _stopActiveController = false;
    }

    public void StopChecking()
    {
        _stopListening = true;
    }

    public void StopCheckingTypingController()
    {
        _lastKeyPressed = XboxControllerButton.Unknown;
        _stopActiveController = true;
    }

    // Method to check if a button or joystick is pressed
    private bool IsButtonPressed(UserIndex index, XboxControllerButton button)
    {
        var gamepad = _controllers[(int)index].GetState().Gamepad;

        switch (button)
        {
            case XboxControllerButton.A:
                return gamepad.Buttons.HasFlag(GamepadButtonFlags.A);
            case XboxControllerButton.B:
                return gamepad.Buttons.HasFlag(GamepadButtonFlags.B);
            case XboxControllerButton.X:
                return gamepad.Buttons.HasFlag(GamepadButtonFlags.X);
            case XboxControllerButton.Y:
                return gamepad.Buttons.HasFlag(GamepadButtonFlags.Y);
            case XboxControllerButton.DPadDown:
                return gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown);
            case XboxControllerButton.DPadUp:
                return gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp);
            case XboxControllerButton.DPadLeft:
                return gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft);
            case XboxControllerButton.DPadRight:
                return gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight);
            case XboxControllerButton.Back:
                return gamepad.Buttons.HasFlag(GamepadButtonFlags.Back);
            case XboxControllerButton.Start:
                return gamepad.Buttons.HasFlag(GamepadButtonFlags.Start);
            case XboxControllerButton.LeftBumper:
                return gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder);
            case XboxControllerButton.RightBumper:
                return gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder);

            // Joysticks
            case XboxControllerButton.LeftStick:
                return IsJoystickMoved(gamepad.LeftThumbX, gamepad.LeftThumbY) || gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb);
            case XboxControllerButton.RightStick:
                return IsJoystickMoved(gamepad.RightThumbX, gamepad.RightThumbY) || gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb);

            // Triggers
            case XboxControllerButton.LeftTrigger:
                return gamepad.LeftTrigger > 0;
            case XboxControllerButton.RightTrigger:
                return gamepad.RightTrigger > 0;

            default:
                return false;
        }
    }

    // Method to check if a joystick is moved
    private bool IsJoystickMoved(short x, short y)
    {
        // You can customize this threshold based on your sensitivity requirement
        int joystickThreshold = 10000;

        return Math.Abs(x) > joystickThreshold || Math.Abs(y) > joystickThreshold;
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            foreach (XboxControllerButton button in Enum.GetValues(typeof(XboxControllerButton)))
            {
                _buttonStates[i].Add(new Tuple<XboxControllerButton, bool, DateTime>(button, false, DateTime.Now));
            }
        }
    }
}