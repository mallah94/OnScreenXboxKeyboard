using SharpDX.XInput;
using XboxControllerHook.Enums;
using XboxControllerHook.Helpers;

namespace XboxControllerHook.Interfaces
{
    // Interface that contains the event "KeyPressed" and the method "GetLastKeyPress"
    public interface IControllerHook
    {
        event EventHandler<KeyPressedEventArgs> KeyPressed;
        event EventHandler<SecretActivatedEventArgs> SecretActivated;
        event EventHandler ActiveControllerLost;
        XboxControllerButton GetLastKeyPress();
        List<XboxControllerButton> GetClickedButtons();
        JoystickValues GetJoystickValues();
        Task StartChecking();
        void StopChecking();
        void StopCheckingTypingController();

    }
}
