using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public LayerMask mask = -1;

    [SerializeField] InputChecker inputChecker;

    Ray ray;
    RaycastHit hit;

    Transform button;


	void Update()
    {
        ListenForScreenCapture();
        ListenForJump();
		ListenForDash();
    }

	private void ListenForScreenCapture()
    {
        if (inputChecker.IsTakeScreenshotInput())
            StartCoroutine(CaptureScreenshot());
    }

    private void ListenForJump()
    {
        if (inputChecker.IsJumpInput())
            TryMoveUpByInput(inputChecker.GetJumpInputPosition());
    }

	private void ListenForDash()
	{
		if (inputChecker.IsDashInput())
            Dash();
    }

    private static void Dash()
    {
        LevelSpawnManager.Instance.ScrollLevelForSeconds(0.5f);
    }

    IEnumerator CaptureScreenshot()
	{
		string filename = GetFileName(Screen.width, Screen.height);
		Debug.LogError("Screenshot saved to " + filename);
		ScreenCapture.CaptureScreenshot(filename);
		yield return new WaitForSeconds(0.1f);
	}

    string GetFileName(int width, int height)
    {
        return string.Format("screenshot_{0}x{1}_{2}.png", width, height, System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
    }

    void TryMoveUpByInput(Vector3 inputPosition)
    {
        ray = Camera.main.ScreenPointToRay(inputPosition);
        TryMoveUpIfNotBlocked();
    }

    private void TryMoveUpIfNotBlocked()
    {
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            TryMoveUp();
    }

    private static void TryMoveUp()
    {
        if (IsShouldMoveUp())
            PlayerManager.Instance.MoveUp();
    }

    private static bool IsShouldMoveUp()
    {
        return (PlayerManager.Instance.transform.position.y < 50
		|| PlayerManager.Instance.isOnTheCar
		|| PlayerManager.Instance.isOnTheBus
		|| PlayerManager.Instance.Crashed())
		&& !UIManager.Instance.isPausedOrFinished;
    }

}
