using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
#region sealed

    private static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EventManager>() ??
                            (new GameObject("EventManager", typeof(EventManager)))
                            .GetComponent<EventManager>();
            }
            return _instance;
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

#endregion
    
    
#region events to be bound
    /// <summary>
    /// called everytime a new set starting <br></br>
    /// the serving player's pos, the waiting player's pos <br></br>
    /// *NOTE: call this will reset camera 
    /// </summary>
    public event System.Action<Vector3, Vector3> onSetStarting;
    public event System.Action onSetEnded;
    
    
    /// <summary>
    /// called everytime the opponent start moving <br></br>
    /// the opponent's expecting pos, moving time <br></br>
    /// *NOTE: called even when the opponent may change their decision
    /// </summary>
    public event System.Action<Vector3, float> onOpponentStartMoving;

    /// <summary>
    /// called when the opponent is moving, after updated position <br></br>
    /// the player's pos <br></br>
    /// </summary>
    public event System.Action<Vector3, float> onOpponentUpdateMoving;
    /// <summary>
    /// called everytime the player start moving <br></br>
    /// the player's expecting pos, moving time <br></br>
    /// *NOTE: called even when the player may change their decision
    /// </summary>
    public event System.Action<Vector3, float> onPlayerStartMoving;
    
    /// <summary>
    /// called when the player is moving, after updated position <br></br>
    /// the player's pos <br></br>
    /// </summary>
    public event System.Action<Vector3> onPlayerUpdateMoving;

    
    /// <summary>
    /// called everytime a ball be hitting by a racket  <br></br>
    /// the ball's current pos  <br></br>
    /// HEY! serving use this too!
    /// </summary>
    public event System.Action<Vector3> onBallBeCatching;
    
    
    /// <summary>
    /// called everytime a ball be hit by a racket <br></br>
    /// the ball's expecting pos to be landed next (in other side), moving time <br></br>
    /// *NOTE: called this even the player/opponent able to catch or not <br></br>
    /// *NOTE2: called after BallBeCatching
    /// </summary>
    public event System.Action<Vector3, float> onBallGoingBeLanded;
    
    
    /// <summary>
    /// called everytime a ball be hit by a racket <br></br>
    /// the ball's expecting pos to be caught next (in other side), moving time <br></br>
    /// *NOTE: called this even the player/opponent may not swipe, as long as their has the ability to reach the ball on time <br></br>
    ///  *NOTE2: called after BallGoingBeLanded
    /// </summary>
    public event System.Action<Vector3, float> onBallGoingBeCaught;
    /// <summary>
    /// called everytime a ball gonna hit wall <br></br>
    /// the ball's position when hit the wall  <br></br>
    /// *NOTE: calculating during hitting by a racket or landing on the court  <br></br>
    /// *NOTE2: called when the player/opponent not swiping and let the ball come outside
    /// </summary>
    public event System.Action<Vector3> onBallHitWall;
#endregion event to be bound


#region invoke events

    /// <summary>
    /// called everytime a new set starting <br></br>
    /// the serving player's pos, the waiting player's pos <br></br>
    /// *NOTE: call this will reset camera 
    /// </summary>
    public void CallSetStarting(Vector3 posPlayer, Vector3 posOpponent)
    {
        this.onSetStarting?.Invoke(posPlayer, posOpponent);
    }
    
    /// <summary>
    /// called everytime a new set starting <br></br>
    /// the serving player's pos, the waiting player's pos <br></br>
    /// *NOTE: call this will reset camera 
    /// </summary>
    public void CallSetEnded()
    {
        this.onSetEnded?.Invoke();
    }
    /// <summary>
    /// called everytime the opponent start moving <br></br>
    /// the opponent's expecting pos, moving time <br></br>
    /// *NOTE: called even when the opponent may change their decision
    /// </summary>
    public void CallOpponentStartMoving(Vector3 posOpponentMoveTo, float duration)
    {
        this.onOpponentStartMoving?.Invoke(posOpponentMoveTo, duration);
    }
    /// <summary>
    /// called when the opponent is moving, after updated position <br></br>
    /// the player's pos <br></br>
    /// </summary>
    public void CallOpponentUpdateMoving(Vector3 posOpponentUpdated)
    {
        this.onPlayerUpdateMoving?.Invoke(posOpponentUpdated);
    }
    /// <summary>
    /// called everytime the player start moving <br></br>
    /// the player's expecting pos, moving time <br></br>
    /// *NOTE: called even when the player may change their decision
    /// </summary>
    public void CallPlayerStartMoving(Vector3 posPlayerMoveTo, float duration)
    {
        this.onPlayerStartMoving?.Invoke(posPlayerMoveTo, duration);
    }
    /// <summary>
    /// called when the player is moving, after updated position <br></br>
    /// the player's pos <br></br>
    /// </summary>
    public void CallPlayerUpdateMoving(Vector3 posPlayerUpdated)
    {
        this.onPlayerUpdateMoving?.Invoke(posPlayerUpdated);
    }
    
    /// <summary>
    /// called everytime a ball be hitting by a racket  <br></br>
    /// the ball's current pos  <br></br>
    /// HEY! serving use this too!
    /// </summary>
    public void CallBallBeCatching(Vector3 posCatchingBall)
    {
        this.onBallBeCatching?.Invoke(posCatchingBall);
    }
    /// <summary>
    /// called everytime a ball be hit by a racket <br></br>
    /// the ball's expecting pos to be landed next (in other side), moving time <br></br>
    /// *NOTE: called this even the player/opponent able to catch or not <br></br>
    /// *NOTE2: called after BallBeCatching
    /// </summary>
    public void CallBallGoingBeLanded(Vector3 posGoingBeLanded, float timeExpected)
    {
        this.onBallGoingBeLanded?.Invoke(posGoingBeLanded, timeExpected);
    }
    /// <summary>
    /// called everytime a ball be hit by a racket <br></br>
    /// the ball's expecting pos to be caught next (in other side), moving time <br></br>
    /// *NOTE: called this even the player/opponent may not swipe, as long as their has the ability to reach the ball on time <br></br>
    ///  *NOTE2: called after BallGoingBeLanded
    /// </summary>
    public void CallBallGoingBeCaught(Vector3 posGoingBeCaught, float timeExpected)
    {
        this.onBallGoingBeCaught?.Invoke(posGoingBeCaught, timeExpected);
    }
    /// <summary>
    /// called everytime a ball gonna hit wall <br></br>
    /// the ball's position when hit the wall  <br></br>
    /// *NOTE: calculating during hitting by a racket or landing on the court  <br></br>
    /// *NOTE2: called when the player/opponent not swiping and let the ball come outside
    /// </summary>
    public void CallBallHitWall(Vector3 posHitWall)
    {
        this.onBallHitWall?.Invoke(posHitWall);
    }

#endregion invoke events
    
}
