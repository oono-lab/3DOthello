using Photon.Pun;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Xml.Linq;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections;

public class OnlineOthelloScript : MonoBehaviourPun
{
    public GameObject OthelloSprite;
    public GameObject Cube;
    public GameObject CubeCliant;
    public GameObject GameOverPanel;
    public GameObject EscapePanel;
    public GameObject Board_surface;
    public GameObject settingsPanel;
    public TextMeshProUGUI BlackNumText;
    public TextMeshProUGUI WhiteNumText;
    public GameObject WinTextObj;
    public GameObject[] UpDownButton;
    public UnityEngine.UI.Image BlackImage;
    public UnityEngine.UI.Image WhiteImage;
    public AudioSource StonePutAudio;
    public AudioSource NotPutAudio;
    public AudioSource CustumPutAudio;
    public AudioSource GameOverAudio;
    public AudioSource CubeMoveAudio;
    public UnityEngine.UI.Button button;
    private GameObject CubeCopy;
    private GameObject CubeCopyCliant;
    private Vector3 originalPosition;
    public int CustumSelectBotton = 0;
    public OthelloScript OthelloScript;
    private int FIELD_SIZE_MAX= OthelloScript.FIELD_SIZE - 1;
    private int DIRECTION_MAXHard = 26;
    private int DIRECTION_MAXNotHard = 6;
    private int MEDIAN_UP = OthelloScript.FIELD_SIZE / 2;
    private int MEDIAN_DOWN = OthelloScript.FIELD_SIZE / 2 - 1;
    private int Cube_Position_X;
    private int Cube_Position_Y;
    private int Cube_Position_Z;
    private int Cube_Position_X_Cliant;
    private int Cube_Position_Y_Cliant;
    private int Cube_Position_Z_Cliant;
    private float moveDelay = 0.15f;
    private float lastMoveTime = 0.0f;
    private bool ResultHantei = false;
    private bool BlackCheckFlag = false;
    private bool WhiteCheckFlag = true;
    private bool isChoicPoint = false;
    [SerializeField] private GameObject ObjectRotationSystem;
    private ObjectRotation ObjectRotationBool;
    private int[] RightLeftUpDownMaster = new int[] { 0, 0, 0, 0 };
    private int[] RightLeftUpDownCliant = new int[] { 0, 0, 0, 0 };
    private List<(int, int, int)> _infoListNotHard = new List<(int, int, int)>();
    private List<(int, int, int)> _infoListHard = new List<(int, int, int)>();
    public enum SpriteState
    {
        None,
        Black,
        White,
        Wall,
        NoneChoice
    }
    public SpriteState[] spriteStates = new SpriteState[4]
    {
    SpriteState.Black,
    SpriteState.White,
    SpriteState.None,
    SpriteState.Wall,
    };
    private SpriteState PlayerTurn = SpriteState.Black;
    private static SpriteState[,,] FieldState = new SpriteState[OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE];
    private static SpriteState[,,] FieldStateCustum = new SpriteState[OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE];
    private SpriteState[,,] FieldStateStart = new SpriteState[OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE];
    private SpriteState[,,] FieldStateNone = new SpriteState[OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE];
    private OnlineSpriteScript[,,] FieldSpriteState = new OnlineSpriteScript[OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE];
    SpriteState[] SerializeFieldState(SpriteState[,,] fieldState)
    {
        SpriteState[] serializedData = new SpriteState[OthelloScript.FIELD_SIZE * OthelloScript.FIELD_SIZE * OthelloScript.FIELD_SIZE];
        for (int x = 0; x < OthelloScript.FIELD_SIZE; x++)
        {
            for (int y = 0; y < OthelloScript.FIELD_SIZE; y++)
            {
                for (int z = 0; z < OthelloScript.FIELD_SIZE; z++)
                {
                    serializedData[x * (OthelloScript.FIELD_SIZE * OthelloScript.FIELD_SIZE) + y * OthelloScript.FIELD_SIZE + z] = fieldState[x, y,z]; 
                }
                
            }
        }
        return serializedData;
    }



    SpriteState[,,] DeserializeFieldState(SpriteState[] serializedData)
    {
        SpriteState[,,] fieldState = new SpriteState[OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE];
        for (int x = 0; x < OthelloScript.FIELD_SIZE; x++)
        {
            for (int y = 0; y < OthelloScript.FIELD_SIZE; y++)
            {
                for (int z = 0; z < OthelloScript.FIELD_SIZE; z++)
                {
                    fieldState[x, y,z] = serializedData[x * (OthelloScript.FIELD_SIZE * OthelloScript.FIELD_SIZE) + y * OthelloScript.FIELD_SIZE + z];
                }
                
            }
        }
        return fieldState;
    }

