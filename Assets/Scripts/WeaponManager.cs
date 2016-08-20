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

	private WeaponGraphics currGraphics;
	private PlayerWeapon currWeapon;

	// Use this for initialization
	void Start () {
		EquipWeapon (primary);
	}
		
	void EquipWeapon(PlayerWeapon newWeapon) {
		currWeapon = newWeapon;

		GameObject _weaponIns = (GameObject) Instantiate (primary.graphics, weaponHolder.position, weaponHolder.rotation);
		_weaponIns.transform.SetParent (weaponHolder);

		currGraphics = _weaponIns.GetComponent<WeaponGraphics> ();
		if (currGraphics == null) {
			Debug.LogError ("No Weapon Graphics on weapon " + _weaponIns.name);
		}

		if (isLocalPlayer) {
			Util.SetLayerRecursive (_weaponIns, LayerMask.NameToLayer(weaponLayerName));
		}
	}

	public WeaponGraphics getCurrGraphics() {
		return currGraphics;
	}

	public PlayerWeapon getCurrWeapon() {
		return currWeapon;
	}
}
