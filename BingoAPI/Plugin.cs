using BepInEx;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Networking;
using UnityEngine;

namespace BingoAPI;

[BepInAutoPlugin]
internal partial class Plugin : BaseUnityPlugin
{
	private void Start()
	{
		Log.SetLogger(Logger);

		A();
	}

	private async Task A()
	{
		using var api = new BingoApiClient();
		using var socket = new BingoSocketClient();

		try
		{
			var key = await api.JoinRoom(new JoinRoomSettings
			{
				Code = "cZyTrIhlSv6t8xaPDUOSuw",
				Password = "abccba",
				IsSpectator = false,
				Nickname = "OwO",
			});

			await socket.Connect(key, Log.Info);

			await api.SendMessage(
				"cZyTrIhlSv6t8xaPDUOSuw",
				"Hello World!"
			);

			while (Application.isPlaying)
			{
				var board = await api.GetBoard("cZyTrIhlSv6t8xaPDUOSuw");

				foreach (var square in board.Squares)
					Log.Info(square.Index + ": " + square.Name + "(" + string.Join(',', square.Teams) + ")");

				Log.Info("---");
				await Task.Delay(1000);
			}
		}
		catch (Exception e)
		{
			Log.Error(e.Message);
		}
	}
}
