using XboxControllerHook.Enums;

namespace XboxControllerHook.Helpers
{
    // Event arguments for the KeyPressed event
    public class KeyPressedEventArgs : EventArgs
    {
        public XboxControllerButton PressedButton { get; set; }
        public bool IsButtonHeld { get; set; }
    }
}
