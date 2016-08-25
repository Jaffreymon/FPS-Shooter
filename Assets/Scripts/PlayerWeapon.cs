using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerWeapon {

	// Attributes to guns
	public string name = "M16";

	public int damage = 10;
	public float range = 120f;
	private int clipSizeMax = 30;
	public int clipSize = 30;
	private float reloadTime = 2f;

	[SerializeField]
	/* Array of AudioClips to be played
	 * Elem 0: EmptyCLick
	 * Elem 1: Shooting
	 */ 
	private AudioClip[] sounds;

	public GameObject graphics;
	public float fireRate = 100f;

	// TODO add weapon fire rate: semi, semi-auto, auto

	public int getMaxClipSize() {
		return clipSizeMax;
	}

	public float getReloadTime() {
		return reloadTime;
	}

	public void playEmptyClick(Vector3 _pos) {
		AudioSource.PlayClipAtPoint (sounds[0], _pos);
	}

	public void playShootSound(Vector3 _pos) {
		_pos.z += 1f;
		AudioSource.PlayClipAtPoint (sounds[1], _pos);
	}
}
