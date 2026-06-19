using System.Runtime.Serialization;
using BingoAPI.Networking.Clients;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoApiClient.CreateRoom"/>
/// </summary>
internal record CreateRoomRequest
{
	private const int RANDOMIZED_VARIANT_TYPE = 172;
	private const int FIXED_BOARD_VARIANT_TYPE = 18;
	private const int LOCKOUT_MODE = 2;
	private const int NON_LOCKOUT_MODE = 1;

	[DataMember(Name = "room_name")]
	public required string RoomName;

	[DataMember(Name = "passphrase")]
	public required string Password;

	[DataMember(Name = "nickname")]
	public required string Nickname;

	[DataMember(Name = "game_type")]
	public int GameType => 18;

	[DataMember(Name = "lockout_mode")]
	private int LockoutMode => IsLockout ? LOCKOUT_MODE : NON_LOCKOUT_MODE;

	public required bool IsLockout;

	// TODO: Change this to be a int
	[DataMember(Name = "seed")]
	public required string Seed;

	// TODO: Make this a parameter
	[DataMember(Name = "is_spectator")]
	public bool IsSpectator => false;

	[DataMember(Name = "variant_type")]
	private int VariantType => IsRandomized ? RANDOMIZED_VARIANT_TYPE : FIXED_BOARD_VARIANT_TYPE;

	public required bool IsRandomized;

	[DataMember(Name = "required")]
	public required string Board;

	// TODO: Make this a parameter
	[DataMember(Name = "hide_card")]
	public bool HideCard => false;

	[DataMember(Name = "csrfmiddlewaretoken")]
	public required string CreationToken;
}
