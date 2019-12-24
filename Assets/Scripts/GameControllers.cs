using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiroozehGameService.Core;
using FiroozehGameService.Core.GSLive;
using FiroozehGameService.Handlers;
using FiroozehGameService.Models;
using FiroozehGameService.Models.Command;
using FiroozehGameService.Models.GSLive;
using FiroozehGameService.Models.GSLive.TB;
using UnityEngine;
using UnityEngine.UI;

public class GameControllers : MonoBehaviour {
    public Canvas startMenu;
    public Canvas LoginCanvas;
    public Canvas GamePlay;
    public Canvas EndGameMenu;
    public Button startGameBtn;
    public Text startMenuText;
    public Text Status;
    
    
    public InputField NickName;
    public InputField Email;
    public InputField Password;
    public Button Submit;
    public Text SwitchToRegisterOrLogin;
    public Text LoginErr;

    
    

    private int _whoTurn; // 0 -> X, 1 -> O
    private int _turnCount;
    public GameObject[] turnIcons; //turn UI signs
    public Sprite[] playIcons;
    public Button[] Spaces;
    private int[] _markTabel; // which player select which button
    public Text WinnerText;
    private int _xPlayerScore = 0;
    private int _oPlayerScore = 0;
    public Text oPlayerTextScore;
    public Text xPlayerTextScore;

    private bool _isAvailable;
    private int gameState = 0;
    // Start is called before the first frame update
    void Start ()
    {
        SetEventListeners();
        ConnectToGamesService ();
        GameInit ();
    }

  
    async Task ConnectToGamesService () {
        //connecting to GamesService
        Status.text = "Status : Connecting...";
        startGameBtn.interactable = false;
        
        if (FileUtil.IsLoginbefore())
        {
            try
            {
                await GameService.Login(FileUtil.GetUserToken());
            }
            catch (Exception e)
            {
                Status.color = Color.red;
                if (e is GameServiceException) Status.text = "GameServiceException : " + e.Message;
                else Status.text = "InternalException : " + e.Message;
            }
           
        }
        else
        {
            // Enable LoginUI
            startMenu.gameObject.SetActive(false);
            LoginCanvas.gameObject.SetActive(true);
            
            Submit.onClick.AddListener(async () =>
            {
                try
                {
                    if (NickName.IsActive()) // is SignUp
                    {
                        var nickName = NickName.text.Trim();
                        var email = Email.text.Trim();
                        var pass = Password.text.Trim();

                        if (string.IsNullOrEmpty(nickName)
                            && string.IsNullOrEmpty(email)
                            && string.IsNullOrEmpty(pass))
                            LoginErr.text = "Invalid Input!";
                        else
                        {
                            var userToken = await GameService.SignUp(nickName, email, pass);
                            FileUtil.SaveUserToken(userToken);
                        }

                    }
                    else
                    {
                        var email = Email.text.Trim();
                        var pass = Password.text.Trim();

                        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(pass))
                            LoginErr.text = "Invalid Input!";
                        else
                        {
                            var userToken = await GameService.Login(email, pass);
                            FileUtil.SaveUserToken(userToken);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e is GameServiceException) LoginErr.text = "GameServiceException : " + e.Message;
                    else LoginErr.text = "InternalException : " + e.Message;
                }
                finally
                {
                    // Disable LoginUI
                    startMenu.gameObject.SetActive(true);
                    LoginCanvas.gameObject.SetActive(false);
                }
               
            });
        }        
    }

    
    private void SetEventListeners()
    {
        TurnBasedEventHandlers.SuccessfullyLogined += OnSuccessfullyLogined;
        TurnBasedEventHandlers.Error += OnError;
        
        TurnBasedEventHandlers.AutoMatchUpdated += OnAutoMatchUpdated;
        

        TurnBasedEventHandlers.JoinRoom += OnJoinRoom;
        TurnBasedEventHandlers.Completed += OnCompleted;
        TurnBasedEventHandlers.Finished += OnFinished;
        TurnBasedEventHandlers.OnChooseNext += OnChooseNext;
        TurnBasedEventHandlers.OnTakeTurn += OnTakeTurn;
        TurnBasedEventHandlers.OnLeaveRoom += OnLeaveRoom;
        TurnBasedEventHandlers.RoomMembersDetailReceived += OnRoomMembersDetailReceived;

    }

    
   
    // Update is called once per frame
    void Update () {
        
        switch (gameState)
        {
            case 0:
                startMenu.enabled = true;
                GamePlay.enabled = false;
                break;
            case 1:
                startMenu.enabled = false;
                GamePlay.enabled = true;
                break;
        }
    }


