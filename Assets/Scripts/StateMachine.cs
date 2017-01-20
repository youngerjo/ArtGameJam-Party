using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine  {

	private Dictionary<string, State> _states = new Dictionary<string, State>();
	
	private State _currentState = null;

	public State currentState {
		get {
			return _currentState;
		}
	}

	public void AddState(State state) {
		_states[state.name] = state;
	}

	public void BeginState(string name) {

		if (_currentState != null && _currentState.name == name) {
			return;
		}

		OnEndState(_currentState);

		_currentState = this[name];

		OnBeginState(_currentState);
	}

	public void Update(float deltaTime) {

		OnUpdateState(_currentState, deltaTime);
	}

	public State this[string name] {
		get { return _states[name]; }
	}

	private void OnBeginState(State state) {

		if (state == null) {
			return;
		}

		Debug.Log(string.Format("Begin {0}", state.name));

		state.Begin();
	}

	private void OnUpdateState(State state, float deltaTime) {

		if (_currentState == null) {
			return;
		}

		state.Update(deltaTime);
	}

	private void OnEndState(State state) {

		if (_currentState == null) {
			return;
		}

		Debug.Log(string.Format("End {0}", state.name));

		state.End();
	}
}
