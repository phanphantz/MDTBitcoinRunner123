using System.Collections;
using UnityEngine;

public class BasicMoveAnimation : BasicTransformAnimation
{
    protected override void OnEnable()
    {
		start = transform.localPosition;
        base.OnEnable();
    }

    protected override void SetValue(float t)
    {
        this.gameObject.transform.localPosition = Vector3.Lerp(start, target, t);
    }

    protected override void SetReverseValue(float t)
    {
        this.gameObject.transform.localPosition = Vector3.Lerp(target, start, t);
    }
		
}
