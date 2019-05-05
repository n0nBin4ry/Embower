using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Base : MonoBehaviour
{
	// actor state
	protected constants.ObjState my_state = constants.ObjState.Active;

	// Start is called before the first frame update
	virtual protected void Start() { }

	// Update is called once per frame
	virtual protected void Update() { }

	// get state of actor
	public constants.ObjState getState()
	{
		return my_state;
	}

	public void setState(constants.ObjState state) { my_state = state; }

	// resets actor
	virtual protected void reset() { }

	// kills actor
	virtual protected void die() { }
}
