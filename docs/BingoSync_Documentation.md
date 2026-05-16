# BingoSync Documentation

This document covers the network information of BingoSync, including API endpoints and socket events.

> [!WARN]
> This information has been gathered manually and may become obsolete over time.

---

## API Endpoints

### Create a Room

#### Endpoint

`POST https://bingosync.com/`

#### Request Body

```json
{
	"room_name": "ROOM NAME",
	"passphrase": "ROOM PASSWORD",
	"nickname": "USERNAME",
	"game_type": 18,
	"variant_type": 18,
	"custom_json": "JSON OF THE BOARD",
	"lockout_mode": 1,
	"seed": "",
	"is_spectator": true,
	"hide_card": true,
	"csrfmiddlewaretoken": "CSRF SECRET TOKEN"
}
```

#### Fields

| Field                 | Type   | Description                                          |
|-----------------------|--------|------------------------------------------------------|
| `room_name`           | string | Display name of the room                             |
| `passphrase`          | string | Password to join the room                            |
| `nickname`            | string | Display name of the player creating the room         |
| `game_type`           | number | `18` for Custom game                                 |
| `variant_type`        | number | `18` for Fixed Board, `172` for Randomized           |
| `custom_json`         | string | JSON string of the board goals                       |
| `lockout_mode`        | number | `1` for Non-Lockout, `2` for Lockout                 |
| `seed`                | string | Seed for board generation. Leave empty for automatic |
| `is_spectator`        | bool   | Whether the creator joins as a spectator             |
| `hide_card`           | bool   | Whether the card is hidden until revealed            |
| `csrfmiddlewaretoken` | string | CSRF token                                           |

#### Notes

To make this request work, you must first call `GET https://bingosync.com` to obtain the `csrftoken` cookie. Both the
`csrfmiddlewaretoken` body field and the `X-CSRFToken` request header must be set to this token.

#### Response Body

`none`

---

### Join a Room

#### Endpoint

`POST https://bingosync.com/api/join-room`

#### Request Body

```json
{
    "room": "ROOM ID",
    "password": "ROOM PASSWORD",
    "nickname": "USERNAME",
    "is_spectator": true
}
```

#### Fields

| Field          | Type   | Description                    |
|----------------|--------|--------------------------------|
| `room`         | string | ID of the room to join         |
| `password`     | string | Password of the room           |
| `nickname`     | string | Display name of the player     |
| `is_spectator` | bool   | Whether to join as a spectator |

#### Response Body

```json
{
    "socket_key": "SOCKET_KEY"
}
```

| Field        | Type   | Description                                      |
|--------------|--------|--------------------------------------------------|
| `socket_key` | string | Key used to authenticate the WebSocket handshake |

---

### Get Board

#### Endpoint

`GET https://bingosync.com/room/ROOM_ID/board`

Replace `ROOM_ID` with the actual room code.

#### Request Body

`none`

#### Response Body

```json
[
	{
		"name": "NAME OF THE OBJECTIVE",
		"slot": "slot1",
		"colors": "blank OR COLORS SEPARATED BY A SPACE (red blue purple)"
	},
	...
]
```

| Field    | Type   | Description                                                         |
|----------|--------|---------------------------------------------------------------------|
| `name`   | string | Display name of the goal                                            |
| `slot`   | string | Slot identifier in the format `slot#` where `#` is 1–25             |
| `colors` | string | `blank` if unmarked, otherwise team color names separated by spaces |

---

### Mark a Square

#### Endpoint

`PUT https://bingosync.com/api/select`

#### Request Body

```json
{
	"room": "ROOM_ID",
	"slot": "slot1",
	"color": "COLOR NAME",
	"remove_color": false
}
```

#### Response Body

`none`

#### Fields

| Field          | Type   | Description                                             |
|----------------|--------|---------------------------------------------------------|
| `room`         | string | ID of the room                                          |
| `slot`         | string | Slot identifier in the format `slot#` where `#` is 1–25 |
| `color`        | string | Team color name                                         |
| `remove_color` | bool   | `false` to mark                                         |

---

### Clear a Square

#### Endpoint

`PUT https://bingosync.com/api/select`

#### Request Body

```json
{
    "room": "ROOM_ID",
    "slot": "slot1",
    "color": "COLOR NAME",
    "remove_color": true
}
```

#### Fields

| Field          | Type   | Description                                             |
|----------------|--------|---------------------------------------------------------|
| `room`         | string | ID of the room                                          |
| `slot`         | string | Slot identifier in the format `slot#` where `#` is 1–25 |
| `color`        | string | Team color name                                         |
| `remove_color` | bool   | `true` to clear                                         |

#### Response Body

`none`

---

### Change Team

#### Endpoint

`PUT https://bingosync.com/api/color`

#### Request Body

```json
{
    "room": "ROOM_ID",
    "color": "COLOR NAME"
}
```

#### Fields

| Field   | Type   | Description         |
|---------|--------|---------------------|
| `room`  | string | ID of the room      |
| `color` | string | New team color name |

#### Response Body

`none`

---

### Send Message

#### Endpoint

`PUT https://bingosync.com/api/chat`

#### Request Body

```json
{
    "room": "ROOM ID",
    "text": "MESSAGE"
}
```

#### Fields

| Field  | Type   | Description     |
|--------|--------|-----------------|
| `room` | string | ID of the room  |
| `text` | string | Message to send |

#### Response Body

`none`

---

## Socket Events

