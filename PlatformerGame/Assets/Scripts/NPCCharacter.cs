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

        commands.rollOverOk = false;
    }

    public void MoveLeft() {
        commands.horizontal = -1.0f;
        commands.btnRight = true;
        commands.btnLeft = true;

        commands.rollOverOk = false;
    }

    public void Grab() {
        commands.btnDownGrab = true;

        commands.rollOverOk = false;
    }

    public void StopGrabbing() {
        commands.btnUpGrab = true;
        

        commands.rollOverOk = false;

        // Copied from Character.Update. This is very hackish -- not sure
        // why setting btnUpGrab doesn't cause this to happen.
        /* sprite.color = Color.white;
        DropObject(); */
    }

    public void Jump() {
        commands.btnDownJump = true;

        commands.rollOverOk = false;
    }

    public void Stop() {
        commands = new CommandState();
    }
}
