using System.Net;

public static partial class AuthController
{
    [Route("/api/getrooms", "GET")]
    public static async Task GetRooms(HttpListenerContext context)
    {
#if DEBUG
        RoomManager.CreateRoom();
        RoomManager.CreateRoom();
        RoomManager.CreateRoom();
#endif
        Room[] rooms = RoomManager.GetRooms();
        await SendResponse(context, 200, rooms);
    }
}