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

public class OthelloScript : MonoBehaviour
{
    public GameObject OthelloSprite;
    public GameObject Cube;
    public GameObject firstSelectedGameObject;
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
    public int CustumSelectBotton = 0;
    public static int FIELD_SIZE;
    public static float positional_complement;
    public static bool isHard;
    public static bool isCustum;
    private int FIELD_SIZE_MAX = FIELD_SIZE-1;
    private int DIRECTION_MAXHard = 26;
    private int DIRECTION_MAXNotHard = 6;
    private int MEDIAN_UP = FIELD_SIZE / 2;
    private int MEDIAN_DOWN = FIELD_SIZE / 2-1;

    //private Image BlackImage;
    //private Image WhiteImage;
    private int Cube_Position_X;
    private int Cube_Position_Y;
    private int Cube_Position_Z;
    private float moveDelay = 0.15f;
    private float lastMoveTime = 0.0f;
    private bool ResultHantei = false;
    private bool BlackCheckFlag = false;
    private bool WhiteCheckFlag = true;
    
    [SerializeField] private GameObject ObjectRotationSystem;
    private ObjectRotation ObjectRotationBool;
    private int[] RightLeftUpDown = new int[] { 0, 0, 0, 0 };
    //private Botton_process bottonProcess;
    private List<(int, int,int)> _infoListNotHard = new List<(int, int, int)>();
    private List<(int, int,int)> _infoListHard = new List<(int, int, int)>();
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
    private SpriteState[,,] FieldState = new SpriteState[FIELD_SIZE, FIELD_SIZE, FIELD_SIZE];
    private SpriteState[,,] FieldStateCustum = new SpriteState[FIELD_SIZE, FIELD_SIZE, FIELD_SIZE];
    private SpriteState[,,] FieldStateStart = new SpriteState[FIELD_SIZE, FIELD_SIZE, FIELD_SIZE];
    private SpriteState[,,] FieldStateNone = new SpriteState[FIELD_SIZE, FIELD_SIZE, FIELD_SIZE];
    private SpriteScript[,,] FieldSpriteState = new SpriteScript[FIELD_SIZE, FIELD_SIZE, FIELD_SIZE];

