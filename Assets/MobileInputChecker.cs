using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MobileInputChecker : InputChecker
{
    public override bool IsJumpInput()
    {
       return Input.touchCount != 0 
       && Input.GetTouch(0).phase == TouchPhase.Began
       && IsTouchValid(Input.GetTouch(0));
    }

	bool IsTouchValid(Touch touch)
	{
		return touch.phase == TouchPhase.Began 
		&& touch.phase != TouchPhase.Canceled;
	}

    public override bool IsDashInput()
    {
       return Input.touchCount == 2;
    }

    public override Vector3 GetJumpInputPosition()
    {
        return Input.GetTouch(0).position;
    }

    public override bool IsTakeScreenshotInput()
    {
         return Input.touchCount == 3;
    }

}