    private void GameInit () {
        _whoTurn = 0;
        _turnCount = 0;
        turnIcons[0].SetActive (true);
        turnIcons[1].SetActive (false);
        foreach (var t in Spaces)
        {
            t.interactable = true;
            t.GetComponent<Image> ().sprite = null;
        }
        for (var i = 0; i < _markTabel.Length; i++) {
            _markTabel[i] = -100; //noBody
        }
    }

    public void OnCellClick (int buttonNumber) {
        Spaces[buttonNumber].image.sprite = playIcons[_whoTurn];
        Spaces[buttonNumber].interactable = false;

        _markTabel[buttonNumber] = _whoTurn + 1; // mark button for player // +1 is for logics
        _turnCount++;
        if (_turnCount > 4)
            WinnerCheck ();
    }

    private void WinnerCheck () {
        var s1 = _markTabel[0] + _markTabel[1] + _markTabel[2];
        var s2 = _markTabel[3] + _markTabel[4] + _markTabel[5];
        var s3 = _markTabel[6] + _markTabel[7] + _markTabel[8];
        var s4 = _markTabel[0] + _markTabel[3] + _markTabel[6];
        var s5 = _markTabel[1] + _markTabel[4] + _markTabel[7];
        var s6 = _markTabel[2] + _markTabel[5] + _markTabel[8];
        var s7 = _markTabel[0] + _markTabel[4] + _markTabel[8];
        var s8 = _markTabel[0] + _markTabel[4] + _markTabel[6];
       
        var results = new[] { s1, s2, s3, s4, s5, s6, s7, s8 };
        if (results.All(t => t != 3 * (_whoTurn + 1))) return;
       
        WinnerText.gameObject.SetActive (true);
        WinnerText.text = "Player " + (_whoTurn == 0 ? "X" : "O") + " wins!";
        if (_whoTurn == 0) {
            _xPlayerScore++;
            xPlayerTextScore.text = _xPlayerScore.ToString ();
        } else {
            _oPlayerScore++;
            oPlayerTextScore.text = _oPlayerScore.ToString ();
        }
        foreach (var t in Spaces)
        {
            t.interactable = false;
        }
    }

    
    
    private void OnRoomMembersDetailReceived(object sender, List<Member> e)
    {
        
    }

    private void OnLeaveRoom(object sender, Member e)
    {
        
    }

    private void OnTakeTurn(object sender, Turn turn)
    {
        var buttonNumber = int.Parse(turn.Data);
        Spaces[buttonNumber].image.sprite = playIcons[_whoTurn];
        Spaces[buttonNumber].interactable = false;

        _markTabel[buttonNumber] = _whoTurn + 1; // mark button for player // +1 is for logics
        _turnCount++;
        if (_turnCount > 4)
            WinnerCheck();
           
        _whoTurn = turn.WhoTakeTurn.User.IsMe ? 1 : 0;
        turnIcons[0].SetActive(_whoTurn == 0);
        turnIcons[1].SetActive(_whoTurn == 1);
    }

    private void OnChooseNext(object sender, Member whoIsNext)
    {
        for (var i = 0; i < 9; i++)
            Spaces[i].interactable = whoIsNext.User.IsMe;
    }

    
    private void OnAutoMatchUpdated(object sender, AutoMatchEvent e)
    {
        
    }

    private void OnFinished(object sender, Finish e)
    {
        
    }

    private void OnCompleted(object sender, Complete e)
    {
        
    }

    private void OnJoinRoom(object sender, JoinEvent e)
    {
        startMenu.gameObject.SetActive(false);
        GamePlay.gameObject.SetActive(true);
    }

    private void OnError(object sender, ErrorEvent e)
    {
        startMenuText.text = "Error : " + e.Error;
    }

    private void OnSuccessfullyLogined(object sender, EventArgs e)
    {
        Status.text = "Status : Connected!";
        startGameBtn.interactable = true;
        _isAvailable = true;
        
        // Start Game
        startGameBtn.onClick.AddListener(async () =>
        {
            await GameService.GSLive.TurnBased.AutoMatch(new GSLiveOption.AutoMatchOption
            {
                Role = "partner" , MinPlayer = 2 , MaxPlayer = 2 , IsPersist = false
            });

            // go to waiting ui
            startGameBtn.interactable = false;
            startMenuText.text = "MatchMaking...";
        });
    }

}

