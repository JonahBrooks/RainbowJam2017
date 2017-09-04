using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Command {

	public enum Type {
		ClaimCamera,
		ArmOtherTrigger,
		MoveRight,
		MoveLeft,
		Jump,
		GlideRight,
		GlideLeft,
		Grab,
		StopGrabbing,
		UseSlowSpeed,
		UseNormalSpeed,
		Stop
	}

	public enum TriggerMode {
		TriggeredByNPC,
		TriggeredByPlayer,
		AlwaysTrigger
	}

	public Command.Type type;
	public TriggerMode triggerMode;
	public float waitSecondsBeforeFiring;
	public float waitSecondsAfterFiring;

	// Only gets pre-defined value if triggerMode is TriggeredByPlayer.
	public GameObject targetNpc;

	// Only relevant for ClaimCamera commands.
	public Transform cameraFocusTarget;
	public float transitionDuration;

	// Only relevant for ArmOtherTrigger commands.
	public GameObject targetTrigger;
}
