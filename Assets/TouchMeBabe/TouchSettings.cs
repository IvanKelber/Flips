using UnityEngine;

[CreateAssetMenu(menuName="Touch/Settings")]
public class TouchSettings : ScriptableObject
{
    //The larger the tapThreshold the more lenient the tap detection will be
    [MinAttribute(0)]
    public float tapThreshold;


    // The larger the swipeThreshold the longer the swipe has to be before registering
    [MinAttribute(0)]
    public float swipeThreshold;

    public bool requireLiftInCollider;

    public bool requireTouchInCollider;
}
