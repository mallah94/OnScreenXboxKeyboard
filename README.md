# OnScreenXboxKeyboard
a simple onscreen keyboard that works with the controller

# INTRO
This project was made for fun and just because I had free from work, please do not assume this keyboard works perfectly.
There will be bugs, I will try and fix them whenever I have time.

In order to get certain functionality to work I had to use some really hacky methods:
 + In order to get the keyboard to come to the foreground (windows does not allow setting foreground if you are not already the foreground window) <br /> so I have made a manual mouse move click to gain focus, this might not work in every app
 + in order to block input to apps that also listen to the controller, the keyboard has to be in the foreground. <br />  To make sure you can still type, the software flips foreground windows back and foth (basically alt+tab back and forth), this also might not work in every app.

# Support
 + XInput in general, up to 4 controllers. 

# How to install
 + Just download and run the installer in the release. 
 + The app will auto create a desktop shortcut and add itself as a startup app, if this behaviour is unwanted please feel free to delete the shortcuts.
 + **If you are updating from an earlier version please uninstall first then re install. otherwise you will get 2 versions running**

# How To Use
 + Once the program starts it an icon will be shown in the tray (task bar hidden icons)
 + The app is now listening for controllers
 + to activate the keyboard just press and hold the right joystick (R3) button on your controller for 2 seconds.
 + You can use the D-Pad to select a letter and the A button to confirm it.
 + To exit the keyboard just press the right joystick (R3) once.
 + special input:
     + "B" is either the steam overlay shortcut or an "alt+F4" combo if capslock is on.
     + "Y" is the space bar.
     + "X" is backspace.
     + "RT" is either the right arrow key or up key if capslock is on
     + "LT" is either the left arrow or down key if the capslock is on.
     + "RB" is a toggle between the letter and number/symbol layout
     + "LB" is the capslock toggle.
     + "Start" is enter.
+ Only one instance of the keyboard can be shown at a time. if multiple controllers are connected only the controller that started the keyboard can type. 
+ If the keyboard is in focus it will have a cyan boarder, otherwise it will not.. when it is not in focus you can regain focus by holding the right joystick (R3) button again.
+ If the keyboard is not in focus then it will not read controller input.

# Work To Be Done
  + Fix random bugs.
  + refactor code and architecture.
  + find a better way of blocking controller input instead of shifting app focus (current solution is very hacky...)
  + Add a configuration manager so that the keyboard trigger button can be changed by game/app
   
# Known bugs:
  + Fix installer not updating
  + Focus border is not always functioning properly (cyan border around the keyboard)
  + More to come I am assuming
