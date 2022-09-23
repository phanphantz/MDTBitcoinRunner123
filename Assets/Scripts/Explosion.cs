using UnityEngine;

public class Explosion : MonoBehaviour
{
    Vector3 originalPos = new Vector3();
	bool isPlaying = false;
	public ParticleSystem particleSystem;

    private void Start() 
    {
        originalPos = particleSystem.transform.position;
		originalPos.x = UIManager.Instance.cameraHorizontalExtent + 10;
		particleSystem.transform.position = originalPos;
    }

    public void Finish()
    {
        LevelSpawnManager.Instance.RemoveExplosion(particleSystem.gameObject);
        isPlaying = false;
        particleSystem.transform.position = originalPos;
    }

    public void PlayAt(float x, float y)
    {
        particleSystem.transform.position = new Vector3(x - 6, y, originalPos.z);
        isPlaying = true;
        LevelSpawnManager.Instance.AddExplosion(particleSystem);
    }

    public void Reset()
    {
        if (isPlaying)
        {
            LevelSpawnManager.Instance.RemoveExplosion(particleSystem.gameObject);
        }
    }

}
