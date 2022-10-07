using UnityEngine;

public abstract class InputChecker : MonoBehaviour
{
    public abstract bool IsDashInput();
    public abstract bool IsTakeScreenshotInput();
    public abstract bool IsJumpInput();
    public abstract Vector3 GetJumpInputPosition();

}
