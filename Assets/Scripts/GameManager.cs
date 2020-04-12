using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(player.name, new Vector3(500, 50, 500), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
