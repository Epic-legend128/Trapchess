using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{

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
    private Vector3 extent;
    private GameObject[,] tiles;
    private Camera CameraAsMain;
    private Vector2Int _Hovering_;
    private ChessPieceClass[,] ActivePiece;
    private ChessPieceClass _Dragging_;
    private List<ChessPieceClass> DeadPieces_White = new List<ChessPieceClass>();
    private List<ChessPieceClass> DeadPieces_Red = new List<ChessPieceClass>();
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private bool _Turn_;

    // ----------------------------------------------------------------------------------------------------------------- //

    private void Awake()
    {
        GenerateGrid(1, X_TILES, Y_TILES);
        GenerateThePieces();
        positionAllPieces();
    }

    private void Update()
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
                    if ((ActivePiece[collisionPosition.x, collisionPosition.y].team == 0 && _Turn_ == true)|| (ActivePiece[collisionPosition.x, collisionPosition.y].team ==1 && _Turn_ == false))
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

            if (_Dragging_ == true)
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

        //pieces in a regular chess board
        /* ActivePiece[0, 0] = GenerateOnePiece(ChessPieceType.Rook, White);
        ActivePiece[1, 0] = GenerateOnePiece(ChessPieceType.Knight, White);
        ActivePiece[2, 0] = GenerateOnePiece(ChessPieceType.Bishop, White);
        ActivePiece[3, 0] = GenerateOnePiece(ChessPieceType.Queen, White);
        ActivePiece[4, 0] = GenerateOnePiece(ChessPieceType.King, White);
        ActivePiece[5, 0] = GenerateOnePiece(ChessPieceType.Bishop, White);
        ActivePiece[6, 0] = GenerateOnePiece(ChessPieceType.Knight, White);
        ActivePiece[7, 0] = GenerateOnePiece(ChessPieceType.Rook, White);
        ActivePiece[0, 7] = GenerateOnePiece(ChessPieceType.Rook, Red);
        ActivePiece[1, 7] = GenerateOnePiece(ChessPieceType.Knight, Red);
        ActivePiece[2, 7] = GenerateOnePiece(ChessPieceType.Bishop, Red);
        ActivePiece[3, 7] = GenerateOnePiece(ChessPieceType.Queen, Red);
        ActivePiece[4, 7] = GenerateOnePiece(ChessPieceType.King, Red);
        ActivePiece[5, 7] = GenerateOnePiece(ChessPieceType.Bishop, Red);
        ActivePiece[6, 7] = GenerateOnePiece(ChessPieceType.Knight, Red);
        ActivePiece[7, 7] = GenerateOnePiece(ChessPieceType.Rook, Red); */

        //pawns
        for (int i = 0; i < X_TILES/2; i++)
        {
            ActivePiece[i, 0] = GenerateOnePiece(ChessPieceType.Pawn, Red);
        }

        for (int i = 0; i < X_TILES/2; i++)
        {
            ActivePiece[i+(X_TILES/2), 0] = GenerateOnePiece(ChessPieceType.Pawn, White);
        }
    }

    private ChessPieceClass GenerateOnePiece(ChessPieceType type, int team)
    {
        ChessPieceClass ChessObj = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPieceClass>();

        ChessObj.type = type;
        ChessObj.team = team;
        ChessObj.GetComponent<MeshRenderer>().material = teamMaterial[team];
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

        if (ActivePiece[x, y] != null)
        {
            ChessPieceClass ocp = ActivePiece[x, y];
            if (cp.team == ocp.team)
            {
                return false;
            }
            if (ocp.team == 0)
            {
                DeadPieces_White.Add(ocp);
                ocp.SetScale(Vector3.one * 0);
                // SIDE ANIMATION : ocp.SetPosition(new Vector3(8 * tileSize, offsetForY, -1 * tileSize) - extent + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.forward * 0.3f) * DeadPieces_White.Count);
            }
            else
            {
                DeadPieces_Red.Add(ocp);
                ocp.SetScale(Vector3.one * 0);
                // SIDE ANIMATION : ocp.SetPosition(new Vector3(-1 * tileSize, offsetForY, 8 * tileSize) - extent + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.back * 0.3f) * DeadPieces_Red.Count);
            }
        }

        ActivePiece[x, y] = cp;
        ActivePiece[previousPosition.x, previousPosition.y] = null;

        positionOnePiece(x, y);
        _Turn_ = !_Turn_;
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
}