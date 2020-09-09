using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeInfo
{
    public enum SwipeDirection
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    public SwipeDirection Direction {get;set;}
    public float Magnitude {
        get {
            return GetVelocity().magnitude;
        }
    }

    public Vector3 StartPosition {get;}
    public Vector3 EndPosition {get;}
    
    public SwipeInfo(Vector3 startPosition, Vector3 endPosition) {
        this.StartPosition = startPosition;
        this.EndPosition = endPosition;
        this.Direction = DetermineDirection();
    }


    public Vector3 GetVelocity() {
        return EndPosition - StartPosition;
    }

    private SwipeDirection DetermineDirection() {
        Vector3 velocity = GetVelocity();
        float angle = (Vector3.SignedAngle(Vector3.right, velocity, Vector3.forward) + 360) % 360;
        if(angle > 315 || angle <= 45) {
            return SwipeDirection.RIGHT;
        } else if (angle > 45 && angle <= 135) {
            return SwipeDirection.UP;
        } else if(angle > 135 && angle <= 225) {
            return SwipeDirection.LEFT;
        } else if(angle > 225 && angle <= 315) {
            return SwipeDirection.DOWN;
        }
        Debug.LogError("Unknown swipe detected");
        return SwipeDirection.RIGHT;
    }

}
