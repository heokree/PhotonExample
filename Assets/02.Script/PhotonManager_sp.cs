using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonManager_sp : MonoBehaviourPunCallbacks
{
#region 전역변수 선언
    // 게임 버전
    [Header("Game Version")]
    [SerializeField]
    private readonly string gameVersion = "v1.0";

    // 유저 ID - 향후 Oculus Authentification 연동 후 ID 지정 로직 추가 필요
    private string userID = "User";

    // UI 요소
    [Header("UI 입력필드")]
    // public TMP_InputField enteredRoomCode;
    public TMP_Text serverConnectionState;
    public TMP_Text currentPlace;
    public TMP_Text userInfo;

    // 룸 코드 길이
    [Header("Room Code 길이")]
    public int codeLength;

    // 룸 당 최대 플레이어 수, 0이면 인원제한없음(byte 타입 변수)
    [Header("최대 플레이어 수")]
    public byte maxPlayers = 20;
#endregion

#region Unity_Methods
    private void Awake()
    {
        userID += Random.Range(0,100).ToString();
        ConnectPhotonServer();
    }
    
    private void Start()
    {
        StartCoroutine(PhotonConnectionState());
    }
#endregion

#region Photon_Event_Methods
    public override void OnConnectedToMaster()
    {
        currentPlace.text = "Current Place: OnConnectedToMaster";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        currentPlace.text = "Current Place: OnJoinedLobby";
    }

    public override void OnCreatedRoom()
    {
        currentPlace.text = "Current Place: OnCreatedRoom";
    }

    public override void OnJoinedRoom()
    {
        currentPlace.text = $"Current Place: OnJoinedRoom, {PhotonNetwork.CurrentRoom.Name}";

        // 씬 전환 필요
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     PhotonNetwork.LoadLevel("Map_Sea");
        // }
    }

    // 랜덤 룸 조인 실패 시, 룸 생성
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        currentPlace.text = "Current Place: OnJoinRandomFailed";
        OnBtn_CreateRoom();
    }
#endregion

#region Custom_Methods
    // 포톤서버 연결 및 게임 정보 지정
    private void ConnectPhotonServer()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.GameVersion   = this.gameVersion;
        PhotonNetwork.NickName      = this.userID;

        PhotonNetwork.ConnectUsingSettings();
    }

    // 0.5초에 한번씩 서버 연결상태 확인 후 표시 (디버깅 용도, 향후 파기가능)
    IEnumerator PhotonConnectionState()
    {
        while (true)
        {
            serverConnectionState.text  = "Network State: " + PhotonNetwork.NetworkClientState.ToString();
            userInfo.text = PhotonNetwork.LocalPlayer.NickName;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void OnBtn_RandomJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // 룸 만들기 버튼 누르면 룸 생성.
    public void OnBtn_CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = this.maxPlayers;
        
        // Try, Catch로 예외처리
        try
        {
            PhotonNetwork.CreateRoom(CreateRoomCode(), roomOptions);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }

    // public void OnBtn_JoinRoom()
    // {
    //     try
    //     {
    //         PhotonNetwork.JoinRoom(enteredRoomCode.text);
    //     }
    //     catch(System.Exception e)
    //     {
    //         Debug.Log(e);
    //     }
    // }

    // 무작위 룸코드 생성 - 짝수면 알파벳 문자 반반, 홀수면 숫자 1개 더
    private string CreateRoomCode()
    {
        string code = "";
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        for (int i = 0; i < codeLength/2; i++)
        {
            int idx = Random.Range(0, alphabet.Length);
            code += alphabet[idx];
        }

        if (codeLength % 2 == 0)
        {
            for (int i = 0; i < codeLength/2; i++)
            {
                int randomNumber = Random.Range(0, 9);
                code += randomNumber.ToString();
            }
        }
        else 
        {
            for (int i = 0; i < (codeLength+1)/2; i++)
            {
                int randomNumber = Random.Range(0, 9);
                code += randomNumber.ToString();
            }

        }
        return code;
    }
#endregion
}
