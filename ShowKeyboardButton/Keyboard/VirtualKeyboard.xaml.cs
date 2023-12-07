using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KeyPad
{
    /// <summary>
    /// Logica di interazione per VirtualKeyboard.xaml
    /// </summary>
    public partial class VirtualKeyboard : Window, INotifyPropertyChanged
    {
        #region Public Properties

        private bool _showNumericKeyboard;
        private bool _isInNumpad = false;
        private bool _controllerClick = false;
        private bool _capsLocked = true;
        public bool ShowNumericKeyboard
        {
            get { return _showNumericKeyboard; }
            set { _showNumericKeyboard = value; this.OnPropertyChanged("ShowNumericKeyboard"); }
        }

        #endregion

        #region Constructor

        public VirtualKeyboard(Window wndOwner)
        {
            InitializeComponent();
            this.Owner = wndOwner;
            this.DataContext = this;

            // Center the window horizontally
            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            // Set the window position in the bottom third vertically
            this.Top = SystemParameters.PrimaryScreenHeight - this.Height;

            FlipCapitalization();
            // Set focus to the VirtualKeyboard window
            this.Loaded += (sender, e) => { this.Focus(); };
        }

        #endregion

        #region Callbacks
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // Set focus to the "q" button
            var qButton = FindButtonByCommandParameter("Q");
            if (qButton != null)
            {
                qButton.Focus();
            }
        }

        private System.Windows.Controls.Button? FindButtonByCommandParameter(string commandParameter)
        {
            foreach (UIElement element in AlfaKeyboard.Children)
            {
                if (element is Grid grid)
                {
                    foreach (UIElement childElement in grid.Children)
                    {
                        if (childElement is System.Windows.Controls.Button button && button.CommandParameter?.ToString() == commandParameter)
                        {
                            return button;
                        }
                    }
                }
            }
            foreach (UIElement element in NumKeyboard.Children)
            {
                if (element is Grid grid)
                {
                    foreach (UIElement childElement in grid.Children)
                    {
                        if (childElement is System.Windows.Controls.Button button && button.CommandParameter?.ToString() == commandParameter)
                        {
                            return button;
                        }
                    }
                }
            }
            return null;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!_controllerClick)
            {
                return;
            }
            System.Windows.Controls.Button? button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                switch (button.CommandParameter.ToString())
                {
                    case "LSHIFT":
                        FlipCapitalization();
                        break;
                    
                    case "RETURN":
                        this.Close();
                        break;

                    case "BACK":
                        TypeResults("{BACKSPACE}");
                        break;

                    case "TAB":
                        TypeResults("{TAB}");
                        break;

                    case "FUNC":
                        if (_capsLocked)
                        {
                            TypeResults("%{F4}");
                        }
                        else
                        {
                            TypeResults("^1");
                        }
                        break;

                    case "CARROW3":
                    case "CARROW1":
                        if (_capsLocked)
                        {
                            TypeResults("{DOWN}");
                        }
                        else 
                        {
                            TypeResults("{LEFT}");
                        }
                        break;

                    case "CARROW4":
                    case "CARROW2":
                        if (_capsLocked)
                        {
                            TypeResults("{UP}");
                        }
                        else
                        {
                            TypeResults("{RIGHT}");
                        }
                        break;
                    
                    default:
                        TypeResults(button.Content.ToString());
                        break;
                }
                _controllerClick = false;
            }            
        }

        private void FlipCapitalization()
        {
            //Handle Letters
            Regex upperCaseRegex = new Regex("[A-Z]");
            Regex lowerCaseRegex = new Regex("[a-z]");
            System.Windows.Controls.Button? btn;
            foreach (UIElement elem in AlfaKeyboard.Children) //iterate the main grid
            {
                Grid? grid = elem as Grid;
                if (grid != null)
                {
                    foreach (UIElement uiElement in grid.Children)  //iterate the single rows
                    {
                        btn = uiElement as System.Windows.Controls.Button;
                        if (btn != null) // if button contains only 1 character
                        {
                            if (btn.Content.ToString()?.Length == 1)
                            {
                                var content = btn.Content.ToString();
                                if (content != null)
                                {
                                    if (upperCaseRegex.Match(content).Success) // if the char is a letter and uppercase
                                        btn.Content = content.ToLower();
                                    else if (lowerCaseRegex.Match(content).Success) // if the char is a letter and lower case
                                        btn.Content = content.ToUpper();
                                }
                            }

                        }
                    }
                }
            }

            //Handle special charachters
            foreach (UIElement elem in NumKeyboard.Children) //iterate the main grid
            {
                Grid? grid = elem as Grid;
                Dictionary<string, string> charMap = new Dictionary<string, string>()
                {
                    { "$", "!"},
                    { "&", "#"},
                    { "@", "%"},
                    { ";", "^"},
                    { ":", "*"},
                    { "[", "{"},
                    { "]", "}"},
                    { "\\", "|"},
                    { ",", "<"},
                    { ".", ">"},
                    { "?", "~"},
                    { "'", "`"}
                };

                if (grid != null)
                {
                    foreach (UIElement uiElement in grid.Children)  //iterate the single rows
                    {
                        btn = uiElement as System.Windows.Controls.Button;
                        if (btn != null) // if button contains only 1 character
                        {
                            var content = btn.Content.ToString();
                            if (content == null)
                            {
                                continue;
                            }
                            if (_capsLocked)
                            {
                                if (charMap.ContainsKey(content))
                                {
                                    content = charMap[content];
                                    btn.Content = content;
                                }
                            }
                            else
                            {
                                var entry = charMap.Where(x => x.Value == content).Select(x => (KeyValuePair<string, string>?)x).FirstOrDefault();
                                if (entry != null)
                                {
                                    content = entry.Value.Key;
                                    btn.Content = content;
                                }
                            }
                        }
                    }
                }
            }

            //HandleArrows
            var arrow1 = this.FindName("Arrow1Path") as Path;
            var arrow2 = this.FindName("Arrow2Path") as Path;
            var arrow3 = this.FindName("Arrow3Path") as Path;
            var arrow4 = this.FindName("Arrow4Path") as Path;
            var func = this.FindName("FuncPath") as Path;
            var func1 = this.FindName("Func1Path") as Path;
            Geometry arrowLeft = Geometry.Parse("M20,11V13H8L13.5,18.5L12.08,19.92L4.16,12L12.08,4.08L13.5,5.5L8,11H20Z");
            Geometry arrowRight = Geometry.Parse("M4,11V13H16L10.5,18.5L11.92,19.92L19.84,12L11.92,4.08L10.5,5.5L16,11H4Z");
            Geometry arrowUp = Geometry.Parse("M13,20H11V8L5.5,13.5L4.08,12.08L12,4.16L19.92,12.08L18.5,13.5L13,8V20Z");
            Geometry arrowDown = Geometry.Parse("M11,4H13V16L18.5,10.5L19.92,11.92L12,19.84L4.08,11.92L5.5,10.5L11,16V4Z");
            Geometry steam = Geometry.Parse("M12,2A10,10 0 0,1 22,12A10,10 0 0,1 12,22C7.4,22 3.55,18.92 2.36,14.73L6.19,16.31C6.45,17.6 7.6,18.58 8.97,18.58C10.53,18.58 11.8,17.31 11.8,15.75V15.62L15.2,13.19H15.28C17.36,13.19 19.05,11.5 19.05,9.42C19.05,7.34 17.36,5.65 15.28,5.65C13.2,5.65 11.5,7.34 11.5,9.42V9.47L9.13,12.93L8.97,12.92C8.38,12.92 7.83,13.1 7.38,13.41L2,11.2C2.43,6.05 6.73,2 12,2M8.28,17.17C9.08,17.5 10,17.13 10.33,16.33C10.66,15.53 10.28,14.62 9.5,14.29L8.22,13.76C8.71,13.58 9.26,13.57 9.78,13.79C10.31,14 10.72,14.41 10.93,14.94C11.15,15.46 11.15,16.04 10.93,16.56C10.5,17.64 9.23,18.16 8.15,17.71C7.65,17.5 7.27,17.12 7.06,16.67L8.28,17.17M17.8,9.42C17.8,10.81 16.67,11.94 15.28,11.94C13.9,11.94 12.77,10.81 12.77,9.42A2.5,2.5 0 0,1 15.28,6.91C16.67,6.91 17.8,8.04 17.8,9.42M13.4,9.42C13.4,10.46 14.24,11.31 15.29,11.31C16.33,11.31 17.17,10.46 17.17,9.42C17.17,8.38 16.33,7.53 15.29,7.53C14.24,7.53 13.4,8.38 13.4,9.42Z");
            Geometry exit = Geometry.Parse("M2 12C2 9.21 3.64 6.8 6 5.68V3.5C2.5 4.76 0 8.09 0 12S2.5 19.24 6 20.5V18.32C3.64 17.2 2 14.79 2 12M15 3C10.04 3 6 7.04 6 12S10.04 21 15 21 24 16.96 24 12 19.96 3 15 3M20 15.59L18.59 17L15 13.41L11.41 17L10 15.59L13.59 12L10 8.41L11.41 7L15 10.59L18.59 7L20 8.41L16.41 12L20 15.59Z");

            if (arrow1 == null || arrow2 == null || arrow3 == null || arrow4 == null || func == null || func1 == null)
            {
                return;
            }
            if (!_capsLocked)
            {
                arrow1.Data = arrowDown;
                arrow3.Data = arrowDown;
                arrow2.Data = arrowUp;
                arrow4.Data = arrowUp;
                func.Data = exit;
                func1.Data = exit;
            }
            else
            {
                arrow1.Data = arrowLeft;
                arrow3.Data = arrowLeft;
                arrow2.Data = arrowRight;
                arrow4.Data = arrowRight;
                func.Data = steam;
                func1.Data = steam;
            }
            _capsLocked = !_capsLocked;
        }

        private void TypeResults(string? text)
        {
            if (text is null)
            { return; }
            var specialChars = new List<string> { "+", "^", "%","~", "(", ")", "{", "}", "[", "]" };
            if (text.Length == 1 && specialChars.Contains(text))
            {
                text = "{" + text + "}";
            }
            try
            {         
                SendKeys.SendWait(text);
            }
            catch (Exception)
            {
                
            }
        }

        public void MoveFocus(FocusNavigationDirection direction)
        {
            var focusedElement = FocusManager.GetFocusedElement(this) as UIElement;
            if (focusedElement != null)
            {
                TraversalRequest request = new TraversalRequest(direction);
                var focusedButton = focusedElement as System.Windows.Controls.Button;
                // Set focus to the next/previous button based on the current keyboard mode
                if (_isInNumpad)
                {
                    var nextButton = FindNextButton(NumKeyboard, focusedButton, direction);
                    nextButton?.Focus();
                }
                else
                {
                    var nextButton = FindNextButton(AlfaKeyboard, focusedButton, direction);
                    nextButton?.Focus();
                }
            }
        }

        private System.Windows.Controls.Button? FindNextButton(System.Windows.Controls.Panel panel, System.Windows.Controls.Button? currentButton, FocusNavigationDirection direction)
        {
            if (currentButton == null)
            { 
                return null;
            }
            var grids = panel.Children.OfType<Grid>().ToList();
            int i = 0;
            foreach (var grid in grids)
            {
                var buttons = grid.Children.OfType<System.Windows.Controls.Button>().ToList();
                var index = buttons.IndexOf(currentButton);

                if (index != -1)
                {
                    var previousGrid = i > 0 ? grids[i - 1] : grids[grids.Count - 1];
                    var nextGrid = i < grids.Count -1 ? grids[i + 1] : grids[0];

                    if (direction == FocusNavigationDirection.Left)
                    {
                        index = (index - 1);
                        index = index < 0 ? buttons.Count - 1 : index;
                    }
                    else if (direction == FocusNavigationDirection.Right)
                    {
                        index = (index + 1);
                        index = index > buttons.Count() - 1 ? 0 : index;
                    }
                    else if (direction == FocusNavigationDirection.Up)
                    {
                        var previousGRidButtons = previousGrid.Children.OfType<System.Windows.Controls.Button>().ToList();
                        if (index > previousGRidButtons.Count() - 1)
                        {
                            index = previousGRidButtons.Count() - 1;
                        }
                        buttons = previousGRidButtons;
                    }
                    else if (direction == FocusNavigationDirection.Down)
                    {
                        var nextGRidButtons = nextGrid.Children.OfType<System.Windows.Controls.Button>().ToList();
                        if (index > nextGRidButtons.Count() - 1)
                        {
                            index = nextGRidButtons.Count() - 1;
                        }
                        buttons = nextGRidButtons;
                    }

                    return buttons[index];
                }
                i++;
            }

            return null;
        }

        public void HandleSpaceClick()
        {
            _controllerClick = true;
            var button = FindButtonByCommandParameter("SPACE");
            button?.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
        }

        public void HandleBackSpaceClick()
        {
            _controllerClick = true;
            var button = FindButtonByCommandParameter("BACK");
            button?.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
        }

        public void HandleFuncClick()
        {
            _controllerClick = true;
            var button = FindButtonByCommandParameter("FUNC");
            button?.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
        }

        public void HandleTabClick()
        {
            _controllerClick = true;
            var button = FindButtonByCommandParameter("TAB");
            button?.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
        }

        public void HandleCapsClick()
        {
            _controllerClick = true;
            var button = FindButtonByCommandParameter("LSHIFT");
            button?.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
        }

        public void HandleSubmitClick()
        {
            _controllerClick = true;
            var button = FindButtonByCommandParameter("RETURN");
            button?.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
        }

        public void HandleRightClick()
        {
            _controllerClick = true;
            var button = FindButtonByCommandParameter("CARROW4");
            button?.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
        }

        public void HandleLeftClick()
        {
            _controllerClick = true;
            var button = FindButtonByCommandParameter("CARROW3");
            button?.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
        }

        public void HandleNumPadSwitch()
        {
            _controllerClick = true;
            var button = _isInNumpad ? this.FindName("NumSwitch") as ToggleButton : this.FindName("AlfaSwitch") as ToggleButton;
            if (button != null)
            {
                PushToggleButton(button);
            }
        }

        public void HandleButtonClick()
        {
            _controllerClick = true;
            // Get the focused element
            var focusedElement = FocusManager.GetFocusedElement(this);

            // Check if the focused element is a button
            if (focusedElement is System.Windows.Controls.Button button)
            {
                button.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            }

            // Check if the focused element is a toggled button
            if (focusedElement is ToggleButton toggleButton)
            {
                PushToggleButton(toggleButton);
            }
        }

        private void PushToggleButton(ToggleButton b)
        {
            _isInNumpad = !_isInNumpad;
            ToggleButtonAutomationPeer peer = new ToggleButtonAutomationPeer(b);
            System.Windows.Automation.Provider.IToggleProvider? toggleProvider = peer.GetPattern(PatternInterface.Toggle) as System.Windows.Automation.Provider.IToggleProvider;
            toggleProvider?.Toggle();

            var qButton = FindButtonByCommandParameter("Q");
            var numButton = FindButtonByCommandParameter("_1");
            if (_isInNumpad)
            {
                numButton?.Focus();
            }
            else 
            {
                qButton?.Focus();
            }
            _controllerClick = false;
        }
        #endregion        

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion        
    }
}
