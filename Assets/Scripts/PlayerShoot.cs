﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(Player_Controller))]
public class PlayerShoot : NetworkBehaviour {

	[SerializeField]
	private PlayerWeapon currWeapon;

	[SerializeField]
	private WeaponManager weaponManager;

	private const string Player_tag = "Player";
	[SerializeField]
	private Camera cam;
	[SerializeField]
	private Transform weaponPos;
	private Vector3 tmpWeaponPos;
	private Vector3 ADSWeaponPos = new Vector3 (0f, -0.4f, 0.81f);

	[SerializeField]
	private GameObject audioSource;

	private WeaponGraphics currGraphics;

	[SerializeField]
	private Player_Controller playerHandler;

	[SerializeField]
	private LayerMask mask;

	// Boolean to watch behavior of reloading during shooting
	private bool isShooting = false;

	// Use this for initialization
	void Start () {
		if (cam == null) {
			Debug.LogError ("Player_Camera null reference!");
			this.enabled = false;
		}
		weaponManager = GetComponent<WeaponManager> ();
		currGraphics = weaponManager.getCurrGraphics ();
		tmpWeaponPos = weaponPos.localPosition;
	}

	// Update is called once per frame
	void Update () {
		// Constant update on what gun the player has
		currWeapon = weaponManager.getCurrWeapon ();
		currGraphics = weaponManager.getCurrGraphics ();

		// Checks if player is reloading to prevent shooting
		if (!currGraphics.am.IsPlaying("Reload")) {
			if (Input.GetButtonDown ("Fire1")) {
				InvokeRepeating ("Shoot", 0f, 1f / (currWeapon.tmpRate));
				isShooting = true;
			} else if (Input.GetButtonUp ("Fire1")) {
				CancelInvoke ("Shoot");
				isShooting = false;
			}
		}

		// Player Reloads when magazine is not full
		if (!isShooting && Input.GetKeyDown (KeyCode.R) && currWeapon.clipSize != currWeapon.getMaxClipSize()) {
			weaponManager.getCurrGraphics ().playReload ();
			currWeapon.playReloadSound (audioSource.transform.position);
			currWeapon.clipSize = currWeapon.getMaxClipSize ();
		}
			
		// Player Movement Anims
		if (playerHandler.isRunning) {
			currGraphics.playSprint();
		}
		// Player walking anim
		else if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) ||
			Input.GetKey(KeyCode.D) ) {
			currGraphics.playWalk ();
		} 
		// Player default idle anim
		else {
			currGraphics.playIdle();
		}

		if (Input.GetKeyDown (KeyCode.Mouse1)) {
			weaponPos.localPosition= ADSWeaponPos;
		}
		else if(Input.GetKeyUp(KeyCode.Mouse1)) {
			weaponPos.localPosition= tmpWeaponPos;
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
			weaponManager.getCurrGraphics().rate_time = Time.time + weaponManager.getCurrGraphics().recoilRateTime;
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
