using UnityEngine;

public class PCInputChecker : InputChecker
{
    public override bool IsDashInput()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    public override bool IsTakeScreenshotInput()
    {
        return Input.GetKey(KeyCode.Z);
    }

    public override bool IsJumpInput()
    {
        return Input.GetMouseButtonDown(0);
    }

    public override Vector3 GetJumpInputPosition()
    {
        return Input.mousePosition;
    }

}
