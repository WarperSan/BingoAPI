using BepInEx;
using BingoAPI.Conditions;
using BingoAPI.Conditions.BuiltIn;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Networking;
using BingoAPI.Session;
using UnityEngine;

namespace BingoAPI;

[BepInAutoPlugin]
internal partial class Plugin : BaseUnityPlugin
{
	private void Start()
	{
		Log.SetLogger(Logger);

		ConditionRegistry.Register("AND",
			data =>
			{
				var children = data.GetChildren();

				return new AndCondition(children);
			});

		ConditionRegistry.Register("OR",
			data =>
			{
				var children = data.GetChildren();

				return new OrCondition(children);
			});

		ConditionRegistry.Register("NOT",
			data =>
			{
				var child = data.GetChild();

				return new NotCondition(child);
			});

		ConditionRegistry.Register("SOME",
			data =>
			{
				var children = data.GetChildren();
				var amount = data.GetOptionalParam<uint>("amount", 1);

				return new SomeCondition(amount, children);
			});

		A();
	}

	private async Task A()
	{
		using var session = new BingoSession();

		try
		{
			await session.JoinRoom(new JoinRoomSettings
			{
				Code = "cZyTrIhlSv6t8xaPDUOSuw",
				Password = "abccba",
				IsSpectator = false,
				Nickname = "OwO",
			});

			await session.SendMessage(
				"Hello World!"
			);

			while (Application.isPlaying)
			{
				/*var board = await api.GetBoard("cZyTrIhlSv6t8xaPDUOSuw");

				foreach (var square in board.Squares)
					Log.Info(square.Index + ": " + square.Name + "(" + string.Join(',', square.Teams) + ")");

				Log.Info("---");

				await api.MarkSquare(
					"cZyTrIhlSv6t8xaPDUOSuw",
					Team.Blue,
					UnityEngine.Random.Range(0, 25)
				);*/
				await Task.Delay(1000);
			}
		}
		catch (Exception e)
		{
			Log.Error(e.Message);
		}
	}
}
