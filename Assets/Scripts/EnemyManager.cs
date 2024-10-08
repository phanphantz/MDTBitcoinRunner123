﻿using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	static EnemyManager _instance;
	static int instances = 0;
	public float movementSpeed = 0;
	public float minSpawnY = -14;
	public float maxSpawnY = 14;
	List<Enemy> deactivatedEnemyList = new List<Enemy>();
	List<Enemy> activatedEnemyList = new List<Enemy>();

	public static EnemyManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(EnemyManager)) as EnemyManager;
			}

			return _instance;
		}
	}

	void Start()
	{
		instances++;

		if (instances > 1)
		{
			Debug.LogWarning("There are more than one EnemyManager");
		}
		else
		{
			_instance = this;
		}

		foreach (Transform child in transform)
		{
			deactivatedEnemyList.Add(child.GetComponent<Enemy>());
		}
	}

	public void SpawnEnemy()
	{
		Enemy enemy = deactivatedEnemyList[0];

		ActivateEnemy(enemy);

		enemy.Spawn(movementSpeed, minSpawnY, maxSpawnY);
	}

	public void ActivateEnemy(Enemy enemy)
	{
		deactivatedEnemyList.Remove(enemy);
		activatedEnemyList.Add(enemy);
	}

	public void ResetEnemy(Enemy sender)
	{
		activatedEnemyList.Remove(sender);
		deactivatedEnemyList.Add(sender);
	}

	public void ResetAll()
	{
		gameObject.BroadcastMessage("ResetObject");
	}

	public void PauseAll()
	{
		this.gameObject.BroadcastMessage("Pause");
	}

	public void ResumeAll()
	{
		this.gameObject.BroadcastMessage("Resume");
	}
}
