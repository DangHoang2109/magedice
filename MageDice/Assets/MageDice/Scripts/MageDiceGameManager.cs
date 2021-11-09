
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
}
