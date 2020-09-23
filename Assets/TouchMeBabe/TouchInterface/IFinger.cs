using UnityEngine;

public interface IFinger : ITouchable
{
    void OnFingerDown(Vector2 position);

    void OnFingerUp(Vector2 position);

    void OnFingerHold(Vector2 position);
}
