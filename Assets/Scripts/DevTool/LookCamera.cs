using UnityEngine;

public class LookCamera : MonoBehaviour
{
    public float offset = 2.0f; 
    void LateUpdate()
    {
        if (Camera.main == null) return;

        Transform camTrans = Camera.main.transform;

        transform.LookAt(transform.position + camTrans.rotation * Vector3.forward, camTrans.rotation * Vector3.up);

      
        transform.position = camTrans.position + camTrans.forward * offset;
    }
}