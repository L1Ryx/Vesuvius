using UnityEngine;

public class MySickScript : MonoBehaviour
{
    // Fields
    private int myNum = 4;
    protected string myString = "Hi";
    public bool myBool = true;

    // GameObject Lifecycle Functions
    private void Start()
    {
        Debug.Log("I am called ONCE when I am created and never again.");
    } 
    private void Update()
    {
        Debug.Log("I am called ONCE EVERY FRAME.");
    }
}

