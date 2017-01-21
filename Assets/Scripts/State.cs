using System.Collections;
using System.Collections.Generic;

public class State {

  public delegate void Handler(State state);

  public event Handler OnBegin = delegate {};
  public event Handler OnUpdate = delegate {};
  public event Handler OnEnd = delegate {};

  private string _name;
  private float _elapsedTime = 0.0f;

  public string name {
    get { return _name; }
  }

  public float elapsedTime {
    get { return _elapsedTime; }
  }

  public State(string name) {
    _name = name;
  }
  
  public void Begin() {
    _elapsedTime = 0.0f;
    OnBegin(this);
  }

  public void Update(float deltaTime) {
    _elapsedTime += deltaTime;
    OnUpdate(this);
  }

  public void End() {
    OnEnd(this);
  }

  public void ResetTime() {
    _elapsedTime = 0.0f;
  }
}
