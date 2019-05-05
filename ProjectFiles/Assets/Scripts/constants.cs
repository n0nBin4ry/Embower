using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class constants : MonoBehaviour
{
    // WORLD CONSTANTS
    // world's gravity (ARBITRARY)
    public const float W_GRAVITY = -16f;

	// PLAYER CONSTANTS
	// player's jump speed
	public const float JUMP_SPD = 8.4f;
	// move-speed coefficient that affects player movement when jumping (keep between 0-1)
	public const float JUMP_MOV_CO = 0.7f;
    // player's move speed (actually acceleration right now) (ARBITRARY)
    //public const float PLAYER_MSPD = 9f;
    public const float PLAYER_MSPD = .15f;
    // player's acceleration limit (ARBITRARY)
    public const float PLAYER_SPD_LIM = 10f;
    // player's jump acceleration (ARBITRARY)
    public const float PLAYER_JACC = 2f;
    // player's jump velocity limit (ARBITRARY)
    public const float PLAYER_JV_LIM = 10f;
	// player's horizontal decceleration rate
	public const float PLAYER_DCELL = 3;//20f;
    // force to gravitate towards crystal (ARBITRARY)
    public const float CRYSTAL_FORCE = 50f;
    // force of crystal returning to player (ARBITRARY)
    public const float RETURN_FORCE = 20f;
	// player's jump velocity (ARBITRARY)
	public const float JUMP_FORCE /*rip*/ = 5f;
	// limit for horizontal speed (ARBITRARY)
	public const float HORZ_SPD_LIM = 10f;
	// limit for verticle speed (ARBITRARY)
	public static float VERT_SPD_LIMIT = 10f;
    // IF PLAYER HAS THROWN THE CYRSTAL
    public static bool isThrown = false;

	// PLAYER GLOBALS
	// tell and set if player is paused
	public static bool g_player_paused = false;
	public static bool g_crystal_paused = false;

    // LAUNCHING CONSTANTS
    // launch force multiplier (ARBITRARY)
    public const float L_FORCE_MULT = 10;
    // max magnitude for the launch
    public static float L_MAX_MAG = 2;

	// TUTORIAL BOOLS
	// did crystal hit unpurified enemy
	public static bool crys_hit_unpurified = false;
	// did player hit purified enemy
	public static bool plyr_hit_purified = false;
    // to pause camera
    public static bool pause_cam = false;
    // for when enemy "dies"
    public static bool enemy_dies = false;


	// enum for states of player
	public enum ObjState {
		Dead,
		Active,
		Paused
	};


	// returns the offset from the minimum overlap from collider2D box a to b
	public static Vector2 getCollisionoffset(Collider2D a, Collider2D b)
    {
        // get a's max and min
        Vector2 a_max = a.bounds.max;
        Vector2 a_min = a.bounds.min;
        // get b's max and min
        Vector2 b_max = b.bounds.max;
        Vector2 b_min = b.bounds.min;

        // get collider overlaps
        float diff_l = Mathf.Abs(a_min.x - b_max.x); // left edge of a with right edge of b
        float diff_r = Mathf.Abs(a_max.x - b_min.x); // right edge of a with left edge of b
        float diff_b = Mathf.Abs(a_min.y - b_max.y); // bottom edge of a with top edge of b
        float diff_t = Mathf.Abs(a_max.y - b_min.y); // top edge of a with bottom edge of b

        // offset vector to return
        Vector2 offset = Vector2.zero;

        // find smallest overlap
        float min;
        if (diff_l < diff_r)
        {
            min = diff_l;
            offset = new Vector2(-min, 0);
        }
        else
        {
            min = diff_r;
            offset = new Vector2(min, 0);
        }
        if (diff_b < min)
        {
            min = diff_b;
			//offset = new Vector2(-min, 0);
			offset = new Vector2(0, -min);
		}
        if (diff_t < min)
        {
            min = diff_t;
			//offset = new Vector2(min, 0);
			offset = new Vector2(0, min);
		}
		Debug.Log("Collision offset: " + offset);
        return offset;
    }


    // TRASH
    void Start() { }

    // TRASH
    void Update() { }
}
