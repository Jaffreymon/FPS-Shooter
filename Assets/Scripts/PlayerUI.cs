using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
	
	[SerializeField]
	private RectTransform staminaFill;

	[SerializeField]
	private Player_Controller controller;

	[SerializeField]
	private PlayerWeapon currWeapon;

	[SerializeField]
	private Text ammoCount;

	public void SetController(Player_Controller _controller) {
		controller = _controller;
	}

	void SetStamina(float _amount) {
		staminaFill.localScale = new Vector3(1f, _amount, 1f);
	}

	void Update() {
		SetStamina(controller.getStamina ());
		//ammoCount.text = "Ammo: " + currWeapon.clipSize.ToString ();
	}
		
}
