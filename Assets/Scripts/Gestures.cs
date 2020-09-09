using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gestures : MonoBehaviour
{

    [SerializeField]
    private bool detectBeforeRelase = true;

    [SerializeField]
    public static float minSwipeDistance = 40f;

    [SerializeField]
    private float tapThreshold = 10;


    private Vector3 fingerUpPosition;
    private Vector3 fingerDownPosition;

    [SerializeField]
    private Camera camera;

    public static event Action<Vector3> OnTap = delegate {};
    public static event Action<SwipeInfo> OnSwipe = delegate { };
    public static event Action SwipeEnded = delegate { };

    public static event Action ThreeTap = delegate {};

    private void Update() {


        if(Input.touchCount == 3) {
            ThreeTap();
            
        } else if(Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began) {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if(detectBeforeRelase && touch.phase == TouchPhase.Moved) {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }

            if(touch.phase == TouchPhase.Ended) {

                fingerDownPosition = touch.position;
                if(DetectTap()) {
                    OnTap(camera.ScreenToWorldPoint(fingerDownPosition));
                } else {
                    DetectSwipe();
                    SwipeEnded();
                }
            }
        }   
    }

    private void DetectSwipe() {
        SwipeInfo swipeInfo = new SwipeInfo(fingerUpPosition, fingerDownPosition);
        if(swipeInfo.Magnitude > minSwipeDistance) {
            // We have detected a valid swipe
            OnSwipe(swipeInfo);
        }
    }

    private bool DetectTap() {
        float distance =  Vector3.Distance(fingerUpPosition, fingerDownPosition);
        return distance <= tapThreshold;
    }


}
