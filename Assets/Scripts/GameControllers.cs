using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControllers : MonoBehaviour {
    public Canvas startMenu;
    public Canvas GamePlay;
    public Canvas EndGameMenu;
    public Button startGameBtn;
    public Text startMenuText;
    public Text Status;

    public int whoTurn; // 0 -> X, 1 -> O
    public int turnCount;
    public GameObject[] turnIcons; //turn UI signs
    public Sprite[] playIcons;
    public Button[] Spaces;
    public int[] markTabel; // which player select which button
    public Text WinnerText;
    public int xPlayerScore = 0;
    public int oPlayerScore = 0;
    public Text oPlayerTextScore;
    public Text xPlayerTextScore;

    private int isAvailable = 0;
    private int gameState = 0;
    // Start is called before the first frame update
    void Start () {
        
        connectToGamesService ();
        gameInit ();
    }

    // Update is called once per frame
    void Update () {
        if(isAvailable == 1){
            // making visible startBtn after init
            startGameBtn.interactable = true;
            isAvailable = 0;

            Debug.LogError("Status : Connected!");
            Status.text = "Status : Connected!";
        }

        if(gameState == 0)
        {
            startMenu.enabled = true;
            GamePlay.enabled = false;
        }
        if (gameState == 1)
        {
            startMenu.enabled = false;
            GamePlay.enabled = true;
        }
    }

    void connectToGamesService () {
        //connecting to GamesService
        Status.text = "Status : Connecting...";
        startGameBtn.interactable = false;
        //var config = new GameServiceClientConfiguration
        //    .Builder (LoginType.Normal)
        //    .SetClientId ("doozid") // put from GamesService devPanel
        //    .SetClientSecret ("8mic0z5mddcbdo7k40vh2r") // put from GamesService devPanel
        //    .IsLogEnable (true)
        //    .IsNotificationEnable (true)
        //    // .SetNotificationListener (onJsonData)
        //    .CheckGameServiceInstallStatus (false)
        //    .CheckGameServiceOptionalUpdate (false)
        //    .Build ();
        //FiroozehGameService.ConfigurationInstance (config);
        //FiroozehGameService.Run(onAfterGSConnectSuccessfully,OnError);
    }

    void OnError(string err)
    {
        Status.text = "Error : " + err;
    }
    void onAfterGSConnectSuccessfully () {
        // connecting to TurnBase and being ready
        //FiroozehGameService.Instance.GSLive.TurnBased.SetListener(this);

        isAvailable = 1;
    }

    public void onStartGameBtnClick () {
        //var option = new FiroozehGameServiceAndroid.Core.GSLive.GSLiveOption.AutoMatchOption();
        //option.IsPersist = false;
        //option.MaxPlayer = 2;
        //option.MinPlayer = 2;
        //option.Role = "partner";
        //FiroozehGameService.Instance.GSLive.TurnBased.AutoMatch(option);

        // go to waiting ui
       // startGameBtn.interactable = false;
        startMenuText.text = "MatchMaking...";
    }

    void gameInit () {
        whoTurn = 0;
        turnCount = 0;
        turnIcons[0].SetActive (true);
        turnIcons[1].SetActive (false);
        for (int i = 0; i < Spaces.Length; i++) {
            Spaces[i].interactable = true;
            Spaces[i].GetComponent<Image> ().sprite = null;

        }
        for (int i = 0; i < markTabel.Length; i++) {
            markTabel[i] = -100; //noBody
        }
    }

    public void onCellClick (int buttonNumber) {
        //Spaces[buttonNumber].image.sprite = playIcons[whoTurn];
        //Spaces[buttonNumber].interactable = false;

        //markTabel[buttonNumber] = whoTurn + 1; // mark button for player // +1 is for logics
        //turnCount++;
        //if (turnCount > 4)
            //winnerCheck ();
    }

    void winnerCheck () {
        int s1 = markTabel[0] + markTabel[1] + markTabel[2];
        int s2 = markTabel[3] + markTabel[4] + markTabel[5];
        int s3 = markTabel[6] + markTabel[7] + markTabel[8];
        int s4 = markTabel[0] + markTabel[3] + markTabel[6];
        int s5 = markTabel[1] + markTabel[4] + markTabel[7];
        int s6 = markTabel[2] + markTabel[5] + markTabel[8];
        int s7 = markTabel[0] + markTabel[4] + markTabel[8];
        int s8 = markTabel[0] + markTabel[4] + markTabel[6];
        var results = new int[] { s1, s2, s3, s4, s5, s6, s7, s8 };
        for (int i = 0; i < results.Length; i++) {
            if (results[i] == 3 * (whoTurn + 1)) {
                // say winner
                WinnerText.gameObject.SetActive (true);
                WinnerText.text = "Player " + (whoTurn == 0 ? "X" : "O") + " wins!";
                if (whoTurn == 0) {
                    xPlayerScore++;
                    xPlayerTextScore.text = xPlayerScore.ToString ();
                } else {
                    oPlayerScore++;
                    oPlayerTextScore.text = oPlayerScore.ToString ();
                }
                for (int j = 0; j < Spaces.Length; j++) {
                    Spaces[j].interactable = false;
                }
                return;
            }
        }
    }

    //public void OnJoin(JoinData joinData, JoinType type)
    //{
    //    // 
    //}

    //public void OnLeave(Leave leave)
    //{

    //}

    //public void OnTakeTurn(Turn turn)
    //{
    //    int buttonNumber = int.Parse(turn.Data);
    //    Spaces[buttonNumber].image.sprite = playIcons[whoTurn];
    //    Spaces[buttonNumber].interactable = false;

    //    markTabel[buttonNumber] = whoTurn + 1; // mark button for player // +1 is for logics
    //    turnCount++;
    //    if (turnCount > 4)
    //        winnerCheck();
    //    whoTurn = (turn.WhoTakeTurn.User.IsMe ? 1 : 0);
    //    turnIcons[0].SetActive(whoTurn == 0);
    //    turnIcons[1].SetActive(whoTurn == 1);

    //}

    //public void OnChooseNext(Member whoIsNext)
    //{
    //    for (int i = 0; i < 9; i++)
    //    {
    //        Spaces[i].interactable = whoIsNext.User.IsMe;
    //    }
    //}

    //public void OnFinish(Finish finish)
    //{
    //}

    //public void OnComplete(Complete complete)
    //{
    //}

    //public void OnTurnBasedError(string error)
    //{
    //}

    //public void OnRoomMembersDetail(List<Member> members)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void OnSuccess()
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void OnAvailableRooms(List<Room> rooms)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void OnAutoMatchUpdate(AutoMatchStatus status, List<User> players)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void OnInviteInbox(List<Invite> invites)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void OnInviteSend()
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void OnFindUsers(List<User> users)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void OnInviteReceive(Invite invite)
    //{
    //    throw new System.NotImplementedException();
    //}
}

