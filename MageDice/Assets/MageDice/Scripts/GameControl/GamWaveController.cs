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

    private int _currentWave;


    public List<WaveConfig> gameWaves;

    public void StartGame(MapConfig map)
    {
        _currentWave = 0;
        this.gameWaves = new List<WaveConfig>(map.waves);
    }
    public WaveConfig GoNextWave()
    {
        WaveConfig w = gameWaves[CurrentWave];
        CurrentWave++;

        
        return w;
    }

}
public class GameWaveUnit
{
    public WaveConfig wave;
    public float startTime;
}
