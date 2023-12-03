using System;
using UnityEngine;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}   
public class ChessPiece : MonoBehaviour
{
    //public int id {get;}
    //public PieceType Type { get; private set; }
    //public Team Team { get; private set; }
    //public Vector2Integer CurrentSquare { get; private set; }
    //public int MoveCounter { get; private set; }
    //public Boolean IsAlive { get; private set; }
    //public Boolean Selected { get; private set; }
    
    //public IPieceObserver observer;

    // private Piece(PieceType type, Team team, Vector2Integer currentSquare, Boolean isAlive = true)
    // {
    //     this.id = (int)team * 200 + (int)type * 100 + currentSquare.X + currentSquare.Y;
    //     this.Type = type;
    //     this.Team = team;
    //     this.CurrentSquare = currentSquare;
    //     this.Selected = false;
    //     this.IsAlive = isAlive;
    //     this.MoveCounter = 0;
    // }

    public int team;
    public int currentX;
    public int currentY;
    public ChessPieceType type;

    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10); //TODO: move to event driven input
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10); //TODO: move to event driven input

    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    { 
        desiredPosition = position;
        if (force)
        {
            transform.position = desiredPosition;
        }
    } //TODO: Virtual needed?
    public virtual void SetScale(Vector3 scale, bool force = false) //TODO: Virtual needed?
    {
        desiredScale = scale;
        if (force)
        {
            transform.localScale = desiredScale;
        }
    }
}
