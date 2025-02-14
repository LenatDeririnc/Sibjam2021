﻿using Common.Components;
using Common.Components.Interfaces;
using Common.Tools;
using Common.UI;
using UnityEngine;

namespace TaskManagment
{
    public class TaskManager : Singleton<TaskManager>, ICoroutineRunner
    {
        [SerializeField] private bool StartOnLoadScene;
        [SerializeField] private bool debugInfo = true;
        [SerializeField] private int gameScoreModifier = 1;

        [SerializeField] private AudioClip breakSoundEffect;
        
        public float breakTimeIntervalInSeconds = 10;
        public float scoreTimeIntervalInSeconds = 1;
        public TaskContainer container;
        
        private Timer _breakTimer;
        private Timer _scoreTimer;
        private int gameScore = 0;

        protected override void BeforeRegister()
        {
            SetSettings(true, true);
        }

        protected override void AfterRegister()
        {
            container = new TaskContainer(breakSoundEffect);
            _breakTimer = new Timer(this, breakTimeIntervalInSeconds, OnBreakTimerEnd, debugInfo: debugInfo, name: $"{name}_breakTimer");
            _scoreTimer = new Timer(this, scoreTimeIntervalInSeconds, OnScoreTimerEnd, debugInfo, $"{name}_scoreTimer");
        }

        private void Start()
        {
            if (StartOnLoadScene)
            {
                _breakTimer.Play();
                _scoreTimer.Play();
            }
        }

        public void StartBreakTimer()
        {
            _breakTimer.Play();
        }

        public void StartScoreTimer()
        {
            _scoreTimer.Play();
        }

        public void Stop()
        {
            _breakTimer.Stop();
            _scoreTimer.Stop();
        }

        public void BreakRandomTask()
        {
            container.BreakRandomTask();
        }

        public void OnBreakTimerEnd()
        {
            BreakRandomTask();
            _breakTimer.Restart();
        }

        public void OnScoreTimerEnd()
        {
            bool isTasksEnds = container.workingTasks.Count <= 0;
            
            var seconds = isTasksEnds ? scoreTimeIntervalInSeconds : scoreTimeIntervalInSeconds / container.workingTasks.Count;

            if (!isTasksEnds) 
                IncreaseScore();

            _scoreTimer.SetStartSeconds(seconds);
            _scoreTimer.Restart();
        }

        private void IncreaseScore()
        {
            gameScore += gameScoreModifier;
            ScoreSetter.UpdateScoreAction?.Invoke(gameScore);
        }
    }
}