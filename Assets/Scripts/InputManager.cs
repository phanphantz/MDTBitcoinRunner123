using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public bool useTouch = false;

    public LayerMask mask = -1;

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
        if (Input.GetKey(KeyCode.Z))
        {
            StartCoroutine(CaptureScreenshot());
        }
    }

    private void ListenForJump()
    {
        if (useTouch)
            GetTouches();
        else
            GetClicks();
    }

	private void ListenForDash()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }

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

    void GetClicks()
    {
        if (Input.GetMouseButtonDown(0))
            TryMoveUp(Input.mousePosition);
    }

    void TryMoveUp(Vector3 inputPosition)
    {
        ray = Camera.main.ScreenPointToRay(inputPosition);

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            if (IsShouldMoveUp())
                PlayerManager.Instance.MoveUp();
        }
    }

    private static bool IsShouldMoveUp()
    {
        return (PlayerManager.Instance.transform.position.y < 50
                    || PlayerManager.Instance.isOnTheCar
                    || PlayerManager.Instance.isOnTheBus
                    || PlayerManager.Instance.Crashed())
                        && !UIManager.Instance.isPausedOrFinished;
    }

    void GetTouches()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began && touch.phase != TouchPhase.Canceled)
            {
               TryMoveUp(touch.position);
            }
        }
    }

    // void OnTap(TapGesture gesture)
    // {
    // 	if ((PlayerManager.Instance.transform.position.y < -22 || PlayerManager.Instance.isOnTheCar || PlayerManager.Instance.isOnTheBus || PlayerManager.Instance.Crashed())
    // 	&& !UIManager.Instance.isPausedOrFinished)
    // 	{
    // 		PlayerManager.Instance.MoveUp();
    // 	}
    // }
}
