using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CubeCtrl : MonoBehaviourPunCallbacks
{
    private Button destroyButton;
    private Button collisionButton;

    private PhotonView pv;
    private PhotonView pv_GM;

    internal static List<PhotonView> fpvs = new List<PhotonView>();
    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log($"start: {GetComponent<PhotonView>().IsMine}");
        // this.gameObject.GetComponentsInChildren<PhotonView>(true, fpvs);
        // Debug.Log($"fpvs : {fpvs}" );
        // Debug.Log($"fpvs[0] : {fpvs[0]}");
        // Debug.Log($"fpvs[0].isMine : {fpvs[0].IsMine}");

        pv = GetComponent<PhotonView>();
        pv_GM = GameObject.Find("GameManager").GetComponent<PhotonView>();

        destroyButton = GameObject.Find("DestroyButton").GetComponent<Button>();
        destroyButton.onClick.AddListener(()=> pv.RPC("DestroyCube", RpcTarget.MasterClient));
        
        // StartCoroutine(DestroyCube_Co());
        //JHY_OctAndRingTest
        collisionButton = GameObject.Find("CollisionButton").GetComponent<Button>();
        collisionButton.onClick.AddListener(()=> CollisionEnter(new Collision()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void DestroyCube()
    {
        // Debug.Log($"destroy버튼 ismine: {GetComponent<PhotonView>().IsMine}");
        // Debug.Log($"Destroy RPC 호출");
        // Debug.Log($"masterClient: {PhotonNetwork.MasterClient}");

        PhotonNetwork.Destroy(this.gameObject);
    }

    IEnumerator DestroyCube_Co()
    {
        yield return new WaitForSeconds(5.0f);

        Debug.Log($"코루틴 ismine: {GetComponent<PhotonView>().IsMine}");
        Debug.Log($"masterClient: {PhotonNetwork.MasterClient}");

        PhotonNetwork.Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision coll)
    {
        // Debug.Log($"콜리전 ismine: {GetComponent<PhotonView>().IsMine}");
        Debug.Log($"콜리전 호출됨");
        Debug.Log($"masterClient: {PhotonNetwork.MasterClient}");

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Debug.Log($"{PhotonNetwork.LocalPlayer} is [MASTER], No RPC");
            DestroyCube();
        }
        else
        {
            Debug.Log($"{PhotonNetwork.LocalPlayer}, Yes RPC");
            pv.RPC("DestroyCube", RpcTarget.MasterClient);
        }
    }

    #region JHY_OctAndRingTest
    public void CollisionEnter(Collision coll)
    {
        pv_GM.GetComponent<GameManager1>().MakeRing(this.gameObject);
    }
#endregion
}
