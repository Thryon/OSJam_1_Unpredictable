using UnityEngine;
using UnityEngine.Events;

public static class GlobalEvents
{
    public class InputEvent : UnityEvent<(int,InputManager.EInputType)>{}
    public static InputEvent OnInputBuffered = new ();
    
    public static UnityEvent OnBufferPhaseDone = new ();
}
