using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCharacter : Character {

    protected override void Start() {
        base.Start();
        playSoundEffects = false;
    }


	protected override void UpdateCommands() {
	}

    public void MoveRight() {
        commands.horizontal = 1.0f;
        commands.btnRight = true;
        commands.btnLeft = true;
    }

    public void MoveLeft() {
        commands.horizontal = -1.0f;
        commands.btnRight = true;
        commands.btnLeft = true;
    }

    public void Grab() {
        commands.btnDownGrab = true;
    }

    public void StopGrabbing() {
        commands.btnUpGrab = true;
    }

    public void Stop() {
        commands = new CommandState();
    }
}
