using UnityEngine;

public class RotateAnimation : TransformAnimation
{
    protected override void SetReverseValue(float t)
    {
        this.gameObject.transform.eulerAngles = Vector3.Lerp(target, start, t);
    }

    protected override void SetValue(float t)
    {
        this.gameObject.transform.eulerAngles = Vector3.Lerp(start, target, t);
    }

}
