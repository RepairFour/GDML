using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Generate an antimatter field that will negate the first hit of damage taken. 
/// Recharges after 25 seconds. 
/// </summary>
public class AntimatterShell : MonoBehaviour
{
    [SerializeField] float cooldown;
    float timer;
    bool on = false;
    [SerializeField] GameObject visual;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        PlayerAbilities.antimatterShellDelegate += update;
    }

    // Update is called once per frame
    void update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0 && on == false)
		{
            TurnOn();
        }
    }

    public bool IsOn()
	{
        return on;
	}
    public void TurnOff()
	{
        on = false;
        timer = cooldown;
        visual.SetActive(false);
    }
    private void TurnOn()
	{
        on = true;
        visual.SetActive(true);
    }
}
