using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();

        // Define the eight possible directions a bishop can move (diagonal)
        Vector2Int[] directions =
        {
            new Vector2Int(1, 1),   // Diagonal top-right
            new Vector2Int(1, -1),  // Diagonal bottom-right
            new Vector2Int(-1, 1),  // Diagonal top-left
            new Vector2Int(-1, -1), // Diagonal bottom-left
        };

        foreach (var direction in directions)
        {
            for (int i = 1; i <= tileCountX; i++)
            {
                Vector2Int targetPosition = new Vector2Int(currentX + i * direction.x, currentY + i * direction.y);

                if (!IsWithinBoardBounds(targetPosition, tileCountX, tileCountY))
                {
                    // Stop searching in this direction if out of board bounds
                    break;
                }

                if (board[targetPosition.x, targetPosition.y] == null)
                {
                    // Empty cell, add as a valid move
                    availableMoves.Add(targetPosition);
                }
                else
                {
                    // Cell is occupied, check if it's an opponent's piece (attack)
                    if (board[targetPosition.x, targetPosition.y].team != team)
                    {
                        availableMoves.Add(targetPosition);
                    }

                    // Stop searching in this direction, as the bishop can't jump over pieces
                    break;
                }
            }
        }

        return availableMoves;
    }

    private bool IsWithinBoardBounds(Vector2Int position, int tileCountX, int tileCountY)
    {
        return position.x >= 0 && position.x < tileCountX && position.y >= 0 && position.y < tileCountY;
    }
}