### Connection

`wss://sockets.bingosync.com/broadcast`

After opening the WebSocket, send the handshake message to authenticate:

```json
{
    "socket_key": "SOCKET_KEY"
}
```

---

### Player Connected

Sent when a player joins the room.

```json
{
	"type": "connection",
	"event_type": "connected",
	"player": {
		"uuid": "-JicCjF8RKOu7fBWaWn9SA",
		"name": "c",
		"color": "green",
		"is_spectator": false
	},
	"player_color": "green",
	"timestamp": 1778427917.947448,
	"room": "cZyTrIhlSv6t8xaPDUOSuw"
}
```

#### Fields (Connected & Disconnected)

| Field                 | Type   | Description                       |
|-----------------------|--------|-----------------------------------|
| `event_type`          | string | `connected` when joining          |
| `player.uuid`         | string | Unique identifier of the player   |
| `player.name`         | string | Display name of the player        |
| `player.color`        | string | Current team color of the player  |
| `player.is_spectator` | bool   | Whether the player is a spectator |
| `player_color`        | string | Same as `player.color`            |
| `timestamp`           | number | Unix timestamp                    |
| `room`                | string | ID of the room                    |

---

### Player Disconnected

Sent when a player leaves the room.

```json
{
	"type": "connection",
	"event_type": "disconnected",
	"player": {
		"uuid": "2DlxR5PkT8eAdmFopVN4qg",
		"name": "Player",
		"color": "red",
		"is_spectator": false
	},
	"player_color": "red",
	"timestamp": 1778428735.196945,
	"room": "cZyTrIhlSv6t8xaPDUOSuw"
}
```

#### Fields

| Field                 | Type   | Description                       |
|-----------------------|--------|-----------------------------------|
| `event_type`          | string | `disconnected` when leaving       |
| `player.uuid`         | string | Unique identifier of the player   |
| `player.name`         | string | Display name of the player        |
| `player.color`        | string | Current team color of the player  |
| `player.is_spectator` | bool   | Whether the player is a spectator |
| `player_color`        | string | Same as `player.color`            |
| `timestamp`           | number | Unix timestamp                    |
| `room`                | string | ID of the room                    |

---

### Goal Marked / Cleared

Sent when a player marks or clears a square.

```json
{
    "type": "goal",
    "player": {
        "uuid": "-JicCjF8RKOu7fBWaWn9SA",
        "name": "c",
        "color": "green",
        "is_spectator": false
    },
    "square": {
        "name": "DISPLAY_NAME",
        "slot": "slot4",
        "colors": "green"
    },
    "player_color": "green",
    "color": "green",
    "remove": false,
    "timestamp": 1778428516.704848,
    "room": ""
}
```

#### Fields

| Field                 | Type   | Description                                      |
|-----------------------|--------|--------------------------------------------------|
| `player.uuid`         | string | Unique identifier of the player                  |
| `player.name`         | string | Display name of the player                       |
| `player.color`        | string | Current team color of the player                 |
| `player.is_spectator` | bool   | Whether the player is a spectator                |
| `square.name`         | string | Display name of the goal                         |
| `square.slot`         | string | Slot identifier in the format `slot#`            |
| `square.colors`       | string | Current colors on the square after this event    |
| `player_color`        | string | Team color of the player who triggered the event |
| `color`               | string | Same as `player_color`                           |
| `remove`              | bool   | `false` if marked, `true` if cleared             |
| `timestamp`           | number | Unix timestamp                                   |
| `room`                | string | ID of the room                                   |

---

### Color Changed

Sent when a player changes their team color.

```json
{
    "type": "color",
    "player": {
        "uuid": "-JicCjF8RKOu7fBWaWn9SA",
        "name": "c",
        "color": "purple",
        "is_spectator": false
    },
    "player_color": "green",
    "color": "purple",
    "timestamp": 1778428663.558285,
    "room": "cZyTrIhlSv6t8xaPDUOSuw"
}
```

#### Fields

| Field                 | Type   | Description                               |
|-----------------------|--------|-------------------------------------------|
| `player.uuid`         | string | Unique identifier of the player           |
| `player.name`         | string | Display name of the player                |
| `player.color`        | string | The **new** team color of the player      |
| `player.is_spectator` | bool   | Whether the player is a spectator         |
| `player_color`        | string | The **previous** team color of the player |
| `color`               | string | Same as `player.color`    |
| `timestamp`           | number | Unix timestamp                            |
| `room`                | string | ID of the room                            |

---

### Chat Message

Sent when a player sends a message in the room.

```json
{
	"type": "chat",
	"player": {
		"uuid": "-JicCjF8RKOu7fBWaWn9SA",
		"name": "c",
		"color": "green",
		"is_spectator": false
	},
	"player_color": "green",
	"text": "TEXT",
	"timestamp": 1778428516.704848,
	"room": "cZyTrIhlSv6t8xaPDUOSuw"
}
```

#### Fields

| Field                 | Type   | Description                       |
|-----------------------|--------|-----------------------------------|
| `player.uuid`         | string | Unique identifier of the player   |
| `player.name`         | string | Display name of the player        |
| `player.color`        | string | Current team color of the player  |
| `player.is_spectator` | bool   | Whether the player is a spectator |
| `player_color`        | string | Same as `player.color`            |
| `text`                | string | Content of the message            |
| `timestamp`           | number | Unix timestamp                    |
| `room`                | string | ID of the room                    |
