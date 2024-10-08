using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSpawnManager : MonoBehaviour
{
	static LevelSpawnManager _instance;
	static int instances = 0;

	public GameObject startTriggers;
	public float scrollSpeed = 0.4f;
	public float maxScrollSpeed = 0.7f;
	public float maxScrollSpeedDist = 1500;
	public float distance;
	public Renderer background;
	public Renderer road;
	public List<GameObject> obstacles;
	public List<GameObject> cloudLayer;
	public List<GameObject> cityBackgroundLayer;
	public List<GameObject> cityLayer;
	public List<GameObject> foregroundLayer;

	public enum PlaceLocation
	{
		Normal,
		Middle
	}

	List<GameObject> activeElements = new List<GameObject>();

	Vector2 scrolling = new Vector2(0, 0);

	float defaultScrollSpeed;
	float backgroundScrollSpeed;
	float clodScrollSpeed;
	float middlegroundScrollSpeed;
	float foregroundScrollSpeed;
	float scrollSpeedBeforeCrash;

	bool isCanSpawnPowerup = true;
	bool isCanSpawnEnemy = true;

	bool isCanSpawn = false;
	bool isPaused => GameStateManager.IsPause;

	bool isScrollStartTriggers = true;
	bool isCanModifySpeed = true;

	public List<GameObject> letterObjects;

	public static LevelSpawnManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(LevelSpawnManager)) as LevelSpawnManager;
			}

			return _instance;
		}
	}

	void Start()
	{
		instances++;

		if (instances > 1)
		{
			Debug.LogWarning("There are more than one LevelSpawnManager");
		}
		else
		{
			_instance = this;
		}

		defaultScrollSpeed = scrollSpeed;

		CalculateScrollSpeeds();

		SetStartTriggersPosition();

		//Place cloud and city layer in the middle of the screen (x position)
		SpawnCloudLayer(PlaceLocation.Middle);
		SpawnCityBackgroundLayer(PlaceLocation.Normal);
		SpawnCityLayer(PlaceLocation.Middle);
	}

	void Update()
	{
		if (isCanSpawn && !isPaused)
		{
			if (isCanSpawnPowerup)
			{
				StartCoroutine(SpawnPowerup());
			}

			if (isCanSpawnEnemy)
			{
				StartCoroutine(SpawnEnemy());
			}

			ScrollLevel();
		}

		if (!isPaused)
		{
			distance += scrollSpeed * Time.deltaTime * 25;
		}
	}

	private void CalculateScrollSpeeds()
	{
		backgroundScrollSpeed = scrollSpeed * 12f;
		clodScrollSpeed = backgroundScrollSpeed * 2;
		middlegroundScrollSpeed = backgroundScrollSpeed * 4;
		foregroundScrollSpeed = backgroundScrollSpeed * 8;
	}

	public void ScrollLevelForSeconds(float targetTime)
	{
		StartCoroutine(ScrollRoutine(targetTime));
	}

	IEnumerator ScrollRoutine(float targetTime)
	{
		var currentTime = 0f;
		while(currentTime < targetTime)
		{
			ScrollLevel();
			yield return new WaitForEndOfFrame();
			currentTime += Time.deltaTime;
		}
	}

	public void ScrollLevel()
    {
        if (isCanModifySpeed)
        {
            scrollSpeed = defaultScrollSpeed + ((distance / maxScrollSpeedDist) * (maxScrollSpeed - defaultScrollSpeed));
        }

        CalculateScrollSpeeds();

        MoveActiveElements();

        if (isScrollStartTriggers)
        {
            startTriggers.transform.position -= Vector3.right * foregroundScrollSpeed * Time.deltaTime;
        }

        scrolling.x = scrollSpeed;

        road.material.mainTextureOffset += scrolling * Time.deltaTime;
    }

    private void MoveActiveElements()
    {
        for (int i = 0; i < activeElements.Count; i++)
        {
            switch (activeElements[i].tag)
            {
                case "CloudLayer":
                    activeElements[i].transform.position -= Vector3.right * clodScrollSpeed * Time.deltaTime;
                    break;

                case "CityBackgroundLayer":
                    activeElements[i].transform.position -= Vector3.right * backgroundScrollSpeed * Time.deltaTime;
                    break;

                case "CityLayer":
                    activeElements[i].transform.position -= Vector3.right * middlegroundScrollSpeed * Time.deltaTime;
                    break;

                case "ForegroundLayer":
                case "Obstacles":
                case "Particle":
                    activeElements[i].transform.position -= Vector3.right * foregroundScrollSpeed * Time.deltaTime;
                    break;
            }
        }
    }

    void ClearMap()
	{
		StopAllCoroutines();

		EnemyManager.Instance.ResetAll();
		PowerupManager.Instance.ResetAll();


		while (activeElements.Count > 0)
		{
			switch (activeElements[0].tag)
			{
				case "CloudLayer":
					cloudLayer.Add(activeElements[0]);
					activeElements[0].transform.localPosition = new Vector3(UIManager.Instance.cameraHorizontalExtent + 75f, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
					activeElements[0].SetActive(false);
					activeElements.Remove(activeElements[0]);
					break;

				case "CityBackgroundLayer":
					cityBackgroundLayer.Add(activeElements[0]);
					activeElements[0].transform.localPosition = new Vector3(UIManager.Instance.cameraHorizontalExtent + 25f, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
					activeElements[0].SetActive(false);
					activeElements.Remove(activeElements[0]);
					break;

				case "CityLayer":
					cityLayer.Add(activeElements[0]);
					activeElements[0].transform.localPosition = new Vector3(UIManager.Instance.cameraHorizontalExtent + 40f, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
					activeElements[0].SetActive(false);
					activeElements.Remove(activeElements[0]);
					break;

				case "ForegroundLayer":
					foregroundLayer.Add(activeElements[0]);
					activeElements[0].transform.localPosition = new Vector3(UIManager.Instance.cameraHorizontalExtent + 50f, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
					activeElements[0].SetActive(false);
					activeElements.Remove(activeElements[0]);
					break;

				case "Obstacles":
					obstacles.Add(activeElements[0]);
					activeElements[0].transform.localPosition = new Vector3(UIManager.Instance.cameraHorizontalExtent + 100f, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
					activeElements.Remove(activeElements[0]);
					break;
			}
		}
	}

	void RandomizeObstacles()
	{
		int n = obstacles.Count;
		GameObject temp;
		while (n > 1)
		{
			n--;
			int k = UnityEngine.Random.Range(0, n + 1);
			temp = (GameObject)obstacles[k];
			obstacles[k] = obstacles[n];
			obstacles[n] = temp;
		}
	}

	IEnumerator SpawnPowerup()
	{
		isCanSpawnPowerup = false;

		int n = Random.Range(15, 30); //power up frequency

		if (distance < 1000)
		{
			n = Random.Range(10, 25);
		}

		if (!isPaused)
		{
			yield return new WaitForSeconds(n);
		}
		PowerupManager.Instance.SpawnPowerup(scrollSpeed / defaultScrollSpeed);
		isCanSpawnPowerup = true;
	}

	IEnumerator SpawnEnemy()
	{
		isCanSpawnEnemy = false;

		int n = Random.Range(15, 30); //enemy frequency

		if (distance >= 1000)
		{
			n = Random.Range(10, 25);
		}

		if (!isPaused)
		{
			yield return new WaitForSeconds(n);
		}

		n = n / 10;

		for (int i = 0; i < n; i++)
		{
			EnemyManager.Instance.SpawnEnemy();

			if (!isPaused)
			{
				yield return new WaitForSeconds(1.0f);
			}
		}
		isCanSpawnEnemy = true;
	}

	IEnumerator StopScrolling(float time)
	{
		isCanModifySpeed = false;

		if (!PlayerManager.Instance.HasSecondChance())
		{
			SoundManager.Instance.StopGameMusic();
			AudioSource.PlayClipAtPoint(PlayerManager.Instance.coinDroppingAudioEffect, Vector3.up, SoundManager.Instance.audioVolume);
		}

		float startValue = scrollSpeed;
		scrollSpeedBeforeCrash = scrollSpeed;

		var rate = 1.0f / time;
		var t = 0.0f;

		while (t < 1.0f)
		{
			t += Time.deltaTime * rate;
			scrollSpeed = Mathf.Lerp(startValue, 0, t);
			yield return new WaitForEndOfFrame();
		}

		isCanSpawn = false;
		GameStateManager.SetIsPause(false);

		yield return new WaitForSeconds(0.5f);

		if (PlayerManager.Instance.HasSecondChance())
		{
			UIManager.Instance.ShowSecondChance();
		}
		else
		{
			UIManager.Instance.ShowEnd();
		}
	}

	public void SpawnCloudLayer(PlaceLocation location)
	{
		int n = Random.Range(0, cloudLayer.Count);
		GameObject randomObject = cloudLayer[n];

		cloudLayer.Remove(randomObject);
		activeElements.Add(randomObject);

		if (location == PlaceLocation.Middle)
		{
			Vector3 newPos = randomObject.transform.localPosition;
			newPos.x = 0;
			randomObject.transform.localPosition = newPos;
		}
		randomObject.SetActive(true);
	}

	public void SpawnCityBackgroundLayer(PlaceLocation location)
	{
		int n = Random.Range(0, cityBackgroundLayer.Count);
		GameObject randomObject = cityBackgroundLayer[n];

		cityBackgroundLayer.Remove(randomObject);
		activeElements.Add(randomObject);

		if (location == PlaceLocation.Middle)
		{
			Vector3 newPos = randomObject.transform.localPosition;
			newPos.x = 0;
			randomObject.transform.localPosition = newPos;
		}
		randomObject.SetActive(true);
	}

	public void SpawnCityLayer(PlaceLocation location)
	{
		int n = Random.Range(0, cityLayer.Count);
		GameObject randomObject = cityLayer[n];

		cityLayer.Remove(randomObject);
		activeElements.Add(randomObject);

		if (location == PlaceLocation.Middle)
		{
			Vector3 newPos = randomObject.transform.localPosition;
			newPos.x = 0;
			randomObject.transform.localPosition = newPos;
		}
		randomObject.SetActive(true);
	}

	public void SpawnForegroundLayer(PlaceLocation location)
	{
		int n = Random.Range(0, foregroundLayer.Count);
		GameObject randomObject = foregroundLayer[n];

		foregroundLayer.Remove(randomObject);
		activeElements.Add(randomObject);

		if (location == PlaceLocation.Middle)
		{
			Vector3 newPos = randomObject.transform.localPosition;
			newPos.x = 0;
			randomObject.transform.localPosition = newPos;
		}
		randomObject.SetActive(true);
	}

	public void SpawnObstacles()
	{
		int n = Random.Range(0, obstacles.Count);
		GameObject randomObject = (GameObject)obstacles[n];

		obstacles.Remove(randomObject);
		activeElements.Add(randomObject);

		randomObject.GetComponent<ObstacleManager>().ActivateAllElements();

		StartCoroutine(StopScrollStartTriggers());
	}

	IEnumerator StopScrollStartTriggers()
	{
		yield return new WaitForSeconds(1.5f);
		isScrollStartTriggers = false;
	}

	public void ResetObject(GameObject obj)
	{
		switch (obj.tag)
		{
			case "CloudLayer":
				activeElements.Remove(obj);
				cloudLayer.Add(obj);
				obj.transform.localPosition = new Vector3(UIManager.Instance.cameraHorizontalExtent + 75f, obj.transform.localPosition.y, obj.transform.localPosition.z);
				break;

			case "CityBackgroundLayer":
				activeElements.Remove(obj);
				cityBackgroundLayer.Add(obj);
				obj.transform.localPosition = new Vector3(UIManager.Instance.cameraHorizontalExtent + 25f, obj.transform.localPosition.y, obj.transform.localPosition.z);
				break;

			case "CityLayer":
				activeElements.Remove(obj);
				cityLayer.Add(obj);
				obj.transform.localPosition = new Vector3(UIManager.Instance.cameraHorizontalExtent + 40f, obj.transform.localPosition.y, obj.transform.localPosition.z);
				break;

			case "ForegroundLayer":
				activeElements.Remove(obj);
				foregroundLayer.Add(obj);
				obj.transform.localPosition = new Vector3(UIManager.Instance.cameraHorizontalExtent + 50f, obj.transform.localPosition.y, obj.transform.localPosition.z);
				break;

			case "Obstacles":
				activeElements.Remove(obj);
				obstacles.Add(obj);
				obj.transform.localPosition = new Vector3(UIManager.Instance.cameraHorizontalExtent + 100f, obj.transform.localPosition.y, obj.transform.localPosition.z);
				break;
		}

		if (obj.tag != "Obstacles")
		{
			obj.SetActive(false);
		}
	}

	public void AddExplosion(ParticleSystem explosion)
	{
        explosion.Play();
		activeElements.Add(explosion.gameObject);
	}

	public void RemoveExplosion(GameObject exp)
	{
		activeElements.Remove(exp);
	}

	public void Restart(bool startToScroll)
	{
		ClearMap();
		SetStartTriggersPosition();

		isCanSpawnEnemy = true;
		isCanSpawnPowerup = true;
		isCanModifySpeed = true;

		distance = 0;
		scrollSpeed = defaultScrollSpeed;

		//Place cloud and city layer in the middle of the screen (x position)
		SpawnCloudLayer(PlaceLocation.Middle);
		SpawnCityBackgroundLayer(PlaceLocation.Normal);
		SpawnCityLayer(PlaceLocation.Middle);

		isScrollStartTriggers = true;

		DailyWordManager.Instance.CalculateDailyWords();

		if (startToScroll)
		{
			StartCoroutine(StartLevelSpawner(1.25f));
		}

		startTriggers.SetActive(true);

	}

	private void SetStartTriggersPosition()
	{
		startTriggers.transform.localPosition = new Vector3(UIManager.Instance.cameraHorizontalExtent, 1, startTriggers.transform.localPosition.z);
	}

	public void Pause()
	{
		isCanSpawn = false;
		GameStateManager.SetIsPause(true);
		EnemyManager.Instance.PauseAll();
		PowerupManager.Instance.PauseAll();
	}

	public void Resume()
	{
		isCanSpawn = true;
		GameStateManager.SetIsPause(true);
		EnemyManager.Instance.ResumeAll();
		PowerupManager.Instance.ResumeAll();
	}

	public void BeginExtraSpeed()
	{
		isCanModifySpeed = false;
	}

	public void EndExtraSpeed()
	{
		isCanModifySpeed = true;
	}

	public void ContinueScrolling()
	{
		scrollSpeed = scrollSpeedBeforeCrash;
		GameStateManager.SetIsPause(false);
		isCanSpawn = true;
		isCanModifySpeed = true;
	}

	public float SpeedMultiplier()
	{
		return scrollSpeed / defaultScrollSpeed;
	}

	public IEnumerator ScrollExplosion(ParticleSystem particle)
	{
		GameObject particleObject = particle.gameObject;
		activeElements.Add(particleObject);

		if (!isPaused)
		{
			yield return new WaitForSeconds(2.0f);
		}

		activeElements.Remove(particleObject);
	}

	public IEnumerator StartLevelSpawner(float waitTime)
	{
		RandomizeObstacles();

		isCanSpawn = true;
		GameStateManager.SetIsPause(false);

		if (!isPaused)
		{
			yield return new WaitForSeconds(waitTime);
		}

		SpawnForegroundLayer(PlaceLocation.Normal);
	}
}
