namespace BingoAPI.Bingo;

public static class Constants
{
    /// <summary>
    /// Height of the board
    /// </summary>
    public const int BINGO_HEIGHT = 5;
    
    /// <summary>
    /// Width of the board
    /// </summary>
    public const int BINGO_WIDTH = 5;
    
    /// <summary>
    /// Size of the board
    /// </summary>
    public const int BINGO_SIZE = BINGO_HEIGHT * BINGO_WIDTH;

    #region URLs

    public const string SOCKETS_URL = "wss://sockets.bingosync.com/broadcast";
    public const string BINGO_URL = "https://bingosync.com";
    public const string CREATE_ROOM_URL = BINGO_URL + "/";
    
    public const string GET_BOARD_URL = BINGO_URL + "/room/{0}/board";
    public const string FEED_URL = BINGO_URL + "/room/{0}/feed";
    
    public const string JOIN_ROOM_URL = BINGO_URL + "/api/join-room";
    public const string CHANGE_TEAM_URL = BINGO_URL + "/api/color";
    public const string SELECT_SQUARE_URL = BINGO_URL + "/api/select";
    public const string SEND_MESSAGE_URL = BINGO_URL + "/api/chat";
    public const string REVEAL_CARD_URL = BINGO_URL + "/api/revealed";
    public const string NEW_CARD_URL = BINGO_URL + "/api/new-card";
    
    #endregion
}