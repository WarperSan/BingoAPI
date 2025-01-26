## Bingo Documentation
Here is the documentation I gathered from BingoSync

## Create a room
Method: `POST`

URL: https://bingosync.com/

Body:
```json
{
  "room_name": "ROOM NAME",
  "passphrase": "ROOM PASSWORD",
  "nickname": "USERNAME",
  "game_type": 18, // CUSTOM
  "variant_type": 18|172, // 18 = Fixed Board, 172 = Randomized
  "custom_json": "JSON OF THE BOARD",
  "lockout_mode": 1|2, // 1 = Non-Lockout, 2 = Lockout
  "seed": ""|"123123", // Leave empty if none
  "is_spectator": true|false,
  "hide_card": true|false,
  "csrfmiddlewaretoken": "CSRF secret token"
}
```

Response: `none`

## Join a room
Method: `POST`

URL: https://bingosync.com/api/join-room

Body:
```json
{
    "room": "ROOM ID",
    "password": "ROOM PASSWORD",
    "nickname": "USERNAME",
    "is_spectator": true|false
}
```

Response:
```json
{
    "socket_key": "SOCKET_KEY"
}
```

## Get board
Method: `GET`

URL: https://bingosync.com/room/ROOM_ID/board

Body: `none`

Response:
```json
[
  {
      "name": "NAME OF THE OBJECTIVE",
      "slot": "SLOT INDEX (#)",
      "colors": "blank OR COLORS SEPARATE BY A SPACE (red blue purple)"
  },
]
```

## Mark a square
Method: `PUT`

URL: https://bingosync.com/api/select

Body:
```json
{
    "room": "ROOM_ID",
    "slot": "SLOT INDEX (#)",
    "color": "COLOR NAME",
    "remove_color": false
}
```

Response: `none`

## Clear a square
Method: `PUT`

URL: https://bingosync.com/api/select

Body:
```json
{
    "room": "ROOM_ID",
    "slot": "SLOT INDEX (#)",
    "color": "COLOR NAME",
    "remove_color": true
}
```

Response: `none`

## Change team
Method: `PUT`

URL: https://bingosync.com/api/color

Body:
```json
{
    "room": "ROOM_ID",
    "color": "COLOR NAME"
}
```

Response: `none`

Possible colors:
- pink
- red
- orange
- brown
- yellow
- green
- teal
- blue
- navy
- purple

## Send message
Method: `PUT`

URL: https://bingosync.com/api/chat

Body:
```json
{
  "room": "ROOM ID",
  "text": "MESSAGE",
}
```

Response: `none`

## Chat Feed
Method: `GET`

URL: https://bingosync.com/room/ROOM_ID/feed

Body: `none`

Response:
```json
{
  "events": [
    ...
  ],
  "allIncluded": true
}
```