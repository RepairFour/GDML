using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// After activating your special weapon, your next melee attack deals +3 additional damage
/// </summary>
public class SpellswordKeystone : MonoBehaviour
{
    [HideInInspector]
    public bool buffOn = false;
    public int dmgEnhancement;
    [SerializeField] Light2D meleeLight;
    Color baseColour;
    [SerializeField] float lightIntensity;
	private void Start()
	{
        baseColour = meleeLight.color;
    }
	public void Activate()
    {
        if (PlayerAbilities.instance.chosenAbility == PlayerAbilities.Ability.spellswordKeystone)
        {
            PlayerAbilities.instance.spellswordKeyStone.buffOn = true;
        }
    }
	private void Update()
	{
		if(buffOn)
		{
            meleeLight.color = Color.yellow;
            meleeLight.intensity = lightIntensity;
        }
        else
		{
            StartCoroutine(ChangeMeleeColour());
		}
	}

    IEnumerator ChangeMeleeColour()
	{
        yield return new WaitForSeconds(0.2f);
        meleeLight.color = baseColour;
        meleeLight.intensity = 0.4f;

    }
}
