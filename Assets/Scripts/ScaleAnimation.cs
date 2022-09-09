using System.Collections;
using UnityEngine;

public class ScaleAnimation : TransformAnimation
{

    protected override void SetReverseValue(float t)
 	{
        this.gameObject.transform.localScale = Vector3.Lerp(target, start, t);
    }

    protected override void SetValue(float t)
	{
        this.gameObject.transform.localScale = Vector3.Lerp(start, target, t);
    }

}
