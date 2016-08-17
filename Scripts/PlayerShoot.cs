using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

	public PlayerWeapon weapon;

	private const string Player_tag = "Player";
	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	// Use this for initialization
	void Start () {
		if (cam == null) {
			Debug.LogError ("Player_Camera null reference!");
			this.enabled = false;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")) {
			Shoot();
		}
	}

	[Client]
	void Shoot() {
		RaycastHit _hit;

		// Checks valid shots
		if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range, mask)) {
			if (_hit.collider.tag == Player_tag) {
				CmdPlayerShot (_hit.collider.name);
			}
		}
	}

	[Command]
	void CmdPlayerShot(string _ID) {
		Debug.Log (_ID + " was hit");
	}
}
