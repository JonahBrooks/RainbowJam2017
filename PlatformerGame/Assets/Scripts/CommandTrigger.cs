using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandTrigger : MonoBehaviour {

	class CommandStatus {
		public bool waitingBefore;
		public bool hasFired;
		public bool waitingAfter;
		public float secondsRemaining;
		public GameObject triggeringNpc;
	}

	// These variables are initialized here so that they have the right default
	// values when the CommandTrigger is first examined with our custom inspector.
	public bool startArmed = true;
	public bool fireOnlyOnce = true;
	public bool printDebugMessages = false;

	bool isArmed;
	public List<Command> commandList = new List<Command>();
	List<CommandStatus> commandStatusList = new List<CommandStatus>();
	int curCommandIndex;


	void Start () {
		//Debug.Log("CommandTrigger.Start(): commandList.Count: " + commandList.Count);

		// There must always be at least one Command in the list.
		if (commandList.Count < 1)
			commandList.Add(new Command());

		 if (startArmed)
			Arm(); 
	}

	public void Arm() {
		commandStatusList = new List<CommandStatus>();
		for (int i = 0; i < commandList.Count; i++)
			commandStatusList.Add(new CommandStatus());

		var curCommand = commandList[0];
		var curStatus = commandStatusList[0];

		if (curCommand.triggerMode == Command.TriggerMode.AlwaysTrigger) {
			curStatus.waitingBefore = true;
			curStatus.secondsRemaining = curCommand.waitSecondsBeforeFiring;
		}

		isArmed = true;
	}
	
	void Update () {
		if (!isArmed)
			return;

		// This is a fire-only-once trigger, and we've finished all our commands.
		if (curCommandIndex >= commandList.Count)
			return;
		
		var curCommand = commandList[curCommandIndex];
		var curStatus = commandStatusList[curCommandIndex];

		// If we're not waiting before, then the command hasn't been triggered yet. If
		// we're not waiting after, it may have been triggered, but hasn't been fired yet.
		// Either way, there's nothing to do yet.
		if (!curStatus.waitingBefore && !curStatus.waitingAfter)
			return;

		// If there's still wait time remaining, there's nothing to do yet.
		curStatus.secondsRemaining -= Time.deltaTime;
		if (curStatus.secondsRemaining > 0.0f)
			return;

		// At this point, we have finished a waiting period.
		if (curStatus.waitingBefore) {
			// We've finished waiting before. Fire the event and being waiting after.
			Fire();
			curStatus.waitingBefore = false;
			curStatus.hasFired = true;
			curStatus.waitingAfter = true;
			curStatus.secondsRemaining = curCommand.waitSecondsAfterFiring;
		}
		else {
			// We've finished waiting after. Queue up the next command.
			curStatus.waitingAfter = false;

			curCommandIndex++;
			if (curCommandIndex >= commandList.Count) {
				if (fireOnlyOnce) {
					return;
				}
				else {
					curCommandIndex = 0;
				}
			}

			curCommand = commandList[curCommandIndex];
			curStatus = commandStatusList[curCommandIndex];

			if (curCommandIndex == 1) {
				Debug.Log("curCommandIndex is 1");
			}

			//if (curCommand.triggerMode == Command.TriggerMode.AlwaysTrigger) {
				curStatus.waitingBefore = true;
				curStatus.secondsRemaining = curCommand.waitSecondsBeforeFiring;
			//}
		}
	}

	void Fire() {
		var curCommand = commandList[curCommandIndex];
		var curStatus = commandStatusList[curCommandIndex];

		// Set NPC target based on triggerMode. Some command types won't use this.
		GameObject npcObject = null;
		switch (curCommand.triggerMode) {
			case Command.TriggerMode.TriggeredByNPC:
				// We may have collided with the NPC object or a child object.
				npcObject = curStatus.triggeringNpc;
				if (npcObject.GetComponent<NPCCharacter>() == null)
					npcObject = curStatus.triggeringNpc.transform.parent.gameObject;
				break;

			case Command.TriggerMode.TriggeredByPlayer:
			case Command.TriggerMode.AlwaysTrigger:
				npcObject = curCommand.targetNpc;
				break;
		}

		// check if null?

		switch (curCommand.type) {
			case Command.Type.ClaimCamera:
				GameObject.Find("Main Camera").GetComponent<CameraController>().ChangeTargetWithLerp(
					curCommand.cameraFocusTarget,
					curCommand.transitionDuration);
				break;

			case Command.Type.ArmOtherTrigger:
				curCommand.targetTrigger.GetComponent<CommandTrigger>().Arm();
				break;

			case Command.Type.MoveRight:
				npcObject.GetComponent<NPCCharacter>().MoveRight();
				break;

			case Command.Type.MoveLeft:
				npcObject.GetComponent<NPCCharacter>().MoveLeft();
				break;

			case Command.Type.Jump:
				npcObject.GetComponent<NPCCharacter>().Jump();
				break;

			case Command.Type.GlideRight:
				Debug.Log("not yet implemented");
				break;

			case Command.Type.GlideLeft:
				Debug.Log("not yet implemented");
				break;

			case Command.Type.Grab:
				npcObject.GetComponent<NPCCharacter>().Grab();
				break;

			case Command.Type.StopGrabbing:
				npcObject.GetComponent<NPCCharacter>().StopGrabbing();
				break;

			case Command.Type.UseSlowSpeed:
				npcObject.GetComponent<NPCCharacter>().UseSlowSpeed();
				break;

			case Command.Type.UseNormalSpeed:
				npcObject.GetComponent<NPCCharacter>().UseNormalSpeed();
				break;

			case Command.Type.Stop:
				npcObject.GetComponent<NPCCharacter>().Stop();
				break;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		Print("CommandTrigger.OnTriggerEnter2D(): Function start");
		if (!isArmed || curCommandIndex >= commandList.Count) {
			Print("CommandTrigger.OnTriggerEnter2D(): Returning; not armed or index too high");
			return;
		}
		
		var curCommand = commandList[curCommandIndex];
		var curStatus = commandStatusList[curCommandIndex];

		// If we're waiting on our current triggered/fired command, we can ignore collisions.
		if (curStatus.waitingBefore || curStatus.waitingAfter) {
			Print("CommandTrigger.OnTriggerEnter2D(): Returning; waiting before or after");
			return;
		}

		// AlwaysTrigger commands don't care about collisions.
		if (curCommand.triggerMode == Command.TriggerMode.AlwaysTrigger) {
			Print("CommandTrigger.OnTriggerEnter2D(): Returning; AlwaysTrigger");
			return;
		}

		// Triggered by NPC, and the other collider is in the NPC layer.
		if (curCommand.triggerMode == Command.TriggerMode.TriggeredByNPC && other.gameObject.layer == 12) {
			curStatus.waitingBefore = true;
			curStatus.secondsRemaining = curCommand.waitSecondsBeforeFiring;

			// We note the NPC that hit the trigger so we can apply the command to it.
			curStatus.triggeringNpc = other.transform.gameObject;
			Print("CommandTrigger.OnTriggerEnter2D(): Triggered by NPC: " + curStatus.triggeringNpc.name);
		}
		// Triggered by player, and the other collider is the player.
		else if (curCommand.triggerMode == Command.TriggerMode.TriggeredByPlayer && other.gameObject.layer == 13) {
			curStatus.waitingBefore = true;
			curStatus.secondsRemaining = curCommand.waitSecondsBeforeFiring;
			Print("CommandTrigger.OnTriggerEnter2D(): Triggered by player");
		}
	}

	void OnDrawGizmos() {
		var box = GetComponent<BoxCollider2D>();
		Gizmos.color = Color.magenta;		
		Gizmos.DrawWireCube(transform.position, box.size);

		if (startArmed || isArmed) {
			Gizmos.DrawIcon(transform.position, "NPC Trigger Icon.png", true);
		}
		else {
			Gizmos.DrawIcon(transform.position, "NPC Trigger Icon Gray.png", true);
		}		
	}

	void Print(string msg) {
		if (printDebugMessages)
			Debug.Log(msg);
	}
}
