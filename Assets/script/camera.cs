using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform lootAt;
    [SerializeField] float height;
    [SerializeField] LayerMask layerCollision;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {       
        Quaternion rootCamDir = Quaternion.Euler(target.transform.rotation.eulerAngles.x, target.transform.rotation.eulerAngles.y, target.transform.rotation.eulerAngles.z); //seguir a rotaçao do sonic
        transform.rotation = Quaternion.Lerp(transform.rotation, rootCamDir, 8 * Time.deltaTime);

        PlaceOfCam();
    }

    void PlaceOfCam()
    {      
        transform.position = Vector3.Lerp(transform.position, target.position, 5 * Time.deltaTime);
    }
    
}
