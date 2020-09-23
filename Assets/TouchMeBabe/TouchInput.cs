using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{

    private Touch fingerDown;
    private HashSet<ITouchable> onFingerDownTouchables;
    private float onFingerDownTime;

    private Touch fingerUp;
    private float onFingerUpTime;

    [SerializeField]
    private Camera camera;
    [SerializeField]
    private TouchSettings touchSettings;

    private void Update() {
        if(Input.touchCount > 0) {
            // Debug.Log("Touching");
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began) {
                fingerDown = touch;
                fingerUp = touch;
                onFingerDownTouchables = GetTouchablesFromPoint(fingerDown.position);
                onFingerDownTime = Time.time;
            } else if(touch.phase == TouchPhase.Ended) {
                fingerUp = touch;
                onFingerUpTime = Time.time;
                if(touchSettings.requireLiftInCollider) {
                    HashSet<ITouchable> onFingerUpTouchables = GetTouchablesFromPoint(fingerUp.position);
                    onFingerDownTouchables.IntersectWith(onFingerUpTouchables);
                }
                 
                foreach(ITouchable touchable in onFingerDownTouchables) {
                    if(IsTap(fingerDown.position, fingerUp.position)) {
                        ITappable tap = (touchable as ITappable);
                        if(tap != null) {
                            tap.OnTap();
                        }
                    } else if(IsSwipe(fingerDown.position, fingerUp.position)) {
                        ISwipeable swipe = (touchable as ISwipeable);
                        if(swipe != null) {
                            swipe.OnSwipe(new SwipeInfo(fingerDown, fingerUp, onFingerUpTime - onFingerDownTime));
                        }
                    }
                }
            }
        }
    }

    private bool IsTap(Vector2 fingerDown, Vector2 fingerUp) {
        return Vector3.Distance(fingerDown, fingerUp) < touchSettings.tapThreshold;
    }

    private bool IsSwipe(Vector2 fingerDown, Vector2 fingerUp) {
        return Vector3.Distance(fingerDown, fingerUp) >= touchSettings.swipeThreshold;
    }


    private bool IsTouchable(GameObject go, out ITouchable touchable) {
        foreach(MonoBehaviour mb in go.GetComponents<MonoBehaviour>()) {
            if(mb is ITouchable) {
                touchable = (ITouchable) mb;
                return true;
            }
        }
        touchable = null;
        return false;
    }

    private HashSet<ITouchable> GetTouchablesFromPoint(Vector2 screenPosition) {
        Ray ray = camera.ScreenPointToRay(screenPosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        HashSet<ITouchable> touchables = new HashSet<ITouchable>();
        foreach(RaycastHit hit in hits) {
            ITouchable touch;
            if(IsTouchable(hit.transform.gameObject, out touch)) {
                touchables.Add(touch);
            } 
        }
        return touchables;
    }
}


