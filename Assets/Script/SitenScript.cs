using UnityEngine;
using System.Collections;

public class SitenScript : MonoBehaviour
{

    public GameObject Camera;
    public GameObject robo;
    public GameObject denkiman;

    void Start()
    {

    }


    void Update()
    {

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            Camera.transform.parent = null;
            denkiman.transform.parent = Camera.transform;

        }
    }
}
