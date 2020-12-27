using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Weather selectedWeather;
    private List<System.Action> actions;

    // Start is called before the first frame update
    void Start()
    {
        selectedWeather = Weather.LIGHTNING;
        actions = new List<System.Action>()
        {
            Lightning,
            Rain,
            Sun,
            Snow,
        };
    }

    // Update is called once per frame
    void Update()
    {
        SimpleMouseOver();

        for (KeyCode kc = KeyCode.Alpha1; kc <= KeyCode.Alpha4; kc++)
        {
            if (Input.GetKeyDown(kc))
            {
                selectedWeather = (Weather) (kc - KeyCode.Alpha1);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            actions[(int)selectedWeather].Invoke();
        }
    }

    private RaycastHit2D MouseRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        return hit;
    }

    protected void SimpleMouseOver()
    {
        RaycastHit2D hit = MouseRayCast();
        if (hit.collider != null)
        {
            //Debug.Log("Raycast hit!");
            Tile t = hit.transform.GetComponent<Tile>();
            if (t != null)
            {
                //Debug.Log("Found Tile object");
                t.Select();
            }
        }
    }

    private void Lightning()
    {
        RaycastHit2D hit = MouseRayCast();
        if (hit.collider != null)
        {
            Tile t = hit.transform.GetComponent<Tile>();
            if (t != null)
            {
                t.ChangeType(Weather.LIGHTNING);
            }
        }
    }

    private void Rain()
    {

    }

    private void Sun()
    {

    }

    private void Snow()
    {

    }
}
