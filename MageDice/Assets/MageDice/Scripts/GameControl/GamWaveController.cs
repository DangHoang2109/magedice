using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamWaveController 
{
    public static GamWaveController Instance => MageDiceGameManager.Instance.WaveController;

    public System.Action<int> OnWaveChange;
    public int CurrentWave { get => _currentWave; 
        set {
            _currentWave = value;
            OnWaveChange.Invoke(_currentWave);
        } 
    }
    public int CompletedtWave
    {
        get => _currentWave - 1;
    }
    public bool IsOutOfWave => CurrentWave >= gameWaves.Count;

    private int _currentWave;


    public List<WaveConfig> gameWaves;

    public WaveConfig GetWave(int waveIndex)
    {
        return gameWaves[waveIndex];

    }
    public void StartGame(MapConfig map)
    {
        _currentWave = 0;
        this.gameWaves = new List<WaveConfig>(map.waves);
    }
    public WaveConfig GoNextWave()
    {
        if(IsOutOfWave)
        {
            return null;
        }

        WaveConfig w = gameWaves[CurrentWave];
        CurrentWave++;

        return w;
    }
    public WaveConfig JumpToWave(int wave)
    {
        this.CurrentWave = wave;
        return gameWaves[CurrentWave];
    }
}
public class GameWaveUnit
{
    public WaveConfig wave;
    public float startTime;
}
