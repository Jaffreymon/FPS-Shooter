using UnityEngine;
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
    private float normalFOV;
    private float adsFOV;

    [SerializeField]
    private float defaultAccuracy = 1f, aimAccuracyOffsetRate = 0.000005f, shootAccuracyOffsetRate = 0.0009f, maxAccurarcyOffset = 0.87f, walkingAccuracyOffset = 0.025f;

    // Starting accuracy of gun
    private float currAccuracy = 0.99f, currOffsetRate = 0.005f;
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
        normalFOV = cam.fieldOfView;
        adsFOV = 30f;
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
                currAccuracy = defaultAccuracy;
			}
		}

        // Moves gun position when player ADS
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            // gain accuracy bonus when ADS
            cam.fieldOfView = adsFOV;
            currOffsetRate = aimAccuracyOffsetRate;
            weaponPos.localPosition = ADSWeaponPos;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            cam.fieldOfView = normalFOV;
            currOffsetRate = shootAccuracyOffsetRate;
            weaponPos.localPosition = tmpWeaponPos;
        }

        // Lower player accuracy if shooting their gun
        if (isShooting)
        {
            currAccuracy = Mathf.Clamp(currAccuracy - currOffsetRate, maxAccurarcyOffset, defaultAccuracy);
        }

        // Player Reloads when magazine is not full
        if (!isShooting && Input.GetKeyDown (KeyCode.R) && currWeapon.clipSize != currWeapon.getMaxClipSize()) {
			weaponManager.getCurrGraphics ().playReload ();
			currWeapon.playReloadSound (audioSource.transform.position);
			currWeapon.clipSize = currWeapon.getMaxClipSize ();
		}
			
		// Player Movement Anims
		if (playerHandler.isRunning) {
            // Player accuracy worsens when running
            currAccuracy = Mathf.Clamp(currAccuracy - walkingAccuracyOffset * 2, maxAccurarcyOffset - 0.02f, defaultAccuracy);
            currGraphics.playSprint();
		}
		// Player walking anim
		else if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) ||
			Input.GetKey(KeyCode.D) ) {
            // Player accuracy is worse when walking
            currAccuracy = Mathf.Clamp(currAccuracy - walkingAccuracyOffset, maxAccurarcyOffset, defaultAccuracy);
            currGraphics.playWalk ();
		} 
		// Player default idle anim
		else {
            // Player accuracy is default when standing and not shooting
            if(!isShooting) { currAccuracy = defaultAccuracy; }
            currGraphics.playIdle();
		}
        Debug.Log(currAccuracy);
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

    private Vector3 getRandomBulletDir(Vector3 cameraDir) {
        // Gets random component values for new vector trajectory
        float randomOffsetX = UnityEngine.Random.Range(-(1 - currAccuracy), 1 - currAccuracy);
        float randomOffsetY = UnityEngine.Random.Range(-(1 - currAccuracy), 1 - currAccuracy);
        float randomOffsetZ = UnityEngine.Random.Range(-(1 - currAccuracy), 1 - currAccuracy);

        return new Vector3(cameraDir.x + randomOffsetX, cameraDir.y + randomOffsetY, cameraDir.z + randomOffsetZ);
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

        // Get forward vector without offset
        Vector3 cameraDir = cam.transform.forward;

        // Checks valid shots
        if (Physics.Raycast(cam.transform.position, getRandomBulletDir(cameraDir), out _hit, currWeapon.range, mask)) {
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
