using UnityEngine;

// General recursive function to set all children's layers to match parent
public class Util{

	public static void SetLayerRecursive(GameObject _obj, int _newLayer) {
		if (_obj == null) {
			return;
		}
		_obj.layer = _newLayer;

		foreach (Transform _child in _obj.transform) {
			if (_child == null) {
				continue;
			}
			SetLayerRecursive (_child.gameObject, _newLayer);
		}
	}
}
