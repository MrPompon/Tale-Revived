using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class scr_UnitSoundManager : MonoBehaviour {

	// Use this for initialization
    private AudioClip spawnSound;
    private AudioClip deathSound;
    private AudioClip attackSound;
    private AudioClip[] unitSounds;
    private AudioClip[] weaponSounds;
    private AudioClip[] projectileHitSounds;
    private AudioSource audioSource;
    private string m_name;
	void Start () {
        audioSource = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void SetSound(string soundName) {
        m_name = transform.name;
        m_name = m_name.Replace("(Clone)", "");
        unitSounds = Resources.LoadAll<AudioClip>("SFX/Warcraft II Sounds/UnitSounds/"+m_name+"/");
    }
    public void PlaySound(string soundName) {
        if(soundName=="Ready"){
            if (unitSounds != null) {
                AudioSource.PlayClipAtPoint(unitSounds[0], transform.position); // note: find clip by name and play by name, like deathsound etc.
            }
        } else if (soundName == "DeathSound") {
            if (deathSound != null) {
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
            }
        }
    }
    public void SetDeathSound(string DeathSound) {
        deathSound = Resources.Load<AudioClip>("SFX/Warcraft II Sounds/UnitSounds/DeathSound/" + DeathSound);
    }
    public void PlayWeaponEffect(){
        if (weaponSounds != null) {
            int rnd = Random.Range(0, weaponSounds.Length);
            AudioSource.PlayClipAtPoint(weaponSounds[rnd], transform.position);
        }
    }
    public AudioClip GetProjectileHitEffect() {
            int rnd = Random.Range(0, projectileHitSounds.Length);
            return projectileHitSounds[rnd];
    }
    public void SetWeaponSoundEffect(string weaponName) {
            weaponSounds = Resources.LoadAll<AudioClip>("SFX/Warcraft II Sounds/Shared Sounds/Miscellaneous/"+weaponName+"/");
       
    }
    public void SetProjectileHitEffect(string weaponName) {
        projectileHitSounds = Resources.LoadAll<AudioClip>("SFX/Warcraft II Sounds/Shared Sounds/Miscellaneous/" + weaponName + "Hit");
    }
}