    void Start()
    {
        
        originalPosition = Cube.transform.position;
        settingsPanel.SetActive(false);
        //FIELD_SIZE_MAX = OthelloScript.FIELD_SIZE - 1;
        //MEDIAN_UP = OthelloScript.FIELD_SIZE / 2;
        //MEDIAN_DOWN = OthelloScript.FIELD_SIZE / 2 - 1;
        Cube_Position_X = FIELD_SIZE_MAX;
        Cube_Position_Y = FIELD_SIZE_MAX;
        Cube_Position_Z = FIELD_SIZE_MAX;
        Cube_Position_X_Cliant = FIELD_SIZE_MAX;
        Cube_Position_Y_Cliant = FIELD_SIZE_MAX;
        Cube_Position_Z_Cliant = FIELD_SIZE_MAX;
        ObjectRotationBool = ObjectRotationSystem.GetComponent<ObjectRotation>();
        Debug.Log($"Sizes: {FieldState.GetLength(0)}, {FieldState.GetLength(1)}, {FieldState.GetLength(2)}");
        Debug.Log($"Indexes: {MEDIAN_DOWN}, {MEDIAN_UP}, {MEDIAN_DOWN}");
        Debug.Log($"OthelloScript.FIELD_SIZE: {OthelloScript.FIELD_SIZE}");
        FieldStateStart[MEDIAN_DOWN, MEDIAN_UP, MEDIAN_DOWN] = SpriteState.Black;
        FieldStateStart[MEDIAN_DOWN, MEDIAN_DOWN, MEDIAN_DOWN] = SpriteState.White;
        FieldStateStart[MEDIAN_UP, MEDIAN_DOWN, MEDIAN_UP] = SpriteState.White;
        FieldStateStart[MEDIAN_UP, MEDIAN_UP, MEDIAN_UP] = SpriteState.Black;
        FieldStateStart[MEDIAN_DOWN, MEDIAN_DOWN, MEDIAN_UP] = SpriteState.Black;
        FieldStateStart[MEDIAN_DOWN, MEDIAN_UP, MEDIAN_UP] = SpriteState.White;
        FieldStateStart[MEDIAN_UP, MEDIAN_DOWN, MEDIAN_DOWN] = SpriteState.Black;
        FieldStateStart[MEDIAN_UP, MEDIAN_UP, MEDIAN_DOWN] = SpriteState.White;

        for (int x = 0; x < OthelloScript.FIELD_SIZE; x++)
        {
            for (int y = 0; y < OthelloScript.FIELD_SIZE; y++)
            {
                for (int z = 0; z < OthelloScript.FIELD_SIZE; z++)
                {
                    Vector3 localPosition = new Vector3(
                            2.0f * (x - OthelloScript.positional_complement),
                            2.0f * (y - OthelloScript.positional_complement),
                            2.0f * (z - OthelloScript.positional_complement)
                        );
                    Cube.transform.position = localPosition;
                    var sprite = Instantiate(OthelloSprite, localPosition, Quaternion.Euler(0, 0, 0));
                    sprite.transform.SetParent(Board_surface.transform, true);


                    FieldStateNone[x, y, z] = SpriteState.None;
                    if (!(x == MEDIAN_UP || x == MEDIAN_DOWN) && !(y == MEDIAN_UP || y == MEDIAN_DOWN) && !(z == MEDIAN_UP || z == MEDIAN_DOWN)) FieldState[x, y, z] = SpriteState.None;

                    FieldSpriteState[x, y, z] = sprite.GetComponent<OnlineSpriteScript>();
                    if (OthelloScript.isCustum || !PhotonNetwork.InRoom)
                    {
                        FieldState[x, y,z] = FieldStateStart[x, y,z];
                        FieldStateCustum[x, y,z] = FieldState[x, y,z];

                    }
                    else FieldState[x, y,z] = FieldStateCustum[x, y,z];
                    FieldSpriteState[x, y, z].SetState(FieldState[x, y, z]);
                    
                }

            }


        }
        if (OthelloScript.isCustum)
        {
            CubeCopy = Instantiate(Cube, originalPosition, Quaternion.identity);
            CubeCopy.transform.SetParent(Board_surface.transform, true);
            CubeCopy.SetActive(true);
            CubeCopy.transform.position = Cube.transform.position;
            

        }else if (!PhotonNetwork.InRoom)
        {
            CubeCopy = Instantiate(Cube, originalPosition, Quaternion.identity);
            CubeCopy.transform.SetParent(Board_surface.transform, true);
            CubeCopy.SetActive(true);
            CubeCopy.transform.position = Cube.transform.position;
            button.onClick.Invoke();
        }
        else
        {   

            CubeCopy = Instantiate(Cube, originalPosition, Quaternion.identity);
            CubeCopy.transform.SetParent(Board_surface.transform, true);
            CubeCopy.SetActive(true);
            CubeCopy.transform.position = Cube.transform.position;

            CubeCopyCliant =Instantiate(CubeCliant, originalPosition, Quaternion.identity);
            CubeCopyCliant.transform.SetParent(Board_surface.transform, true);
            CubeCopyCliant.SetActive(true);
            CubeCopyCliant.transform.position = Cube.transform.position;

        }
        

    }
    void Update()
    {

        if (Time.time - lastMoveTime >= moveDelay)
        {
            if (OthelloScript.isCustum)
            {
                InputControll(RightLeftUpDownMaster, Cube_Position_X, Cube_Position_Y, Cube_Position_Z, true);
                
            }
            else
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    InputControll(RightLeftUpDownMaster, Cube_Position_X, Cube_Position_Y, Cube_Position_Z, true);
                    //CubeCopy.transform.position = Cube.transform.position;
                }
                else if (!PhotonNetwork.IsMasterClient)
                {
                    InputControll(RightLeftUpDownCliant, Cube_Position_X_Cliant, Cube_Position_Y_Cliant, Cube_Position_Z_Cliant, false);
                   //CubeCopyCliant.transform.position = CubeCliant.transform.position;
                }
            }
        }


        if (!ResultHantei)
        {
            if (OthelloScript.isCustum) CustumMode();
            else
            {
                if (OthelloScript.isHard)
                {
                    if (PlayerTurn == SpriteState.White && PhotonNetwork.IsMasterClient) photonView.RPC("GameHardMode", RpcTarget.All, Cube_Position_X, Cube_Position_Y, Cube_Position_Z);
                    else if (PlayerTurn == SpriteState.Black && !PhotonNetwork.IsMasterClient) photonView.RPC("GameHardMode", RpcTarget.All, Cube_Position_X_Cliant, Cube_Position_Y_Cliant, Cube_Position_Z_Cliant);

                }
                else {
                    if (PlayerTurn == SpriteState.Black && PhotonNetwork.IsMasterClient) photonView.RPC("GameNotHardMode", RpcTarget.All, Cube_Position_X, Cube_Position_Y, Cube_Position_Z);
                    else if (PlayerTurn == SpriteState.White && !PhotonNetwork.IsMasterClient) photonView.RPC("GameNotHardMode", RpcTarget.All, Cube_Position_X_Cliant, Cube_Position_Y_Cliant, Cube_Position_Z_Cliant);

                }
            }

        }


    }


    private void InputControll(int[] List,int PosX,int PosY,int PosZ,bool PlayerHnatei)
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            List[0] = 1;
            CubeControllBool(List, PosX, PosY, PosZ, PlayerHnatei);
            List[0] = 0;

        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            List[1] = 1;
            CubeControllBool(List, PosX, PosY, PosZ, PlayerHnatei);
            List[1] = 0;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            List[2] = 1;
            CubeControllBool(List, PosX, PosY, PosZ, PlayerHnatei);
            List[2] = 0;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            List[3] = 1;
            CubeControllBool(List, PosX, PosY, PosZ, PlayerHnatei);
            List[3] = 0;
        }
        
    }
    [PunRPC]
    private void GameHardMode(int Cube_Position_X_This, int Cube_Position_Y_This,int Cube_Position_Z_This)
    {
        var turnCheck = false;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            for (int i = 0; i < DIRECTION_MAXHard; i++)
            {
                if (TurnCheckHard(i, Cube_Position_X_This, Cube_Position_Y_This, Cube_Position_Z_This)) turnCheck = true;
            }
            #region　プレイヤーが置ける石として表示するマスを初期化
            for (int x = 0; x < OthelloScript.FIELD_SIZE; x++)
            {
                for (int y = 0; y < OthelloScript.FIELD_SIZE; y++)
                {
                    for (int z = 0; z < OthelloScript.FIELD_SIZE; z++)
                    {
                        if (FieldState[x, y, z] == SpriteState.NoneChoice) FieldState[x, y, z] = SpriteState.None;
                    }

                }
            }
            #endregion
            if (turnCheck && (FieldState[Cube_Position_X_This, Cube_Position_Y_This, Cube_Position_Z_This] == SpriteState.None || FieldState[Cube_Position_X_This, Cube_Position_Y_This, Cube_Position_Z_This] == SpriteState.NoneChoice))
            {
                StonePutAudio.Play();
                foreach (var info in _infoListHard)
                {
                    var position_X = info.Item1;
                    var position_Y = info.Item2;
                    var position_Z = info.Item3;
                    FieldState[position_X, position_Y, position_Z] = PlayerTurn;

                }
                FieldState[Cube_Position_X_This, Cube_Position_Y_This, Cube_Position_Z_This] = PlayerTurn;
                ChangePlayerTurn();
                isChoicPoint = true;
                

            }
            else
            {
                NotPutAudio.Play();
            }



        }
        _infoListHard = new List<(int, int, int)>();

        turnCheck = false;
        for (int x = 0; x < OthelloScript.FIELD_SIZE; x++)
        {
            for (int y = 0; y < OthelloScript.FIELD_SIZE; y++)
            {
                for (int z = 0; z < OthelloScript.FIELD_SIZE; z++)
                {
                    for (int i = 0; i < DIRECTION_MAXHard; i++)
                    {
                        if (TurnCheckHard(i, x, y, z) && (FieldState[x, y, z] == SpriteState.None || FieldState[x, y, z] == SpriteState.NoneChoice))
                        {
                            FieldState[x, y, z] = SpriteState.NoneChoice;
                            if (PlayerTurn == SpriteState.Black)
                            {
                                turnCheck = true;
                                BlackCheckFlag = true;
                                if (!WhiteCheckFlag) WhiteCheckFlag = true;
                                break;
                            }
                            else if (PlayerTurn == SpriteState.White)
                            {
                                turnCheck = true;
                                WhiteCheckFlag = true;
                                if (!BlackCheckFlag) BlackCheckFlag = true;
                                break;
                            }

                        }


                    }
                }
            }
        }
        if (!turnCheck)
        {
            if (PlayerTurn == SpriteState.Black) BlackCheckFlag = false;
            else if (PlayerTurn == SpriteState.White) WhiteCheckFlag = false;
            ChangePlayerTurn();
        }
        _infoListHard = new List<(int, int, int)>();
        UpdateBoard();

    }
    [PunRPC]
    private void GameNotHardMode(int Cube_Position_X_This, int Cube_Position_Y_This, int Cube_Position_Z_This)
    {
        var turnCheck = false;

        if (Input.GetKeyDown(KeyCode.Return))
        {   
            for (int i = 0; i < DIRECTION_MAXNotHard; i++)
            {
                if (TurnCheckNotHard(i, Cube_Position_X_This, Cube_Position_Y_This, Cube_Position_Z_This)) turnCheck = true;
            }
            Debug.Log(turnCheck);
            #region　プレイヤーが置ける石として表示するマスを初期化
            for (int x = 0; x < OthelloScript.FIELD_SIZE; x++)
            {
                for (int y = 0; y < OthelloScript.FIELD_SIZE; y++)
                {
                    for (int z = 0; z < OthelloScript.FIELD_SIZE; z++)
                    {
                        if (FieldState[x, y, z] == SpriteState.NoneChoice) FieldState[x, y, z] = SpriteState.None;
                    }

                }
            }
            #endregion
            if (turnCheck && (FieldState[Cube_Position_X_This, Cube_Position_Y_This, Cube_Position_Z_This] == SpriteState.None || FieldState[Cube_Position_X_This, Cube_Position_Y_This, Cube_Position_Z_This] == SpriteState.NoneChoice))
            {
                StonePutAudio.Play();
                foreach (var info in _infoListNotHard)
                {
                    var position_X = info.Item1;
                    var position_Y = info.Item2;
                    var position_Z = info.Item3;
                    FieldState[position_X, position_Y, position_Z] = PlayerTurn;

                }
                FieldState[Cube_Position_X_This, Cube_Position_Y_This, Cube_Position_Z_This] = PlayerTurn;
                ChangePlayerTurn();
                isChoicPoint = true;

            }
            else
            {
                NotPutAudio.Play();
            }



        }
        _infoListNotHard = new List<(int, int, int)>();

        turnCheck = false;
        for (int x = 0; x < OthelloScript.FIELD_SIZE; x++)
        {
            for (int y = 0; y < OthelloScript.FIELD_SIZE; y++)
            {
                for (int z = 0; z < OthelloScript.FIELD_SIZE; z++)
                {
                    for (int i = 0; i < DIRECTION_MAXNotHard; i++)
                    {
                        if (TurnCheckNotHard(i, x, y, z) && (FieldState[x, y, z] == SpriteState.None || FieldState[x, y, z] == SpriteState.NoneChoice))
                        {
                            

                            if (PlayerTurn == SpriteState.Black)
                            {   if(PhotonNetwork.IsMasterClient) FieldState[x, y, z] = SpriteState.NoneChoice;
                                turnCheck = true;
                                BlackCheckFlag = true;
                                if (!WhiteCheckFlag) WhiteCheckFlag = true;
                                break;
                            }
                            else if (PlayerTurn == SpriteState.White)
                            {
                                if (!PhotonNetwork.IsMasterClient) FieldState[x, y, z] = SpriteState.NoneChoice;
                                turnCheck = true;
                                WhiteCheckFlag = true;
                                if (!BlackCheckFlag) BlackCheckFlag = true;
                                break;
                            }

                        }


                    }
                }
            }
        }
        if (!turnCheck)
        {
            if (PlayerTurn == SpriteState.Black) BlackCheckFlag = false;
            else if (PlayerTurn == SpriteState.White) WhiteCheckFlag = false;
            ChangePlayerTurn();
        }
        _infoListNotHard = new List<(int, int, int)>();
        UpdateBoard();

    }
    #region　カスタムモードの処理部分
    private void CustumMode()
    {
      
        
            if (Input.GetKeyDown(KeyCode.Return))
            {
                FieldState[Cube_Position_X, Cube_Position_Y, Cube_Position_Z] = spriteStates[CustumSelectBotton];
                FieldStateCustum[Cube_Position_X, Cube_Position_Y, Cube_Position_Z] = FieldState[Cube_Position_X, Cube_Position_Y, Cube_Position_Z];
                StonePutAudio.Play();
            }
            
        
        ShowSpriteBoard();
    }
    #endregion
    private void UpdateBoard()
    {
        var WhiteNum = default(int);
        var BlackNum = default(int);
        for (int x = 0; x < OthelloScript.FIELD_SIZE; x++)
        {
            for (int y = 0; y < OthelloScript.FIELD_SIZE; y++)
            {
                for (int z = 0; z < OthelloScript.FIELD_SIZE; z++)
                {
                    if (FieldState[x, y, z] == SpriteState.White) WhiteNum++;
                    else if (FieldState[x, y, z] == SpriteState.Black) BlackNum++;
                }

            }


        }
        if (Input.GetKeyDown(KeyCode.Return) && isChoicPoint) SendFieldStateToParticipantsFieldState();


        ShowSpriteBoard();
        


        #region　黒と白どちらのターンなのかを表示。現在の黒石と白石の数を表示。
        if (PlayerTurn == SpriteState.White)
        {
            BlackImage.enabled = false;
            WhiteImage.enabled = true;
        }
        else if (PlayerTurn == SpriteState.Black)
        {
            BlackImage.enabled = true;
            WhiteImage.enabled = false;
        }


        BlackNumText.text = BlackNum.ToString();
        WhiteNumText.text = WhiteNum.ToString();
        #endregion
        if (WhiteNum + BlackNum == OthelloScript.FIELD_SIZE * OthelloScript.FIELD_SIZE * OthelloScript.FIELD_SIZE || !WhiteCheckFlag && !BlackCheckFlag) GameOver(WhiteNum, BlackNum);
    }
    #region 対戦終了時のUI表示
    private void GameOver(int WhiteNum, int BlackNum)
    {
        GameObject Black = WinTextObj.transform.Find("Black").gameObject;
        GameObject White = WinTextObj.transform.Find("White").gameObject;
        GameObject Hikiwake = WinTextObj.transform.Find("Hikiwake").gameObject;
        WinTextObj.SetActive(true);
        GameOverPanel.SetActive(true);
        EscapePanel.SetActive(false);
        GameOverAudio.Play();
        ResultHantei = true;
        BlackImage.enabled = false;
        WhiteImage.enabled = false;
        if (WhiteNum == BlackNum)
        {
            Hikiwake.SetActive(true);
            Black.SetActive(false);
            White.SetActive(false);
        }
        else if (WhiteNum > BlackNum)
        {
            Hikiwake.SetActive(false);
            Black.SetActive(false);
            White.SetActive(true);
        }
        else
        {

            Hikiwake.SetActive(false);
            Black.SetActive(true);
            White.SetActive(false);
        }



    }
    #endregion

    #region 斜め方向なしの関数。
    private bool TurnCheckNotHard(int Direction, int field_size_x, int field_size_y, int field_size_z)
    {
        var turnCheck = false;
        var position_x = field_size_x;
        var position_y = field_size_y;
        var position_z = field_size_z;
        var OpponentPlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.White : SpriteState.Black;
        var infoList = new List<(int, int, int)>();

        while (0 <= position_x && FIELD_SIZE_MAX >= position_x && 0 <= position_y && FIELD_SIZE_MAX >= position_y && 0 <= position_z && FIELD_SIZE_MAX >= position_z)
        {
            switch (Direction)
            {
                case 0:
                    if (position_x == 0) return turnCheck;
                    position_x--;
                    break;
                case 1:
                    if (position_x == FIELD_SIZE_MAX) return turnCheck;
                    position_x++;
                    break;
                case 2:
                    if (position_y == 0) return turnCheck;
                    position_y--;
                    break;
                case 3:
                    if (position_y == FIELD_SIZE_MAX) return turnCheck;
                    position_y++;
                    break;
                case 4:
                    if (position_z == FIELD_SIZE_MAX) return turnCheck;
                    position_z++;
                    break;
                case 5:
                    if (position_z == 0) return turnCheck;
                    position_z--;
                    break;



            }


            if (FieldState[position_x, position_y, position_z] == OpponentPlayerTurn)
            {
                infoList.Add((position_x, position_y, position_z));
            }
            if (infoList.Count == 0 && FieldState[position_x, position_y, position_z] == PlayerTurn || FieldState[position_x, position_y, position_z] == SpriteState.None || FieldState[position_x, position_y, position_z] == SpriteState.Wall || FieldState[position_x, position_y, position_z] == SpriteState.NoneChoice)
            {
                break;
            }
            if (infoList.Count > 0 && FieldState[position_x, position_y, position_z] == PlayerTurn)
            {
                turnCheck = true;
                foreach (var info in infoList) _infoListNotHard.Add(info);
                break;
            }
        }





        return turnCheck;
    }
    #endregion
    #region 斜め方向ありの関数。
    private bool TurnCheckHard(int Direction, int field_size_x, int field_size_y, int field_size_z)
    {
        var turnCheck = false;
        var position_x = field_size_x;
        var position_y = field_size_y;
        var position_z = field_size_z;
        var OpponentPlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.White : SpriteState.Black;
        var infoList = new List<(int, int, int)>();

        while (0 <= position_x && FIELD_SIZE_MAX >= position_x && 0 <= position_y && FIELD_SIZE_MAX >= position_y && 0 <= position_z && FIELD_SIZE_MAX >= position_z)
        {
            switch (Direction)
            {
                case 0:
                    if (position_x == 0) return turnCheck;
                    position_x--;
                    break;
                case 1:
                    if (position_x == FIELD_SIZE_MAX) return turnCheck;
                    position_x++;
                    break;
                case 2:
                    if (position_y == 0) return turnCheck;
                    position_y--;
                    break;
                case 3:
                    if (position_y == FIELD_SIZE_MAX) return turnCheck;
                    position_y++;
                    break;
                case 4:
                    if (position_z == FIELD_SIZE_MAX) return turnCheck;
                    position_z++;
                    break;
                case 5:
                    if (position_z == 0) return turnCheck;
                    position_z--;
                    break;
                case 6:
                    if (position_x == FIELD_SIZE_MAX) return turnCheck;
                    if (position_y == FIELD_SIZE_MAX) return turnCheck;
                    position_x++;
                    position_y++;
                    break;
                case 7:
                    if (position_x == 0) return turnCheck;
                    if (position_y == 0) return turnCheck;
                    position_x--;
                    position_y--;
                    break;
                case 8:
                    if (position_x == 0) return turnCheck;
                    if (position_y == FIELD_SIZE_MAX) return turnCheck;
                    position_x--;
                    position_y++;
                    break;
                case 9:
                    if (position_x == FIELD_SIZE_MAX) return turnCheck;
                    if (position_y == 0) return turnCheck;
                    position_x++;
                    position_y--;
                    break;
                case 10:
                    if (position_z == FIELD_SIZE_MAX) return turnCheck;
                    if (position_y == FIELD_SIZE_MAX) return turnCheck;
                    position_z++;
                    position_y++;
                    break;
                case 11:
                    if (position_z == 0) return turnCheck;
                    if (position_y == 0) return turnCheck;
                    position_z--;
                    position_y--;
                    break;
                case 12:
                    if (position_z == 0) return turnCheck;
                    if (position_y == FIELD_SIZE_MAX) return turnCheck;
                    position_z--;
                    position_y++;
                    break;
                case 13:
                    if (position_z == FIELD_SIZE_MAX) return turnCheck;
                    if (position_y == 0) return turnCheck;
                    position_z++;
                    position_y--;
                    break;
                case 14:
                    if (position_z == FIELD_SIZE_MAX) return turnCheck;
                    if (position_x == FIELD_SIZE_MAX) return turnCheck;
                    position_z++;
                    position_x++;
                    break;
                case 15:
                    if (position_z == 0) return turnCheck;
                    if (position_x == 0) return turnCheck;
                    position_z--;
                    position_x--;
                    break;
                case 16:
                    if (position_z == 0) return turnCheck;
                    if (position_x == FIELD_SIZE_MAX) return turnCheck;
                    position_z--;
                    position_x++;
                    break;
                case 17:
                    if (position_z == FIELD_SIZE_MAX) return turnCheck;
                    if (position_x == 0) return turnCheck;
                    position_z++;
                    position_x--;
                    break;
                case 18:
                    if (position_x == FIELD_SIZE_MAX) return turnCheck;
                    if (position_y == FIELD_SIZE_MAX) return turnCheck;
                    if (position_z == FIELD_SIZE_MAX) return turnCheck;
                    position_x++;
                    position_y++;
                    position_z++;
                    break;
                case 19:
                    if (position_x == 0) return turnCheck;
                    if (position_y == FIELD_SIZE_MAX) return turnCheck;
                    if (position_z == FIELD_SIZE_MAX) return turnCheck;
                    position_x--;
                    position_y++;
                    position_z++;
                    break;
                case 20:
                    if (position_x == FIELD_SIZE_MAX) return turnCheck;
                    if (position_y == 0) return turnCheck;
                    if (position_z == FIELD_SIZE_MAX) return turnCheck;
                    position_x++;
                    position_y--;
                    position_z++;
                    break;
                case 21:
                    if (position_x == FIELD_SIZE_MAX) return turnCheck;
                    if (position_y == 0) return turnCheck;
                    if (position_z == 0) return turnCheck;
                    position_x++;
                    position_y--;
                    position_z--;
                    break;
                case 22:
                    if (position_x == FIELD_SIZE_MAX) return turnCheck;
                    if (position_y == FIELD_SIZE_MAX) return turnCheck;
                    if (position_z == 0) return turnCheck;
                    position_x++;
                    position_y++;
                    position_z--;
                    break;
                case 23:
                    if (position_x == 0) return turnCheck;
                    if (position_y == FIELD_SIZE_MAX) return turnCheck;
                    if (position_z == 0) return turnCheck;
                    position_x--;
                    position_y++;
                    position_z--;
                    break;
                case 24:
                    if (position_x == 0) return turnCheck;
                    if (position_y == 0) return turnCheck;
                    if (position_z == FIELD_SIZE_MAX) return turnCheck;
                    position_x--;
                    position_y--;
                    position_z++;
                    break;
                case 25:
                    if (position_x == 0) return turnCheck;
                    if (position_y == 0) return turnCheck;
                    if (position_z == 0) return turnCheck;
                    position_x--;
                    position_y--;
                    position_z--;
                    break;
            }


            if (FieldState[position_x, position_y, position_z] == OpponentPlayerTurn)
            {
                infoList.Add((position_x, position_y, position_z));
            }
            if (infoList.Count == 0 && FieldState[position_x, position_y, position_z] == PlayerTurn || FieldState[position_x, position_y, position_z] == SpriteState.None || FieldState[position_x, position_y, position_z] == SpriteState.Wall || FieldState[position_x, position_y, position_z] == SpriteState.NoneChoice)
            {
                break;
            }
            if (infoList.Count > 0 && FieldState[position_x, position_y, position_z] == PlayerTurn)
            {
                turnCheck = true;
                foreach (var info in infoList) _infoListHard.Add(info);
                break;
            }
        }





        return turnCheck;
    }
    #endregion
    #region 対戦モードでの盤面の状態をカスタム盤面の状態にリセット
    public void ResetBoardGame()
    {
        ResultHantei = false;
        WinTextObj.SetActive(false);
        ResetBoardProcess(FieldState, FieldStateCustum);
    }
    #endregion

    #region カスタムモードでの盤面の状態を初期状態にリセット
    public void ResetBoardCustum()
    {
        ResetBoardProcess(FieldState, FieldStateStart);
    }
    #endregion

    #region １つの盤面のデータ情報を全てもう一つの盤面のデータ情報に代入。また、Cubeの初期位置及びプレイヤーターンの情報を黒のターンに変える。
    void ResetBoardProcess(SpriteState[,,] MainSpriteState, SpriteState[,,] TargetSpriteState)
    {
        for (int x = 0; x < OthelloScript.FIELD_SIZE; x++)
        {
            for (int y = 0; y < OthelloScript.FIELD_SIZE; y++)
            {
                for (int z = 0; z < OthelloScript.FIELD_SIZE; z++)
                {
                    MainSpriteState[x, y, z] = TargetSpriteState[x, y, z];
                }

            }
        }
        PlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.Black : SpriteState.Black;
    }
    #endregion

    #region 現在の盤面を全て表示する
    void ShowSpriteBoard()
    {
        for (int x = 0; x < OthelloScript.FIELD_SIZE; x++)
        {
            for (int y = 0; y < OthelloScript.FIELD_SIZE; y++)
            {
                for (int z = 0; z < OthelloScript.FIELD_SIZE; z++)
                {
                    FieldSpriteState[x, y, z].SetState(FieldState[x, y, z]);
                }

            }
        }
    }
    #endregion

    private void CubeControllBool(int[] List, int PosX, int PosY, int PosZ, bool PlayerHnatei)
    {
        
        if (ObjectRotationBool.FieldPosition[0] == 0 || ObjectRotationBool.FieldPosition[0] == 3)
        {
            if (ObjectRotationBool.FieldPosition[1] == 0)
            {
                //Debug.Log(ObjectRotationBool.FieldPosition);
                if (List[0] == 1 && PosX < FIELD_SIZE_MAX)
                {
                    PosX++;
                    if (OthelloScript.isCustum) PositionX(true, PosX, PlayerHnatei);
                    else photonView.RPC("PositionX", RpcTarget.All, true, PosX, PlayerHnatei);
                }
                else if (List[1] == 1 && PosX > 0)
                {   
                    PosX--;
                    if (OthelloScript.isCustum) PositionX(false, PosX, PlayerHnatei);
                    else photonView.RPC("PositionX", RpcTarget.All, false, PosX, PlayerHnatei);
                }
                else if (List[2] == 1 && PosZ < FIELD_SIZE_MAX)
                {
                    PosZ++;
                    if (OthelloScript.isCustum) PositionZ(true, PosZ, PlayerHnatei);
                    else photonView.RPC("PositionZ", RpcTarget.All, true, PosZ, PlayerHnatei);
                }
                else if (List[3] == 1 && PosZ > 0)
                {
                    PosZ--;
                    if (OthelloScript.isCustum) PositionZ(false, PosZ, PlayerHnatei);
                    else photonView.RPC("PositionZ", RpcTarget.All, false, PosZ, PlayerHnatei);
                }
            }
            else if (ObjectRotationBool.FieldPosition[1] == 1)
            {
                Debug.Log(ObjectRotationBool.FieldPosition);
                if (List[0] == 1 && PosZ < FIELD_SIZE_MAX)
                {
                    PosZ++;
                    if (OthelloScript.isCustum) PositionZ(true, PosZ, PlayerHnatei);
                    else photonView.RPC("PositionZ", RpcTarget.All, true, PosZ, PlayerHnatei);

                }
                else if (List[1] == 1 && PosZ > 0)
                {
                    PosZ--;
                    if (OthelloScript.isCustum) PositionZ(false, PosZ, PlayerHnatei);
                    else photonView.RPC("PositionZ", RpcTarget.All, false, PosZ, PlayerHnatei);


                }
                else if (List[2] == 1 && PosX > 0)
                {
                    PosX--;
                    if (OthelloScript.isCustum) PositionX(false, PosX, PlayerHnatei);
                    else photonView.RPC("PositionX", RpcTarget.All, false, PosX, PlayerHnatei);


                }
                else if (List[3] == 1 && PosX < FIELD_SIZE_MAX)
                {
                    PosX++;
                    if (OthelloScript.isCustum) PositionX(true, PosX, PlayerHnatei);
                    else photonView.RPC("PositionX", RpcTarget.All, true, PosX, PlayerHnatei);



                }
            }
            else if (ObjectRotationBool.FieldPosition[1] == 2)
            {

                if (List[0] == 1 && PosZ > 0)
                {
                    PosZ--;
                    if (OthelloScript.isCustum) PositionZ(false, PosZ, PlayerHnatei);
                    else photonView.RPC("PositionZ", RpcTarget.All, false, PosZ, PlayerHnatei);


                }
                else if (List[1] == 1 && PosZ  < FIELD_SIZE_MAX)
                {

                    PosZ++;
                    if (OthelloScript.isCustum) PositionZ(true, PosZ, PlayerHnatei);
                    else photonView.RPC("PositionZ", RpcTarget.All, true, PosZ, PlayerHnatei);

                }
                else if (List[2] == 1 && PosX  < FIELD_SIZE_MAX)
                {

                    PosX++;
                    if (OthelloScript.isCustum) PositionX(true, PosX, PlayerHnatei);
                    else photonView.RPC("PositionX", RpcTarget.All, true, PosX, PlayerHnatei);

                }
                else if (List[3] == 1 && PosX > 0)
                {
                    PosX--;
                    if (OthelloScript.isCustum) PositionX(false, PosX, PlayerHnatei);
                    else photonView.RPC("PositionX", RpcTarget.All, false, PosX, PlayerHnatei);


                }
            }
            else if (ObjectRotationBool.FieldPosition[1] == 3)
            {

                if (List[0] == 1 && PosX  > 0)
                {
                    PosX--;
                    if (OthelloScript.isCustum) PositionX(false, PosX, PlayerHnatei);
                    else photonView.RPC("PositionX", RpcTarget.All, false, PosX, PlayerHnatei);

                }
                else if (List[1] == 1 && PosX < FIELD_SIZE_MAX)
                {
                    PosX++;
                    if (OthelloScript.isCustum) PositionX(true, PosX, PlayerHnatei);
                    else photonView.RPC("PositionX", RpcTarget.All, true, PosX, PlayerHnatei);
                }
                else if (List[2] == 1 && PosZ > 0)
                {
                    PosZ--;
                    if (OthelloScript.isCustum) PositionZ(false, PosZ, PlayerHnatei);
                    else photonView.RPC("PositionZ", RpcTarget.All, false, PosZ, PlayerHnatei);
                }
                else if (List[3] == 1 && PosZ < FIELD_SIZE_MAX)
                {
                    PosZ++;
                    if (OthelloScript.isCustum) PositionZ(true, PosZ, PlayerHnatei);
                    else photonView.RPC("PositionZ", RpcTarget.All, true, PosZ, PlayerHnatei);

                }
            }



        }
        else if (ObjectRotationBool.FieldPosition[0] == 1)
        {
            
            if (List[0] == 1 && PosX < FIELD_SIZE_MAX)
            {
                PosX++;
                if (OthelloScript.isCustum) PositionX(true, PosX, PlayerHnatei);
                else photonView.RPC("PositionX", RpcTarget.All, true, PosX, PlayerHnatei);
            }
            else if (List[1] == 1 && PosX > 0)
            {
                PosX--;
                if (OthelloScript.isCustum) PositionX(false, PosX, PlayerHnatei);
                else photonView.RPC("PositionX", RpcTarget.All, false, PosX, PlayerHnatei);
            }
            else if (List[2] == 1 && PosY < FIELD_SIZE_MAX)
            {
                PosY++;
                if (OthelloScript.isCustum) PositionY(true, PosY, PlayerHnatei);
                else photonView.RPC("PositionY", RpcTarget.All, true, PosY, PlayerHnatei);
            }
            else if (List[3] == 1 && PosY > 0)
            {
                PosY--;
                if (OthelloScript.isCustum) PositionY(false, PosY, PlayerHnatei);
                else photonView.RPC("PositionY", RpcTarget.All, false, PosY, PlayerHnatei);

            }
        }
        else if (ObjectRotationBool.FieldPosition[0] == 2)
        {

            if (List[0] == 1 && PosX < FIELD_SIZE_MAX)
            {
                PosX++;
                if (OthelloScript.isCustum) PositionX(true, PosX, PlayerHnatei);
                else photonView.RPC("PositionX", RpcTarget.All,true, PosX, PlayerHnatei);

            }
            else if (List[1] == 1 && PosX > 0)
            {
                PosX--;
                if (OthelloScript.isCustum) PositionX(false, PosX, PlayerHnatei);
                else photonView.RPC("PositionX", RpcTarget.All, false, PosX, PlayerHnatei);
            }
            else if (List[2] == 1 && PosY > 0)
            {
                PosY--;
                if (OthelloScript.isCustum) PositionY(false, PosY, PlayerHnatei);
                else photonView.RPC("PositionY", RpcTarget.All, false, PosY, PlayerHnatei);
                
            }
            else if (List[3] == 1 && PosY  < FIELD_SIZE_MAX)
            {
                PosY++;
                if (OthelloScript.isCustum) PositionY(true, PosY, PlayerHnatei);
                else photonView.RPC("PositionY", RpcTarget.All, true, PosY, PlayerHnatei);


            }
        }
        //Debug.Log("x,y,z"+Cube_Position_X+","+ Cube_Position_Y+","+ Cube_Position_Z);
        lastMoveTime = Time.time;
    }

    void Contoll_Cube(int CubePositionRight, int CubePositionLeft)
    {

    }
    [PunRPC]
    void PositionX(bool ValueHantei, int PosX, bool PlayerHnatei)
    {

        if (ValueHantei)
        {
            if (PlayerHnatei)
            {
                CubeCopy.transform.localPosition = new Vector3(CubeCopy.transform.localPosition.x + 2.0f, CubeCopy.transform.localPosition.y , CubeCopy.transform.localPosition.z);
                Cube_Position_X = PosX;
                //CubeCopy.transform.position = Cube.transform.localPosition;

            }
            else
            {
                CubeCopyCliant.transform.localPosition = new Vector3(CubeCopyCliant.transform.localPosition.x + 2.0f, CubeCopyCliant.transform.localPosition.y, CubeCopyCliant.transform.localPosition.z);
                Cube_Position_X_Cliant = PosX;
                //CubeCopyCliant.transform.position = CubeCliant.transform.position;
            }

        }
        else
        {
            if (PlayerHnatei)
            {
                CubeCopy.transform.localPosition = new Vector3(CubeCopy.transform.localPosition.x - 2.0f, CubeCopy.transform.localPosition.y, CubeCopy.transform.localPosition.z);
                Cube_Position_X = PosX;
                //CubeCopy.transform.position = Cube.transform.localPosition;

            }
            else
            {
                CubeCopyCliant.transform.localPosition = new Vector3(CubeCopyCliant.transform.localPosition.x - 2.0f, CubeCopyCliant.transform.localPosition.y, CubeCopyCliant.transform.localPosition.z);
                Cube_Position_X_Cliant = PosX;
                //CubeCopyCliant.transform.position = CubeCliant.transform.position;
            }
        }
        CubeMoveAudio.Play();
        //Debug.Log("PosX" + PosX+ " CubeCopy:"+ Cube.transform.localPosition);
    }
    [PunRPC]
    void PositionY(bool ValueHantei,int PosY, bool PlayerHnatei)
    {

        if (ValueHantei)
        {
            if (PlayerHnatei)
            {
                CubeCopy.transform.localPosition = new Vector3(CubeCopy.transform.localPosition.x, CubeCopy.transform.localPosition.y + 2.0f, CubeCopy.transform.localPosition.z);
                Cube_Position_Y = PosY;
                //CubeCopy.transform.position = Cube.transform.localPosition;

            }
            else
            {
                CubeCopyCliant.transform.localPosition = new Vector3(CubeCopyCliant.transform.localPosition.x, CubeCopyCliant.transform.localPosition.y + 2.0f, CubeCopyCliant.transform.localPosition.z);
                Cube_Position_Y_Cliant = PosY;
                //CubeCopyCliant.transform.position = CubeCliant.transform.position;
            }
        }
        else
        {
            if (PlayerHnatei)
            {
                CubeCopy.transform.localPosition = new Vector3(CubeCopy.transform.localPosition.x, CubeCopy.transform.localPosition.y - 2.0f, CubeCopy.transform.localPosition.z);
                Cube_Position_Y = PosY;
                //CubeCopy.transform.position = Cube.transform.localPosition;

            }
            else
            {
                CubeCopyCliant.transform.localPosition = new Vector3(CubeCopyCliant.transform.localPosition.x, CubeCopyCliant.transform.localPosition.y - 2.0f, CubeCopyCliant.transform.localPosition.z);
                Cube_Position_Y_Cliant = PosY;
                //CubeCopyCliant.transform.position = CubeCliant.transform.position;
            }
        }
        CubeMoveAudio.Play();
    }
    [PunRPC]
    void PositionZ(bool ValueHantei,int PosZ, bool PlayerHnatei)
    {
        if (ValueHantei)
        {
            if (PlayerHnatei)
            {
                CubeCopy.transform.localPosition = new Vector3(CubeCopy.transform.localPosition.x, CubeCopy.transform.localPosition.y, CubeCopy.transform.localPosition.z + 2.0f);
                Cube_Position_Z = PosZ;
                //CubeCopy.transform.position = Cube.transform.localPosition;

            }
            else
            {
                CubeCopyCliant.transform.localPosition = new Vector3(CubeCopyCliant.transform.localPosition.x, CubeCopyCliant.transform.localPosition.y, CubeCopyCliant.transform.localPosition.z + 2.0f);
                Cube_Position_Z_Cliant = PosZ;
                //CubeCopyCliant.transform.position = CubeCliant.transform.position;
            }
            
        }
        else
        {
            if (PlayerHnatei)
            {
                CubeCopy.transform.localPosition = new Vector3(CubeCopy.transform.localPosition.x, CubeCopy.transform.localPosition.y, CubeCopy.transform.localPosition.z - 2.0f);
                Cube_Position_Z = PosZ;
                //CubeCopy.transform.position = Cube.transform.localPosition;

            }
            else
            {
                CubeCopyCliant.transform.localPosition = new Vector3(CubeCopyCliant.transform.localPosition.x, CubeCopyCliant.transform.localPosition.y, CubeCopyCliant.transform.localPosition.z - 2.0f);
                Cube_Position_Z_Cliant = PosZ;
                //CubeCopyCliant.transform.position = CubeCliant.transform.position;
            }
        }

        
        CubeMoveAudio.Play();
    }
    public void UPDOWN(bool hantei) {

        if (PhotonNetwork.IsMasterClient || OthelloScript.isCustum)
        {
            Cube_Position_UPDOWN_Controll(hantei, Cube_Position_Z, Cube_Position_Y,true);
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            Cube_Position_UPDOWN_Controll(hantei, Cube_Position_Z_Cliant, Cube_Position_Y_Cliant, false);
        }
    }
    private void Cube_Position_UPDOWN_Controll(bool hantei, int PosZ, int PosY,bool PlayerHnatei)
    {

        if (ObjectRotationBool.FieldPosition[0] == 1)
        {
            if (hantei && PosZ < FIELD_SIZE_MAX)
            {
                PosZ++;
                if (OthelloScript.isCustum) PositionZ(true, PosZ, PlayerHnatei);
                else photonView.RPC("PositionZ", RpcTarget.All, true, PosZ, PlayerHnatei);


            }
            else if (!hantei && PosZ > 0)
            {
                PosZ--;
                if (OthelloScript.isCustum) PositionZ(false, PosZ, PlayerHnatei);
                else photonView.RPC("PositionZ", RpcTarget.All, false, PosZ, PlayerHnatei);
            }

        }
        else if (ObjectRotationBool.FieldPosition[0] == 2)
        {
            if (hantei && PosZ > 0)
            {
                PosZ--;
                if (OthelloScript.isCustum) PositionZ(false, PosZ, PlayerHnatei);
                else photonView.RPC("PositionZ", RpcTarget.All, false, PosZ, PlayerHnatei);



            }
            else if (!hantei && PosZ < FIELD_SIZE_MAX)
            {
                PosZ++;
                if (OthelloScript.isCustum) PositionZ(true, PosZ, PlayerHnatei);
                else photonView.RPC("PositionZ", RpcTarget.All, true, PosZ, PlayerHnatei);
            }
        }
        else
        {
            if (hantei && PosY > 0)
            {
                PosY--;
                if (OthelloScript.isCustum) PositionY(false, PosY, PlayerHnatei);
                else photonView.RPC("PositionY", RpcTarget.All, false, PosY, PlayerHnatei);

            }
            else if (!hantei && PosY < FIELD_SIZE_MAX)
            {
                PosY++;
                if (OthelloScript.isCustum) PositionY(true, PosY, PlayerHnatei);
                else photonView.RPC("PositionY", RpcTarget.All, true, PosY, PlayerHnatei);
            }

        }
        CubeMoveAudio.Play();
        EventSystem.current.SetSelectedGameObject(null);

    }
    public void StartFiledState()
    {
        FieldState = new SpriteState[OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE];
        FieldStateCustum = new SpriteState[OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE, OthelloScript.FIELD_SIZE];
    }
    private void Awake()
    {
        RegisterCustomTypes();
        
    }
    private void RegisterCustomTypes()
    {
        PhotonPeer.RegisterType(typeof(SpriteState), (byte)'S', SerializeSpriteState, DeserializeSpriteState);
        
    }

    private static byte[] SerializeSpriteState(object customObject)
    {
        SpriteState spriteState = (SpriteState)customObject;
        return new byte[] { (byte)spriteState };
    }

    private static object DeserializeSpriteState(byte[] data)
    {
        return (SpriteState)data[0];
    }
    public void SendFieldStateToParticipantsCustum()
    {   
        SpriteState[] serializedFieldState = SerializeFieldState(FieldStateCustum);
        
        photonView.RPC("SyncFieldStateCustum", RpcTarget.Others, serializedFieldState);
    }
    public void SendFieldStateToParticipantsFieldState()
    {
        SpriteState[] serializedFieldState = SerializeFieldState(FieldState);
        photonView.RPC("SyncFieldStateFieldState", RpcTarget.All, serializedFieldState);

    }
    [PunRPC]
    void SyncFieldStateFieldState(SpriteState[] serializedFieldState)
    {
        // デシリアライズしてフィールド状態を適用
        FieldState = DeserializeFieldState(serializedFieldState);
        UpdateField();
    }


    void UpdateField()
    {
        isChoicPoint = false;
        for (int x = 0; x < OthelloScript.FIELD_SIZE; x++)
        {
            for (int y = 0; y < OthelloScript.FIELD_SIZE; y++)
            {
                for (int z = 0; z < OthelloScript.FIELD_SIZE; z++)
                {
                    if (FieldState[x, y, z] == SpriteState.NoneChoice) FieldState[x, y, z] = SpriteState.None;
                    FieldSpriteState[x, y,z].SetState(FieldState[x, y,z]);
                }

            }


        }
    }
    [PunRPC]
    void SyncFieldStateCustum(SpriteState[] serializedFieldState)
    {
        FieldStateCustum = DeserializeFieldState(serializedFieldState);
    }


    [PunRPC]
    void SyncPlayerTurn(SpriteState newTurn)
    {
        PlayerTurn = newTurn;

    }
    void ChangePlayerTurn()
    {
        // プレイヤーのターンを切り替える
        PlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.White : SpriteState.Black;

        // RPCで全クライアントに同期
        photonView.RPC("SyncPlayerTurn", RpcTarget.All, PlayerTurn);
    }
}



