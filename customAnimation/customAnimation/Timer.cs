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
        private bool timerRunning = false;      // Stores if the timer is running or not.
        private bool timerCompleted = false;    // Stores if the timer has completed or not.

        public float ElapsedTime
        {
            get { return elapsedTime; }
        }

        public bool TimerStarted
        {
            get { return timerRunning; }
        }

        public bool TimerCompleted
        {
            get { return timerCompleted; }
        }

        public Timer(float endTime)
        {
            this.timerEndTime = endTime;
            this.timerRunning = false;
            this.timerCompleted = false;
        }

        public void startTimer()
        {
            timerRunning = true;
            timerCompleted = false;
        }

        public void stopTimer()
        {
            timerRunning = false;
            elapsedTime = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (timerRunning == true && elapsedTime < timerEndTime)
            {
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                timerCompleted = true;
            }   
        }
    }
}
