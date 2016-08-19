using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

	// Attributes to guns
	public string name = "M16";

	public int damage = 10;
	public float range = 120f;

	public AudioSource gunSound;

	// TODO add weapon fire rate: semi, semi-auto, auto
}
