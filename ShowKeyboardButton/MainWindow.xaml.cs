using KeyPad;
using SharpDX;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using XboxControllerHook.Helpers;
using XboxControllerHook.Interfaces;
using Application = System.Windows.Application;

namespace FloatingButton
{
    public partial class MainWindow : Window
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        private IControllerHook _hook;
        private VirtualKeyboard? _keyboardWindow;
        public MainWindow()
        {
            InitializeComponent();

            // Set the window position to the top-left corner of the screen
            Left = 0;
            Top = SystemParameters.WorkArea.Height - Height;

            // Set ResizeMode to NoResize
            ResizeMode = ResizeMode.NoResize;

            //Start and subscribe to controller hook
            _hook = new ControllerHook();
            _hook.SecretActivated += HandleControllerActivation;
            _hook.ActiveControllerLost += HandleActiveControllerDiscounnected;
            _hook.KeyPressed += HandleControllerKeyPressed;
            _hook.StartChecking();
        }
        private void HandleClosingKeyboard(object? sender, EventArgs e) 
        {
            _keyboardWindow = null;
            _hook.StopCheckingTypingController();
        }

        private void HandleControllerKeyPressed(object? sender, KeyPressedEventArgs e)
        {
            var button = e.PressedButton;

            if (button == XboxControllerHook.Enums.XboxControllerButton.DPadLeft)
            {
                _keyboardWindow?.MoveFocus(FocusNavigationDirection.Left);
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.DPadRight)
            {
                _keyboardWindow?.MoveFocus(FocusNavigationDirection.Right);
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.DPadUp)
            {
                _keyboardWindow?.MoveFocus(FocusNavigationDirection.Up);
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.DPadDown)
            {
                _keyboardWindow?.MoveFocus(FocusNavigationDirection.Down);
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.A)
            {
                _keyboardWindow?.HandleButtonClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.Y)
            {
                _keyboardWindow?.HandleSpaceClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.B)
            {
                _keyboardWindow?.HandleFuncClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.X)
            {
                _keyboardWindow?.HandleBackSpaceClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.LeftBumper)
            {
                _keyboardWindow?.HandleCapsClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.RightBumper)
            {
                _keyboardWindow?.HandleNumPadSwitch();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.Start)
            {
                _keyboardWindow?.HandleSubmitClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.LeftTrigger)
            {
                _keyboardWindow?.HandleLeftClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.RightTrigger)
            {
                _keyboardWindow?.HandleRightClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.Back)
            {
                if (!e.IsButtonHeld)
                {
                    _keyboardWindow?.HandleTabClick();
                }
            }
        }

        private void HandleActiveControllerDiscounnected(object? sender, EventArgs e)
        {
            if (_keyboardWindow != null)
            {
                _keyboardWindow.Close();
            }
        }

        private void HandleControllerActivation(object? sender, SecretActivatedEventArgs e)
        {
            if (_keyboardWindow == null)
            {
                ShowHideKeyboard();
            }
        }

        private void HandleButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging when the handle button is clicked
            ReleaseCapture();
            SendMessage(new System.Windows.Interop.WindowInteropHelper(this).Handle, WM_NCLBUTTONDOWN, (IntPtr)HT_CAPTION, IntPtr.Zero);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the application when the close button is clicked
            _hook.StopCheckingTypingController();
            _hook.StopChecking();
            _hook.SecretActivated -= HandleControllerActivation;
            _hook.ActiveControllerLost -= HandleActiveControllerDiscounnected;
            _hook.KeyPressed -= HandleControllerKeyPressed;
            Close();
        }
        private void ShowHideKeyboard()
        {
            if (_keyboardWindow != null)
            {
                return;
            }
            _keyboardWindow = new VirtualKeyboard(this);
            _keyboardWindow.Show();
            _keyboardWindow.Closed += HandleClosingKeyboard;
        }
    }
}
