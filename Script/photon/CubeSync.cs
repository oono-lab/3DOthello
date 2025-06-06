using UnityEngine;
using Photon.Pun;

public class CubeSync : MonoBehaviourPun
{
    [PunRPC]
    public void InitCube(string parentName, Vector3 position)
    {
        Transform parent = GameObject.Find(parentName)?.transform;
        if (parent != null)
        {
            transform.SetParent(parent, true);
        }

        gameObject.SetActive(true);
        transform.position = position;
    }
}
