using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public Weather selectedWeather;
    private List<System.Action> actions;
    private Tile previousSelected;
    public LayerMask layerMask;
    [SerializeField] private GameObject lightning;
    private Animator lightningAnim;
    private Coroutine lightingCoroutine;

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
        lightningAnim = lightning.GetComponent<Animator>();
        lightning.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        SimpleMouseOver();

        for (KeyCode kc = KeyCode.Alpha1; kc <= KeyCode.Alpha4; kc++)
        {
            if (Input.GetKeyDown(kc))
            {
                selectedWeather = (Weather)(kc - KeyCode.Alpha1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!GlobalManager.Instance.IsPaused())
            {
                GlobalManager.Instance.Pause();
            }
            else
            {
                GlobalManager.Instance.Unpause();
            }
        }

        // Don't let player change game state besides picking whether if paused
        if (GlobalManager.Instance.IsPaused())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            actions[(int)selectedWeather].Invoke();
        }

        //if (lightningAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        //{
        //    lightning.transform.position = new Vector3(0, 0, -1000);
        //}
    }

    private RaycastHit2D MouseRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000, layerMask);
        return hit;
    }

    protected void SimpleMouseOver()
    {
        RaycastHit2D hit = MouseRayCast();
        if (hit.collider != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.GetPoint(0), ray.direction, Color.red);
            RaycastHit2D hit2 = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit2.collider != null)
            {
                //Debug.Log("Raycast hit!");
                if (previousSelected != null) previousSelected.Unselect();
                Tile t = hit.transform.GetComponent<Tile>();
                if (t != null)
                {
                    //Debug.Log("Found Tile object");
                    previousSelected = t;
                    t.Select();
                }
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
                if (lightingCoroutine != null) StopCoroutine(lightingCoroutine);
                lightingCoroutine = StartCoroutine(LightningZap());
                t.ChangeType(Weather.LIGHTNING);
                Jukebox.Instance.PlaySFX("Lightning");
            }
        }
    }

    public IEnumerator LightningZap()
    {
        lightning.SetActive(true);
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lightning.transform.position = new Vector3(mousePoint.x, mousePoint.y, 0);
        lightningAnim.Play(0);
        yield return new WaitForSeconds(0.5f);
        lightning.SetActive(false);
    }

    private void Rain()
    {
        RaycastHit2D hit = MouseRayCast();
        if (hit.collider != null)
        {
            Tile t = hit.transform.GetComponent<Tile>();
            if (t != null)
            {
                t.ChangeType(Weather.RAIN);
            }
        }
    }

    private void Sun()
    {
        RaycastHit2D hit = MouseRayCast();
        if (hit.collider != null)
        {
            Tile t = hit.transform.GetComponent<Tile>();
            if (t != null)
            {
                t.ChangeType(Weather.SUN);
            }
        }
    }

    private void Snow()
    {
        RaycastHit2D hit = MouseRayCast();
        if (hit.collider != null)
        {
            Tile t = hit.transform.GetComponent<Tile>();
            if (t != null)
            {
                t.ChangeType(Weather.SNOW);
            }
        }
    }
}
