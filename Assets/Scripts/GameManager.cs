using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private const string PLAYER_ID_PREFIX = "Player";

    public static GameManager instance;

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void RegisterPlayer(string _netID, Player _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;

        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    public void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }

    public Player GetPlayer(string _playerID)
    {
        return players[_playerID];
    }


    //private void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(200, 200, 200, 500));
    //    GUILayout.BeginVertical();

    //    foreach (string _playerID in players.Keys)
    //    {
    //        GUILayout.Label(_playerID + " _ " + players[_playerID].transform.name);
    //    }

    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();
    //}
}