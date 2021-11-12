
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageDiceGameManager : MonoSingleton<MageDiceGameManager>
{
    [SerializeField] private Transform _tfTower;
    public Transform TfTower => this._tfTower;

    [SerializeField] private Vector3 _tfMaxSafeRight;
    public Vector3 TfMaxSafeRight => this._tfMaxSafeRight;

    [SerializeField] private Vector3 _tfMaxSafeLeft;
    public Vector3 TfMaxSafeLeft => this._tfMaxSafeLeft;

    public GamWaveController WaveController;

    private void Start()
    {
        this.WaveController = new GamWaveController();
        this.WaveController.StartGame(GameMapConfigs.Instance.GetMap(MapName.GREENLAND));
        this.WaveController.OnWaveChange += OnWaveChange;

        GameBoardManager.Instance.StartPlay();
        MonsterManager.Instance.StartGame(GameMapConfigs.Instance.GetMap(MapName.GREENLAND));
    }

    private void OnWaveChange(int wave)
    {
        //show popup wave
        //update text wave top

        Debug.Log($"Wave {wave}");
    }
}

