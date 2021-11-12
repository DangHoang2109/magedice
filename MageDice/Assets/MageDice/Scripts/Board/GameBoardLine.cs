using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardLine : MonoBehaviour
{
    private GameBoardManager _BoardManager;
    private GameBoardManager BoardManager
    {
        get
        {
            if (this._BoardManager == null)
                this._BoardManager = GameBoardManager.Instance;

            return _BoardManager;
        }
    }

    [SerializeField] private Vector3 _startPos;
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private GameBoardCollumn[] collumn;
    public GameBoardCollumn NextCollumn
    {
        get
        {
            if (indexCollumnNext >= 0 && indexCollumnNext < this.collumn.Length)
                return collumn[indexCollumnNext];

            return null;
        }
    }

    [SerializeField] private int indexCollumnNext;

    [SerializeField] private float _speed;

    private bool isMoving;
    private bool isMaxCollumn;



    private void Start()
    {
        this.collumn = GameBoardManager.Instance.Collumns;
        ResetData();
    }
    public void StartMove()
    {
        this.isMoving = true;
    }
    public void StopMove()
    {
        this.isMoving = false;
    }
    public void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(this._endPos.x, this.transform.position.y), _speed * Time.deltaTime);
            //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(this._endPos.x, this.transform.position.y), ref velocity, FlyTime);

            if (GameUtils.IsNear(transform.position.x, NextCollumn.gEntryLine.transform.position.x, 2.5f)) //pixel
            {
                ActiveLine();
                return;
            }

            if(this.isMaxCollumn && GameUtils.IsNear(transform.localPosition.x, _endPos.x, 30f)) //30 is line image wisth
            {
                ResetData();
            }
        }
    }

    public void ResetData()
    {
        isMaxCollumn = false;
        indexCollumnNext = 0;
        this.transform.localPosition = this._startPos;
        BoardManager.ResetData();
    }

    public void ActiveLine()
    {
        NextCollumn.Active();

        indexCollumnNext++;
        if (indexCollumnNext >= this.collumn.Length)
        {
            isMaxCollumn = true;
            indexCollumnNext = 0;
        }
    }
}
