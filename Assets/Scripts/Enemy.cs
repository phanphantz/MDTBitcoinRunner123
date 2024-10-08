﻿using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public GameObject enemyIndicatorGameObj;
	public GameObject enemyGameObj;
	bool canMove = false;
	bool isPaused = false;

	float originalSpeed = 0;
	float currentSpeed = 0;
	Vector3 originalPos = new Vector3();

	public Explosion explosionComponent;
	public float explosionDurationInSeconds = 2f;
	
	[SerializeField] OnTriggerReceiver onTriggerReceiver;

	void Start()
	{
		onTriggerReceiver.onTriggerEnter += ResetObject;

		originalPos = enemyGameObj.transform.position;
		originalPos.x = UIManager.Instance.cameraHorizontalExtent + 10;
		enemyGameObj.transform.position = originalPos;
	}

	void OnDestroy() 
	{
		onTriggerReceiver.onTriggerEnter -= ResetObject;
	}

	void Update()
	{
		if (canMove && !isPaused)
		{
			enemyGameObj.transform.position -= Vector3.right * currentSpeed * Time.deltaTime;
		}
	}

	IEnumerator PlaceEnemyIndicator(float minY, float maxY)
	{
		float randomYPos = Random.Range(minY, maxY);

		enemyIndicatorGameObj.transform.position = new Vector3(UIManager.Instance.cameraHorizontalExtent - 10, randomYPos, -5f);
		enemyIndicatorGameObj.SetActive(true);

		if (!isPaused)
		{
			yield return new WaitForSeconds(3.0f);
		}

		enemyIndicatorGameObj.SetActive(false);
		enemyIndicatorGameObj.transform.position = new Vector3(UIManager.Instance.cameraHorizontalExtent - 10, 0, -5f);

		Vector3 pos = enemyGameObj.transform.position;
		pos.y = randomYPos;
		enemyGameObj.transform.position = pos;

		this.currentSpeed = originalSpeed * LevelSpawnManager.Instance.SpeedMultiplier();

		enemyGameObj.SetActive(true);

		AudioSource.PlayClipAtPoint(PlayerManager.Instance.enemyAudioEffect, Vector3.up, SoundManager.Instance.audioVolume);

		canMove = true;
	}

	IEnumerator PlaceExplosionForSeconds(float x, float y)
    {
        explosionComponent.PlayAt(x, y);

        if (!isPaused)
            yield return new WaitForSeconds(explosionDurationInSeconds);

         explosionComponent.Finish();
    }

    public void Spawn(float s, float minY, float maxY)
	{
		originalSpeed = s;
		StartCoroutine(PlaceEnemyIndicator(minY, maxY));
	}

	void ResetObject()
    {
        StopAllCoroutines();
        explosionComponent.Reset();

        canMove = false;
        isPaused = false;

        enemyGameObj.transform.position = originalPos;
        enemyGameObj.SetActive(false);

        enemyIndicatorGameObj.SetActive(false);
        enemyIndicatorGameObj.transform.position = new Vector3(UIManager.Instance.cameraHorizontalExtent - 10, 0, -5f);

        EnemyManager.Instance.ResetEnemy(this);
    }

    public void TargetHit(bool playExplosion)
	{
		canMove = false;
		isPaused = false;

		if (playExplosion)
		{
			StartCoroutine(PlaceExplosionForSeconds(enemyGameObj.transform.position.x, enemyGameObj.transform.position.y));
		}

		enemyGameObj.transform.position = originalPos;
		enemyGameObj.SetActive(false);

		EnemyManager.Instance.ResetEnemy(this);
	}

	// It is called from broadcast messages by EnemyManager
	public void Pause()
	{
		isPaused = true;
	}

	// It is called from broadcast messages by EnemyManager
	public void Resume()
	{
		isPaused = false;
	}
}
