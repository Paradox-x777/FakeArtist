using FreeDraw;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Players")] 
    public int PlayersInGame;    
    public int ActivePlayer;
    public int FakeArtist;

    [Header("UI")]
    public GameObject Frame;
    public GameObject PlayerList;
    public TextMeshProUGUI WordToGuess;    

    [Header("Components")]
    public PhotonView photonView;
   
    public static GameManager instance;

    private PlayerListController PlayerListController;
    private Texture2D image => Frame.GetComponent<SpriteRenderer>().sprite.texture;
    private int numberOfDrawnLines;
    public List<Color> PenColors = new List<Color>()
    {
        Color.red, Color.green, Color.blue,Color.magenta
    };

    private List<string> WordsToGuess = new List<string>() {
        "Apfel", "Haus", "Staubsauger", "Fußball", "Kühlschrank"
    };


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Random.InitState((int)System.DateTime.Now.Ticks);        
        ActivePlayer = 1;
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }
        
    [PunRPC]
    void ImInGame()
    {
        PlayersInGame++;
        Drawable.Pen_Colour = PenColors[PhotonNetwork.LocalPlayer.ActorNumber - 1];
        if (PlayersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        PlayerListController = PlayerList.GetComponent<PlayerListController>();
        PlayerListController.photonView.RPC("UpdateList", RpcTarget.All);
        photonView.RPC("SetActivePlayer", RpcTarget.All);
        Drawable.OnPenReleased += PlayersPenReleased;
    }

    public void NewRound()
    {
        var newWord = WordsToGuess[Random.Range(0, WordsToGuess.Count())];
        var newFakeArtist = Random.Range(1, PhotonNetwork.PlayerList.Length + 1);
        photonView.RPC("SetupNewRound", RpcTarget.All, newFakeArtist, newWord);  
    }

    [PunRPC]
    private void SetupNewRound(int newFakeArtist, string newWord)
    {
        numberOfDrawnLines = 0;
        Frame.GetComponent<Drawable>().ResetCanvas();
        FakeArtist = newFakeArtist;
        WordToGuess.text = PhotonNetwork.LocalPlayer.ActorNumber == FakeArtist ? "" : newWord;        
    }

    void PlayersPenReleased()
    {
        photonView.RPC("LineWasDrawn", RpcTarget.All);                         
    }

    [PunRPC]
    public void LineWasDrawn()
    {                         
        numberOfDrawnLines++;
        if (numberOfDrawnLines == 2)
        {
            NextPlayer();
        }        
    }
   

    public void NextPlayer()
    {        
        numberOfDrawnLines = 0;

        if (ActivePlayer < PhotonNetwork.PlayerList.Length)        
            ActivePlayer++;        
        else       
            ActivePlayer = 1;        

        photonView.RPC("SetActivePlayer", RpcTarget.All);
        PlayerListController.photonView.RPC("UpdateList", RpcTarget.All);
    }

    [PunRPC]
    void SetActivePlayer()
    {     
        Drawable.IsEnabled = PhotonNetwork.LocalPlayer.ActorNumber == ActivePlayer;        
    }
}
