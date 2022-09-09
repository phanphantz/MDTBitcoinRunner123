using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerManager : MonoBehaviour
{
    static PlayerManager _instance;
    static int instances = 0;

    [Header("Objects & Colliders")]
    public Transform shield;
    public SphereCollider shieldCollider;
    public Transform coinMagnet;
    public SphereCollider coinMagnetCollider;

    [Header("Particles")]
    public GameObject speedParticle;
    public GameObject speedTrail;
    public GameObject obstacleBlasterParticle;
    public ParticleSystem smokeParticle;
    public ParticleSystem secondChanceParticle;

    [Header("Game Parameters")]
    public float maxY = 26f;
    public float minY = -26f;
    public float maxVerticalSpeed = 45.0f;
    float tempMaxVerticalSpeed = 45.0f;
    public float maxLongJumpSpeed = 60.0f;
    public float safetyZoneEdge = 15.0f;

    [Header("Powerups")]
    public float longJumpActivationDuration = 10.0f;
    public float flingerActivationDuration = 0.5f;
    public float coinMagnetActivationDuration = 15.0f;
    public float doubleBitcoinActivationDuration = 10.0f;
    public float speedActivationDuration = 5.0f;

    [Tooltip("Value can be Speed, DoubleBitcoin, CoinMagnet, Flinger, LongJump, Shield, SecondChance, ObstacleBlaster.")]
    public String forcedPowerup = String.Empty;

    [Header("Player Collisions")]
    public bool disableEnemyTrigger = false;
    public bool disablePowerUpTrigger = false;
    public bool disableCoinTrigger = false;
    public bool disableLetterTrigger = false;

    float verticalSpeed = 0.0f;
    float newSpeed = 0.0f;
    Vector3 newRotation = new Vector3(0, 0, 0);
    float distanceToMax;
    float distanceToMin;
    float xPos = -30;
    float startingPos = -37;

    bool isMovingUpward = false;
    bool isPlayerControllerEnabled = false;
    bool isCanCrash = true;
    bool isFalling = false;
    bool isCrashed = false;
    bool isFirstObstacleSpawned = false;
    bool isSecondChanceCollected = false;
    bool isSecondChancePowerupEnabled = false;
    bool isBoughtSecondChanceUsed = false;
    bool isShieldPowerupEnabled = false;
    bool isCoinMagnetPowerupEnabled = false;
    bool isLongJumpPowerupEnabled = false;
    bool isSpeedPowerupEnabled = false; // speed and flinger powerups use this variable for enable/disable control
    bool isPaused = false;

    [Header("Animator & Animation Parameters")]
    public Animator characterAnimator;
    public bool isOnTheBus = false;
    public bool isOnTheCar = false;

    [Header("Audio Clips")]
    public AudioClip shieldAudioEffect;
    public AudioClip coinAudioEffect;
    public AudioClip coinMagnetAudioEffect;
    public AudioClip secondChanceAudioEffect;
    public AudioClip obstacleBlasterAudioEffect;
    public AudioClip speedAudioEffect;
    public AudioClip enemyAudioEffect;
    public AudioClip wordsAudioEffect;
    public AudioClip flingerAudioEffect;
    public AudioClip longJumpAudioEffect;
    public AudioClip doubleBitcoinAudioEffect;
    public AudioClip coinDroppingAudioEffect;

    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(PlayerManager)) as PlayerManager;
            }

            return _instance;
        }
    }

    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

    void Start()
    {
        instances++;
        WarnAboutInstances();
    }

    void WarnAboutInstances()
    {
        if (instances > 1)
            Debug.LogWarning("There are more than one PlayerManager");
        else
            _instance = this;
    }

    void Update()
    {
        if (isPlayerControllerEnabled)
        {
            UpdateCharacterAnimationAndMovement();
            return;
        }

        if (isFalling)
        {
            Crash();
            return;
        }

        verticalSpeed = 0;
    }

    private void UpdateCharacterAnimationAndMovement()
    {
        UpdateCharacterMovement();
        UpdateAnimatorParameters();
    }

    private void UpdateCharacterMovement()
    {
        distanceToMax = transform.position.y - minY;
        distanceToMin = maxY - transform.position.y;
        OnMovingForward();
        transform.position += Vector3.up * verticalSpeed * 2 * Time.deltaTime;
    }

    void OnMovingForward()
    {
        if (isMovingUpward)
        {
            OnMovingUp();
            return;
        }

        OnNotMovingUp();
    }

    private void OnNotMovingUp()
    {
        verticalSpeed -= Time.deltaTime * maxVerticalSpeed;
        if (distanceToMax < safetyZoneEdge)
        {
            newSpeed = maxVerticalSpeed * (minY - transform.position.y) / safetyZoneEdge;

            if (newSpeed > verticalSpeed)
                verticalSpeed = newSpeed;

			return;
        }
        
		if (distanceToMin < safetyZoneEdge)
        {
            newSpeed = maxVerticalSpeed * (maxY - transform.position.y) / safetyZoneEdge;

            if (newSpeed < verticalSpeed)
                verticalSpeed = newSpeed;
        }
    }

    private void OnMovingUp()
    {
        verticalSpeed += Time.deltaTime * 1500;
        ChangeSpeed();
        isMovingUpward = false;
    }

    void UpdateAnimatorParameters()
    {
        characterAnimator.SetFloat("CharacterSpeed", verticalSpeed);

        SetCharacterAnimatorBool("OnTheCar", isOnTheCar);
        SetCharacterAnimatorBool("OnTheBus", isOnTheBus);

        if (isOnTheBus || isOnTheCar)
        {
            characterAnimator.enabled = true;
        }
    }

    void ChangeSpeed()
    {
        if (distanceToMin < safetyZoneEdge)
        {
            newSpeed = maxVerticalSpeed * (maxY - transform.position.y) / safetyZoneEdge;

            if (newSpeed < verticalSpeed)
                verticalSpeed = newSpeed;

			return;
        }

        if (distanceToMax < safetyZoneEdge)
        {
            newSpeed = 1500 * (minY - transform.position.y) / safetyZoneEdge;

            if (newSpeed > verticalSpeed)
                verticalSpeed = newSpeed;
        }
		
    }

    void SetCharacterAnimatorBool(string parameterName, bool value)
    {
        characterAnimator.SetBool(parameterName, value);
    }

    void Crash()
    {
        isCrashed = true;

        SetCharacterAnimatorBool("Crashed", isCrashed);

        verticalSpeed = 0;
        isFalling = false;

        ParticleSystem.EmissionModule smokeEmissionModule = smokeParticle.emission;
        smokeEmissionModule.enabled = true;

        LevelSpawnManager.Instance.StartCoroutine("StopScrolling", 0.5f);
        StartCoroutine(FallEffects());

        StartCoroutine(MoveToPosition(transform, new Vector3(xPos, minY, transform.position.z), 0.65f, false));
    }

    void SetRendererAndColliderDisabled(Collider other)
    {
        other.GetComponent<Renderer>().enabled = false;
        other.GetComponent<Collider>().enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacles" && other.name == "ObstacleGenTriggerer" && !isFirstObstacleSpawned)
        {
            isFirstObstacleSpawned = true;
            LevelSpawnManager.Instance.SpawnObstacles();
        }
        else if (other.tag == "Coin" && !disableCoinTrigger)
        {
            SetRendererAndColliderDisabled(other);
            other.transform.Find("CoinParticle").gameObject.GetComponent<ParticleSystem>().Play();
            CoinInfo info = other.transform.gameObject.GetComponent<CoinInfo>();
            if (info != null)
            {
                LevelManager.Instance.CoinGathered(info.coinValue);
            }

            AudioSource.PlayClipAtPoint(coinAudioEffect, Vector3.up, SoundManager.Instance.audioVolume);
        }
        else if (other.tag == "Letter" && !disableLetterTrigger)
        {
            LetterInfo letterInfo = other.gameObject.GetComponent<LetterInfo>();

            string dailyWord = DailyWordManager.Instance.GetDailyWord();
            string completedWord = PreferencesManager.Instance.GetCompletedWord();

            int dailyWordIndex = dailyWord.IndexOf(letterInfo.letter);

            if (dailyWordIndex > -1 && completedWord[dailyWordIndex] == '-')
            {
                completedWord = completedWord.Insert(dailyWordIndex, letterInfo.letter.ToString()).Remove(dailyWordIndex + 1, 1);
            }
            else
            {
                int i = 0;
                while ((i = dailyWord.IndexOf(letterInfo.letter, i)) != -1)
                {
                    dailyWordIndex = i;
                    if (completedWord[dailyWordIndex] == '-')
                    {
                        completedWord = completedWord.Insert(dailyWordIndex, letterInfo.letter.ToString()).Remove(dailyWordIndex + 1, 1);
                        break;
                    }

                    i++;
                }
            }

            PlaySoundClip(wordsAudioEffect);
            PreferencesManager.Instance.SetCompletedWord(completedWord);

            SetRendererAndColliderDisabled(other);
        }
        else if (other.tag == "Enemy" && !disableEnemyTrigger)
        {
            if (!isFalling && isCanCrash && !isShieldPowerupEnabled)
            {
                isFalling = true;
                DisablePlayerControls();
            }
            else if (isShieldPowerupEnabled && !isSpeedPowerupEnabled)
            {
                StartCoroutine(DisableShield());
            }
            PlayExplosion(other.transform);
        }
        else if (other.tag == "PowerUps" && !disablePowerUpTrigger)
        {
            String collidedPowerup = other.transform.name;

            if (isShieldPowerupEnabled && !isSpeedPowerupEnabled && !collidedPowerup.Equals("Shield"))
            {
                StartCoroutine(DisableShield());
            }

            if (forcedPowerup != String.Empty)
            {
                collidedPowerup = forcedPowerup;
            }

            switch (collidedPowerup)
            {
                case "Speed":
                    ActivateSpeed();
                    break;

                case "DoubleBitcoin":
                    ActivateDoubleBitcoin();
                    break;

                case "CoinMagnet":
                    ActivateCoinMagnet();
                    break;

                case "Flinger":
                    ActivateFlinger();
                    break;

                case "LongJump":
                    ActivateLongJump();
                    break;

                case "Shield":
                    ActivateShield();
                    break;

                case "SecondChance":
                    ActivateSecondChance();
                    break;

                case "ObstacleBlaster":
                    ActivateObstacleBlaster();
                    break;
            }

            other.GetComponent<Powerup>().ResetObject();
        }
    }

    void PlaySoundClip(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, Vector3.up, SoundManager.Instance.audioVolume);
    }

    public void ActivateSecondChance()
    {
        if (isPlayerControllerEnabled)
        {
            SecondChanceCollected();
            PlaySoundClip(secondChanceAudioEffect);
        }
    }

    public void ActivateObstacleBlaster()
    {
        if (isPlayerControllerEnabled)
        {
            StartCoroutine(LaunchObstacleBlaster());
            PlaySoundClip(obstacleBlasterAudioEffect);
        }
    }

    void PlayExplosion(Transform parent)
    {
        if (parent.name == "Enemy")
        {
            parent.transform.parent.gameObject.GetComponent<Enemy>().TargetHit(true);
        }
    }

    IEnumerator FallEffects()
    {
        yield return new WaitForSeconds(0.3f);

        ParticleSystem.EmissionModule smokeEmissionModule = smokeParticle.emission;
        smokeEmissionModule.enabled = false;
    }

    void SecondChanceCollected()
    {
        isSecondChanceCollected = true;
        UIManager.Instance.collectedSecondChance.gameObject.SetActive(true);
        PowerupManager.Instance.DisableSecondChanceSpawn();
    }

    IEnumerator ScaleObject(Transform obj, Vector3 scale, float time, bool deactivate)
    {
        obj.gameObject.SetActive(true);

        Vector3 startScale = obj.localScale;

        var rate = 1.0f / time;
        var t = 0.0f;

        while (t < 1.0f)
        {
            if (!isPaused)
            {
                t += Time.deltaTime * rate;
                obj.localScale = Vector3.Lerp(startScale, scale, t);
            }

            yield return new WaitForEndOfFrame();
        }

        if (deactivate)
        {
            obj.gameObject.SetActive(false);
        }

        if (obj.name == "Shield")
        {
            shieldCollider.enabled = true;
        }
    }

    IEnumerator MoveToPosition(Transform obj, Vector3 endPos, float time, bool enableObj)
    {
        float i = 0.0f;
        float rate = 1.0f / time;

        Vector3 startPos = obj.position;

        while (i < 1.0)
        {
            if (!isPaused)
            {
                i += Time.deltaTime * rate;
                obj.position = Vector3.Lerp(startPos, endPos, i);
            }

            yield return new WaitForEndOfFrame();
        }

        if (enableObj)
        {
            isPlayerControllerEnabled = true;
        }
    }

    public void ActivateSpeed()
    {
        if (isSpeedPowerupEnabled || isFalling || !isPlayerControllerEnabled)
        {
            return;
        }

        isSpeedPowerupEnabled = true;
        isCanCrash = false;

        speedParticle.SetActive(true);
        speedTrail.SetActive(true);

        LevelSpawnManager.Instance.BeginExtraSpeed();
        UIManager.Instance.collectedSpeed.gameObject.SetActive(true);
        StartCoroutine(ExtraSpeedEffect(speedActivationDuration));

        gameObject.GetComponent<AudioSource>().clip = speedAudioEffect;
        gameObject.GetComponent<AudioSource>().volume = SoundManager.Instance.audioVolume;
        gameObject.GetComponent<AudioSource>().Play();
    }

    IEnumerator ExtraSpeedEffect(float time)
    {
        newSpeed = LevelSpawnManager.Instance.scrollSpeed;
        LevelSpawnManager.Instance.scrollSpeed = 3;

        if (!isPaused)
        {
            yield return new WaitForSeconds(time);
        }

        ResetExtraSpeed();

        //wait after returned old speed to collide with enemy triggers
        disableEnemyTrigger = true;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(EnableEnemyTrigger());
    }

    public void ResetExtraSpeed()
    {
        LevelSpawnManager.Instance.scrollSpeed = newSpeed;
        isSpeedPowerupEnabled = false;
        isCanCrash = true;

        speedParticle.SetActive(false);
        speedTrail.SetActive(false);

        LevelSpawnManager.Instance.EndExtraSpeed();
        UIManager.Instance.collectedSpeed.gameObject.SetActive(false);

        gameObject.GetComponent<AudioSource>().Stop();
    }

    public void ActivateDoubleBitcoin()
    {
        if (isSpeedPowerupEnabled || isFalling || !isPlayerControllerEnabled)
        {
            return;
        }

        gameObject.GetComponent<AudioSource>().Stop();
        AudioSource.PlayClipAtPoint(doubleBitcoinAudioEffect, Vector3.up, SoundManager.Instance.audioVolume);

        UIManager.Instance.collectedDoubleBitcoin.gameObject.SetActive(true);

        LevelManager.Instance.CoinParameterDouble();
        StartCoroutine(DoubleBitcoinEffect(doubleBitcoinActivationDuration));
    }

    IEnumerator DoubleBitcoinEffect(float time)
    {
        if (!isPaused)
        {
            yield return new WaitForSeconds(time);
        }

        ResetDoubleBitcoin();
    }

    public void ResetDoubleBitcoin()
    {
        UIManager.Instance.collectedDoubleBitcoin.gameObject.SetActive(false);
        LevelManager.Instance.CoinParameterNormal();
    }

    public void ActivateCoinMagnet()
    {
        if (isCoinMagnetPowerupEnabled || isSpeedPowerupEnabled || isFalling || !isPlayerControllerEnabled)
        {
            return;
        }

        isCoinMagnetPowerupEnabled = true;

        UIManager.Instance.collectedCoinMagnet.gameObject.SetActive(true);

        coinMagnet.transform.gameObject.SetActive(true);
        coinMagnetCollider.enabled = true;

        gameObject.GetComponent<AudioSource>().clip = coinMagnetAudioEffect;
        gameObject.GetComponent<AudioSource>().volume = SoundManager.Instance.audioVolume;
        gameObject.GetComponent<AudioSource>().Play();

        StartCoroutine(DisableCoinMagnet(coinMagnetActivationDuration));
    }

    IEnumerator DisableCoinMagnet(float time)
    {
        if (!isPaused)
        {
            yield return new WaitForSeconds(time);
        }

        ResetCoinMagnet();
    }

    public void ResetCoinMagnet()
    {
        coinMagnet.transform.gameObject.SetActive(false);
        coinMagnetCollider.enabled = false;
        UIManager.Instance.collectedCoinMagnet.gameObject.SetActive(false);
        isCoinMagnetPowerupEnabled = false;
        gameObject.GetComponent<AudioSource>().Stop();
    }

    public void ActivateFlinger()
    {
        if (isSpeedPowerupEnabled || isFalling || !isPlayerControllerEnabled)
        {
            return;
        }

        gameObject.GetComponent<AudioSource>().Stop();
        AudioSource.PlayClipAtPoint(flingerAudioEffect, Vector3.up, SoundManager.Instance.audioVolume);

        isSpeedPowerupEnabled = true;
        isCanCrash = false;

        LevelSpawnManager.Instance.BeginExtraSpeed(); // It uses the same method because this is like speed effect in a short time.
        StartCoroutine(FlingerEffect(flingerActivationDuration));
    }

    IEnumerator FlingerEffect(float time)
    {
        newSpeed = LevelSpawnManager.Instance.scrollSpeed;
        LevelSpawnManager.Instance.scrollSpeed = 2;

        if (!isPaused)
        {
            yield return new WaitForSeconds(time);
        }

        ResetFlinger();

        //wait after returned old speed to collide with enemy triggers
        disableEnemyTrigger = true;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(EnableEnemyTrigger());
    }

    public void ResetFlinger()
    {
        LevelSpawnManager.Instance.scrollSpeed = newSpeed;
        isSpeedPowerupEnabled = false;
        isCanCrash = true;

        speedParticle.SetActive(false);
        speedTrail.SetActive(false);

        LevelSpawnManager.Instance.EndExtraSpeed();
    }

    public void ActivateLongJump()
    {
        if (isSpeedPowerupEnabled || isFalling || !isPlayerControllerEnabled)
        {
            return;
        }

        gameObject.GetComponent<AudioSource>().Stop();
        AudioSource.PlayClipAtPoint(longJumpAudioEffect, Vector3.up, SoundManager.Instance.audioVolume);

        isLongJumpPowerupEnabled = true;
        UIManager.Instance.collectedLongJump.gameObject.SetActive(true);

        tempMaxVerticalSpeed = maxVerticalSpeed;
        maxVerticalSpeed = maxLongJumpSpeed;
        StartCoroutine(DisableLongJump(longJumpActivationDuration));
    }

    IEnumerator DisableLongJump(float time)
    {
        if (!isPaused)
        {
            yield return new WaitForSeconds(time);
        }

        ResetLongJump();
    }

    public void ResetLongJump()
    {
        isLongJumpPowerupEnabled = false;
        maxVerticalSpeed = tempMaxVerticalSpeed;
        UIManager.Instance.collectedLongJump.gameObject.SetActive(false);
    }

    public void ActivateShield()
    {
        if (isShieldPowerupEnabled || isFalling || !isPlayerControllerEnabled)
        {
            return;
        }

        isShieldPowerupEnabled = true;
        UIManager.Instance.collectedShield.gameObject.SetActive(true);
        StartCoroutine(ScaleObject(shield.transform, new Vector3(21, 21, 1), 0.35f, false));

        gameObject.GetComponent<AudioSource>().clip = shieldAudioEffect;
        gameObject.GetComponent<AudioSource>().volume = SoundManager.Instance.audioVolume;
        gameObject.GetComponent<AudioSource>().Play();
    }

    IEnumerator DisableShield()
    {
        StartCoroutine(ScaleObject(shield.transform, new Vector3(0.01f, 1, 0.01f), 0.35f, true));
        shieldCollider.enabled = false;

        if (!isPaused)
        {
            yield return new WaitForSeconds(0.5f);
        }

        ResetSheild();
    }

    public void ResetSheild()
    {
        isShieldPowerupEnabled = false;
        UIManager.Instance.collectedShield.gameObject.SetActive(false);
        gameObject.GetComponent<AudioSource>().Stop();
    }

    IEnumerator LaunchObstacleBlaster()
    {
        obstacleBlasterParticle.SetActive(true);

        // move the effect object from left to right at the screen
        StartCoroutine(MoveToPosition(obstacleBlasterParticle.transform, new Vector3((UIManager.Instance.cameraHorizontalExtent + 10), 0, -5), 1.25f, false));

        if (!isPaused)
        {
            yield return new WaitForSeconds(2.0f);
        }

        obstacleBlasterParticle.GetComponent<ObstacleBlaster>().Disable();
    }

    public void MoveUp()
    {
        if (isLongJumpPowerupEnabled)
        {
            maxVerticalSpeed = maxLongJumpSpeed;
        }
        else
        {
            maxVerticalSpeed = 85.0f;
        }

        if (distanceToMin > 0 && isPlayerControllerEnabled)
        {
            isMovingUpward = true;
        }
    }

    public void MoveUpLonger()
    {
        maxVerticalSpeed = maxLongJumpSpeed;

        if (distanceToMin > 0 && isPlayerControllerEnabled)
        {
            isMovingUpward = true;
        }
    }

    public void MoveDown()
    {
        if (distanceToMax > 0 && isPlayerControllerEnabled)
        {
            isMovingUpward = false;
        }

    }

    public bool HasSecondChance()
    {
        if (isSecondChanceCollected || (PreferencesManager.Instance.GetPowerup("SecondChance") > 0 && !isBoughtSecondChanceUsed))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator UseSecondChance()
    {
        if (!isSecondChancePowerupEnabled)
        {
            isSecondChancePowerupEnabled = true;

            if (isSecondChanceCollected)
            {
                isSecondChanceCollected = false;
                UIManager.Instance.collectedSecondChance.gameObject.SetActive(false);
            }
            else
            {
                isBoughtSecondChanceUsed = true;
                PreferencesManager.Instance.ModifyPowerup("SecondChance", -1);
                UIManager.Instance.boughtSecondChance.gameObject.SetActive(false);
            }

            verticalSpeed = 0;
            secondChanceParticle.Play();

            newRotation = new Vector3(0, 0, 0);
            this.transform.eulerAngles = newRotation;
            StartCoroutine(LaunchObstacleBlaster());

            yield return new WaitForSeconds(0.4f);
            StartCoroutine(MoveToPosition(this.transform, new Vector3(xPos, minY, transform.position.z), 1.0f, false));

            disableEnemyTrigger = true;
            yield return new WaitForSeconds(1.2f);
            LevelSpawnManager.Instance.ContinueScrolling();
            StartCoroutine(EnableEnemyTrigger());

            isCrashed = false;
            isCanCrash = true;
            isPlayerControllerEnabled = true;
            isMovingUpward = false;
            isSecondChancePowerupEnabled = false;

            SetCharacterAnimatorBool("Crashed", isCrashed);
        }

        yield return new WaitForEndOfFrame();
    }

    IEnumerator EnableEnemyTrigger()
    {
        yield return new WaitForSeconds(3f);
        disableEnemyTrigger = false;
    }

    public void ResetStatus(bool moveToStart)
    {
        StopAllCoroutines();

        ResetCoinMagnet();
        ResetSheild();
        ResetLongJump();
        ResetFlinger();
        ResetDoubleBitcoin();
        ResetExtraSpeed();

        verticalSpeed = 0;
        isCrashed = false;
        SetCharacterAnimatorBool("Crashed", isCrashed);
        SetIsPaused(false);
        isMovingUpward = false;
        isCanCrash = true;

        isSecondChancePowerupEnabled = false;
        isSecondChanceCollected = false;
        isBoughtSecondChanceUsed = false;
        isSpeedPowerupEnabled = false;
        isShieldPowerupEnabled = false;
        isCoinMagnetPowerupEnabled = false;
        isLongJumpPowerupEnabled = false;

        LevelManager.Instance.CoinParameterNormal();

        shield.transform.localScale = new Vector3(0.01f, 1, 0.01f);
        speedParticle.SetActive(false);
        speedTrail.SetActive(false);
        isFirstObstacleSpawned = false;

        newRotation = new Vector3(0, 0, 0);

        this.transform.position = new Vector3(startingPos, minY, transform.position.z);
        this.transform.eulerAngles = newRotation;

        if (moveToStart)
        {
            StartCoroutine(MoveToPosition(this.transform, new Vector3(xPos, minY, transform.position.z), 1.0f, true));
        }

        characterAnimator.Rebind();
        characterAnimator.enabled = true;
    }

    private void SetIsPaused(bool value)
    {
        isPaused = value;
    }

    public void EnablePlayerControls()
    {
        isPlayerControllerEnabled = true;
    }

    public void DisablePlayerControls()
    {
        isPlayerControllerEnabled = false;
    }

    public void Pause()
    {
        SetIsPaused(true);
        characterAnimator.enabled = false;
    }

    public void Resume()
    {
        SetIsPaused(false);
        characterAnimator.enabled = true;
    }

    public bool Crashed()
    {
        return isCrashed;
    }
}