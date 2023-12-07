# OnScreenXboxKeyboard
a simple onscreen keyboard that works with the controller

# INTRO
This project was made for fun and just because I had free from work, please do not assume this keyboard works perfectly.
There will be bugs, I will try and fix them whenever I have time.

# Support
 + XInput in general, up to 4 controllers. 

# How to install
 + Just download and run the installer in the release. 

# How To Use
 + Once the program starts it will show a small keyboard icon at the left corner of your desktop (yes this is unnecessary and is a remnant from a way older version of this code that I had locally I will remove it soon I hope)
 + The app is now listening for controllers
 + to activate the keyboard just press and hold the "Select"/"Back" button on your controller for 2 seconds.
 + You can use the D-Pad to select a letter and the A button to confirm it.
 + To exit the keyboard just press the "start button" once.
 + special input:
     + "B" is either the steam overlay shortcut or an "alt+F4" combo if capslock is on.
     + "Y" is the space bar.
     + "X" is backspace.
     + "RT" is either the right arrow key or up key if capslock is on
     + "LT" is either the left arrow or down key if the capslock is on.
     + "RB" is a toggle between the letter and number/symbol layout
     + "LB" is the capslock toggle.
+ Only one instance of the keyboard can be shown at a time. if multiple controllers are connected only the controller that the started the keyboard can type. 
  
# Work To Be Done
  + As mentioned above rewrite the software so it does not have the icon at the button corner of the screen. (it is useless in the current version of the app)
      + Move into a taskbar app with a hidden icon.
   
# Known bugs:
  + Some apps that also read controller input will continue to function while the keyboard is on. ==> The Windows settings app, for example, has this problem which means that you can not type there since the focus moves away from the selected text box if you do.
  + More to come I am assuming
  
