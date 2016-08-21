using UnityEngine;

public class PlayerUI : MonoBehaviour {
	
	[SerializeField]
	private RectTransform staminaFill;

	private Player_Controller controller;

	public void SetController(Player_Controller _controller) {
		controller = _controller;
	}

	void SetStamina(float _amount) {
		staminaFill.localScale = new Vector3(1f, _amount, 1f);
	}

	void Update() {
		SetStamina(controller.getStamina ());
	}
}
