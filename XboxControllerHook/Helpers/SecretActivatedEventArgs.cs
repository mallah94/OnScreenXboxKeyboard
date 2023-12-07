using SharpDX.XInput;
using XboxControllerHook.Enums;

namespace XboxControllerHook.Helpers
{
    // Event arguments for the KeyPressed event
    public class SecretActivatedEventArgs : EventArgs
    {
        public UserIndex Index { get; set; }
    }
}
