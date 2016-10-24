using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace customAnimation
{
	class Shoes : Character
	{
		// The states of the keyboard.
		KeyboardState oldKeyboardState;
		KeyboardState newKeyboardState;

		Keys right;
		Keys left;
		Keys up;
		Keys down;

		public float airMovementSpeed = 600f;
		public float groundMovementSpeed = 300f;
		public float jumpImpulse = 20f;
		public float fallFromTileRate = 25f;

		public string preset;
		public string debug;
		public string debug2;

		public bool interfaceLinked = true;
		private bool interfaceEnabled = true;

		private ContentManager content;

		private int bouncingHorizontally = 0;       // -1 represents left, 0 represents no bouncing, 1 represents right.
		public bool delaySpringCollision = false;   // The Shoes can't use a Spring right after another Spring has already been used.
		Timer delaySpringCollisionTimer;            // Delays movement of the Shoes from using a Spring too quickly.
		Timer launcherTimer;                        // Delay before the Guy is launched.
		Timer delayBetweenLaunchesTimer;            // Delay so the Guy doesn't use another launcher too quickly.

		Vector2 launcherVelocity;                   // Used for handling velocity while the Shoes are being launched.
		public bool usingLauncher = false;
		bool hasBeenLaunched = false;
		private int collX;
		private int collY;

		bool turnOffFalling = false;

		public Shoes(Texture2D texture, State state, int currentFrame, int spriteWidth, int spriteHeight, int totalFrames, SpriteBatch spriteBatch, int screenHeight, int screenWidth, Keys up, Keys left, Keys down, Keys right, ContentManager content)
		{
			this.spriteTexture = texture;       // The sprite sheet we will be drawing from.
			this.state = state;                 // The initial state of the player.
			this.currentFrame = currentFrame;   // The current frame that we are drawing.
			this.spriteWidth = spriteWidth;     // The width of the individual sprite.
			this.spriteHeight = spriteHeight;   // The height of the individual sprite.
			this.totalFrames = totalFrames;     // The total frames in the current sprite sheet.
			this.spriteBatch = spriteBatch;     // The spriteBatch we will use to draw the player.
			this.screenHeight = screenHeight;
			this.screenWidth = screenWidth;
			this.right = right;
			this.left = left;
			this.up = up;
			this.down = down;
			this.content = content;

			gravity = 30f;
			debug = "";
			debug2 = "";
			collX = 0;
			collY = 0;

			this.delaySpringCollisionTimer = new Timer(0.3f);
			this.launcherTimer = new Timer(2f);
			this.delayBetweenLaunchesTimer = new Timer(2f);
		}

		public void Update(GameTime gameTime, ref Guy guy)
		{
			this.handleAnimation(gameTime);
			handleMovement(gameTime, ref guy);
			doInterface(guy.beingShot);

			oldKeyboardState = newKeyboardState; // In Update() so the interface works. Commented out at the bottom of handleMovement.
		}

		private void handleMovement(GameTime gameTime, ref Guy guy)
		{
			//debug = "delayBetweenLaunchesTimer: " + delayBetweenLaunchesTimer.TimerStarted.ToString();
			debug = "usingLauncher: " + usingLauncher.ToString();
			//debug = "isJumping: " + isJumping.ToString();
			debug2 = "hasBeenLaunched: " + hasBeenLaunched.ToString();
			

			float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
			newKeyboardState = Keyboard.GetState(); // Get the new state of the keyboard.

			// ******************************************************
			// Handles delaying the collision between the Guy and multiple launchers so he doesn't use another one too quickly.
			// This comment isn't consitent with what's written.
			// ******************************************************
			if (delaySpringCollisionTimer.TimerCompleted == true)
			{
				delaySpringCollisionTimer.stopTimer();
				delaySpringCollision = false;
			}

			// ******************************************************
			// Set the velocity based on if the Shoes are on the ground or in the air.
			// ******************************************************
			if (isJumping == true && usingLauncher == false && hasBeenLaunched == false)
			{
				velocity.X = airMovementSpeed;
			}
			else if (hasBeenLaunched == false) velocity.X = groundMovementSpeed;
			else
			{
				velocity.X = airMovementSpeed;
				velocity.X *= Math.Sign(launcherVelocity.X);
				velocity.X *= -1;
			}

			// ******************************************************
			// The player wants to jump.
			// The velocity is set to a negative number so that the Shoes will move upwards.
			// ******************************************************
			if (isJumping == false 
				&& ((newKeyboardState.IsKeyDown(up) && !oldKeyboardState.IsKeyDown(up)) || (newKeyboardState.IsKeyDown(Keys.Space) && !oldKeyboardState.IsKeyDown(Keys.Space))) 
				&& standingOnGround() == true 
				&& underTile() == false 
				/*&& guy.tileAbove() == false*/)
			{
				isJumping = true;
				velocity.Y = jumpImpulse * -1;
			}

			// ******************************************************
			// Move if the player has pressed a key.
			// ******************************************************
			// Don't allow the Shoes to move while they are using a Launcher.
			if (newKeyboardState.IsKeyDown(right) && delaySpringCollision == false && usingLauncher == false)
			{
				bouncingHorizontally = 0;
				position.X += velocity.X * delta;

				// Create the rectangle for the player's future position.
				// Draw a rectangle around the player's position after they move.
				updateRectangles(1, 0);
				handleCollisions(State.RunningRight);
				changeState(State.RunningRight);
			}
			if (newKeyboardState.IsKeyDown(left) && delaySpringCollision == false && usingLauncher == false)
			{
				bouncingHorizontally = 0;
				position.X -= velocity.X * delta;

				updateRectangles(-1, 0);
				handleCollisions(State.RunningLeft);
				changeState(State.RunningLeft);
			}
			if (newKeyboardState.IsKeyDown(Keys.G))
			{
				if (turnOffFalling == false) turnOffFalling = true;
				else turnOffFalling = false;
			}

			// ******************************************************
			// Make the Shoes jump if the player has pressed the jump key, or the Shoes are in the middle of a jump.
			// ******************************************************
			if (isJumping == true && usingLauncher == false)
			{
				doPlayerJump(delta);
			}
			else if (usingLauncher == false)
			{
				handleFalling(delta); // Handles for when the player walks off the edge of a platform.
			}

			// ******************************************************
			// Handle movement from springs over time.
			// ******************************************************
			if (bouncingHorizontally != 0 && standingOnGround() == false)   doHorizontalBounce(delta);
			else                                                            bouncingHorizontally = 0; // Stop horizontal movement.

			// ******************************************************
			// The player has fallen to the bottom of the map. Reset to the beginning of the level.
			// ******************************************************
			if (Position.Y > 704)
			{
				Position = Level.playerStartingPosition;
				guy.Position = Position;
			}

			if (delayBetweenLaunchesTimer.TimerCompleted == true)
			{
				delayBetweenLaunchesTimer.stopTimer();
			}

			if (usingLauncher == true)
			{
				doLauncher(gameTime, guy.power);
			}

			// Stops the Shoes from bouncing off the side of the screen during a launch.
			if (FutureRectangleRect.Left < 0 && hasBeenLaunched == true && launcherTimer.TimerCompleted == true)
			{
				velocity.X = 0.0f;
				hasBeenLaunched = false;
			}
			else if (FutureRectangleRect.Right > 1280 && hasBeenLaunched == true && launcherTimer.TimerCompleted == true)
			{
				position.X = screenWidth - spriteWidth;
				hasBeenLaunched = false;
			}

			if (hasBeenLaunched == true)
			{
				position += velocity * delta;
			}

			// Update timers.
			delaySpringCollisionTimer.Update(gameTime);
			launcherTimer.Update(gameTime);
			delayBetweenLaunchesTimer.Update(gameTime);

			// Get the old state of the keyboard.
			//oldKeyboardState = newKeyboardState; // Commented out so the interface works.
		}

		// Update the jump is the player if jumping.
		// Note: doPlayerJump handles its over gravity. That is, handleFalling does not come into play when the Shoes are jumping.
		private void doPlayerJump(float delta)
		{
			// W was down last update, but it's not down. Begin descent. This is for short hops.
			if (!newKeyboardState.IsKeyDown(up) && oldKeyboardState.IsKeyDown(up) && falling == false && velocity.Y < 0f)
			{
				velocity.Y = 0f; // CHANGE TO SOME PARTICULAR RESET VALUE
				falling = true;
			}

			position.Y += velocity.Y;       // Up   -            
			velocity.Y += gravity * delta;  // Down +, gravity      

			// If the velocity begins to pull the player down, we're falling. 
			if (velocity.Y > 0f)
			{
				falling = true;
				isJumping = false;
			}

			// Depending on which direction the player is moving, we need to check the top or bottom.
			if (falling == true)
			{
				updateRectangles(0, 1);
				handleCollisions(State.Decending);
				changeState(State.Decending);
			}
			else
			{
				updateRectangles(0, -1);
				handleCollisions(State.Jumping);
				changeState(State.Jumping);
			}

			// Different speed while in the air.
			if (isJumping == true) spriteSpeed = airMovementSpeed;
			else spriteSpeed = groundMovementSpeed;
		}

		// Handles for when the player walks off the edge of a platform.
		// ********************************** NOTE: handleFalling handles gravity at all times except for when the Shoes are jumping. doPlayerJump takes care of that.
		// TO DO: Change method name to make actual sense.
		private void handleFalling(float delta)
		{

			position.Y += velocity.Y;
			velocity.Y += fallFromTileRate * delta;

			// If we're not standing on the ground, fall.
			if (!standingOnGround())
			{
				falling = true;
			}
			else
			{
				// Determine if the Shoes have fallen onto a spring.
				// If so, allow the Shoes to bounce. Otherwise, stop falling.
				setTileArrayCoordinates(this.position.X, this.position.Y);
				if (Level.tiles[(int)this.TileArrayCoordinates.X, (int)this.TileArrayCoordinates.Y].TileRepresentation == 'S' && (velocity.Y > 4f/* || hasBeenLaunched == true*/))
				{
					doSpring(State.Decending);
				}
				else
				{
					velocity.Y = 0f;
					falling = false;
					//hasBeenLaunched = false;
				}
			}

			if (falling == true)
			{
				updateRectangles(0, 1);
				handleCollisions(State.Decending);
				changeState(State.Decending);
			}
			else
			{
				//debug = hasBeenLaunched.ToString();
				//hasBeenLaunched = false;
				updateRectangles(0, -1);
				handleCollisions(State.Jumping);
				changeState(State.Jumping);
			}
		}

		// Only if there is an actual collision will any of these statments execute.
		protected override void specializedCollision(State currentState, int y, int x)
		{
			if (delayBetweenLaunchesTimer.TimerStarted == false)    // Ensures the Shoes don't use another Launcher too quickly.
			{
				if (currentState == State.RunningRight)
				{
					if (Level.tiles[y, x].TileRepresentation == 'S')
					{
						position.X = Level.tiles[y, x].Position.X - spriteWidth;
						delaySpringCollision = true;
						doSpring(currentState);
					}
					else if (Level.tiles[y, x].IsLauncher == true)
					{
						usingLauncher = true;
						collY = y;
						collX = x;
					}
					else
					{
						position.X = Level.tiles[y, x].Position.X - spriteWidth;
						hasBeenLaunched = false;
					}

				}
				else if (currentState == State.RunningLeft)
				{
					if (Level.tiles[y, x].TileRepresentation == 'S')
					{
						position.X = Level.tiles[y, x].Position.X + Level.tiles[y, x].Texture.Width;
						delaySpringCollision = true;
						doSpring(currentState);
					}
					else if (Level.tiles[y, x].IsLauncher == true)
					{
						usingLauncher = true;
						collY = y;
						collX = x;
					}
					else
					{
						position.X = Level.tiles[y, x].Position.X + Level.tiles[y, x].Texture.Width;
						hasBeenLaunched = false;
					}

				}
				else if (currentState == State.Jumping)
				{
					if (Level.tiles[y, x].TileRepresentation == 'S')
					{
						if (hasBeenLaunched == true)
						{
							//position.Y = Level.tiles[y, x].Position.Y;
							position.X += 9f;
						}
						else
						{
							position.Y = Level.tiles[y, x].Position.Y + Level.tiles[y, x].Texture.Height + 2;
						}
						doSpring(State.Decending);
					}
					else if (Level.tiles[y, x].IsLauncher == true)
					{
						usingLauncher = true;
						collY = y;
						collX = x;
					}
					else
					{
						position.Y = Level.tiles[y, x].Position.Y + Level.tiles[y, x].Texture.Height + 2;
						velocity.Y = -1f;
						falling = true;
						hasBeenLaunched = false;
					}
				}
				else if (currentState == State.Decending)
				{
					if (Level.tiles[y, x].TileRepresentation == 'S')
					{
						position.Y = Level.tiles[y, x].Position.Y - spriteHeight;
						doSpring(State.Decending);
					}
					else if (Level.tiles[y, x].IsLauncher == true)
					{
						usingLauncher = true;
						collY = y;
						collX = x;
					}
					else
					{
						position.Y = Level.tiles[y, x].Position.Y - spriteHeight;
						spriteSpeed = 300f;
						isJumping = false;
						falling = false;
						hasBeenLaunched = false;
					}
				}
			}
		}

		private void doInterface(bool beingShot)
		{
			// Speed Interface
			if (newKeyboardState.IsKeyDown(Keys.NumPad7) || newKeyboardState.IsKeyDown(Keys.D7)) if (airMovementSpeed > 0) airMovementSpeed -= 5f;
			if (newKeyboardState.IsKeyDown(Keys.NumPad8) || newKeyboardState.IsKeyDown(Keys.D8)) if (airMovementSpeed < 1020) airMovementSpeed += 5f;
			if (newKeyboardState.IsKeyDown(Keys.NumPad4) || newKeyboardState.IsKeyDown(Keys.D4)) if (groundMovementSpeed > 0) groundMovementSpeed -= 5f;
			if (newKeyboardState.IsKeyDown(Keys.NumPad5) || newKeyboardState.IsKeyDown(Keys.D5)) groundMovementSpeed += 5f;
			if ((!newKeyboardState.IsKeyDown(Keys.NumPad1) && oldKeyboardState.IsKeyDown(Keys.NumPad1)) || (!newKeyboardState.IsKeyDown(Keys.D1) && oldKeyboardState.IsKeyDown(Keys.D1))) jumpImpulse--;
			if ((!newKeyboardState.IsKeyDown(Keys.NumPad2) && oldKeyboardState.IsKeyDown(Keys.NumPad2)) || (!newKeyboardState.IsKeyDown(Keys.D2) && oldKeyboardState.IsKeyDown(Keys.D2))) jumpImpulse++;
			if ((!newKeyboardState.IsKeyDown(Keys.NumPad6) && oldKeyboardState.IsKeyDown(Keys.NumPad6)) || (!newKeyboardState.IsKeyDown(Keys.D6) && oldKeyboardState.IsKeyDown(Keys.D6))) if (gravity > 0) gravity -= 5f;
			if ((!newKeyboardState.IsKeyDown(Keys.NumPad9) && oldKeyboardState.IsKeyDown(Keys.NumPad9)) || (!newKeyboardState.IsKeyDown(Keys.D9) && oldKeyboardState.IsKeyDown(Keys.D9))) gravity += 5f;
			if ((!newKeyboardState.IsKeyDown(Keys.Divide) && oldKeyboardState.IsKeyDown(Keys.Divide)) || (!newKeyboardState.IsKeyDown(Keys.Left) && oldKeyboardState.IsKeyDown(Keys.Left))) if (fallFromTileRate > 0) fallFromTileRate--;
			if ((!newKeyboardState.IsKeyDown(Keys.Subtract) && oldKeyboardState.IsKeyDown(Keys.Subtract)) || (!newKeyboardState.IsKeyDown(Keys.Right) && oldKeyboardState.IsKeyDown(Keys.Right))) fallFromTileRate++;

			// Presets
			if (newKeyboardState.IsKeyDown(Keys.F1))
			{
				preset = "Average - F1";
				airMovementSpeed = 375f;
				groundMovementSpeed = 355f;
				jumpImpulse = 17f;
				gravity = 30f;
				fallFromTileRate = 25f;
			}
			if (newKeyboardState.IsKeyDown(Keys.F2))
			{
				preset = "Derp - F2";
				airMovementSpeed = 1020f;
				groundMovementSpeed = 90f;
				jumpImpulse = 21f;
				gravity = 530f;
				fallFromTileRate = 80f;
			}
			if (newKeyboardState.IsKeyDown(Keys.F3))
			{
				preset = "Guy - F3";
				airMovementSpeed = 80f;
				groundMovementSpeed = 195f;
				jumpImpulse = 6f;
				gravity = 70f;
				fallFromTileRate = 40f;
			}
			if (newKeyboardState.IsKeyDown(Keys.F4))
			{
				preset = "Shoes - F4";
				airMovementSpeed = 405f;
				groundMovementSpeed = 425f;
				jumpImpulse = 8f;
				gravity = 25f;
				fallFromTileRate = 20f;
			}

			// Toggles the interface on and off
			if (!newKeyboardState.IsKeyDown(Keys.F11) && oldKeyboardState.IsKeyDown(Keys.F11))
			{
				if (interfaceEnabled == true)
				{
					preset = "Interface Disabled";
					interfaceEnabled = false;
				}
				else
				{
					preset = "Interface Enabled";
					interfaceEnabled = true;
				}
			}

			if (!newKeyboardState.IsKeyDown(Keys.F12) && oldKeyboardState.IsKeyDown(Keys.F12))
			{
				// Allows the player to have different speeds when the Guy is being shot or not.
				if (interfaceLinked == true) interfaceLinked = false;
				else interfaceLinked = true;
			}

			if (interfaceLinked == true && interfaceEnabled == true)
			{
				// Shoes Movement
				if (beingShot == true)
				{
					preset = "Shoes - F4";
					airMovementSpeed = 405f;
					groundMovementSpeed = 425f;
					jumpImpulse = 9.0f;
					gravity = 25f;
					fallFromTileRate = 20f;
				}
				else // Guy Movement
				{
					preset = "Guy - F3";
					airMovementSpeed = 80f;
					groundMovementSpeed = 195f;
					jumpImpulse = 6f;
					gravity = 70f;
					fallFromTileRate = 40f;
				}
			}
		}

		// Swaps the texture and dimensions of the Shoes. Used to switch between the Guy being shot and the Guy traveling with the Shoes.
		public void swapTexture(bool currentLinkedState)
		{
			if (currentLinkedState == true)
			{
				this.spriteHeight = 48;
				this.Texture = content.Load<Texture2D>("Sprites/Shoes32x48"); // Bottom
				position.Y -= 32f;
			}
			else
			{
				this.spriteHeight = 16;
				this.Texture = content.Load<Texture2D>("Sprites/Shoes32x48_Top");
				position.Y += 32f;
			}
		}

		private void doSpring(State currentState)
		{
			if (hasBeenLaunched == true)
			{
				//debug = "SPRINGS HERE";
				hasBeenLaunched = false; // Guessing that the Spring logic needs to take over if the Shoes were launched, then hit a Spring.
				delaySpringCollisionTimer.startTimer();
				delaySpringCollision = true;
				bouncingHorizontally = -1;
				velocity.Y *= -1;       // Flips the velocity to the player bounces up.
				velocity.Y *= 0.55f;    // Decrease the power of the next bounce.
				position.Y += velocity.Y;
			}

			if (currentState == State.Decending || currentState == State.Jumping)
			{
				velocity.Y *= -1;       // Flips the velocity to the player bounces up.
				velocity.Y *= 0.55f;    // Decrease the power of the next bounce.
				position.Y += velocity.Y;
			}
			else if (currentState == State.RunningRight)                
			{
				delaySpringCollisionTimer.startTimer();
				delaySpringCollision = true;
				bouncingHorizontally = 1;
			}
			else if (currentState == State.RunningLeft)
			{
				delaySpringCollisionTimer.startTimer();
				delaySpringCollision = true;
				bouncingHorizontally = -1;
			}

			// If the velocity begins to pull the player down, we're falling. 
			if (velocity.Y > 0f) falling = true;
			else falling = false;

			// Depending on which direction the player is moving, we need to check the top or bottom.
			if (falling == true)
			{
				updateRectangles(0, 1);
				handleCollisions(State.Decending);
				changeState(State.Decending);
			}
			else
			{
				updateRectangles(0, -1);
				handleCollisions(State.Jumping);
				changeState(State.Jumping);
			}
		}

		private void doHorizontalBounce(float delta)
		{
			float xSpeed = 5f;

			if (bouncingHorizontally == 1)
			{
				position.X -= xSpeed;

				updateRectangles(-1, 0);
				handleCollisions(State.RunningLeft);
				changeState(State.RunningLeft);
			}
			else
			{
				position.X += xSpeed;

				updateRectangles(1, 0);
				handleCollisions(State.RunningRight);
				changeState(State.RunningRight);
			}

			xSpeed *= delta;
		}

		private void doLauncher(GameTime gameTime, float power)
		{
			// Set the Guy at the top of the launcher.
			position.Y = Level.tiles[collY, collX].Position.Y - 48;
			position.Y += 32f;
			position.X = Level.tiles[collY, collX].Position.X - 8;
			updateRectangles(0, -1);
			velocity = new Vector2(0f, 0f);
			isJumping = false;

			// NEED TO ACCOUNT FOR WHEN THE GUY AND SHOES ARE TOGETHER
			
			if (launcherTimer.TimerStarted == true)
			{
				if (launcherTimer.TimerCompleted == true)
				{
					// Prevent the Guy from using any other Launchers for a time period.
					delayBetweenLaunchesTimer.startTimer();

					// Launch the Guy.
					launcherTimer.stopTimer();
					launcherVelocity = Utilities.Vector2FromAngle(MathHelper.ToRadians(Tile.getLauncherAngleInDegrees(Level.tiles[collY, collX]))) * power;
					velocity = launcherVelocity;
					velocity.Y *= -1;
					usingLauncher = false;
					hasBeenLaunched = true;
				}
			}
			else
			{
				launcherTimer.startTimer();
			}
		}
	}
}