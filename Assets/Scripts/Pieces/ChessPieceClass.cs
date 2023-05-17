using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1,
    invinc = 2,
    die = 3,
    back = 4,
    paralyse = 5,
    respawn = 6,
    quit = 7,
    extra = 8
}

public class ChessPieceClass : MonoBehaviour
{
    public int team;
    public int currentX;
    public int currentY;
    public ChessPieceType type;

    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one;
    private bool invinc = false;
    
    public bool paralysed = false;
    public int[] TRAPS = new int[8];

    public ChessPieceClass() {
        for (int i = 0; i<TRAPS.Length; i++) {
            TRAPS[i] = i+2;
        }
    }

    //fix effects later
    public int trapEffects(ref ChessPieceClass[,] board, ref int x, ref int y, ref bool _Turn_, int type) {
        if (board[x, y] == null || type == 0) return -1;
        Debug.Log("START");
        /* Debug.Log(x);
        Debug.Log(y); */
        Debug.Log(type);
        Debug.Log("END");
        if (type == 2) {
            //works
            invinc = true;
        }
        else if (type == 3) {
            //works
            if (invinc)
                invinc = false;
            else {
                board[currentX, currentY] = null;
                SetScale(new Vector3(0, 0, 0), true);
                return 2;
            }
        }
        else if (type == 4) {
            //works
            return (0 > y-2 ? 0 : y-2);
        }
        else if (type == 5) {
            //works
            paralysed = true;
        }
        else if (type == 6) {
            return 3; // spawn a piece on that place
        }
        else if (type == 7) {
            //works
            _Turn_ = !_Turn_;
            SceneManager.LoadScene(4);
        }
        else if (type == 8) {
            //works
            _Turn_ = !_Turn_;
        }

        return -1;
    }

    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPieceClass[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        
        if (paralysed || (int)type > 1) return r;

        r.Add(new Vector2Int(2, 3));

        return r;
    }

    public bool isAvailable(ref ChessPieceClass[,] board, int x, int y) {
        if (board[x, y] == null) return true;
        for (int i = 0; i<TRAPS.Length; i++) {
            if (TRAPS[i] == (int) board[x, y].type)
                return true;
        }
        return false;
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if (force)
        {
            transform.position = desiredPosition;
        }
    }
    
    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
        {
            transform.localScale = desiredScale;
        }
    }
    
    private void Update()
    {
        if ((int)type <= 1) {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
            transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
        }
    }
}
