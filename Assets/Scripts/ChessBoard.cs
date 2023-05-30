using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ChessBoard : MonoBehaviour
{
    /* EDITOR VARIABLES */

    [Header("Board Settings")]
    [SerializeField] private int X_TILES;
    [SerializeField] private int Y_TILES;

    [Header("Board Graphics and Assets")]
    [SerializeField] private Material Texture;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float offsetForY = 0.2f;
    [SerializeField] private Vector3 Center = Vector3.zero;
    [SerializeField] private float dragOffset = 1.5f;

    [Header("Prefab Assets and Material")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterial;

    // ----------------------------------------------------------------------------------------------------------------- //

    // Game Variables

    private Vector3 extent;
    private GameObject[,] tiles;
    private Camera CameraAsMain;
    private Vector2Int _Hovering_;
    private ChessPieceClass[,] ActivePiece;
    private ChessPieceClass _Dragging_;
    private List<ChessPieceClass> DeadPieces_White = new List<ChessPieceClass>();
    private List<ChessPieceClass> DeadPieces_Red = new List<ChessPieceClass>();
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    public bool _Turn_;
    public TMP_Text TurnDisplayText;
    // ----------------------------------------------------------------------------------------------------------------- //

    // Functions

    protected void Awake()
    {
        GenerateGrid(1, X_TILES, Y_TILES);
        GenerateThePieces();
        putTraps(1);
        positionAllPieces();
    }

    protected void Update()
    {
        if (!CameraAsMain)
        {
            CameraAsMain = Camera.main;
            return;
        }

        RaycastHit MyRayCast;
        Ray ray = CameraAsMain.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out MyRayCast, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            Vector2Int collisionPosition = TilePosition(MyRayCast.transform.gameObject);

            if (_Hovering_ == -Vector2Int.one)
            {
                _Hovering_ = collisionPosition;
                tiles[collisionPosition.x, collisionPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            if (_Hovering_ != -Vector2Int.one)
            {
                tiles[_Hovering_.x, _Hovering_.y].layer = (IsValidMove(ref availableMoves, _Hovering_)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                _Hovering_ = collisionPosition;
                tiles[collisionPosition.x, collisionPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            else
            {
                if (_Hovering_ != -Vector2Int.one)
                {
                    tiles[_Hovering_.x, _Hovering_.y].layer = (IsValidMove(ref availableMoves, _Hovering_)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                    _Hovering_ = -Vector2Int.one;
                }
                if (_Dragging_ && Input.GetMouseButtonUp(0))
                {
                    _Dragging_ = ActivePiece[_Dragging_.currentX, _Dragging_.currentY];
                    _Dragging_ = null;
                    RemoveHighlight();
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (ActivePiece[collisionPosition.x, collisionPosition.y] != null)
                {
                    if ((ActivePiece[collisionPosition.x, collisionPosition.y].team == 1 && _Turn_) || (ActivePiece[collisionPosition.x, collisionPosition.y].team == 0 && !_Turn_))
                    {
                        _Dragging_ = ActivePiece[collisionPosition.x, collisionPosition.y];

                        availableMoves = _Dragging_.GetAvailableMoves(ref ActivePiece, X_TILES, Y_TILES);
                        AddHighlightTiles();
                    }
                }
            }
            if (_Dragging_ != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previous_postion = new Vector2Int(_Dragging_.currentX, _Dragging_.currentY);
                bool moveRegular = MoveTo(_Dragging_, collisionPosition.x, collisionPosition.y);

                if (!moveRegular)
                {
                    _Dragging_.SetPosition(GetTileCenter(previous_postion.x, previous_postion.y));
                }
                _Dragging_ = null;
                RemoveHighlight();
            }

            if (_Dragging_)
            {
                Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * offsetForY);
                float distance = 0.0f;
                if (horizontalPlane.Raycast(ray, out distance))
                {
                    _Dragging_.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
                }
            }
        }
    }

    private void GenerateGrid(float tileGenScale, int tilesX, int tilesY)
    {
        offsetForY += transform.position.y;
        extent = new Vector3((tilesX / 2) * tileGenScale, 0, (tilesY / 2) * tileGenScale) + Center;

        tiles = new GameObject[tilesX, tilesY];
        for (int x = 0; x < tilesX; x++)
        {
            for (int y = 0; y < tilesY; y++)
            {
                tiles[x, y] = CreateTile(tileGenScale, x, y);
            }
        }
    }

    private GameObject CreateTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject($"SQUARE x:{x}, y:{y}, z:{0}");
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();

        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = Texture;

        Vector3[] lateral = new Vector3[4];

        lateral[0] = new Vector3(x * tileSize, offsetForY, y * tileSize) - extent;

        lateral[1] = new Vector3(x * tileSize, offsetForY, (y + 1) * tileSize) - extent;

        lateral[2] = new Vector3((x + 1) * tileSize, offsetForY, y * tileSize) - extent;

        lateral[3] = new Vector3((x + 1) * tileSize, offsetForY, (y + 1) * tileSize) - extent;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = lateral;
        mesh.triangles = tris;

        tileObject.layer = LayerMask.NameToLayer("Tile");

        tileObject.AddComponent<BoxCollider>();

        // mesh.RecalculateNormals();

        return tileObject;
    }

    private void GenerateThePieces()
    {
        ActivePiece = new ChessPieceClass[X_TILES, Y_TILES];

        int White = 0;
        int Red = 1;

        //pawns
        for (int i = 0; i < X_TILES / 2; i++)
        {
            ActivePiece[i, 0] = GenerateOnePiece(ChessPieceType.Pawn, White);
        }

        for (int i = 0; i < X_TILES / 2; i++)
        {
            ActivePiece[i + (X_TILES / 2), 0] = GenerateOnePiece(ChessPieceType.Pawn, Red);
        }
    }

    private ChessPieceClass GenerateOnePiece(ChessPieceType type, int team)
    {
        ChessPieceClass ChessObj = Instantiate(prefabs[1], transform).GetComponent<ChessPieceClass>();

        ChessObj.type = type;
        ChessObj.team = team;
        ChessObj.GetComponent<MeshRenderer>().material = (int)type <= 1 ? teamMaterial[team] : teamMaterial[2];
        return ChessObj;
    }

    private void positionAllPieces()
    {
        for (int x = 0; x < X_TILES; x++)
        {
            for (int y = 0; y < Y_TILES; y++)
            {
                if (ActivePiece[x, y] != null)
                {
                    positionOnePiece(x, y, true);
                }
            }
        }
    }

    private void positionOnePiece(int x, int y, bool force = false)
    {
        ActivePiece[x, y].currentX = x;
        ActivePiece[x, y].currentY = y;
        ActivePiece[x, y].SetPosition(GetTileCenter(x, y), force);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, offsetForY, y * tileSize) - extent + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    private bool MoveTo(ChessPieceClass cp, int x, int y)
    {
        if (!IsValidMove(ref availableMoves, new Vector2(x, y)))
        {
            return false;
        }

        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        int type = ActivePiece[x, y] != null ? (int)ActivePiece[x, y].type : 0;
        ActivePiece[x, y] = cp;
        ActivePiece[previousPosition.x, previousPosition.y] = null;

        int tempY = ActivePiece[x, y].trapEffects(ref ActivePiece, ref x, ref y, ref _Turn_, type);

        if (tempY == 2)
        {
            ActivePiece[x, y] = null;
        }
        else if (tempY == -1 || tempY == 3)
        {
            if (tempY == 3)
            {
                ActivePiece[previousPosition.x, previousPosition.y] = GenerateOnePiece(ChessPieceType.Pawn, cp.team);
                positionOnePiece(previousPosition.x, previousPosition.y);
            }
            positionOnePiece(x, y);
        }
        else
        {
            ActivePiece[x, tempY] = cp;
            ActivePiece[x, y] = null;
            y = tempY;
            positionOnePiece(x, tempY, true);
        }

        if (y == Y_TILES - 1)
        {
            SceneManager.LoadScene(4);
        }

        _Turn_ = !_Turn_;

        TurnDisplayText.text = (_Turn_ ? "Black" : "White") + " is playing!";
        TurnDisplayText.color = (_Turn_ ? Color.black : Color.white);

        return true;
    }

    private void AddHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }

    private void RemoveHighlight()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");

        }
        availableMoves.Clear();
    }

    private Vector2Int TilePosition(GameObject collisionInformation)
    {
        for (int x = 0; x < X_TILES; x++)
        {
            for (int y = 0; y < Y_TILES; y++)
            {
                if (tiles[x, y] == collisionInformation)
                    return new Vector2Int(x, y);
            }
        }
        return -Vector2Int.one;
    }

    private bool IsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y)
            {
                return true;
            }
        }
        return false;
    }
    //place 4 traps per row(2 for each side)

    private void putTraps(int lowY)
    {
        for (int y = lowY; y < 8; y++)
        {
            List<int> moves = new List<int> { 0, 1, 2, 3 };
            for (int i = 0; i < 2; i++)
            {
                int rand = Random.Range(0, moves.Count);
                int effect = Random.Range(0, 60);
                const int range = 19;

<<<<<<< HEAD
                if (effect < range)
                {
                    ActivePiece[moves[rand], y] = GenerateOnePiece(ChessPieceType.invinc, 0); // to only have two per game
                    ActivePiece[X_TILES / 2 + moves[rand], y] = GenerateOnePiece(ChessPieceType.invinc, 0);
=======
                if (effect < range) {
                    ActivePiece[moves[2], y] = GenerateOnePiece(ChessPieceType.invinc, 0); // to only have two per game
                    ActivePiece[X_TILES/2+moves[rand], y] = GenerateOnePiece(ChessPieceType.invinc, 0);
>>>>>>> 62d6a31154163663e68c4c1fc0d4ea973bf1c8d6
                }
                else if (effect < range * 2)
                {
                    ActivePiece[moves[rand], y] = GenerateOnePiece(ChessPieceType.die, 0);
                    ActivePiece[X_TILES / 2 + moves[rand], y] = GenerateOnePiece(ChessPieceType.die, 0);
                }
                else if (effect < range * 3)
                {
                    ActivePiece[moves[rand], y] = GenerateOnePiece(ChessPieceType.back, 0);
                    ActivePiece[X_TILES / 2 + moves[rand], y] = GenerateOnePiece(ChessPieceType.back, 0);
                }
                else if (effect < range * 4)
                {
                    ActivePiece[moves[rand], y] = GenerateOnePiece(ChessPieceType.paralyse, 0);
                    ActivePiece[X_TILES / 2 + moves[rand], y] = GenerateOnePiece(ChessPieceType.paralyse, 0);
                }
                else if (effect < range * 5)
                {
                    ActivePiece[moves[rand], y] = GenerateOnePiece(ChessPieceType.respawn, 0);
                    ActivePiece[X_TILES / 2 + moves[rand], y] = GenerateOnePiece(ChessPieceType.respawn, 0);
                }
                else if (effect < range * 6)
                {
                    ActivePiece[moves[rand], y] = GenerateOnePiece(ChessPieceType.extra, 0);
                    ActivePiece[X_TILES / 2 + moves[rand], y] = GenerateOnePiece(ChessPieceType.extra, 0);
                }
                else if (effect < range * 7)
                {
                    //5% chance of spawning
                    ActivePiece[moves[rand], y] = GenerateOnePiece(ChessPieceType.quit, 0);
                    ActivePiece[X_TILES / 2 + moves[rand], y] = GenerateOnePiece(ChessPieceType.quit, 0);
                }
                moves.Remove(moves[rand]);
            }
        }
    }
}
