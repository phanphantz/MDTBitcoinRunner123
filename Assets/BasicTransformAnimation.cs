using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class BasicTransformAnimation : MonoBehaviour 
{
    protected float time;
	protected Vector3 start;
	protected Vector3 target;

    private Coroutine coroutine;

	protected virtual void OnEnable()
	{
		coroutine = StartCoroutine(AnimationRoutine());
	}

	private void OnDisable()
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
		}
	}

    protected IEnumerator AnimationRoutine()
    {
        var rate = 1.0f / time;
		var t = 0.0f;
		while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            SetValue(t);
            yield return new WaitForEndOfFrame();
        }

        t = 0.0f;
		while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            SetReverseValue(t);
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(AnimationRoutine());
    }

    protected abstract void SetValue(float t);
    protected abstract void SetReverseValue(float t);
   
}
