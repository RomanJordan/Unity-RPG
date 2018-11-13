using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour {

    public Tilemap groundTileMap;
    public Tilemap obstacleTileMap;
    public Tilemap doorTileMap;

    public bool isMoving = false;
    public bool onCooldown = false;

    private float moveTime = 0.1f;


    private AudioSource walkingSound;

	void Start () {
       
	}
	
	void Update () {
        //If we are moving, we do nothing
        if (isMoving){
            return;
        }

        //Store movement directions
        int horizontal = 0;
        int vertical = 0;

        //Getting directions for movement 
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //If horizontal/verticl are NOT equal to zero, we are trying to move
        if (horizontal !=0 ||vertical !=0) {
            StartCoroutine(actionCooldown(1f));
            Move(horizontal, vertical);
        }
 
    }

    private void Move(int xDir, int yDir) {

        //Obtaining the starting cell, which is cell the player occupies 
        Vector2 startCell = transform.position;

        //Obtaining the targetCell, which is the startCell + a direction 
        Vector2 targetCell = startCell + new Vector2(xDir, yDir);


        bool isOnGround = getCell(groundTileMap, startCell) != null; //Checking if the player is on ground tile
        bool hasGroundTile = getCell(groundTileMap, startCell) != null; //Checking if target cell is a ground tile
        bool hasObstacleTile = getCell(obstacleTileMap, targetCell) != null; //Checking if the target cell is an obstacle 
        bool hasDoorTile = getCell(doorTileMap, targetCell) != null; //Checking if the target cell is a door

        //If the player starts from a ground tile
        if (isOnGround) {
          if(hasGroundTile && !hasObstacleTile) {
                StartCoroutine(smoothMovement(targetCell));
            }
           else {
                StartCoroutine(blockedMovement(targetCell));
            }
        }
    }

    private IEnumerator actionCooldown(float cooldown) {
        onCooldown = true;

        //float cooldown = 0.2f;
        while (cooldown > 0f) {
            cooldown -= Time.deltaTime;
            yield return null;
        }
        onCooldown = false;
    }

    /*
     * 
     * Movement
     * 
     */

    private IEnumerator smoothMovement(Vector3 end) {
        isMoving = true;

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        float inverseMoveTime = 1 / moveTime;

        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        isMoving = false;
    }

    private IEnumerator blockedMovement(Vector3 end) {
        isMoving = true;

        Vector3 originalPos = transform.position;

        end = transform.position + ((end - transform.position) / 3);
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        float inverseMoveTime = (1 / (moveTime * 2));

        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        sqrRemainingDistance = (transform.position - originalPos).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, originalPos, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - originalPos).sqrMagnitude;

            yield return null;
        }

        isMoving = false;
    }

    /*
     * 
     * Movement
     * 
     */

    public Collider2D whatsThere(Vector2 targetPos) {
        RaycastHit2D hit;
        hit = Physics2D.Linecast(targetPos, targetPos);
        return hit.collider;
    }

    private TileBase getCell(Tilemap tilemap, Vector2 cellWorldPos) {
        return tilemap.GetTile(tilemap.WorldToCell(cellWorldPos));
    }

    private bool hasTile(Tilemap tilemap, Vector2 cellWorldPos) {
        return tilemap.HasTile(tilemap.WorldToCell(cellWorldPos));
    }

}
