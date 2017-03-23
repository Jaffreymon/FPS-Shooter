using UnityEngine;

public class WeaponGraphics : MonoBehaviour {

	public ParticleSystem muzzleFlash;
	public GameObject hitEffectPrefab;

	public Animation am;
	public AnimationClip walk;
	public AnimationClip idle;
	public AnimationClip recoil;
	public AnimationClip reload;
	public AnimationClip sprinting;

	public float rate_time = 0f;
	public float recoilRateTime = 0.1f;

	void Start() {
		am [reload.name].layer = 1;
	}

	public void playWalk() {
		am.CrossFade(walk.name);
	}

	public void playIdle() {
		am.CrossFade(idle.name);
	}

	public void stopAnim() {
		am.Stop ();
	}

	public void playRecoil() {
		am.Stop ();
		am.Play(recoil.name);
	}

	public void playSprint() {
		am.CrossFade (sprinting.name);
	}

	public void playReload() {
		am.Stop ();
		am.Play (reload.name, PlayMode.StopAll);
	}
}