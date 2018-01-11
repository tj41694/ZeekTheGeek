using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {

    [HideInInspector]
    public bool isOpen;
    private Animation doorOpen;
	void Start () {
        doorOpen = GetComponent<Animation>();
	}
	public void OpenDoor()
    {
        isOpen = true;
        doorOpen.Play("DoorOpen");
    }
}
