using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character {

	protected override void Start(){
		base.Start();
		playSoundEffects = true;
	}

	protected override void UpdateCommands () {
		commands.horizontal = Input.GetAxis("Horizontal");

		commands.btnLeft = Input.GetButton("Left");
		commands.btnRight = Input.GetButton("Right");

		commands.btnDownSlam = Input.GetButtonDown("Slam");
		commands.btnSlam = Input.GetButton("Slam");
		commands.btnUpSlam = Input.GetButtonUp("Slam");
		
		commands.btnDownJump = Input.GetButtonDown("Jump");
		
		commands.btnDownDash = Input.GetButtonDown("Dash");
		commands.btnDash = Input.GetButton("Dash");
		commands.btnUpDash = Input.GetButtonUp("Dash");

		commands.btnDownGrab = Input.GetButtonDown("Grab");
		commands.btnGrab = Input.GetButton("Grab");
		commands.btnUpGrab = Input.GetButtonUp("Grab");
	}
}
