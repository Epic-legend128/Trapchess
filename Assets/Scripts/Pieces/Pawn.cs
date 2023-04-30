using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPieceClass
{
    //maximum distance a pawn can travel at once
    const int MAX_STEPS = 3;

    public override List<Vector2Int> GetAvailableMoves(ref ChessPieceClass[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

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
                if (board[currentX, currentY + direction * i] == null)
                    r.Add(new Vector2Int(currentX, currentY + direction * i));
                else
                    fwd = false;
            }

            //top right diagonal
            if (right && currentY + direction * i < tileCountY && currentX + i < lowLimit + tileCountX / 2)
            {
                if  (board[currentX + i, currentY + direction * i] == null)
                    r.Add(new Vector2Int(currentX + i, currentY + direction * i));
                else
                    right = false;
            }

            //top left diagonal
            if (left && currentY + direction * i < tileCountY && currentX - i >= lowLimit)
            {
                if (board[currentX - i, currentY + direction * i] == null)
                    r.Add(new Vector2Int(currentX - i, currentY + direction * i));
                else
                    left = false;
            }
        }

        //the extra step at the second rank
        /* if (board[currentX, currentY + direction] == null)
        {
            if (team == 0 && currentY == 1 && board[currentX, currentY + (direction * 2)] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }
            if (team == 1 && currentY == 6 && board[currentX, currentY + (direction * 2)] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }
        } */

        //eating
        /* if (currentX != tileCountX - 1)
        {
            if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team)
            {
                r.Add(new Vector2Int(currentX + 1, currentY + direction));
            }

        }
        if (currentX != 0)
        {
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
            {
                r.Add(new Vector2Int(currentX - 1, currentY + direction));
            }

        } */

        return r;
    }
}
