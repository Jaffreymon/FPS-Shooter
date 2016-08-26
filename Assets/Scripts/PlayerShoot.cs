using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

	[SerializeField]
	private PlayerWeapon currWeapon;
	[SerializeField]
	private WeaponManager weaponManager;

	private const string Player_tag = "Player";
	[SerializeField]
	private Camera cam;

	[SerializeField]
	private GameObject audioSource;

	private WeaponGraphics currGraphics;

	[SerializeField]
	private LayerMask mask;

	// Use this for initialization
	void Start () {
		if (cam == null) {
			Debug.LogError ("Player_Camera null reference!");
			this.enabled = false;
		}
		weaponManager = GetComponent<WeaponManager> ();
		currGraphics = weaponManager.getCurrGraphics ();
	}

	// Update is called once per frame
	void Update () {
		// Constant update on what gun the player has
		currWeapon = weaponManager.getCurrWeapon ();
		currGraphics = weaponManager.getCurrGraphics ();

		if (!currGraphics.am.IsPlaying("Reload")) {
			// Player Shoots semi-auto
			if (currWeapon.fireRate < 0f) {
				if (Input.GetButtonDown ("Fire1")) {
					Shoot ();
				}
			} 
		// Player shoots full auto
		else {
				if (Input.GetButtonDown ("Fire1")) {
					InvokeRepeating ("Shoot", 0f, 1f / (currWeapon.fireRate));
				} else if (Input.GetButtonUp ("Fire1")) {
					CancelInvoke ("Shoot");
				}
			}
		}

		// Player Moves
		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D)) {
			currGraphics.playWalk ();
		} else {
			currGraphics.playIdle();
		}

		// Player Reloads when magazine is not full
		if (Input.GetKeyDown (KeyCode.R) && currWeapon.clipSize != currWeapon.getMaxClipSize()) {
			currWeapon.clipSize = currWeapon.getMaxClipSize ();
			weaponManager.getCurrGraphics ().playReload ();
		}
	}

	// Called on server when player shoots
	[Command]
	void CmdOnShoot() {
		RpcDoMuzzleFlash ();
	}

	// All clients call hitting effect for each shot
	// Takes in the hit point and the normal vector of the surface
	[Command]
	void CmdOnHit(Vector3 _pos, Vector3 _normal) {
		RpcDoHitEffect (_pos, _normal);
	}

	// Is called on all clients to emit particle effect of ray collision
	[ClientRpc]
	void RpcDoHitEffect(Vector3 _pos, Vector3 _normal) {
		GameObject _hitEffect = (GameObject) Instantiate (weaponManager.getCurrGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation (_normal));
		Destroy (_hitEffect, 1.1f);
	}

	// All clients call shooting effect for each shot
	[ClientRpc]
	void RpcDoMuzzleFlash() {
		weaponManager.getCurrGraphics ().muzzleFlash.Play ();
		weaponManager.getCurrWeapon ().playShootSound (audioSource.transform.position);
		if (Time.time > weaponManager.getCurrGraphics().rate_time) {
			weaponManager.getCurrGraphics().rate_time = Time.time + weaponManager.getCurrGraphics().rate;
			weaponManager.getCurrGraphics().playRecoil ();
		}
	}

	[Client]
	void Shoot() {
		// Only Local player can shoot from all players on server
		if (!isLocalPlayer) {
			return;
		}

		// If gun has 0 clipSize, it's empty
		if(currWeapon.clipSize < 1) {
			currWeapon.playEmptyClick (audioSource.transform.position);
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

			// Show hit particles when a ray makes a collision
			CmdOnHit (_hit.point, _hit.normal);
		}

		currWeapon.clipSize--;
	}

	[Command]
	void CmdPlayerShot(string _playerID, int _damage) {
		Player _player = GameManager.GetPlayer(_playerID);
		_player.RpcTakeDamage (_damage);
	}
}
