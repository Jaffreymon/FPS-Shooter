using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

	// Attributes to guns
	public string name = "M16";

	public int damage = 10;
	public float range = 120f;
	private int clipSizeMax = 30;
	public int clipSize = 30;
	private float reloadTime = 2f;


	public GameObject graphics;
	public float fireRate = 100f;

	// TODO add weapon fire rate: semi, semi-auto, auto

	public int getMaxClipSize() {
		return clipSizeMax;
	}

	public float getReloadTime() {
		return reloadTime;
	}
}
