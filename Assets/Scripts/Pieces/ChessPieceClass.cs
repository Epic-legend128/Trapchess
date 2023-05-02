using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool paralysed = false;

    public int[] TRAPS = new int[8];

    public ChessPieceClass() {
        for (int i = 0; i<TRAPS.Length; i++) {
            TRAPS[i] = i+2;
        }
    }

    public void trapEffects(ref ChessPieceClass[,] board, int x, int y, ref bool _Turn_) {
        if ((int)board[x, y].type == 2) {
            invinc = true;
        }
        else if ((int)board[x, y].type == 3) {
            if (invinc)
                invinc = false;
            else
                board[currentX, currentY] = null;
        }
        else if ((int)board[x, y].type == 4) {
            currentY = 0 > y-3 ? 0 : y-3;
        }
        else if ((int)board[x, y].type == 5) {
            paralysed = true;
        }
        else if ((int)board[x, y].type == 6) {
            //respawn piece later
        }
        else if ((int)board[x, y].type == 7) {
            //quit the game for the player
        }
        else if ((int)board[x, y].type == 8) {
            _Turn_ = !_Turn_;
        }
    }

    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPieceClass[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        
        if (paralysed) return r;

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
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
    }
}
