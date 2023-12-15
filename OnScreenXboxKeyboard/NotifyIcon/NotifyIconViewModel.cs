using KeyPad;
using OnScreenXboxKeyboard.Helpers;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using XboxControllerHook.Helpers;
using XboxControllerHook.Interfaces;
using Application = System.Windows.Application;

namespace OnScreenXboxKeyboard.NotifyIcon
{
    public class NotifyIconViewModel
    {
        private IControllerHook _hook;
        private VirtualKeyboard? _keyboardWindow;
        private IntPtr _foregroundWindow;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        public NotifyIconViewModel()
        {
            //Start and subscribe to controller hook
            _hook = new ControllerHook();
            _hook.SecretActivated -= HandleControllerActivation;
            _hook.ActiveControllerLost -= HandleActiveControllerDiscounnected;
            _hook.KeyPressed -= HandleControllerKeyPressed;
            _hook.SecretActivated += HandleControllerActivation;
            _hook.ActiveControllerLost += HandleActiveControllerDiscounnected;
            _hook.KeyPressed += HandleControllerKeyPressed;
            _hook.StartChecking();

            //Get processes
            _foregroundWindow = GetForegroundWindow();
        }

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }

        private void HandleClosingKeyboard(object? sender, EventArgs e)
        {
            _keyboardWindow = null;
            _hook.StopCheckingTypingController();
        }
        private string GetWindowTitle(IntPtr hWnd)
        {
            var length = GetWindowTextLength(hWnd) + 1;
            var title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }

        private void HandleControllerKeyPressed(object? sender, KeyPressedEventArgs e)
        {
            var button = e.PressedButton;

            if (_keyboardWindow == null && button != XboxControllerHook.Enums.XboxControllerButton.RightStick)
            {
                return;
            }

            var currentForeGroundWindow = GetForegroundWindow();
            var wantedWindow = FindWindow(null, _keyboardWindow?.Title);

            if (currentForeGroundWindow != wantedWindow && button != XboxControllerHook.Enums.XboxControllerButton.RightStick)
            {
                return;
            }
            if (button == XboxControllerHook.Enums.XboxControllerButton.DPadLeft)
            {
                _keyboardWindow.MoveFocus(FocusNavigationDirection.Left);
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.DPadRight)
            {
                _keyboardWindow.MoveFocus(FocusNavigationDirection.Right);
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.DPadUp)
            {
                _keyboardWindow.MoveFocus(FocusNavigationDirection.Up);
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.DPadDown)
            {
                _keyboardWindow.MoveFocus(FocusNavigationDirection.Down);
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.A)
            {
                _keyboardWindow.HandleButtonClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.Y)
            {
                _keyboardWindow.HandleSpaceClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.B)
            {
                _keyboardWindow.HandleFuncClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.X)
            {
                _keyboardWindow.HandleBackSpaceClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.LeftBumper)
            {
                _keyboardWindow.HandleCapsClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.RightBumper)
            {
                _keyboardWindow.HandleNumPadSwitch();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.Start)
            {
                if (!e.IsButtonHeld)
                {
                    _keyboardWindow?.HandleSubmitClick();
                }
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.LeftTrigger)
            {
                _keyboardWindow.HandleLeftClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.RightTrigger)
            {
                _keyboardWindow.HandleRightClick();
            }
            else if (button == XboxControllerHook.Enums.XboxControllerButton.RightStick)
            {
                if (!e.IsButtonHeld && wantedWindow == currentForeGroundWindow)
                {
                    _keyboardWindow?.HandleCLoseClick();
                }
                else if (e.IsButtonHeld && wantedWindow != currentForeGroundWindow)
                {
                    if (_foregroundWindow != GetForegroundWindow())
                    {
                        _foregroundWindow = currentForeGroundWindow;
                        _keyboardWindow?.SetForeGroundWindow(_foregroundWindow);
                    }

                    _keyboardWindow.SwitchWindow(wantedWindow);
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
        private void ShowHideKeyboard()
        {
            if (_keyboardWindow != null)
            {
                return;
            }
            // Get processes
            _foregroundWindow = GetForegroundWindow();
            // start keyboard
            _keyboardWindow = new VirtualKeyboard();
            _keyboardWindow.Show();
            GetWindowThreadProcessId(FindWindow(null, _keyboardWindow?.Title), out uint id);
            _keyboardWindow.SetProcess(_foregroundWindow);
            _keyboardWindow.Closed += HandleClosingKeyboard;
        }
    }
}
