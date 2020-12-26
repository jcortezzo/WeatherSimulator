using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Weather selectedWeather;

    // Start is called before the first frame update
    void Start()
    {
        selectedWeather = (Weather) 0;
    }

    // Update is called once per frame
    void Update()
    {
        SimpleMouseOver();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeather = (Weather) 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeather = (Weather) 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeather = (Weather) 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedWeather = (Weather) 3;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (selectedWeather == Weather.LIGHTNING)
            {

            }
        }
    }

    protected void SimpleMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.GetPoint(0), ray.direction, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            Debug.Log("Raycast hit!");
            Tile t = hit.transform.GetComponent<Tile>();
            if (t != null)
            {
                Debug.Log("Found Tile object");
                t.Select();
            }
        }
    }

    private enum Weather
    {
        LIGHTNING,
        RAIN,
        SUN,
        SNOW,
    }
}
