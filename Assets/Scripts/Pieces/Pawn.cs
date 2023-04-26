using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPieceClass
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPieceClass[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = 1;/*(team == 0) ? 1 : -1*/
        int lowLimit = (team == 1) ? 0 : tileCountX / 2;

        for (int i = 1; i <= 3; i++)
        {
            //forward
            if (currentY + direction * i < tileCountY && board[currentX, currentY + direction * i] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + direction * i));
            }

            //top right
            if (currentY + direction * i < tileCountY && currentX + i < lowLimit + tileCountX / 2 && board[currentX + i, currentY + direction * i] == null)
            {
                r.Add(new Vector2Int(currentX + i, currentY + direction * i));
            }

            //top left
            if (currentY + direction * i < tileCountY && currentX - i >= lowLimit && board[currentX - i, currentY + direction * i] == null)
            {
                r.Add(new Vector2Int(currentX - i, currentY + direction * i));
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
