using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace customAnimation
{
    class Timer
    {
        private float elapsedTime = 0f;         // Total time the timer has been running.
        public float timerEndTime;              // End time for the timer. Stop when elapsedTime is equal to timerEndTime.
        private bool isTimerRunning = false;      // Stores if the timer is running or not.
        private bool isTimerCompleted = false;    // Stores if the timer has completed or not.

        public float ElapsedTime
        {
            get { return elapsedTime; }
        }

        public bool TimerStarted
        {
			get { return isTimerRunning; }
        }

        public bool TimerCompleted
        {
			get { return isTimerCompleted; }
        }

        public Timer(float endTime)
        {
            timerEndTime = endTime;
			isTimerRunning = false;
			isTimerCompleted = false;
        }

        public void startTimer()
        {
			isTimerRunning = true;
			isTimerCompleted = false;
        }

        public void stopTimer()
        {
			isTimerRunning = false;
            elapsedTime = 0;
        }

        public void Update(GameTime gameTime)
        {
			if (isTimerRunning && elapsedTime < timerEndTime)
            {
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
			else if (isTimerRunning)
            {
				isTimerRunning = false;
				isTimerCompleted = true;
            }   
        }
    }
}
