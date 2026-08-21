using System.Runtime.Serialization;
using BingoAPI.Clients.BingoSync;
using BingoAPI.Configuration.Settings;

namespace BingoAPI.DTOs.BingoSync.CreateRoom;

/// <summary>
/// Model used as the payload of <see cref="BingoSyncApiClient.CreateRoom(CreateRoomSettings,CancellationToken)"/>
/// </summary>
internal sealed record Request
{
	private const int CUSTOM_GAME_TYPE = 18;
	private const int RANDOMIZED_VARIANT_TYPE = 172;
	private const int FIXED_BOARD_VARIANT_TYPE = 18;
	private const int LOCKOUT_MODE = 2;
	private const int NON_LOCKOUT_MODE = 1;

	[DataMember(Name = "room_name")]
	public required string RoomName { get; init; }

	[DataMember(Name = "passphrase")]
	public required string Password { get; init; }

	[DataMember(Name = "nickname")]
	public required string Nickname { get; init; }

	[DataMember(Name = "game_type")]
	public int GameType => CUSTOM_GAME_TYPE;

	[DataMember(Name = "lockout_mode")]
	private int LockoutMode => IsLockout ? LOCKOUT_MODE : NON_LOCKOUT_MODE;

	public required bool IsLockout { get; init; }

	// TODO: Change this to be a int
	[DataMember(Name = "seed")]
	public required string Seed { get; init; }

	[DataMember(Name = "is_spectator")]
	public required bool IsSpectator { get; init; }

	[DataMember(Name = "variant_type")]
	private int VariantType => IsRandomized ? RANDOMIZED_VARIANT_TYPE : FIXED_BOARD_VARIANT_TYPE;

	public required bool IsRandomized { get; init; }

	[DataMember(Name = "custom_json")]
	public required string Board { get; init; }

	[DataMember(Name = "hide_card")]
	public required bool HideCard { get; init; }

	// ReSharper disable once StringLiteralTypo
	[DataMember(Name = "csrfmiddlewaretoken")]
	public required string CreationToken { get; init; }
}
