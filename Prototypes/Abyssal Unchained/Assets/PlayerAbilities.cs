using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAbilities : MonoBehaviour
{
	public static PlayerAbilities instance;
	public enum Ability
	{ 
		antimatterShell,
		spellswordKeystone
	}
	[SerializeField] Ability chosenAbility;

	public static Action antimatterShellDelegate = delegate { };

	private void Start()
	{
		instance = this;
	}

	private void Update()
	{
		switch (chosenAbility)
		{
			case Ability.antimatterShell:
				antimatterShellDelegate();
				break;
			case Ability.spellswordKeystone:
				break;
		}
	}

}
