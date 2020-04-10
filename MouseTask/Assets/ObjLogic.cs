using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjLogic : MonoBehaviour
{
    const int STATE_DEFAULT = 0;
    const int STATE_MOVE = 1;
    const int STATE_PLACE = 2;


    public GameObject area;
    public GameObject barrier;
    GameObject curObj = null;

    Vector2 mouseCoords;
    Vector3 objPos;

    int state = 0;
    public int objType = 0;
    float moveMultiplier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveMultiplier = Camera.main.gameObject.GetComponent<MoveCamera>().moveMultiplier;
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag != "Mouse")
                {
                    Destroy(hit.transform.gameObject);
                }
            }
        } else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "Arena")
                {
                    state = STATE_PLACE;
                    if (objType == 0)
                    {
                        curObj = barrier;
                    }
                    else
                    {
                        curObj = area;
                    }
                    float mult = -ray.origin.y / ray.direction.y;
                    Vector3 coords = new Vector3(ray.origin.x + ray.direction.x * mult, 0f, ray.origin.z + ray.direction.z * mult);
                    curObj = Instantiate(curObj, coords, Quaternion.identity);
                    objPos = coords;
                }
                else
                {
                    state = STATE_MOVE;
                    curObj = hit.transform.gameObject;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            state = STATE_DEFAULT;
        }
        Vector2 moveCoords = new Vector2(Input.mousePosition.x - mouseCoords.x, Input.mousePosition.y - mouseCoords.y);
        if (state == STATE_MOVE)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float mult = -ray.origin.y / ray.direction.y;
            Vector3 coords = new Vector3(ray.origin.x + ray.direction.x * mult, 0f, ray.origin.z + ray.direction.z * mult);
            curObj.transform.position = coords;
        } else if (state == STATE_PLACE)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float mult = -ray.origin.y / ray.direction.y;
            Vector3 coords = new Vector3(ray.origin.x + ray.direction.x * mult, 0f, ray.origin.z + ray.direction.z * mult);
            Vector3 distVec = coords - objPos;
            if (objType == 0)
            {
                curObj.transform.position = objPos + new Vector3(distVec.x / 2, 0f, distVec.z / 2);
                curObj.transform.localScale = new Vector3(System.Math.Abs(distVec.x) / 10, curObj.transform.localScale.y, System.Math.Abs(distVec.z) / 10);
            } else
            {
                float newScale = (float)System.Math.Sqrt(distVec.x * distVec.x + distVec.z * distVec.z) * 2;
                curObj.transform.localScale = new Vector3(newScale, curObj.transform.localScale.y, newScale);
            }
        }



        mouseCoords = Input.mousePosition;
    }

    public void ChangeObject(int newObjInd)
    {
        objType = newObjInd;
    }
}
