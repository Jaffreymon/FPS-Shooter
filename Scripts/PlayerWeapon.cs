using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

	// Attributes to guns
	public string name = "M16";

	public int damage = 10;
	public float range = 120f;

	public GameObject graphics;
	public float fireRate = 100f;

	// TODO add weapon fire rate: semi, semi-auto, auto
}
