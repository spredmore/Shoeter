-----------------------------------------------------------------------

SSSSSSSSSS H	    H OOOOOOOOOO EEEEEEEEEE TTTTTTTTT EEEEEEEEEE RRRRRR
S	       H	    H O	       O E              T     E          R    R
SSSSSSSSSS HHHHHHHHHH O	       O EEEEEEEEEE     T     EEEEEEEEEE RRRRRR 
	     S H	    H O	       O E              T     E          R RR
SSSSSSSSSS H	    H OOOOOOOOOO EEEEEEEEEE     T     EEEEEEEEEE R   RR

------------------------------------------------------------------------

How To Install:

- Run setup.exe. It should install the dependencies you need.
- Dependencies are:
	XNA Framework Redistributable 4.0
	Microsoft .NET 4.0
- You can also run the ClickOne Application "customAnimation" to install the game as well.
- NOTE: You only need to do one or the other. It will probably work fine if you do both installation methods though.

How To Run:

- Run "customAnimation" from the Start Menu, or where ever you installed the game to.

Controls:

- WASD to move.
- W and Space are to jump.
- Use the mouse to control the power and angle of the shot.
- Scroll wheel controls the power.
- Left click shoots the Guy.
- Right click to snap the Guy back to the Shoes. (Use this when you fall off and the Guy is sitting by himself)

Game Premise:

- The goal of the game is to shoot the Guy, and catch him with the Shoes.
- You control the angle at which the Guy is shot by moving the mouse around the screen. The Guy will travel in a parabola upon being shot.
	NOTE: The angle which the Guy will be shot is displayed in the Interface in the top left corner of the screen.
- You control the Power of the shot with the scroll wheel.
- Use the Power of the shot in conjunction with the angle to shoot the Guy where you want.
- You run faster and jump higher with the Shoes, so moving around the Guy is not recommended.
- The goal is the blue square. The level automatically switches to the next level upon collision.
- If you fall to the bottom of the screen, the Shoes are automatically transported to the beginning of the level.
	NOTE: If you shot the Guy and he is sitting somewhere, you can use Right Click to call him back to you.
- There may be more than one way to solve a level.

Interface Control:
- The Interface controls the movement of the Guy and Shoes. You can change these however you like.
- To change the values in the Interface, you must first unlink the Guy and Shoes. To do this, press F12. If you don't do this, you will not be able to change the values.
- Once the Guy and Shoes are unlinked, you can use the numbers specified next to the particular attribute to change that value.
- Example: To change the Shoes gravity, first unlink them. Then, press 9 to increase gravity, and 6 to decrease gravity.
- The use of a Numpad is preferred, as the Interface was designed to be used with a Numpad. Normal number keys work as well.
	NOTE: XNA doesn't support some keys, so certain attribute keys are different than what is displayed on the screen. They are listed below:
		Left/Right Arrow Keys control "Shoes - Fall From Tile Rate"
		Up/Down Arrow Keys control "Shoes - Gravity"
- To select a Preset, click one of the Function keys (F1, F2, etc). Presets are grouped values for attributes that have already been set. 
	NOTE: Some presets don't work correctly.

Other:

- This is a demo. Some features have not been implemented, or may not work correctly.
- If you experience performance issues, close some open programs. Sometimes XNA doesn't play nice when other programs are open.
- This game has only been tested in Windows 7. This game is not guarenteed to work, let alone on any operating system other than Windows 7.
