using UnityEngine;
using System.Collections;

public class scr_AudioManager : MonoBehaviour 
{
    public AudioSource efxSource;
    public AudioSource musicSource;
    public static scr_AudioManager instance = null;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;


    public AudioClip[] m_playerSteps;
    public AudioClip[] m_JumpLanding;

    public AudioClip DrawBowSound_1;
    public AudioClip ShootBowSound_1;


    private float m_PlayDelayTimer;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }
    public void PlayBowShoot()
    {
        efxSource.clip = ShootBowSound_1;
        efxSource.Play();
    }
    public void PlayDrawBow()
    {
        efxSource.clip = DrawBowSound_1;
        efxSource.Play();
    }
    public AudioClip[] GetFootStepSounds()
    {
        return m_playerSteps;
    }
    public AudioClip[] GetJumpLandingSounds()
    {
        return m_JumpLanding;
    }
    public void RandomizeSfx(float playDelay, params AudioClip[] clips)
    {
        if(m_PlayDelayTimer > playDelay)
        {

      
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
        m_PlayDelayTimer = 0;
        }
        else
        {
            m_PlayDelayTimer += Time.deltaTime;
        }
    }
    void Update()
    {

    }

}
