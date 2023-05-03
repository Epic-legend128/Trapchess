using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPieceClass
{
    //maximum distance a pawn can travel at once
    const int MAX_STEPS = 2;

    public override List<Vector2Int> GetAvailableMoves(ref ChessPieceClass[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        if (board[currentX, currentY] == null || (int)board[currentX, currentY].type > 1) return r;

        int direction = 1;
        int lowLimit = (team == 0) ? 0 : tileCountX / 2;

        bool fwd = true;
        bool right = true;
        bool left = true;
        for (int i = 1; i <= MAX_STEPS; i++)
        {
            //forward
            if (fwd && currentY + direction * i < tileCountY)
            {
                if (board[currentX, currentY].isAvailable(ref board, currentX, currentY + direction * i))
                    r.Add(new Vector2Int(currentX, currentY + direction * i));
                else
                    fwd = false;
            }

            //top right diagonal
            if (right && currentY + direction * i < tileCountY && currentX + i < lowLimit + tileCountX / 2)
            {
                if  (board[currentX, currentY].isAvailable(ref board, currentX + i, currentY + direction * i))
                    r.Add(new Vector2Int(currentX + i, currentY + direction * i));
                else
                    right = false;
            }

            //top left diagonal
            if (left && currentY + direction * i < tileCountY && currentX - i >= lowLimit)
            {
                if (board[currentX, currentY].isAvailable(ref board, currentX - i, currentY + direction * i))
                    r.Add(new Vector2Int(currentX - i, currentY + direction * i));
                else
                    left = false;
            }
        }

        return r;
    }
}
