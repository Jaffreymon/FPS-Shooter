using UnityEngine;

public class WeaponGraphics : MonoBehaviour {

	public ParticleSystem muzzleFlash;
	public GameObject hitEffectPrefab;
	public AudioSource gunAudio;

	public Animation am;
	public AnimationClip walk;
	public AnimationClip idle;


	// TODO Animation: Run, Shoot

	void Update() {
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
			playAnim (walk.name);
		} else {
			playAnim (idle.name);
		}
	}

	public void playAnim( string animName) {
		am.CrossFade(animName);
	}

	public void stopAnim() {
		am.Stop ();
	}
}