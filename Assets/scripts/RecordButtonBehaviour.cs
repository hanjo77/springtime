using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecordButtonBehaviour : ButtonBehaviour {

	public InputField levelNameInput;

	protected override void ButtonClick(BaseEventData data) {
		Level.Save (levelNameInput.text);
	}
}
