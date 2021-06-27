using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManager1 : MonoBehaviourPunCallbacks
{
    private Button createButton;
    
    private PhotonView pv;

    private GameObject cube;
    public bool CubeExist { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        
        createButton = GameObject.Find("CreateButton").GetComponent<Button>();
    }

    public override void OnJoinedRoom()
    {
        // if (PhotonNetwork.LocalPlayer.IsMasterClient)
        // {
        //     Debug.Log($"{PhotonNetwork.LocalPlayer} is [MASTER], No RPC");
        //     createButton.onClick.AddListener(()=> CreateCube());
        // }
        // else
        // {
            Debug.Log($"{PhotonNetwork.LocalPlayer}, Yes RPC");
            createButton.onClick.AddListener(()=> pv.RPC("CreateCube", RpcTarget.All));
        // }


    }

    [PunRPC]
    public void CreateCube()
    {
        PhotonNetwork.InstantiateRoomObject("Cube", new Vector3(2, 1, 0.2f), Quaternion.identity);
        CubeExist = true;
        // PhotonNetwork.Instantiate("Cube", new Vector3(2, 1, 0.2f), Quaternion.identity);
    }

#region JHY_OctAndRingTest
    public void MakeRing(GameObject cube)
    {
        Debug.Log("MakeRing Called!!!");
    
        if (CubeExist)
        {
            Debug.Log("[Cube] 있음. Destroy Cube Start!");
            
            cube.GetComponent<PhotonView>().RPC("DestroyCube", RpcTarget.MasterClient);

            Debug.Log("[Cube] Destroy Done. Create Start!");
            
            pv.RPC("CreateCube", RpcTarget.MasterClient);
        }
    }
#endregion
}
