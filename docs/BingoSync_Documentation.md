# BingoSync API Documentation

This document covers the basic endpoints of BingoSync. This information has been gathered by me and, as time goes by,
become obsolete.

## Create a room

### Endpoint:

`POST https://bingosync.com/`

### Request Body:

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
  "csrfmiddlewaretoken": "CSRF secret token"
}
```

`game_type` is set to 18, because it represents the `Custom` game type.

If `variant_type` is set to 18, the game will generate as a `Fixed Board`. If it is set to 172, the game will generate
as a `Randomized`.

If `lockout_mode` is set to 1, the room will generate as a `Non-Lockout`. If it is set to 2, the room will generate as a
`Lockout`.

Unless `seed` is set to a number, the game will generate a seed automatically.

In order to make the request work, you need the cookie `csrftoken` from calling `GET https://bingosync.com`, and you
need to set both `csrfmiddlewaretoken` and the header `X-CSRFToken` to te secret CSRF token.

### Response Body:

`none`

## Join a room

### Endpoint:

`POST https://bingosync.com/api/join-room`

### Request Body:

```json
{
    "room": "ROOM ID",
    "password": "ROOM PASSWORD",
    "nickname": "USERNAME",
    "is_spectator": true
}
```

### Response Body:

```json
{
    "socket_key": "SOCKET_KEY"
}
```

## Get board

### Endpoint:

`GET https://bingosync.com/room/ROOM_ID/board`

You need to replace `ROOM_ID` with the actual code of the room.

### Request Body:

`none`

### Response Body:

```json
[
  {
      "name": "NAME OF THE OBJECTIVE",
      "slot": "SLOT INDEX (#)",
      "colors": "blank OR COLORS SEPARATE BY A SPACE (red blue purple)"
  }
]
```

## Mark a square

### Endpoint:

`PUT https://bingosync.com/api/select`

### Request Body:

```json
{
    "room": "ROOM_ID",
    "slot": "SLOT INDEX (#)",
    "color": "COLOR NAME",
    "remove_color": false
}
```

### Response Body:

`none`

## Clear a square

### Endpoint:

`PUT https://bingosync.com/api/select`

### Request Body:

```json
{
    "room": "ROOM_ID",
    "slot": "SLOT INDEX (#)",
    "color": "COLOR NAME",
    "remove_color": true
}
```

### Response Body:

`none`

## Change team

### Endpoint:

`PUT https://bingosync.com/api/color`

### Request Body:

```json
{
    "room": "ROOM_ID",
    "color": "COLOR NAME"
}
```

### Response Body:

`none`

## Send message

### Endpoint:

`PUT https://bingosync.com/api/chat`

### Request Body:

```json
{
  "room": "ROOM ID",
  "text": "MESSAGE"
}
```

### Request Response:

`none`

## Chat Feed

### Endpoint:

`GET https://bingosync.com/room/ROOM_ID/feed`

You need to replace `ROOM_ID` with the actual code of the room.

### Request Body:

`none`

### Response Body:

```json
{
  "events": [
    
  ],
  "allIncluded": true
}
```