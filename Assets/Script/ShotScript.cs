using UnityEngine;
using System.Collections;

public class ShotScript : MonoBehaviour
{

    // bullet prefab
    public GameObject bullet;

    // 弾丸発射点
    public Transform muzzle;

    // 弾丸の速度
    public float speed = 100;

	public GameObject Base14;

	public GameObject Base13;

	public GameObject Base12;

	public GameObject Base11;

	public GameObject Base10;

	public GameObject Base9;

	public GameObject Base8;

	public GameObject Base7;

	public GameObject Base6;

	public GameObject Base5;

	public GameObject Base4;

	public GameObject Base3;

	public GameObject Base2;

	public GameObject Base1;

	public GameObject Base0;

	public GameObject PowerBar;
    
	public int CreateNumbers = 14;
    
	public float Space = 0.5f;

	public Vector3 CreatePositon = new Vector3(-89, -41, -172.1834f);

	public GameObject CloneObj;

	// Use this for initialization
	void Start()
	{
		
	}

    // Update is called once per frame
    void Update()
    {      
        if (Input.GetKeyDown(KeyCode.Z))
		{
			if (CreateNumbers == 14)
			{
				GameObject bullets = Instantiate(bullet) as GameObject;

				Vector3 force = Input.mousePosition;
				// Rigidbodyに力を加えて発射
				bullets.GetComponent<Rigidbody>().AddForce(force);
				// 弾丸の位置を調整
				bullets.transform.position = muzzle.position;

				Base14.gameObject.SetActive(false);

				Base13.gameObject.SetActive(true);

				CreateNumbers--;
			}

			if (CreateNumbers == 13)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;

                Base13.gameObject.SetActive(false);

                Base12.gameObject.SetActive(true);

				CreateNumbers--;
            }

			if (CreateNumbers == 12)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;

                Base14.gameObject.SetActive(false);

                Base13.gameObject.SetActive(true);

				CreateNumbers--;
            }

			if (CreateNumbers == 11)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;

                Base13.gameObject.SetActive(false);

                Base12.gameObject.SetActive(true);

                CreateNumbers--;
            }

			if (CreateNumbers == 10)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;

                Base12.gameObject.SetActive(false);

                Base11.gameObject.SetActive(true);

                CreateNumbers--;
            }

			if (CreateNumbers == 9)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;

                Base11.gameObject.SetActive(false);

                Base10.gameObject.SetActive(true);

                CreateNumbers--;
            }
            
			if (CreateNumbers == 8)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;

                Base10.gameObject.SetActive(false);

                Base9.gameObject.SetActive(true);

                CreateNumbers--;
            }

			if (CreateNumbers == 7)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;

                Base9.gameObject.SetActive(false);

                Base8.gameObject.SetActive(true);

                CreateNumbers--;
            }

			if (CreateNumbers == 6)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;

                Base8.gameObject.SetActive(false);

                Base7.gameObject.SetActive(true);

                CreateNumbers--;
            }

			if (CreateNumbers == 5)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;

                Base7.gameObject.SetActive(false);

                Base6.gameObject.SetActive(true);

                CreateNumbers--;
            }
            
			if (CreateNumbers == 4)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;

                Base6.gameObject.SetActive(false);

                Base5.gameObject.SetActive(true);

                CreateNumbers--;
            }

			if (CreateNumbers == 3)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;

                Base5.gameObject.SetActive(false);

                Base4.gameObject.SetActive(true);

                CreateNumbers--;
            }
            
			if (CreateNumbers == 2)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;
                
                Base3.gameObject.SetActive(false);

                Base2.gameObject.SetActive(true);

                CreateNumbers--;
            }

			if (CreateNumbers == 1)
            {
                GameObject bullets = Instantiate(bullet) as GameObject;

                Vector3 force = Input.mousePosition;
                // Rigidbodyに力を加えて発射
                bullets.GetComponent<Rigidbody>().AddForce(force);
                // 弾丸の位置を調整
                bullets.transform.position = muzzle.position;

                Base2.gameObject.SetActive(false);

                Base1.gameObject.SetActive(true);
                
                CreateNumbers--;
            }

			if(CreateNumbers <= 0)
			{
				Base1.gameObject.SetActive(false);

                Base0.gameObject.SetActive(true);

				GetComponent<ShotScript>().enabled = false;
			}
         
        }
    }


}
