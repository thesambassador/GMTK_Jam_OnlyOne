using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class AbilityPanel : MonoBehaviour
{
	static AbilityPanel instance;
	private void Start() {
		instance = this;
	}

	public Image AbilitySprite;
	public Text NameText;
	public Text DescriptionText;

    public static void SetAbilityInfo(PlayerAbility ability) {
		instance.AbilitySprite.sprite = ability.AbilitySprite;
		instance.AbilitySprite.enabled = true;
		instance.NameText.text = ability.AbilityName;
		instance.DescriptionText.text = ability.AbilityDescription;
	}
}
