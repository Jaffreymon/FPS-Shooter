using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

	private PlayerWeapon currWeapon;
	private WeaponManager weaponManager;

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
		weaponManager = GetComponent<WeaponManager> ();
	}

	// Update is called once per frame
	void Update () {
		currWeapon = weaponManager.getCurrWeapon ();
		if (currWeapon.fireRate <= 0f) {
			if (Input.GetButtonDown ("Fire1")) {
				Shoot ();
			}
		} else {
			if (Input.GetButtonDown ("Fire1")) {
				InvokeRepeating ("Shoot", 0f, 1f/(currWeapon.fireRate));
			}
			else if(Input.GetButtonUp("Fire1")) {
				CancelInvoke ("Shoot");
			}
		}
	}

	// Called on server when player shoots
	[Command]
	void CmdOnShoot() {
		RpcDoMuzzleFlash ();
	}

	// All clients call shooting effect for each shot
	[ClientRpc]
	void RpcDoMuzzleFlash() {
		weaponManager.getCurrGraphics ().muzzleFlash.Play ();
	}

	[Client]
	void Shoot() {
		// Only Local player can shoot from all players on server
		if (!isLocalPlayer) {
			return;
		}

		// Player is shooting, let server know
		CmdOnShoot ();

		RaycastHit _hit;

		// Checks valid shots
		if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currWeapon.range, mask)) {
			if (_hit.collider.tag == Player_tag) {
				CmdPlayerShot (_hit.collider.name, currWeapon.damage);
			}
		}
	}

	[Command]
	void CmdPlayerShot(string _playerID, int _damage) {
		Player _player = GameManager.GetPlayer(_playerID);
		_player.RpcTakeDamage (_damage);
	}
}
