## Bingo Documentation
Here is the documentation I gathered from BingoSync

## Join a room
Method: `POST`

Url: https://bingosync.com/api/join-room

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
    "socket_key": "SOCKT_KEY"
}
```

## Get board
Method: `GET`

Url: https://bingosync.com/room/ROOM_ID/board

Body: `none`

Response:
```json
[
  {
      "name": "NAME OF THE OBJECTIVE",
      "slot": "SLOT INDEX (slot1)",
      "colors": "blank OR COLORS SEPARATE BY A SPACE (red blue purple)"
  },
]
```

## Mark a square
Method: `PUT`

Url: https://bingosync.com/api/select

Body:
```json
{
    "room": "ROOM_ID",
    "slot": "SLOT INDEX (1)",
    "color": "COLOR NAME",
    "remove_color": false
}
```

Response: `none`

## Clear a square
Method: `PUT`

Url: https://bingosync.com/api/select

Body:
```json
{
    "room": "ROOM_ID",
    "slot": "SLOT INDEX (1)",
    "color": "COLOR NAME",
    "remove_color": true
}
```

Response: `none`

## Change color
Method: `PUT`

Url: https://bingosync.com/api/color

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

## Chat Feed
Method: `GET`

Url: https://bingosync.com/room/ROOM_ID/feed

Body: `none`

Response:
```json
[
  "events": [ ... ],
  "allIncluded": true
]
```