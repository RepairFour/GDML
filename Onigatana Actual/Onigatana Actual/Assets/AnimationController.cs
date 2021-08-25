using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    Controller controller;
    public Animator animator;
    PlayerMap input;

    private void Start()
    {
        controller = GetComponent<Controller>();
        input = new PlayerMap();
        input.Enable();
        //animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        

        
    }

}
