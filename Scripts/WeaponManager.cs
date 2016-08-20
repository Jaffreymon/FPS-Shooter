using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponManager : NetworkBehaviour {

	[SerializeField]
	private string weaponLayerName = "Weapon";

	[SerializeField]
	private PlayerWeapon primary;

	[SerializeField]
	private Transform weaponHolder;

	private PlayerWeapon currWeapon;

	// Use this for initialization
	void Start () {
		EquipWeapon (primary);
	}

	void EquipWeapon(PlayerWeapon newWeapon) {
		currWeapon = newWeapon;

		GameObject _weaponIns = (GameObject) Instantiate (primary.graphics, weaponHolder.position, weaponHolder.rotation);
		_weaponIns.transform.SetParent (weaponHolder);

		if (isLocalPlayer) {
			_weaponIns.layer = LayerMask.NameToLayer(weaponLayerName);
		}
	}

	public PlayerWeapon getCurrWeapon() {
		return currWeapon;
	}
}
