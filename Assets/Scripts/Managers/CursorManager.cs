using UnityEngine;

public class CursorManager : MonoBehaviour
{

    [SerializeField] private Texture2D cursorNormal;

    private Vector2 hotSpot =new Vector2(0.0873726606f,0.922570348f);   
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.SetCursor(cursorNormal, hotSpot, CursorMode.Auto);
    }

    // Update is called once per fram
}
