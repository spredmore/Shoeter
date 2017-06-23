using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace customAnimation
{
	class Timer
	{
		private float elapsedTime = 0f;				// Total time the timer has been running.
		private float timerEndTime;					// End time for the timer. Stop when elapsedTime is equal to timerEndTime.
		private bool isTimerRunning = false;		// Stores if the timer is running or not.
		private bool isTimerCompleted = false;		// Stores if the timer has completed or not.

		/// <summary>
		/// Property for the elapsed time of the Timer.
		/// </summary>
		public float ElapsedTime
		{
			get { return elapsedTime; }
			set { elapsedTime = value; }
		}

		/// <summary>
		/// Property for if the Timer is running or not.
		/// </summary>
		public bool TimerStarted
		{
			get { return isTimerRunning; }
		}

		/// <summary>
		/// Property for if the Timer is completed or not.
		/// </summary>
		public bool TimerCompleted
		{
			get { return isTimerCompleted; }
		}

		/// <summary>
		/// Constructor for the Timer class.
		/// </summary>
		/// <param name="endTime"></param>
		public Timer(float endTime)
		{
			timerEndTime = endTime;
			isTimerRunning = false;
			isTimerCompleted = false;
		}

		/// <summary>
		/// Starts the Timer.
		/// </summary>
		public void startTimer()
		{
			isTimerRunning = true;
			isTimerCompleted = false;
		}

		/// <summary>
		/// Stops the Timer.
		/// </summary>
		public void stopTimer()
		{
			isTimerRunning = false;
			elapsedTime = 0;
		}

		/// <summary>
		/// Resets the Timer.
		/// </summary>
		public void resetTimer()
		{
			isTimerRunning = false;
			isTimerCompleted = false;
			elapsedTime = 0;
		}

		/// <summary>
		/// Updates the Timer.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		public void Update(GameTime gameTime)
		{
			if (isTimerRunning)
			{
				if (elapsedTime < timerEndTime)
				{
					elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				}
				else
				{
					isTimerCompleted = true;
				}
			}
		}
	}
}