    void Start()
    {
        

        settingsPanel.SetActive(false);

        Cube_Position_X = FIELD_SIZE_MAX;
        Cube_Position_Y = FIELD_SIZE_MAX;
        Cube_Position_Z = FIELD_SIZE_MAX;
        ObjectRotationBool = ObjectRotationSystem.GetComponent<ObjectRotation>();
        
        FieldState[MEDIAN_DOWN, MEDIAN_UP, MEDIAN_DOWN] = SpriteState.Black;
        FieldState[MEDIAN_DOWN, MEDIAN_DOWN, MEDIAN_DOWN] = SpriteState.White;
        FieldState[MEDIAN_UP, MEDIAN_DOWN, MEDIAN_UP] = SpriteState.White;
        FieldState[MEDIAN_UP, MEDIAN_UP, MEDIAN_UP] = SpriteState.Black;
        FieldState[MEDIAN_DOWN, MEDIAN_DOWN, MEDIAN_UP] = SpriteState.Black;
        FieldState[MEDIAN_DOWN, MEDIAN_UP, MEDIAN_UP] = SpriteState.White;
        FieldState[MEDIAN_UP, MEDIAN_DOWN, MEDIAN_DOWN] = SpriteState.Black;
        FieldState[MEDIAN_UP, MEDIAN_UP, MEDIAN_DOWN] = SpriteState.White;
        for (int x = 0; x < FIELD_SIZE; x++)
        {
            for (int y = 0; y < FIELD_SIZE; y++)
            {
                for (int z = 0; z < FIELD_SIZE; z++)
                {
                    Vector3 localPosition = new Vector3(
                            2.0f * (x - positional_complement),
                            2.0f * (y - positional_complement),
                            2.0f * (z - positional_complement)
                        );
                    Cube.transform.position = localPosition;



                    var sprite = Instantiate(OthelloSprite, localPosition, Quaternion.Euler(0, 0, 0));

                    sprite.transform.SetParent(Board_surface.transform, true);
                    FieldStateNone[x, y, z] = SpriteState.None;
                    if (!(x == MEDIAN_UP || x == MEDIAN_DOWN) && !(y == MEDIAN_UP || y == MEDIAN_DOWN) && !(z == MEDIAN_UP || z == MEDIAN_DOWN)) FieldState[x, y, z] = SpriteState.None;

                    FieldSpriteState[x, y, z] = sprite.GetComponent<SpriteScript>();
                    FieldSpriteState[x, y, z].SetState(FieldState[x, y, z]);
                    FieldStateStart[x, y, z] = FieldState[x, y, z];
                    FieldStateCustum[x, y, z] = FieldState[x, y, z];
                    }

                }
            

        }
        

    }
    void Update()
    {

        

        if (Time.time - lastMoveTime >= moveDelay)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                RightLeftUpDown[0] = 1;
                CubeControllBool();
                RightLeftUpDown[0] = 0;
                
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                RightLeftUpDown[1] = 1;
                CubeControllBool();
                RightLeftUpDown[1] = 0;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                RightLeftUpDown[2] = 1;
                CubeControllBool();
                RightLeftUpDown[2] = 0;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                RightLeftUpDown[3] = 1;
                CubeControllBool();
                RightLeftUpDown[3] = 0;
            }
        }
        if (!ResultHantei) {
            if (isCustum) CustumMode();
            else {
                if(isHard) GameHardMode();
                else GameNotHardMode();
            }
            
        }
        

    }

    private void GameHardMode()
    {
        var turnCheck = false;
        
            if (Input.GetKeyDown(KeyCode.Return))
            {
                for (int i = 0; i < DIRECTION_MAXHard; i++)
                {   
                    if (TurnCheckHard(i, Cube_Position_X, Cube_Position_Y, Cube_Position_Z)) turnCheck = true;
            }
                #region　プレイヤーが置ける石として表示するマスを初期化
                for (int x = 0; x < FIELD_SIZE; x++)
                {
                    for (int y = 0; y < FIELD_SIZE; y++)
                    {
                        for (int z = 0; z < FIELD_SIZE; z++)
                        {
                            if (FieldState[x, y,z] == SpriteState.NoneChoice) FieldState[x, y,z] = SpriteState.None;
                        }
                        
                    }
                }
                #endregion
                if (turnCheck && (FieldState[Cube_Position_X, Cube_Position_Y, Cube_Position_Z] == SpriteState.None || FieldState[Cube_Position_X, Cube_Position_Y, Cube_Position_Z] == SpriteState.NoneChoice))
                {
                    StonePutAudio.Play();
                    foreach (var info in _infoListHard)
                    {
                        var position_X = info.Item1;
                        var position_Y = info.Item2;
                        var position_Z = info.Item3;
                        FieldState[position_X, position_Y, position_Z] = PlayerTurn;

                    }
                    FieldState[Cube_Position_X, Cube_Position_Y, Cube_Position_Z] = PlayerTurn;
                    PlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.White : SpriteState.Black;

                }
                else
                {
                    NotPutAudio.Play();
                }



            }
        _infoListHard = new List<(int, int, int)>();

        turnCheck = false;
        for (int x = 0; x < FIELD_SIZE; x++)
        {
            for (int y = 0; y < FIELD_SIZE; y++)
            {
                for (int z = 0; z < FIELD_SIZE; z++)
                {
                    for (int i = 0; i < DIRECTION_MAXHard; i++)
                    {
                        if (TurnCheckHard(i, x, y,z) && (FieldState[x, y,z] == SpriteState.None || FieldState[x, y,z] == SpriteState.NoneChoice))
                        {
                            FieldState[x, y,z] = SpriteState.NoneChoice;
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
            PlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.White : SpriteState.Black;
        }
        _infoListHard = new List<(int, int, int)>();
        UpdateBoard();

    }
    private void GameNotHardMode()
    {
        var turnCheck = false;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            for (int i = 0; i < DIRECTION_MAXNotHard; i++)
            {
                if (TurnCheckNotHard(i, Cube_Position_X, Cube_Position_Y, Cube_Position_Z)) turnCheck = true;
            }
            #region　プレイヤーが置ける石として表示するマスを初期化
            for (int x = 0; x < FIELD_SIZE; x++)
            {
                for (int y = 0; y < FIELD_SIZE; y++)
                {
                    for (int z = 0; z < FIELD_SIZE; z++)
                    {
                        if (FieldState[x, y, z] == SpriteState.NoneChoice) FieldState[x, y, z] = SpriteState.None;
                    }

                }
            }
            #endregion
            if (turnCheck && (FieldState[Cube_Position_X, Cube_Position_Y, Cube_Position_Z] == SpriteState.None || FieldState[Cube_Position_X, Cube_Position_Y, Cube_Position_Z] == SpriteState.NoneChoice))
            {
                StonePutAudio.Play();
                foreach (var info in _infoListNotHard)
                {
                    var position_X = info.Item1;
                    var position_Y = info.Item2;
                    var position_Z = info.Item3;
                    FieldState[position_X, position_Y, position_Z] = PlayerTurn;

                }
                FieldState[Cube_Position_X, Cube_Position_Y, Cube_Position_Z] = PlayerTurn;
                PlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.White : SpriteState.Black;

            }
            else
            {
                NotPutAudio.Play();
            }



        }
        _infoListNotHard = new List<(int, int, int)>();

        turnCheck = false;
        for (int x = 0; x < FIELD_SIZE; x++)
        {
            for (int y = 0; y < FIELD_SIZE; y++)
            {
                for (int z = 0; z < FIELD_SIZE; z++)
                {
                    for (int i = 0; i < DIRECTION_MAXNotHard; i++)
                    {
                        if (TurnCheckNotHard(i, x, y, z) && (FieldState[x, y, z] == SpriteState.None || FieldState[x, y, z] == SpriteState.NoneChoice))
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
            PlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.White : SpriteState.Black;
        }
        _infoListNotHard = new List<(int, int, int)>();
        UpdateBoard();

    }
    #region　カスタムモードの処理部分
    private void CustumMode()
    {
        if (EventSystem.current.currentSelectedGameObject == firstSelectedGameObject)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                FieldState[Cube_Position_X, Cube_Position_Y, Cube_Position_Z] = spriteStates[CustumSelectBotton];
                FieldStateCustum[Cube_Position_X, Cube_Position_Y, Cube_Position_Z] = FieldState[Cube_Position_X, Cube_Position_Y, Cube_Position_Z];
                StonePutAudio.Play();
            }
            
        }
        ShowSpriteBoard();
    }
    #endregion
    private void UpdateBoard()
    {
        var WhiteNum = default(int);
        var BlackNum = default(int);
        for (int x = 0; x < FIELD_SIZE; x++)
        {
            for (int y = 0; y < FIELD_SIZE; y++)
            {
                for (int z = 0; z < FIELD_SIZE; z++)
                {
                    if (FieldState[x, y,z] == SpriteState.White) WhiteNum++;
                    else if (FieldState[x, y,z] == SpriteState.Black) BlackNum++;
                }
                    
            }


        }
        ShowSpriteBoard();
        #region　黒と白どちらのターンなのかを表示。現在の黒石と白石の数を表示。
        if (PlayerTurn == SpriteState.White)
        {
            BlackImage.enabled = false;
            WhiteImage.enabled = true;
        }
        else if (PlayerTurn == SpriteState.Black) {
            BlackImage.enabled = true;
            WhiteImage.enabled = false;
        }


        BlackNumText.text = BlackNum.ToString();
        WhiteNumText.text = WhiteNum.ToString();
        #endregion
        if (WhiteNum + BlackNum == FIELD_SIZE * FIELD_SIZE * FIELD_SIZE || !WhiteCheckFlag && !BlackCheckFlag) GameOver(WhiteNum, BlackNum);
    }
    #region 対戦終了時のUI表示
    private void GameOver(int WhiteNum, int BlackNum)
    {
        GameObject Black = WinTextObj.transform.Find("Black").gameObject;
        GameObject White = WinTextObj.transform.Find("White").gameObject;
        GameObject Hikiwake = WinTextObj.transform.Find("Hikiwake").gameObject;
        WinTextObj.SetActive(true);
        GameOverAudio.Play();
        ResultHantei = true;
        BlackImage.enabled = false;
        WhiteImage.enabled = false;
        if (WhiteNum == BlackNum) {
            Hikiwake.SetActive(true);
            Black.SetActive(false);
            White.SetActive(false);
        } 
        else if(WhiteNum > BlackNum)
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
    private bool TurnCheckHard(int Direction, int field_size_x, int field_size_y,int field_size_z)
    {
        var turnCheck = false;
        var position_x = field_size_x;
        var position_y = field_size_y;
        var position_z = field_size_z;
        var OpponentPlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.White : SpriteState.Black;
        var infoList = new List<(int, int,int)>();

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
        for (int x = 0; x < FIELD_SIZE; x++)
        {
            for (int y = 0; y < FIELD_SIZE; y++)
            {
                for (int z = 0; z < FIELD_SIZE; z++)
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
        for (int x = 0; x < FIELD_SIZE; x++)
        {
            for (int y = 0; y < FIELD_SIZE; y++)
            {
                for (int z = 0; z < FIELD_SIZE; z++) 
                {
                    FieldSpriteState[x, y, z].SetState(FieldState[x, y, z]);
                }

            }
        }
    }
    #endregion

    void CubeControllBool() 
    {
        if (ObjectRotationBool.FieldPosition[0] == 0|| ObjectRotationBool.FieldPosition[0] == 3)
        {   if(ObjectRotationBool.FieldPosition[1] == 0)
            {
                
                if (RightLeftUpDown[0] == 1&& Cube_Position_X< FIELD_SIZE_MAX)
                {
                    Cube_Position_X++;
                    PositionX(true);
                }
                else if (RightLeftUpDown[1] == 1&& Cube_Position_X>0)
                {
                    Cube_Position_X--;
                    PositionX(false);
                }
                else if (RightLeftUpDown[2] == 1&& Cube_Position_Z < FIELD_SIZE_MAX)
                {
                    Cube_Position_Z++;
                    PositionZ(true);
                }
                else if (RightLeftUpDown[3] == 1 && Cube_Position_Z > 0)
                {
                    Cube_Position_Z--;
                    PositionZ(false);
                }
            }else if (ObjectRotationBool.FieldPosition[1] == 1)
            {
                if (RightLeftUpDown[0] == 1 && Cube_Position_Z < FIELD_SIZE_MAX)
                {
                    Cube_Position_Z++;
                    PositionZ(true);
                }
                else if (RightLeftUpDown[1] == 1 && Cube_Position_Z>0 )
                {
                    Cube_Position_Z--;
                    PositionZ(false);
                }
                else if (RightLeftUpDown[2] == 1 && Cube_Position_X >0)
                {
                    Cube_Position_X--;
                    PositionX(false);
                }
                else if (RightLeftUpDown[3] == 1 && Cube_Position_X < FIELD_SIZE_MAX)
                {
                    Cube_Position_X++;
                    PositionX(true);
                }
            }
            else if (ObjectRotationBool.FieldPosition[1] == 2)
            {
                if (RightLeftUpDown[0] == 1 && Cube_Position_Z > 0)
                {
                    Cube_Position_Z--;
                    PositionZ(false);
                }
                else if (RightLeftUpDown[1] == 1 && Cube_Position_Z < FIELD_SIZE_MAX)
                {
                    Cube_Position_Z++;
                    PositionZ(true);
                }
                else if (RightLeftUpDown[2] == 1 && Cube_Position_X < FIELD_SIZE_MAX)
                {
                    Cube_Position_X++;
                    PositionX(true);
                }
                else if (RightLeftUpDown[3] == 1 && Cube_Position_X > 0)
                {
                    Cube_Position_X--;
                    PositionX(false);
                }
            }
            else if (ObjectRotationBool.FieldPosition[1] == 3)
            {
                if (RightLeftUpDown[0] == 1 && Cube_Position_X > 0)
                {
                    Cube_Position_X--;
                    PositionX(false);
                }
                else if (RightLeftUpDown[1] == 1 && Cube_Position_X < FIELD_SIZE_MAX)
                {
                    Cube_Position_X++;
                    PositionX(true);
                }
                else if (RightLeftUpDown[2] == 1 && Cube_Position_Z > 0)
                {
                    Cube_Position_Z--;
                    PositionZ(false);
                }
                else if (RightLeftUpDown[3] == 1 && Cube_Position_Z < FIELD_SIZE_MAX)
                {
                    Cube_Position_Z++;
                    PositionZ(true);
                }
            }



        }
        else if(ObjectRotationBool.FieldPosition[0] == 1)
        {
            if (RightLeftUpDown[0] == 1 && Cube_Position_X < FIELD_SIZE_MAX)
            {
                Cube_Position_X++;
                PositionX(true);
            }
            else if (RightLeftUpDown[1] == 1 && Cube_Position_X > 0)
            {
                Cube_Position_X--;
                PositionX(false);
            }
            else if (RightLeftUpDown[2] == 1 && Cube_Position_Y < FIELD_SIZE_MAX)
            {
                Cube_Position_Y++;
                PositionY(true);
            }
            else if (RightLeftUpDown[3] == 1 && Cube_Position_Y > 0)
            {
                Cube_Position_Y--;
                PositionY(false);
            }
        }
        else if (ObjectRotationBool.FieldPosition[0] == 2)
        {
            if (RightLeftUpDown[0] == 1 && Cube_Position_X < FIELD_SIZE_MAX)
            {
                Cube_Position_X++;
                PositionX(true);
            }
            else if (RightLeftUpDown[1] == 1 && Cube_Position_X > 0)
            {
                Cube_Position_X--;
                PositionX(false);
            }
            else if (RightLeftUpDown[2] == 1 && Cube_Position_Y >0)
            {
                Cube_Position_Y--;
                PositionY(false);
            }
            else if (RightLeftUpDown[3] == 1 && Cube_Position_Y < FIELD_SIZE_MAX)
            {
                Cube_Position_Y++;
                PositionY(true);
            }
        }
        //Debug.Log("x,y,z"+Cube_Position_X+","+ Cube_Position_Y+","+ Cube_Position_Z);
        lastMoveTime = Time.time;
    }

    void Contoll_Cube(int CubePositionRight,int CubePositionLeft)
    {
        

    }
    
    void PositionX(bool ValueHantei)
    {   
        var position = Cube.transform.localPosition;
        if (ValueHantei)
        {
            Cube.transform.localPosition = new Vector3(position.x + 2.0f, position.y, position.z);
            
        }
        else
        {
            Cube.transform.localPosition = new Vector3(position.x - 2.0f, position.y, position.z);
        }
        CubeMoveAudio.Play();
    }

    void PositionY(bool ValueHantei)
    {
        var position = Cube.transform.localPosition;
        if (ValueHantei)
        {
            Cube.transform.localPosition = new Vector3(position.x , position.y + 2.0f, position.z);
        }
        else
        {
            Cube.transform.localPosition = new Vector3(position.x, position.y - 2.0f, position.z);
        }
        CubeMoveAudio.Play();
    }
    void PositionZ(bool ValueHantei)
    {
        var position = Cube.transform.localPosition;
        if (ValueHantei)
        {   
            Cube.transform.localPosition = new Vector3(position.x, position.y , position.z + 2.0f);
        }
        else
        {
            Cube.transform.localPosition = new Vector3(position.x, position.y , position.z - 2.0f);
        }
        CubeMoveAudio.Play();
    }
    public void Cube_Position_UPDOWN_Controll(bool hantei) 
    {
        if (ObjectRotationBool.FieldPosition[0] == 1)
        {
            if (hantei && Cube_Position_Z < FIELD_SIZE_MAX)
            {
                Cube_Position_Z++;
                PositionZ(true);
            }
            else if (!hantei && Cube_Position_Z > 0)
            {
                Cube_Position_Z--;
                PositionZ(false);
            }

        }
        else if (ObjectRotationBool.FieldPosition[0] == 2)
        {
            if (hantei && Cube_Position_Z > 0)
            {
                Cube_Position_Z--;
                PositionZ(false);
            }
            else if (!hantei && Cube_Position_Z < FIELD_SIZE_MAX)
            {
                Cube_Position_Z++;
                PositionZ(true);
            }
        }
        else 
        {
            if (hantei && Cube_Position_Y > 0)
            {
                Cube_Position_Y--;
                PositionY(false);
            }
            else if (!hantei && Cube_Position_Y < FIELD_SIZE_MAX)
            {
                Cube_Position_Y++;
                PositionY(true);
            }

        }
        CubeMoveAudio.Play();
        EventSystem.current.SetSelectedGameObject(null);

    }
    
}


 
