using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

public class GameController : MonoBehaviour{

	// [WP,WKn,WB,WR,WQ,WK, bp,bkn,bb,br,bq,bk]
	public GameObject fromtwo;
	public GameObject fromthree;
	public GameObject totwo;
	public GameObject tothree;
	public List<GameObject> piecePrefabs;
	public List<GameObject> twoDimensionPieces;
	public Camera camera;
	public GameObject selection;
	public GameObject twoSelection;
	public GameObject chosen;
	public GameObject twoChosen;
	public GameObject gameOverButton;
	public PieceVals pieceVals;
	public GameObject threePieces;
	public GameObject twoPieces;
	public Text whiteTimeText;
	public Text blackTimeText;
	public Text moveLogText;
	public GameObject whiteTurn;
	public GameObject blackTurn;



	ulong[] knightMoves = new ulong[] { 0x20400, 0x50800, 0xa1100, 0x142200, 0x284400, 0x508800, 0xa01000, 0x402000, 0x2040004, 0x5080008, 0xa110011,
		0x14220022, 0x28440044, 0x50880088, 0xa0100010, 0x40200020, 0x204000402, 0x508000805, 0xa1100110a, 0x1422002214, 0x2844004428, 0x5088008850, 0xa0100010a0,
		0x4020002040, 0x20400040200, 0x50800080500, 0xa1100110a00, 0x142200221400, 0x284400442800, 0x508800885000, 0xa0100010a000, 0x402000204000, 0x2040004020000,
		0x5080008050000, 0xa1100110a0000, 0x14220022140000, 0x28440044280000, 0x50880088500000, 0xa0100010a00000, 0x40200020400000, 0x204000402000000, 0x508000805000000,
		0xa1100110a000000, 0x1422002214000000, 0x2844004428000000, 0x5088008850000000, 0xa0100010a0000000, 0x4020002040000000, 0x400040200000000, 0x800080500000000,
		0x1100110a00000000, 0x2200221400000000, 0x4400442800000000, 0x8800885000000000, 0x100010a000000000, 0x2000204000000000, 0x4020000000000, 0x8050000000000,
		0x110a0000000000, 0x22140000000000, 0x44280000000000, 0x88500000000000, 0x10a00000000000, 0x20400000000000};

	ulong[] kingMoves = new ulong[] {0x302, 0x705, 0xe0a, 0x1c14, 0x3828, 0x7050, 0xe0a0, 0xc040, 0x30203, 0x70507, 0xe0a0e, 0x1c141c, 0x382838, 0x705070, 0xe0a0e0,
		0xc040c0, 0x3020300, 0x7050700, 0xe0a0e00, 0x1c141c00, 0x38283800, 0x70507000, 0xe0a0e000, 0xc040c000, 0x302030000, 0x705070000, 0xe0a0e0000, 0x1c141c0000,
		0x3828380000, 0x7050700000, 0xe0a0e00000, 0xc040c00000, 0x30203000000, 0x70507000000, 0xe0a0e000000, 0x1c141c000000, 0x382838000000, 0x705070000000,
		0xe0a0e0000000, 0xc040c0000000, 0x3020300000000, 0x7050700000000, 0xe0a0e00000000, 0x1c141c00000000, 0x38283800000000, 0x70507000000000, 0xe0a0e000000000,
		0xc040c000000000, 0x302030000000000, 0x705070000000000, 0xe0a0e0000000000, 0x1c141c0000000000, 0x3828380000000000, 0x7050700000000000, 0xe0a0e00000000000,
		0xc040c00000000000, 0x203000000000000, 0x507000000000000, 0xa0e000000000000, 0x141c000000000000, 0x2838000000000000, 0x5070000000000000, 0xa0e0000000000000,
		0x40c0000000000000};

	// order is N, NE, E, SE, S, SW, W, NW
	ulong[][] rayAttacks = new ulong[][] {
		new ulong[] {0x101010101010100, 0x202020202020200, 0x404040404040400, 0x808080808080800, 0x1010101010101000, 0x2020202020202000, 0x4040404040404000, 0x8080808080808000,
			0x101010101010000, 0x202020202020000, 0x404040404040000, 0x808080808080000, 0x1010101010100000, 0x2020202020200000, 0x4040404040400000, 0x8080808080800000,
			0x101010101000000, 0x202020202000000, 0x404040404000000, 0x808080808000000, 0x1010101010000000, 0x2020202020000000, 0x4040404040000000, 0x8080808080000000,
			0x101010100000000, 0x202020200000000, 0x404040400000000, 0x808080800000000, 0x1010101000000000, 0x2020202000000000, 0x4040404000000000, 0x8080808000000000,
			0x101010000000000, 0x202020000000000, 0x404040000000000, 0x808080000000000, 0x1010100000000000, 0x2020200000000000, 0x4040400000000000, 0x8080800000000000,
			0x101000000000000, 0x202000000000000, 0x404000000000000, 0x808000000000000, 0x1010000000000000, 0x2020000000000000, 0x4040000000000000, 0x8080000000000000,
			0x100000000000000, 0x200000000000000, 0x400000000000000, 0x800000000000000, 0x1000000000000000, 0x2000000000000000, 0x4000000000000000, 0x8000000000000000,
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0},
		new ulong[] {0x8040201008040200, 0x80402010080400, 0x804020100800, 0x8040201000, 0x80402000, 0x804000, 0x8000, 0x0, 0x4020100804020000, 0x8040201008040000, 
			0x80402010080000, 0x804020100000, 0x8040200000, 0x80400000, 0x800000, 0x0, 0x2010080402000000, 0x4020100804000000, 0x8040201008000000, 0x80402010000000, 
			0x804020000000, 0x8040000000, 0x80000000, 0x0, 0x1008040200000000, 0x2010080400000000, 0x4020100800000000, 0x8040201000000000, 0x80402000000000, 
			0x804000000000, 0x8000000000, 0x0, 0x804020000000000, 0x1008040000000000, 0x2010080000000000, 0x4020100000000000, 0x8040200000000000, 0x80400000000000, 
			0x800000000000, 0x0, 0x402000000000000, 0x804000000000000, 0x1008000000000000, 0x2010000000000000, 0x4020000000000000, 0x8040000000000000, 0x80000000000000, 
			0x0, 0x200000000000000, 0x400000000000000, 0x800000000000000, 0x1000000000000000, 0x2000000000000000, 0x4000000000000000, 0x8000000000000000, 0x0, 0x0, 0x0, 
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0},
		new ulong[] {0xfe, 0xfc, 0xf8, 0xf0, 0xe0, 0xc0, 0x80, 0x0, 0xfe00, 0xfc00, 0xf800, 0xf000, 0xe000, 0xc000, 0x8000, 0x0, 0xfe0000, 0xfc0000, 0xf80000, 0xf00000, 
			0xe00000, 0xc00000, 0x800000, 0x0, 0xfe000000, 0xfc000000, 0xf8000000, 0xf0000000, 0xe0000000, 0xc0000000, 0x80000000, 0x0, 0xfe00000000, 0xfc00000000, 
			0xf800000000, 0xf000000000, 0xe000000000, 0xc000000000, 0x8000000000, 0x0, 0xfe0000000000, 0xfc0000000000, 0xf80000000000, 0xf00000000000, 0xe00000000000, 
			0xc00000000000, 0x800000000000, 0x0, 0xfe000000000000, 0xfc000000000000, 0xf8000000000000, 0xf0000000000000, 0xe0000000000000, 0xc0000000000000,
			0x80000000000000, 0x0, 0xfe00000000000000, 0xfc00000000000000, 0xf800000000000000, 0xf000000000000000, 0xe000000000000000, 0xc000000000000000,
			0x8000000000000000, 0x0},
		new ulong[] {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80, 0x0, 0x204, 0x408, 0x810, 0x1020, 0x2040, 0x4080, 0x8000, 0x0, 0x20408,
			0x40810, 0x81020, 0x102040, 0x204080, 0x408000, 0x800000, 0x0, 0x2040810, 0x4081020, 0x8102040, 0x10204080, 0x20408000, 0x40800000, 0x80000000, 0x0,
			0x204081020, 0x408102040, 0x810204080, 0x1020408000, 0x2040800000, 0x4080000000, 0x8000000000, 0x0, 0x20408102040, 0x40810204080, 0x81020408000,
			0x102040800000, 0x204080000000, 0x408000000000, 0x800000000000, 0x0, 0x2040810204080, 0x4081020408000, 0x8102040800000, 0x10204080000000, 0x20408000000000,
			0x40800000000000, 0x80000000000000, 0x0},
		new ulong[] {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80, 0x101, 0x202, 0x404,
			0x808, 0x1010, 0x2020, 0x4040, 0x8080, 0x10101, 0x20202, 0x40404, 0x80808, 0x101010, 0x202020, 0x404040, 0x808080, 0x1010101,
			0x2020202, 0x4040404, 0x8080808, 0x10101010, 0x20202020, 0x40404040, 0x80808080, 0x101010101, 0x202020202, 0x404040404, 0x808080808,
			0x1010101010, 0x2020202020, 0x4040404040, 0x8080808080, 0x10101010101, 0x20202020202, 0x40404040404, 0x80808080808, 0x101010101010,
			0x202020202020, 0x404040404040, 0x808080808080, 0x1010101010101, 0x2020202020202, 0x4040404040404, 0x8080808080808, 0x10101010101010,
			0x20202020202020, 0x40404040404040, 0x80808080808080},
		new ulong[] {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x0, 0x100, 0x201, 0x402, 0x804, 0x1008, 0x2010, 0x4020, 0x0,
			0x10000, 0x20100, 0x40201, 0x80402, 0x100804, 0x201008, 0x402010, 0x0, 0x1000000, 0x2010000, 0x4020100, 0x8040201, 0x10080402, 0x20100804, 0x40201008, 0x0,
			0x100000000, 0x201000000, 0x402010000, 0x804020100, 0x1008040201, 0x2010080402, 0x4020100804, 0x0, 0x10000000000, 0x20100000000, 0x40201000000, 0x80402010000,
			0x100804020100, 0x201008040201, 0x402010080402, 0x0, 0x1000000000000, 0x2010000000000, 0x4020100000000, 0x8040201000000, 0x10080402010000, 0x20100804020100,
			0x40201008040201},
		new ulong[] {0x0, 0x1, 0x3, 0x7, 0xf, 0x1f, 0x3f, 0x7f, 0x0, 0x100, 0x300, 0x700, 0xf00, 0x1f00, 0x3f00, 0x7f00, 0x0, 0x10000, 0x30000, 0x70000, 0xf0000, 0x1f0000,
			0x3f0000, 0x7f0000, 0x0, 0x1000000, 0x3000000, 0x7000000, 0xf000000, 0x1f000000, 0x3f000000, 0x7f000000, 0x0, 0x100000000, 0x300000000, 0x700000000,
			0xf00000000, 0x1f00000000, 0x3f00000000, 0x7f00000000, 0x0, 0x10000000000, 0x30000000000, 0x70000000000, 0xf0000000000, 0x1f0000000000, 0x3f0000000000,
			0x7f0000000000, 0x0, 0x1000000000000, 0x3000000000000, 0x7000000000000, 0xf000000000000, 0x1f000000000000, 0x3f000000000000, 0x7f000000000000, 0x0,
			0x100000000000000, 0x300000000000000, 0x700000000000000, 0xf00000000000000, 0x1f00000000000000, 0x3f00000000000000, 0x7f00000000000000},
		new ulong[] {0x0, 0x100, 0x10200, 0x1020400, 0x102040800, 0x10204081000, 0x1020408102000, 0x102040810204000, 0x0, 0x10000, 0x1020000, 0x102040000, 0x10204080000,
			0x1020408100000, 0x102040810200000, 0x204081020400000, 0x0, 0x1000000, 0x102000000, 0x10204000000, 0x1020408000000, 0x102040810000000, 0x204081020000000,
			0x408102040000000, 0x0, 0x100000000, 0x10200000000, 0x1020400000000, 0x102040800000000, 0x204081000000000, 0x408102000000000, 0x810204000000000, 0x0,
			0x10000000000, 0x1020000000000, 0x102040000000000, 0x204080000000000, 0x408100000000000, 0x810200000000000, 0x1020400000000000, 0x0, 0x1000000000000,
			0x102000000000000, 0x204000000000000, 0x408000000000000, 0x810000000000000, 0x1020000000000000, 0x2040000000000000, 0x0, 0x100000000000000, 0x200000000000000,
			0x400000000000000, 0x800000000000000, 0x1000000000000000, 0x2000000000000000, 0x4000000000000000, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0}
	};

	// initial game conditions
//	private ushort gameState = 0x1e;
	// { (white) pawn, knight, bishop, rook, queen, king, (black) pawn, knight ...} Gamestate added on as 12th item (0x1e)

	// STANDARD GAME SETUP: {0xff00, 0x42, 0x24, 0x81, 0x8, 0x10, 0xff000000000000, 0x4200000000000000, 0x2400000000000000,
	//                      0x8100000000000000, 0x800000000000000, 0x1000000000000000, 0x1e}

	// PERFT TESTING ONLY: (positions refered to my chessprogramming.wkikispaces/Perft+Results)
	// Position 3: {0x200005000, 0, 0, 0x2000000, 0, 0x100000000, 0x4080020000000, 0, 0, 0x8000000000, 0, 0x80000000, 0}



	//private ulong[] bitboardArray = new ulong[] {0x200005000, 0, 0, 0x2000000, 0, 0x100000000, 0x4080020000000, 0, 0, 0x8000000000, 0, 0x80000000, 0};

	private ulong[] bitboardArray = new ulong[] {0xff00, 0x42, 0x24, 0x81, 0x8, 0x10,
		0xff000000000000, 0x4200000000000000, 0x2400000000000000, 0x8100000000000000, 0x800000000000000, 0x1000000000000000, 0x1e};




	// human (0) or comp (1) for white 1st and black 2nd
	public int[] players;

	private int selectionX;
	private int selectionY;
	private int[] startTile = new int[] {-1,-1};
	private int[] endTile = new int[] {-1,-1};
	int startIndex;
	int endIndex;
	private int startPieceIndex;
	private int endPieceIndex;
	private bool computerMove = false;
	private bool goToComputer = false;
	private bool threeD = true;
	private int whiteTime;
	private int blackTime;
	private bool gameDone = false;
	// experimental to try to make better opening
	private int numberOfMoves = 0;
	private int maxDepth;
	List<uint> bestMoves = new List<uint>();

	private string[] across = new string[] {"a","b","c","d","e","f","g","h"};
	private string[] up = new string[] {"1","2","3","4","5","6","7","8"};
	private List<string> moveLog = new List<string>();


	private bool threadComplete = false;
	static System.Random rnd = new System.Random();

	// castling represented by moving king across two
	private int[] basicValues = new int[] {1,3,3,5,9,0,1,3,3,5,9,0};
	bool theEndGame = false;

	uint[] leastSigLookup = {
		0,  1, 48,  2, 57, 49, 28,  3,
		61, 58, 50, 42, 38, 29, 17,  4,
		62, 55, 59, 36, 53, 51, 43, 22,
		45, 39, 33, 30, 24, 18, 12,  5,
		63, 47, 56, 27, 60, 41, 37, 16,
		54, 35, 52, 21, 44, 32, 23, 11,
		46, 26, 40, 15, 34, 20, 31, 10,
		25, 14, 19,  9, 13,  8,  7,  6
	}; 
	ulong debruijnleast = 0x03f79d71b4cb0a89;

	private static ulong MostSigBitSet(ulong value)
	{
		value |= (value >> 1);
		value |= (value >> 2);
		value |= (value >> 4);
		value |= (value >> 8);
		value |= (value >> 16);
		value |= (value >> 32);

		return (value & ~(value >> 1));
	}


	private List<uint> allValidMoves(ulong[] bitboards){
		bool insertStart = false;
		List<uint> validMoves = new List<uint>(); 
		List<uint> captureMoves = new List<uint>();
		ulong whitePieces = bitboards [0] | bitboards [1] | bitboards [2] | bitboards [3] | bitboards [4] | bitboards [5];
		ulong blackPieces = bitboards [6] | bitboards [7] | bitboards [8] | bitboards [9] | bitboards [10] | bitboards [11];
		ulong occupied = whitePieces | blackPieces;
		uint capturedPiece;
		ulong destinations;
		ulong gameState = bitboards [12];

		// white turn
		if (gameState % 2 == 0) {
			
			// knights
			uint lsbIndex = 0;
			ulong knightBitBoard = bitboards [1];
			while (knightBitBoard != 0) {
				
				lsbIndex = leastSigLookup [((knightBitBoard & (~knightBitBoard+1)) * debruijnleast) >> 58];
				// lsbIndex = index of knight on board

				destinations = knightMoves [lsbIndex] & (~whitePieces);
				uint destinationIndex = 0;
				while (destinations  != 0) {
					ulong destinationBit = destinations & (~destinations+1);
					if ((destinationBit & blackPieces) != 0) {
						insertStart = true;
						if ((destinationBit & bitboards [6]) != 0) {
							capturedPiece = 0;
						} else if ((destinationBit & bitboards [7]) != 0) {
							capturedPiece = 1 << 4;
						} else if ((destinationBit & bitboards [8]) != 0) {
							capturedPiece = 2 << 4;
						} else if ((destinationBit & bitboards [9]) != 0) {
							capturedPiece = 3 << 4;
						} else if ((destinationBit & bitboards [10]) != 0) {
							capturedPiece = 4 << 4;
						} else if ((destinationBit & bitboards [11]) != 0) {
							capturedPiece = 5 << 4;
						}
					} else {
						insertStart = false;
						capturedPiece = 7 << 4;
					}
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					// destinationIndex = index of destination tile
					if (insertStart) {
						captureMoves.Add ( lsbIndex << 17 | destinationIndex << 11 | 1 << 7 | capturedPiece);
					} else {
						validMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 1 << 7 | capturedPiece);
					}
					destinations &= destinations - 1;
				}
				knightBitBoard &= knightBitBoard - 1;
			}

			// king
			// commented to avoid stack
			ulong kingBitBoard = bitboards [5];
			ulong[] switchTurn = (ulong[])bitboards.Clone ();
			switchTurn [12] ^= 1;
			if (kingBitBoard == 0x10 && !CanTakeKing(switchTurn)) {
				// castling left
				if ((bitboards [3] & 0x1)!=0 && ((whitePieces | blackPieces) & 0xe)==0 && (gameState & 0x8)==0x8) {
					ulong[] boardClone = (ulong[])bitboards.Clone ();
					MakeMove (boardClone, 4 << 17 | 3 << 11 | 5 << 7 | 7 << 4);
					if (!CanTakeKing (boardClone)) {
						captureMoves.Add (4 << 17 | 2 << 11 | 5 << 7 | 7 << 4 | 8);
					}
				}
				// castling right
				if ((bitboards [3] & 0x80)!=0 && ((whitePieces | blackPieces) & 0x60)==0 && (gameState & 0x10)==0x10) {
					ulong[] boardClone = (ulong[])bitboards.Clone ();
					MakeMove (boardClone, 4 << 17 | 5 << 11 | 5 << 7 | 7 << 4);
					if (!CanTakeKing (boardClone)) {
						captureMoves.Add (4 << 17 | 6 << 11 | 5 << 7 | 7 << 4 | 4);
					}
				}
			}
			while (kingBitBoard != 0) {

				lsbIndex = leastSigLookup [((kingBitBoard & (~kingBitBoard+1)) * debruijnleast) >> 58];
				// lsbIndex = index of knight on board

				destinations = kingMoves [lsbIndex] & (~whitePieces);
				uint destinationIndex = 0;
				while (destinations  != 0) {
					ulong destinationBit = destinations & (~destinations+1);;
					if ((destinationBit & blackPieces) != 0) {
						insertStart = true;
						if ((destinationBit & bitboards [6]) != 0) {
							capturedPiece = 0;
						} else if ((destinationBit & bitboards [7]) != 0) {
							capturedPiece = 1 << 4;
						} else if ((destinationBit & bitboards [8]) != 0) {
							capturedPiece = 2 << 4;
						} else if ((destinationBit & bitboards [9]) != 0) {
							capturedPiece = 3 << 4;
						} else if ((destinationBit & bitboards [10]) != 0) {
							capturedPiece = 4 << 4;
						} else if ((destinationBit & bitboards [11]) != 0) {
							capturedPiece = 5 << 4;
						}
					} else {
						insertStart = false;
						capturedPiece = 7 << 4;
					}
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					// destinationIndex = index of destination tile
					if (insertStart) {
						captureMoves.Add ( lsbIndex << 17 | destinationIndex << 11 | 5 << 7 | capturedPiece);
					} else {
						validMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 5 << 7 | capturedPiece);
					}
					destinations &= destinations - 1;
				}
				kingBitBoard &= kingBitBoard - 1;
			}

			// pawns
			ulong pawnBitBoard = bitboards[0];
			while (pawnBitBoard != 0) {
				destinations = 0;
				ulong lsb = pawnBitBoard & (~pawnBitBoard+1);
				lsbIndex = leastSigLookup [(lsb * debruijnleast) >> 58];
				// basic move
				if (((lsb << 8) & occupied) == 0) {
					if ((lsb & 0xff000000000000) != 0) {
						// promote to knight, queen detected automatically in next line (0 = queen on end of line)
						captureMoves.Add (lsbIndex << 17 | (lsbIndex + 8) << 11 | 7 << 4 | 1);
					}
					captureMoves.Add (lsbIndex << 17 | (lsbIndex + 8) << 11 | 7 << 4);
				}
				// 2 move
				if (((lsb << 16) & ~occupied & 0xff000000) != 0 && ((lsb << 8) & occupied) == 0) {
					validMoves.Add (lsbIndex << 17 | (lsbIndex + 16) << 11 | 7 << 4);
				}
				// capture left
				ulong destinationBit = (lsb&0xfefefefefefefefe) << 7;
				if ((destinationBit & blackPieces) != 0) {
					if ((destinationBit & bitboards [6]) != 0) {
						capturedPiece = 0;
					} else if ((destinationBit & bitboards [7]) != 0) {
						capturedPiece = 1 << 4;
					} else if ((destinationBit & bitboards [8]) != 0) {
						capturedPiece = 2 << 4;
					} else if ((destinationBit & bitboards [9]) != 0) {
						capturedPiece = 3 << 4;
					} else if ((destinationBit & bitboards [10]) != 0) {
						capturedPiece = 4 << 4;
					} else if ((destinationBit & bitboards [11]) != 0) {
						capturedPiece = 5 << 4;
					}
					if ((lsb & 0xff000000000000) != 0) {
						// promote to knight, queen detected automatically in next line (0 = queen on end of line)
						captureMoves.Add (lsbIndex << 17 | (lsbIndex + 7) << 11 |capturedPiece | 1);
					}
					captureMoves.Add (lsbIndex << 17 | (lsbIndex + 7) << 11 | capturedPiece); 
				}
				// capture right
				destinationBit = (lsb&0x7f7f7f7f7f7f7f7f) << 9;
				if ((destinationBit & blackPieces) != 0) {
					if ((destinationBit & bitboards [6]) != 0) {
						capturedPiece = 0;
					} else if ((destinationBit & bitboards [7]) != 0) {
						capturedPiece = 1 << 4;
					} else if ((destinationBit & bitboards [8]) != 0) {
						capturedPiece = 2 << 4;
					} else if ((destinationBit & bitboards [9]) != 0) {
						capturedPiece = 3 << 4;
					} else if ((destinationBit & bitboards [10]) != 0) {
						capturedPiece = 4 << 4;
					} else if ((destinationBit & bitboards [11]) != 0) {
						capturedPiece = 5 << 4;
					}
					if ((lsb & 0xff000000000000) != 0) {
						// promote to knight, queen detected automatically in next line (0 = queen on end of line)
						captureMoves.Add (lsbIndex << 17 | (lsbIndex + 9) << 11 |capturedPiece | 1);
					}
					captureMoves.Add (lsbIndex << 17 | (lsbIndex + 9) << 11 | capturedPiece); 
				}
				// en passant to the left
				if (lsbIndex + 7 == ((gameState & 0x7e0) >> 5) && (gameState & 0x800) != 0 && (lsb&0xfefefefefefefefe)!=0) {
					captureMoves.Add (lsbIndex << 17 | (lsbIndex + 7) << 11 | 6 << 4); 
				} 
				// EP right
				else if (lsbIndex + 9 == ((gameState & 0x7e0) >> 5) && (gameState & 0x800) != 0 && (lsb&0x7f7f7f7f7f7f7f7f)!=0) {
					captureMoves.Add (lsbIndex << 17 | (lsbIndex + 9) << 11 | 6 << 4); 
				}
				pawnBitBoard &= pawnBitBoard - 1;
			}

				
			// bishops
			ulong bishopBitBoard = bitboards [2];
			while (bishopBitBoard != 0) {
				destinations = 0;
				lsbIndex = leastSigLookup [((bishopBitBoard & (~bishopBitBoard+1)) * debruijnleast) >> 58];

				ulong attacks = rayAttacks[1][lsbIndex];
				ulong blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [1] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				attacks = rayAttacks[7][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [7] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				attacks = rayAttacks[3][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [3] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				attacks = rayAttacks[5][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [5] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				uint destinationIndex = 0;
				while (destinations  != 0) {
					ulong destinationBit = destinations & (~destinations+1);;
					if ((destinationBit & blackPieces) != 0) {
						insertStart = true;
						if ((destinationBit & bitboards [6]) != 0) {
							capturedPiece = 0;
						} else if ((destinationBit & bitboards [7]) != 0) {
							capturedPiece = 1 << 4;
						} else if ((destinationBit & bitboards [8]) != 0) {
							capturedPiece = 2 << 4;
						} else if ((destinationBit & bitboards [9]) != 0) {
							capturedPiece = 3 << 4;
						} else if ((destinationBit & bitboards [10]) != 0) {
							capturedPiece = 4 << 4;
						} else if ((destinationBit & bitboards [11]) != 0) {
							capturedPiece = 5 << 4;
						}
					} else {
						insertStart = false;
						capturedPiece = 7 << 4;
					}
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					// destinationIndex = index of destination tile
					if (insertStart) {
						captureMoves.Add ( lsbIndex << 17 | destinationIndex << 11 | 2 << 7 | capturedPiece);
					} else {
						validMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 2 << 7 | capturedPiece);
					}
					destinations &= destinations - 1;
				}
					
				bishopBitBoard &= bishopBitBoard - 1;
			}

			// rooks
			ulong rookBitBoard = bitboards [3];
			while (rookBitBoard != 0) {
				destinations = 0;
				lsbIndex = leastSigLookup [((rookBitBoard & (~rookBitBoard+1)) * debruijnleast) >> 58];

				ulong attacks = rayAttacks[0][lsbIndex];
				ulong blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [0] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				attacks = rayAttacks[2][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [2] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				attacks = rayAttacks[4][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [4] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				attacks = rayAttacks[6][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [6] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);
				uint destinationIndex = 0;
				while (destinations  != 0) {
					ulong destinationBit = destinations & (~destinations+1);;
					if ((destinationBit & blackPieces) != 0) {
						insertStart = true;
						if ((destinationBit & bitboards [6]) != 0) {
							capturedPiece = 0;
						} else if ((destinationBit & bitboards [7]) != 0) {
							capturedPiece = 1 << 4;
						} else if ((destinationBit & bitboards [8]) != 0) {
							capturedPiece = 2 << 4;
						} else if ((destinationBit & bitboards [9]) != 0) {
							capturedPiece = 3 << 4;
						} else if ((destinationBit & bitboards [10]) != 0) {
							capturedPiece = 4 << 4;
						} else if ((destinationBit & bitboards [11]) != 0) {
							capturedPiece = 5 << 4;
						}
					} else {
						insertStart = false;
						capturedPiece = 7 << 4;
					}
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					// destinationIndex = index of destination tile
					if (insertStart) {
						captureMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 3 << 7 | capturedPiece);
					} else {
						validMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 3 << 7 | capturedPiece);
					}
					destinations &= destinations - 1;
				}

				rookBitBoard &= rookBitBoard - 1;
			}
			// queen
			ulong queenBitBoard = bitboards [4];

			while (queenBitBoard != 0) {
				ulong attacks;
				ulong blockers;
				destinations = 0;
				lsbIndex = leastSigLookup [((queenBitBoard & (~queenBitBoard+1)) * debruijnleast) >> 58];
				for (int dir = 0; dir < 3; dir++) {
					attacks = rayAttacks [dir] [lsbIndex];
					blockers = attacks & (blackPieces | whitePieces);
					if (blockers != 0) {
						uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
						attacks ^= rayAttacks [dir] [blockIndex];
					}
					destinations |= attacks & (~whitePieces);
				}

				attacks = rayAttacks[7][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [7] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				for (int dir = 3; dir < 7; dir++) {
					attacks = rayAttacks [dir] [lsbIndex];
					blockers = attacks & (blackPieces | whitePieces);
					if (blockers != 0) {
						uint blockIndex = leastSigLookup [(MostSigBitSet (blockers) * debruijnleast) >> 58];
						attacks ^= rayAttacks [dir] [blockIndex];
					}
					destinations |= attacks & (~whitePieces);
				}

				uint destinationIndex = 0;
				while (destinations  != 0) {
					ulong destinationBit = destinations & (~destinations+1);;
					if ((destinationBit & blackPieces) != 0) {
						insertStart = true;
						if ((destinationBit & bitboards [6]) != 0) {
							capturedPiece = 0;
						} else if ((destinationBit & bitboards [7]) != 0) {
							capturedPiece = 1 << 4;
						} else if ((destinationBit & bitboards [8]) != 0) {
							capturedPiece = 2 << 4;
						} else if ((destinationBit & bitboards [9]) != 0) {
							capturedPiece = 3 << 4;
						} else if ((destinationBit & bitboards [10]) != 0) {
							capturedPiece = 4 << 4;
						} else if ((destinationBit & bitboards [11]) != 0) {
							capturedPiece = 5 << 4;
						}
					} else {
						insertStart = false;
						capturedPiece = 7 << 4;
					}
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					// destinationIndex = index of destination tile
					if (insertStart) {
						captureMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 4 << 7 | capturedPiece);
					} else {
						validMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 4 << 7 | capturedPiece);
					}
					destinations &= destinations - 1;
				}

				queenBitBoard &= queenBitBoard - 1;
			}

			// black turn
		} else {
			// knights
			uint lsbIndex = 0;
			ulong knightBitBoard = bitboards [7];
			while (knightBitBoard != 0) {
				lsbIndex = leastSigLookup [((knightBitBoard & (~knightBitBoard+1)) * debruijnleast) >> 58];
				// lsbIndex = index of knight on board

				destinations = knightMoves [lsbIndex] & (~blackPieces);
				uint destinationIndex = 0;
				while (destinations != 0) {
					ulong destinationBit = destinations & (~destinations+1);;
					if ((destinationBit & whitePieces) != 0) {
						insertStart = true;
						if ((destinationBit & bitboards [0]) != 0) {
							capturedPiece = 0;
						} else if ((destinationBit & bitboards [1]) != 0) {
							capturedPiece = 1 << 4;
						} else if ((destinationBit & bitboards [2]) != 0) {
							capturedPiece = 2 << 4;
						} else if ((destinationBit & bitboards [3]) != 0) {
							capturedPiece = 3 << 4;
						} else if ((destinationBit & bitboards [4]) != 0) {
							capturedPiece = 4 << 4;
						} else if ((destinationBit & bitboards [5]) != 0) {
							capturedPiece = 5 << 4;
						}
					} else {
						insertStart = false;
						capturedPiece = 7 << 4;
					}
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					// destinationIndex = index of destination tile
					if (insertStart) {
						captureMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 9 << 7 | capturedPiece);
					} else {
						validMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 9 << 7 | capturedPiece);
					}
					destinations &= destinations - 1;
				}
				knightBitBoard &= knightBitBoard - 1;
			}
			// king
			// commented to avoid stack
			ulong kingBitBoard = bitboards [11];
			ulong[] switchTurn = (ulong[])bitboards.Clone ();
			switchTurn [12] ^= 1;
			if (kingBitBoard == 0x1000000000000000 && !CanTakeKing(switchTurn)) {
				if ((bitboards [9] & unchecked((ulong)0x100000000000000)) != 0 && ((whitePieces | blackPieces) & unchecked((ulong)0xe00000000000000))==0  && (gameState & 0x2)==0x2) {
					ulong[] boardClone = (ulong[])bitboards.Clone ();
					MakeMove (boardClone, 60 << 17 | 59 << 11 | 13 << 7 | 7 << 4);
					if (!CanTakeKing (boardClone)) {
						captureMoves.Add ((60 << 17 | 58 << 11 | 13 << 7 | 7 << 4 | 8));
					}
				}
				if ((bitboards [9] & unchecked((ulong)0x8000000000000000)) != 0 && ((whitePieces | blackPieces) & unchecked((ulong)0x6000000000000000))==0 && (gameState & 0x4)==0x4) {
					ulong[] boardClone = (ulong[])bitboards.Clone ();
					MakeMove (boardClone, 60 << 17 | 61 << 11 | 13 << 7 | 7 << 4);
					if (!CanTakeKing (boardClone)) {
						captureMoves.Add (60 << 17 | 62 << 11 | 13 << 7 | 7 << 4 | 4);
					}
				}
			}
			while (kingBitBoard != 0) {

				lsbIndex = leastSigLookup [((kingBitBoard & (~kingBitBoard+1)) * debruijnleast) >> 58];
				// lsbIndex = index of knight on board

				destinations = kingMoves [lsbIndex] & (~blackPieces);
				uint destinationIndex = 0;
				while (destinations != 0) {
					ulong destinationBit = destinations & (~destinations+1);;
					if ((destinationBit & whitePieces) != 0) {
						insertStart = true;
						if ((destinationBit & bitboards [0]) != 0) {
							capturedPiece = 0;
						} else if ((destinationBit & bitboards [1]) != 0) {
							capturedPiece = 1 << 4;
						} else if ((destinationBit & bitboards [2]) != 0) {
							capturedPiece = 2 << 4;
						} else if ((destinationBit & bitboards [3]) != 0) {
							capturedPiece = 3 << 4;
						} else if ((destinationBit & bitboards [4]) != 0) {
							capturedPiece = 4 << 4;
						} else if ((destinationBit & bitboards [5]) != 0) {
							capturedPiece = 5 << 4;
						}
					} else {
						insertStart = false;
						capturedPiece = 7 << 4;
					}
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					// destinationIndex = index of destination tile
					if (insertStart) {
						captureMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 13 << 7 | capturedPiece);
					} else {
						validMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 13 << 7 | capturedPiece);
					}
					destinations &= destinations - 1;
				}
				kingBitBoard &= kingBitBoard - 1;
			}

			// pawns
			ulong pawnBitBoard = bitboards[6];
			while (pawnBitBoard != 0) {
				destinations = 0;
				ulong lsb = pawnBitBoard & (~pawnBitBoard+1);
				lsbIndex = leastSigLookup [(lsb * debruijnleast) >> 58];
				// basic move
				if (((lsb >> 8) & occupied) == 0) {
					if ((lsb & 0xff00) != 0) {
						// promote to knight, queen detected automatically in next line (0 = queen on end of line)
						validMoves.Add (lsbIndex << 17 | (lsbIndex - 8) << 11 | 8 << 7 | 7 << 4 | 1);
					}
					validMoves.Add (lsbIndex << 17 | (lsbIndex - 8) << 11 | 8 << 7| 7 << 4);
				}
				// 2 move
				if (((lsb >> 16) & ~occupied & 0xff00000000) != 0 && ((lsb >> 8) & occupied) == 0) {
					validMoves.Add (lsbIndex << 17 | (lsbIndex - 16) << 11 |8 << 7 | 7 << 4);
				}
				// capture right
				ulong destinationBit = (lsb&0x7f7f7f7f7f7f7f7f) >> 7;
				if ((destinationBit & whitePieces) != 0) {
					if ((destinationBit & bitboards [0]) != 0) {
						capturedPiece = 0;
					} else if ((destinationBit & bitboards [1]) != 0) {
						capturedPiece = 1 << 4;
					} else if ((destinationBit & bitboards [2]) != 0) {
						capturedPiece = 2 << 4;
					} else if ((destinationBit & bitboards [3]) != 0) {
						capturedPiece = 3 << 4;
					} else if ((destinationBit & bitboards [4]) != 0) {
						capturedPiece = 4 << 4;
					} else if ((destinationBit & bitboards [5]) != 0) {
						capturedPiece = 5 << 4;
					}
					if ((lsb & 0xff00) != 0) {
						// promote to knight, queen detected automatically in next line (0 = queen on end of line)
						captureMoves.Add (lsbIndex << 17 | (lsbIndex - 7) << 11 |8 << 7 |capturedPiece | 1);
					}
					captureMoves.Add (lsbIndex << 17 | (lsbIndex - 7) << 11 |8 << 7 | capturedPiece); 
				}
				// capture left
				destinationBit = (lsb&0xfefefefefefefefe) >> 9;
				if ((destinationBit & whitePieces) != 0) {
					if ((destinationBit & bitboards [0]) != 0) {
						capturedPiece = 0;
					} else if ((destinationBit & bitboards [1]) != 0) {
						capturedPiece = 1 << 4;
					} else if ((destinationBit & bitboards [2]) != 0) {
						capturedPiece = 2 << 4;
					} else if ((destinationBit & bitboards [3]) != 0) {
						capturedPiece = 3 << 4;
					} else if ((destinationBit & bitboards [4]) != 0) {
						capturedPiece = 4 << 4;
					} else if ((destinationBit & bitboards [5]) != 0) {
						capturedPiece = 5 << 4;
					}
					if ((lsb & 0xff00) != 0) {
						// promote to knight, queen detected automatically in next line (0 = queen on end of line)
						captureMoves.Add (lsbIndex << 17 | (lsbIndex - 9) << 11 |8 << 7 |capturedPiece | 1);
					}
					captureMoves.Add (lsbIndex << 17 | (lsbIndex - 9) << 11 | 8 << 7 |capturedPiece); 
				}
				// en passant to the right
				if (lsbIndex - 7 == ((gameState & 0x7e0) >> 5) && (gameState & 0x800) != 0 && (lsb&0x7f7f7f7f7f7f7f7f)!=0) {
					captureMoves.Add (lsbIndex << 17 | (lsbIndex - 7) << 11 |8 << 7 | 6 << 4); 
				} 
				// EP left
				else if (lsbIndex - 9 == ((gameState & 0x7e0) >> 5) && (gameState & 0x800) != 0 && (lsb&0xfefefefefefefefe)!=0) {
					captureMoves.Add (lsbIndex << 17 | (lsbIndex - 9) << 11 |8 << 7 | 6 << 4); 
				}
				pawnBitBoard &= pawnBitBoard - 1;
			}

			// bishops
			ulong bishopBitBoard = bitboards [8];
			while (bishopBitBoard != 0) {
				destinations = 0;
				lsbIndex = leastSigLookup [((bishopBitBoard & (~bishopBitBoard+1)) * debruijnleast) >> 58];

				ulong attacks = rayAttacks[1][lsbIndex];
				ulong blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [1] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				attacks = rayAttacks[7][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [7] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				attacks = rayAttacks[3][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [3] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				attacks = rayAttacks[5][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [5] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				uint destinationIndex = 0;
				while (destinations != 0) {
					ulong destinationBit = destinations & (~destinations+1);;
					if ((destinationBit & whitePieces) != 0) {
						insertStart = true;
						if ((destinationBit & bitboards [0]) != 0) {
							capturedPiece = 0;
						} else if ((destinationBit & bitboards [1]) != 0) {
							capturedPiece = 1 << 4;
						} else if ((destinationBit & bitboards [2]) != 0) {
							capturedPiece = 2 << 4;
						} else if ((destinationBit & bitboards [3]) != 0) {
							capturedPiece = 3 << 4;
						} else if ((destinationBit & bitboards [4]) != 0) {
							capturedPiece = 4 << 4;
						} else if ((destinationBit & bitboards [5]) != 0) {
							capturedPiece = 5 << 4;
						}
					} else {
						insertStart = false;
						capturedPiece = 7 << 4;
					}
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					// destinationIndex = index of destination tile
					if (insertStart) {
						captureMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 10 << 7 | capturedPiece);
					} else {
						validMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 10 << 7 | capturedPiece);
					}
					destinations &= destinations - 1;
				}

				bishopBitBoard &= bishopBitBoard - 1;
			}
			// rooks
			ulong rookBitBoard = bitboards [9];
			while (rookBitBoard != 0) {
				destinations = 0;
				lsbIndex = leastSigLookup [((rookBitBoard & (~rookBitBoard+1)) * debruijnleast) >> 58];

				ulong attacks = rayAttacks[0][lsbIndex];
				ulong blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [0] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				attacks = rayAttacks[2][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [2] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				attacks = rayAttacks[4][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [4] [blockIndex];
				}

				destinations |= attacks & (~blackPieces);

				attacks = rayAttacks[6][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [6] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				uint destinationIndex = 0;
				while (destinations != 0) {
					ulong destinationBit = destinations & (~destinations+1);;
					if ((destinationBit & whitePieces) != 0) {
						insertStart = true;
						if ((destinationBit & bitboards [0]) != 0) {
							capturedPiece = 0;
						} else if ((destinationBit & bitboards [1]) != 0) {
							capturedPiece = 1 << 4;
						} else if ((destinationBit & bitboards [2]) != 0) {
							capturedPiece = 2 << 4;
						} else if ((destinationBit & bitboards [3]) != 0) {
							capturedPiece = 3 << 4;
						} else if ((destinationBit & bitboards [4]) != 0) {
							capturedPiece = 4 << 4;
						} else if ((destinationBit & bitboards [5]) != 0) {
							capturedPiece = 5 << 4;
						}
					} else {
						insertStart = false;
						capturedPiece = 7 << 4;
					}
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					// destinationIndex = index of destination tile
					if (insertStart) {
						captureMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 11 << 7 | capturedPiece);
					} else {
						validMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 11 << 7 | capturedPiece);
					}
					destinations &= destinations - 1;
				}

				rookBitBoard &= rookBitBoard - 1;
			}
			// queen
			ulong queenBitBoard = bitboards [10];

			while (queenBitBoard != 0) {
				ulong attacks;
				ulong blockers;
				destinations = 0;
				lsbIndex = leastSigLookup [((queenBitBoard & (~queenBitBoard+1)) * debruijnleast) >> 58];
				for (int dir = 0; dir < 3; dir++) {
					attacks = rayAttacks [dir] [lsbIndex];
					blockers = attacks & (blackPieces | whitePieces);
					if (blockers != 0) {
						uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
						attacks ^= rayAttacks [dir] [blockIndex];
					}
					destinations |= attacks & (~blackPieces);
				}

				attacks = rayAttacks[7][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [7] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				for (int dir = 3; dir < 7; dir++) {
					attacks = rayAttacks [dir] [lsbIndex];
					blockers = attacks & (blackPieces | whitePieces);
					if (blockers != 0) {
						uint blockIndex = leastSigLookup [(MostSigBitSet (blockers) * debruijnleast) >> 58];
						attacks ^= rayAttacks [dir] [blockIndex];
					}
					destinations |= attacks & (~blackPieces);
				}

				uint destinationIndex = 0;
				while (destinations != 0) {
					ulong destinationBit = destinations & (~destinations+1);;
					if ((destinationBit & whitePieces) != 0) {
						insertStart = true;
						if ((destinationBit & bitboards [0]) != 0) {
							capturedPiece = 0;
						} else if ((destinationBit & bitboards [1]) != 0) {
							capturedPiece = 1 << 4;
						} else if ((destinationBit & bitboards [2]) != 0) {
							capturedPiece = 2 << 4;
						} else if ((destinationBit & bitboards [3]) != 0) {
							capturedPiece = 3 << 4;
						} else if ((destinationBit & bitboards [4]) != 0) {
							capturedPiece = 4 << 4;
						} else if ((destinationBit & bitboards [5]) != 0) {
							capturedPiece = 5 << 4;
						}
					} else {
						insertStart = false;
						capturedPiece = 7 << 4;
					}
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					// destinationIndex = index of destination tile
					if (insertStart) {
						captureMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 12 << 7 | capturedPiece);
					} else {
						validMoves.Add (lsbIndex << 17 | destinationIndex << 11 | 12 << 7 | capturedPiece);
					}
					destinations &= destinations - 1;
				}

				queenBitBoard &= queenBitBoard - 1;
			}
		}
		captureMoves.AddRange (validMoves);
		return captureMoves;
	}



	private void UpdateMoveLog(uint move){
		
		string moveText;
		if ((move>>2)%2 == 1){
			moveText = "O-O";
		}else if ((move>>3)%2 == 1){
			moveText = "O-O-O";
		} else {
			uint fromTile = move >> 17;
			uint toTile = (move >> 11) & 0x3f;
			moveText = across [fromTile % 8] + up [fromTile / 8] + across [toTile % 8] + up [toTile / 8];
		}
		ulong[] boardClone = (ulong[])bitboardArray.Clone ();
		boardClone [12] ^= 1;
		if (CanTakeKing (boardClone)) {
			moveText = moveText + "+";
		}
		if (numberOfMoves % 2 == 0) {
			moveText = ((numberOfMoves + 2) / 2).ToString() + ": " + moveText;
		}
		moveLog.Add (moveText);
		if (moveLog.Count >= 9) {
			moveLog.RemoveAt (0);
			moveLog.RemoveAt (0);
		}

		moveLogText.text = "";

		if (moveLog.Count % 2 == 1) {
			moveLogText.text = moveLog [moveLog.Count - 1] + "\n";
		}
		for (int i = moveLog.Count / 2; i > 0; i--) {
			moveLogText.text += moveLog [2 * i - 2] + "  " + moveLog [2 * i - 1] + "\n";
		}
//		if (!theEndGame) {
//			int blackSum = 0;
//			int whiteSum = 0;
//			for (int x = 0; x < 8; x++) {
//				for (int y = 0; y < 8; y++) {
//					if (boardData [x, y] != -1) {
//						if (boardData [x, y] < 6) {
//							whiteSum += basicValues [boardData [x, y]];
//						} else {
//							blackSum += basicValues [boardData [x, y]];
//						}
//					}
//				}
//			}
//			if (blackSum < 14 && whiteSum < 14) {
//				pieceVals.EnterEndGame ();
//				theEndGame = true;
//			}
//		}
	}

	public void changeDimension(){
		threeD = !threeD;
		if (!threeD) {
			threePieces.SetActive (false);
			twoPieces.SetActive (true);
		} else { 
			threePieces.SetActive (true);
			twoPieces.SetActive (false);
		}
	}

	public void GameOver(){
		SceneManager.LoadScene("Menu");
	}

	void Update(){
		if (threadComplete) {
			threadComplete = false;
			CompleteComp ();
		}

		if (goToComputer) {
			goToComputer = false;
			if (numberOfMoves > 200 && players[0] == 1 && players[1] == 1) {
				return;
			}
			CompTurn ();
		}
		if (Input.GetMouseButtonUp(0) && !gameDone) {
			MouseUp ();
		}
		UpdateSelection ();
		if (computerMove) {
			computerMove = false;
			goToComputer = true;
		}
	}



	private void MouseUp(){
		int gameTurn = (int)(bitboardArray [12] % 2);
		List<uint> allMoves = allValidMoves (bitboardArray);
		// player's turn
		if (selectionX != -1 && players [gameTurn] == 0) {
			int pieceIndex = -1;
			for (int a = 0; a < 12; a++) {
				if ((bitboardArray [a] >> (selectionX + 8 * selectionY)) % 2 == 1) {
					pieceIndex = a;
					break;
				}
			}

			// own piece selected
			if (pieceIndex > -1 + 6 * gameTurn && pieceIndex < 6 + 6 * gameTurn) {
				chosen.transform.position = new Vector3 (selectionX, 0.01f, selectionY);
				chosen.SetActive (true);
				twoChosen.transform.position = new Vector3 (selectionX, selectionY, 0.01f);
				twoChosen.SetActive (true);
				startIndex = selectionX + 8 * selectionY;
				// old, needed for next else if
				startTile = new int[] { selectionX, selectionY };
				startPieceIndex = pieceIndex;

			} else if (startTile[0] != -1){
				endIndex = selectionX + 8 * selectionY;
				endPieceIndex = pieceIndex;
				endTile = new int[] { selectionX, selectionY };
				foreach (uint move in allMoves) {
					if ((move & 0x7e0000) >> 17 == startIndex && (move & 0x1f800) >> 11 == endIndex && move%2 == 0) {
						ulong[] boardClone = (ulong[])bitboardArray.Clone ();
						MakeMove (boardClone, move);
						if (!CanTakeKing (boardClone)) {
							



							if (whiteTurn.activeSelf) {
								whiteTurn.SetActive (false);
								blackTurn.SetActive (true);
							} else {
								whiteTurn.SetActive (true);
								blackTurn.SetActive (false);
							}
							chosen.SetActive (false);
							twoChosen.SetActive (false);

							// drawing castling
							if ((startPieceIndex == 5 || startPieceIndex == 11) && (startTile [0] - endTile [0]) * (startTile [0] - endTile [0]) == 4) {
								if (endTile [0] - startTile [0] == -2) {
									// left
									VisualUpdate ((uint)((startTile [1] * 8) << 17) + ((uint)(startTile [1] * 8 + 3) << 11));
								} else {
									// right
									VisualUpdate((uint)((startTile[1]*8+7)<<17) + ((uint)(startTile [1] * 8 + 5) << 11));
								}
								// drawing en passant
							} else if ((startPieceIndex == 0 || startPieceIndex == 6) && startTile [0] != endTile [0] && endPieceIndex == -1) {
								VisualUpdate ((uint)((endTile [0] + startTile [1] * 8) << 17) + (uint)((endTile [0] + startTile [1] * 8) << 11));
						
							}
							VisualUpdate ((uint)((startTile [0] + startTile [1] * 8) << 17) + (uint)((endTile [0] + endTile [1] * 8) << 11));

							// drawing promotions
							if ((startPieceIndex == 0 || startPieceIndex == 6) && endTile [1] % 7 == 0) {
								VisualUpdate ((uint)((endTile [0] + endTile [1] * 8) << 17) + (uint)((endTile [0] + endTile [1] * 8) << 11));
								GeneratePiece (endTile [0], endTile [1], 4 + 6 * gameTurn);
							}
							MakeMove (bitboardArray, move);
							startTile = new int[] { -1, -1 };
							UpdateMoveLog (move);
							numberOfMoves += 1;

							// avoiding stack
							if (InCheckmate (bitboardArray)) {
								gameDone = true;
								gameOverButton.SetActive (true);
								Invoke ("Ending", 4.0f);

							// need to change to } else
							} else if (players [1 - gameTurn] == 1) { 
								computerMove = true; 
							}
							break;
						}
					}
				}
			}
		}
	}

	private void Ending(){
		gameOverButton.SetActive (false);
	}


	// new


	private void CompTurn(){
	// removed from background for profiling
		new Thread(() => CompStart()) { IsBackground = true }.Start();

	}

	private void CompStart(){

		uint move;
		int turn = (int)bitboardArray[12]%2;
		if (numberOfMoves < 2) {
			ulong[] boardClone = (ulong[])bitboardArray.Clone ();
			if (turn == 1) {
				boardClone [5 + 6 * turn] ^= 0x1000001000000000;
			} else {
				boardClone [5 + 6 * turn] ^= 0x10000010;
			}
			boardClone [12] ^= (ulong)1;
			if (CanTakeKing (boardClone)) {
				if (turn == 1) {
					bestMoves = new List<uint> { 0x671c70 };
				} else {
					bestMoves = new List<uint> { 0x16d870 };
				}
			} else if (turn == 1) {
				bestMoves = new List<uint> { 0x692470 };
			} else {
				bestMoves = new List<uint> { 0x18e070 };
			}

		} else {
		
			NegaSearch (bitboardArray, maxDepth, pieceVals.FullEvaluate ((ulong[])(bitboardArray.Clone ())) * (1 - 2 * turn), -100000, 100000);

//		Debug.Log (Perft(bitboardArray,maxDepth));
//		Debug.Log (checksFound);
//			// }
		}
			threadComplete = true; 
			return;

	}

	private void CompleteComp(){
		if (bestMoves.Count == 0) {
			gameDone = true;
			gameOverButton.SetActive (true);
			Invoke ("Ending", 4.0f);
			return;
		}else{
			int index = rnd.Next (bestMoves.Count);
			uint move = bestMoves[index];


			MakeMove (bitboardArray, move);
			// updatemovelog(move)
			UpdateMoveLog (move);
			if (whiteTurn.activeSelf) {
				whiteTurn.SetActive (false);
				blackTurn.SetActive (true);
			} else {
				whiteTurn.SetActive (true);
				blackTurn.SetActive (false);
			}
			chosen.SetActive (false);
			twoChosen.SetActive (false);

			// to make next bit work
			int startPieceIndex = (int)(((move >> 7) & 7)+6*((move>>10)&1));
			int endPieceIndex = (int)(((move >> 4) & 7)+6 - 6*((move>>10)&1));
			if (endPieceIndex > 5){
				endPieceIndex = -1;
			}
			int[] startTile = new int[] { (int)((move >> 17) % 8), (int)((move >> 17) / 8 )};
			int[] endTile = new int[] { (int)(((move >> 11)&0x3f) % 8), (int)(((move >> 11)&0x3f) / 8 )};
			int gameTurn = (int)(bitboardArray [12] % 2);

			// drawing castling
			if ((startPieceIndex == 5 || startPieceIndex == 11) && (startTile [0] - endTile [0]) * (startTile [0] - endTile [0]) == 4) {
				if (endTile [0] - startTile [0] == -2) {
					// left
					VisualUpdate ((uint)((startTile [1] * 8) << 17) + ((uint)(startTile [1] * 8 + 3) << 11));
				} else {
					// right
					VisualUpdate((uint)((startTile[1]*8+7)<<17) + ((uint)(startTile [1] * 8 + 5) << 11));
				}
				// drawing en passant
			} else if ((startPieceIndex == 0 || startPieceIndex == 6) && startTile [0] != endTile [0] && endPieceIndex == -1) {
				VisualUpdate ((uint)((endTile [0] + startTile [1] * 8) << 17) + (uint)((endTile [0] + startTile [1] * 8) << 11));

			}
			VisualUpdate ((uint)((startTile [0] + startTile [1] * 8) << 17) + (uint)((endTile [0] + endTile [1] * 8) << 11));
			// drawing promotions
			if ((startPieceIndex == 0 || startPieceIndex == 6) && endTile [1] % 7 == 0) {
				VisualUpdate ((uint)((endTile [0] + endTile [1] * 8) << 17) + (uint)((endTile [0] + endTile [1] * 8) << 11));
				if (move % 2 == 0) {
					GeneratePiece (endTile [0], endTile [1], 4 + 6 * gameTurn);
				} else {
					GeneratePiece (endTile [0], endTile [1], 1 + 6 * gameTurn);
				}
			}
			numberOfMoves += 1;

			// avoiding stack
			if (InCheckmate (bitboardArray)) {
				gameDone = true;
				gameOverButton.SetActive (true);
				Invoke ("Ending", 4.0f);

				// need to change to } else
			} else if (players [gameTurn] == 1) { 
				computerMove = true; 
			}
		}
	}

	int checksFound = 0;

	private int Perft(ulong[] bitboards, int depthLeft){
		if (depthLeft == maxDepth) {
			checksFound = 0;
		}
		if (depthLeft == 0) {
			bitboards [12] ^= 1;
			if (CanTakeKing (bitboards)) {
				checksFound += 1;
			}
			return 1;
		}
		int perft = 0;

		// find moves
		List<uint> validMoves = allValidMoves(bitboards);
		ulong gameState = bitboards [12];
		foreach (uint move in validMoves){
			MakeMove (bitboards, move);
			if(!CanTakeKing(bitboards)){
				// actually a valid move
				perft += Perft(bitboards,depthLeft-1);
			}
			bitboards [12] = gameState;
			UnMakeMove (bitboards, move);
		}
		return perft;
	}



	private int NegaSearch(ulong[] bitboards, int depthLeft, int baseValue, int lowerCutoff, int upperCutOff){

		if (depthLeft == 0) {
			return  baseValue;
		}
		if (depthLeft == maxDepth) {
			bestMoves = new List<uint>();
		}

		// find moves
		List<uint> validMoves = allValidMoves(bitboards);
		if (validMoves.Count == 0) {
			bitboards [12] ^= 1;
			if (CanTakeKing(bitboards)){
				return  -10000; 
			} else{
				return  0;
			}
		}

		int bestValue = -100000; 
		ulong gameState = bitboards [12];
		foreach (uint move in validMoves){


//			ulong[] boardClone = (ulong[])bitboards.Clone ();
//			boardClone = MakeMove (boardClone, move);
			MakeMove(bitboards, move);

//			if(!CanTakeKing(boardClone)){
			if (!CanTakeKing(bitboards)){
				// actually a valid move
				// find true value of this branch
				int testValue = baseValue + pieceVals.AdjustScore (move);

				// check score adjustment
//				if (depthLeft == 1) {
//					boardClone[12] ^= 1;
//					if (CanTakeKing(boardClone)) {
//						testValue += 5;
//						boardClone[12] ^= 1;
//							if (InCheckmate (boardClone)) {
//							testValue += 4000;
//							}
//						}
//					}

//				testValue = -NegaSearch (boardClone, depthLeft - 1, -testValue,  -upperCutOff, -lowerCutoff);
				testValue = -NegaSearch (bitboards, depthLeft - 1, -testValue,  -upperCutOff, -lowerCutoff);
				if (testValue > bestValue) {
					bestValue = testValue;
					if (depthLeft == maxDepth) {
						bestMoves = new List<uint> { move };
					}
				} else if (testValue == bestValue && depthLeft == maxDepth) {
					bestMoves.Add(move);
				}
				
				if (testValue > lowerCutoff) {
					lowerCutoff = testValue;
				} 
				if (lowerCutoff >= upperCutOff) {
					bitboards [12] = gameState;
					UnMakeMove (bitboards, move);
					break;
				}
			}
			bitboards [12] = gameState;
			UnMakeMove (bitboards, move);
		}
		if (depthLeft == maxDepth) {
			Debug.Log (bestValue);
		}
		return bestValue;
	}




	private bool CanTakeKing(ulong[] bitboards){
		ulong whitePieces = bitboards [0] | bitboards [1] | bitboards [2] | bitboards [3] | bitboards [4] | bitboards [5];
		ulong blackPieces = bitboards [6] | bitboards [7] | bitboards [8] | bitboards [9] | bitboards [10] | bitboards [11];
		ulong occupied = whitePieces | blackPieces;
		ulong destinations;
		ulong gameState = bitboards [12];

		// white turn
		if (gameState % 2 == 0) {

			// knights
			uint lsbIndex = 0;
			ulong knightBitBoard = bitboards [1];
			while (knightBitBoard != 0) {

				lsbIndex = leastSigLookup [((knightBitBoard & (~knightBitBoard+1)) * debruijnleast) >> 58];
				// lsbIndex = index of knight on board

				destinations = knightMoves [lsbIndex] & (~whitePieces);
				if ((destinations & bitboards [11]) != 0) {
					return true;
				}
				knightBitBoard &= knightBitBoard - 1;
			}

			// king
			ulong kingBitBoard = bitboards [5];

			while (kingBitBoard != 0) {

				lsbIndex = leastSigLookup [((kingBitBoard & (~kingBitBoard+1)) * debruijnleast) >> 58];
				// lsbIndex = index of knight on board

				destinations = kingMoves [lsbIndex] & (~whitePieces);
				if ((destinations & bitboards [11]) != 0) {
					return true;
				}
				kingBitBoard &= kingBitBoard - 1;
			}

			// pawns
			ulong pawnBitBoard = bitboards[0];
			while (pawnBitBoard != 0) {
				ulong lsb = pawnBitBoard & (~pawnBitBoard+1);
				lsbIndex = leastSigLookup [(lsb * debruijnleast) >> 58];

				// capture left
				ulong destinationBit = (lsb&0xfefefefefefefefe) << 7;
				if ((destinationBit & bitboards[11]) != 0) {
					return true;
				}
				// capture right
				if ((lsb & 0x7f7f7f7f7f7f7f7f) == 0x400000) {
				}
				destinationBit = (lsb&0x7f7f7f7f7f7f7f7f) << 9;
			
				if ((destinationBit & bitboards [11]) != 0) {
					return true;
				}
				pawnBitBoard &= pawnBitBoard - 1;
			}


			// bishops
			ulong bishopBitBoard = bitboards [2];
			while (bishopBitBoard != 0) {
				destinations = 0;
				lsbIndex = leastSigLookup [((bishopBitBoard & (~bishopBitBoard+1)) * debruijnleast) >> 58];

				ulong attacks = rayAttacks[1][lsbIndex];
				ulong blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [1] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				attacks = rayAttacks[7][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [7] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				attacks = rayAttacks[3][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [3] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				attacks = rayAttacks[5][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [5] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				if ((destinations & bitboards [11]) != 0) {
					return true;
				}

				bishopBitBoard &= bishopBitBoard - 1;
			}

			// rooks
			ulong rookBitBoard = bitboards [3];
			while (rookBitBoard != 0) {
				destinations = 0;
				lsbIndex = leastSigLookup [((rookBitBoard & (~rookBitBoard+1)) * debruijnleast) >> 58];

				ulong attacks = rayAttacks[0][lsbIndex];
				ulong blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [0] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				attacks = rayAttacks[2][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [2] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				attacks = rayAttacks[4][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [4] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				attacks = rayAttacks[6][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [6] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);


				if ((destinations & bitboards [11]) != 0) {
					return true;
				}
				rookBitBoard &= rookBitBoard - 1;
			}
			// queen
			ulong queenBitBoard = bitboards [4];

			while (queenBitBoard != 0) {
				ulong attacks;
				ulong blockers;
				destinations = 0;
				lsbIndex = leastSigLookup [((queenBitBoard & (~queenBitBoard+1)) * debruijnleast) >> 58];
				for (int dir = 0; dir < 3; dir++) {
					attacks = rayAttacks [dir] [lsbIndex];
					blockers = attacks & (blackPieces | whitePieces);
					if (blockers != 0) {
						uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
						attacks ^= rayAttacks [dir] [blockIndex];
					}
					destinations |= attacks & (~whitePieces);
				}

				attacks = rayAttacks[7][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [7] [blockIndex];
				}
				destinations |= attacks & (~whitePieces);

				for (int dir = 3; dir < 7; dir++) {
					attacks = rayAttacks [dir] [lsbIndex];
					blockers = attacks & (blackPieces | whitePieces);
					if (blockers != 0) {
						uint blockIndex = leastSigLookup [(MostSigBitSet (blockers) * debruijnleast) >> 58];
						attacks ^= rayAttacks [dir] [blockIndex];
					}
					destinations |= attacks & (~whitePieces);
				}

				if ((destinations & bitboards [11]) != 0) {
					return true;
				}

				queenBitBoard &= queenBitBoard - 1;
			}

			// black turn
		} else {
			// knights
			uint lsbIndex = 0;
			ulong knightBitBoard = bitboards [7];
			while (knightBitBoard != 0) {
				lsbIndex = leastSigLookup [((knightBitBoard & (~knightBitBoard+1)) * debruijnleast) >> 58];
				// lsbIndex = index of knight on board

				destinations = knightMoves [lsbIndex] & (~blackPieces);
				if ((destinations & bitboards [5]) != 0) {
					return true;
				}
				knightBitBoard &= knightBitBoard - 1;
			}
			// king
			// commented to avoid stack
			ulong kingBitBoard = bitboards [11];

			while (kingBitBoard != 0) {

				lsbIndex = leastSigLookup [((kingBitBoard & (~kingBitBoard+1)) * debruijnleast) >> 58];
				// lsbIndex = index of knight on board

				destinations = kingMoves [lsbIndex] & (~blackPieces);
				if ((destinations & bitboards [5]) != 0) {
					return true;
				}
				kingBitBoard &= kingBitBoard - 1;
			}

			// pawns
			ulong pawnBitBoard = bitboards[6];
			while (pawnBitBoard != 0) {
				destinations = 0;
				ulong lsb = pawnBitBoard & (~pawnBitBoard+1);
				lsbIndex = leastSigLookup [(lsb * debruijnleast) >> 58];

				// capture right
				ulong destinationBit = (lsb&0x7f7f7f7f7f7f7f7f) >> 7;
				if ((destinationBit & bitboards[5]) != 0) {
					return true;
				}
				// capture left
				destinationBit = (lsb&0xfefefefefefefefe) >> 9;
				if ((destinationBit & bitboards[5]) != 0) {
					return true;
				}
				pawnBitBoard &= pawnBitBoard - 1;
			}

			// bishops
			ulong bishopBitBoard = bitboards [8];
			while (bishopBitBoard != 0) {
				destinations = 0;
				lsbIndex = leastSigLookup [((bishopBitBoard & (~bishopBitBoard+1)) * debruijnleast) >> 58];

				ulong attacks = rayAttacks[1][lsbIndex];
				ulong blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [1] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				attacks = rayAttacks[7][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [7] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				attacks = rayAttacks[3][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [3] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				attacks = rayAttacks[5][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [5] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				if ((destinations & bitboards [5]) != 0) {
					return true;
				}

				bishopBitBoard &= bishopBitBoard - 1;
			}
			// rooks
			ulong rookBitBoard = bitboards [9];
			while (rookBitBoard != 0) {
				destinations = 0;
				lsbIndex = leastSigLookup [((rookBitBoard & (~rookBitBoard+1)) * debruijnleast) >> 58];

				ulong attacks = rayAttacks[0][lsbIndex];
				ulong blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [0] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				attacks = rayAttacks[2][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [2] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				attacks = rayAttacks[4][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [4] [blockIndex];
				}

				destinations |= attacks & (~blackPieces);

				attacks = rayAttacks[6][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [(MostSigBitSet(blockers) * debruijnleast) >> 58];
					attacks ^= rayAttacks [6] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				if ((destinations & bitboards [5]) != 0) {
					return true;
				}

				rookBitBoard &= rookBitBoard - 1;
			}
			// queen
			ulong queenBitBoard = bitboards [10];

			while (queenBitBoard != 0) {
				ulong attacks;
				ulong blockers;
				destinations = 0;
				lsbIndex = leastSigLookup [((queenBitBoard & (~queenBitBoard+1)) * debruijnleast) >> 58];
				for (int dir = 0; dir < 3; dir++) {
					attacks = rayAttacks [dir] [lsbIndex];
					blockers = attacks & (blackPieces | whitePieces);
					if (blockers != 0) {
						uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
						attacks ^= rayAttacks [dir] [blockIndex];
					}
					destinations |= attacks & (~blackPieces);
				}

				attacks = rayAttacks[7][lsbIndex];
				blockers = attacks & (blackPieces | whitePieces);
				if (blockers != 0) {
					uint blockIndex = leastSigLookup [((blockers & (~blockers+1)) * debruijnleast) >> 58];
					attacks ^= rayAttacks [7] [blockIndex];
				}
				destinations |= attacks & (~blackPieces);

				for (int dir = 3; dir < 7; dir++) {
					attacks = rayAttacks [dir] [lsbIndex];
					blockers = attacks & (blackPieces | whitePieces);
					if (blockers != 0) {
						uint blockIndex = leastSigLookup [(MostSigBitSet (blockers) * debruijnleast) >> 58];
						attacks ^= rayAttacks [dir] [blockIndex];
					}
					destinations |= attacks & (~blackPieces);
				}

				if ((destinations & bitboards [5]) != 0) {
					return true;
				}
				queenBitBoard &= queenBitBoard - 1;
			}
		}
		return false;
	}

	private bool InCheckmate(ulong[] bitboards){
		List<uint> possibleMoves = allValidMoves(bitboards);
		ulong gameState = bitboards [12];
		foreach (uint move in possibleMoves) {
			MakeMove (bitboards, move);
			if (!CanTakeKing (bitboards)) {
				bitboards [12] = gameState;
				UnMakeMove (bitboards, move);
				return false;

				//				return false;
//			ulong[] boardClone = (ulong[])bitboards.Clone();
//			if (!CanTakeKing(MakeMove(boardClone, possibleMoves[a]))) {
//				return false;
			}
			bitboards [12] = gameState;
			UnMakeMove (bitboards, move);
		}
		return true;
	}



	//private ulong[] UnMakeMove(ulong[] bitboards, uint move){
	private void UnMakeMove(ulong[] bitboards, uint move){
		
		uint turn = ((move & 0x400) >> 10);
		uint movingPieceIndex = ((move & 0x380) >> 7) + 6 * turn;

		// castling
		if ((move & 0xc) == 4) {
			// king side
			bitboards [3 + 6 * turn] ^= (ulong)0xa0 << 56 * (int)turn;
		} else if ((move & 0xc) == 8) {
			// queen side
			bitboards [3 + 6 * turn] ^= (ulong)0x9 << 56 * (int)turn;
		}
		// possible captured piece
		uint capturedType = ((move & 0x70) >> 4);
		if (capturedType < 6) {
			bitboards [capturedType + 6 - 6 * turn] ^= ((ulong)1 << (int)((move & 0x1f800) >> 11));
		} else if (capturedType == 6) {
			// en passant, correct pawn bitboard based on en passant tile
			bitboards[6-6*turn] ^= (ulong)1<<(((int)(bitboards[12] & 0x7e0) >> 5)-8+16*(int)turn);
		}
		//swap end line pawns for promoted pieces
		if (movingPieceIndex % 6 == 0) {
			if ((move & 0x3) == 0) {
				// promote to queen
				bitboards [4 + 6 * turn] ^= (((ulong)1 << (int)((move & 0x1f800)>>11))& 0xff000000000000ff);
			} else {
				bitboards [1 + 6 * turn] ^= (((ulong)1 << (int)((move & 0x1f800)>>11))& 0xff000000000000ff);
			}
			bitboards [6 * turn] ^= (((ulong)1 << (int)((move & 0x1f800)>>11))& 0xff000000000000ff);
		}

		bitboards[movingPieceIndex] ^= ((ulong)1 << (int)((move & 0x7e0000)>>17)) + ((ulong)1 << (int)((move & 0x1f800)>>11));

		//return bitboards;
	}
	// private ulong[] MakeMove(ulong[] bitboards, uint move){
	private void MakeMove(ulong[] bitboards, uint move){
		uint turn = ((move & 0x400) >> 10);
		// move piece on its bitboard
		uint movingPieceIndex = ((move & 0x380) >> 7) + 6 * turn;
		bitboards[movingPieceIndex] ^= ((ulong)1 << (int)((move & 0x7e0000)>>17)) + ((ulong)1 << (int)((move & 0x1f800)>>11));

		//swap end line pawns for promoted pieces
		if ((move & 0x3) == 0) {
			// promote to queen
			bitboards [4 + 6 * turn] ^= (bitboards [turn * 6] & 0xff000000000000ff);
		} else {
			bitboards [1 + 6 * turn] ^= (bitboards [turn * 6] & 0xff000000000000ff);
		}
		bitboards [6 * turn] &= 0xffffffffffff00;

		// possible captured piece
		uint capturedType = ((move & 0x70) >> 4);
		if (capturedType < 6) {
			bitboards [capturedType + 6 - 6 * turn] ^= ((ulong)1 << (int)((move & 0x1f800) >> 11));
		} else if (capturedType == 6) {
			// en passant, correct pawn bitboard based on en passant tile
			bitboards[6-6*turn] ^= (ulong)1<<(((int)(bitboards[12] & 0x7e0) >> 5)-8+16*(int)turn);
		}

		// castling
		if ((move & 0xc) == 4) {
			// king side
			bitboards [3 + 6 * turn] ^= (ulong)0xa0 << 56 * (int)turn;
		} else if ((move & 0xc) == 8) {
			// queen side
			bitboards [3 + 6 * turn] ^= (ulong)0x9 << 56 * (int)turn;
		}

		// set en passant tile, en passant allowed, castling variables

		// castling
		//white
		if ((bitboards [12] & 24) != 0) {
			// king
			if (bitboards [5] != 16) {
				bitboards [12] &= 0xfe7;
			} else { 
				// rooks
				if ((bitboards [3] & 1) == 0) {
					bitboards [12] &= 0xff7;
				}
				if ((bitboards [3] & 0x80) == 0) {
					bitboards [12] &= 0xfef;
				}
			}
		}
		// black
		if ((bitboards [12] & 6) != 0) {
			// king
			if (bitboards [11] != 0x1000000000000000) {
				bitboards [12] &= 0xff9;
			} else { 
				// rooks
				if ((bitboards [9] & 0x100000000000000) == 0) {
					bitboards [12] &= 0xffd;
				}
				if ((bitboards [9] & 0x8000000000000000) == 0) {
					bitboards [12] &= 0xffb;
				}
			}
		}
		// reset en passant
		bitboards [12] &= 0x1f;
		if ((move & 0x380) == 0 && (((move & 0x7e0000) >> 17) + 16 - 32 * turn) == ((move & 0x1f800) >> 11)) {
			// en passant
		
			uint epTile = ((move & 0x7e0000) >> 17) + 8 - 16 * turn;
			bitboards [12] |= 0x800 + (epTile << 5);
		} 
			
		// change whos turn it is
		bitboards [12] ^= 1;
		// return bitboards;
	}

	private void VisualUpdate(uint move){
		uint fromIndex = ((move & 0x7e0000) >> 17);
		uint toIndex = ((move & 0x1f800) >> 11);
		int[] fromTile = new int[] { (int)(fromIndex % 8), (int)(fromIndex / 8) };
		int[] toTile = new int[] { (int)(toIndex % 8), (int)(toIndex / 8) };

		fromtwo.SetActive (true);
		fromthree.SetActive (true);
		totwo.SetActive (true);
		tothree.SetActive (true);
		fromtwo.transform.position = new Vector3 (fromTile [0], fromTile[1], 0.02f);
		totwo.transform.position = new Vector3 (toTile [0], toTile[1], 0.02f);
		fromthree.transform.position = new Vector3 (fromTile [0], 0.02f, fromTile[1]);
		tothree.transform.position = new Vector3 (toTile [0], 0.02f, toTile[1]);

		foreach (Transform child in threePieces.transform) {
				if (child.position.x == toTile [0] && child.position.z == toTile [1] && child.gameObject.CompareTag ("Piece")) {
					Destroy (child.gameObject);
				} 
			}
			foreach (Transform child in twoPieces.transform) {
				if (child.position.x == toTile [0] && child.position.y == toTile [1] && child.gameObject.CompareTag ("twoDimensionPiece")) {
					Destroy (child.gameObject);
				}
			} 


		foreach (Transform child in threePieces.transform) {
			if (child.position.x == fromTile [0] && child.position.z == fromTile [1] && child.gameObject.CompareTag ("Piece")) {
				child.position = new Vector3 (toTile [0], 0.0f, toTile [1]);

			} 
		}
		
		foreach (Transform child in twoPieces.transform){
			if (child.position.x == fromTile [0] && child.position.y == fromTile [1] && child.gameObject.CompareTag ("twoDimensionPiece")) {
				child.position = new Vector3 (toTile [0], toTile[1], 0.0f);
			}
		}
			
	}
	/// edited
	public Transform topRight;
	public Transform bottomLeft;

	private void UpdateSelection(){
		if (!Camera.main) {
			return;
		}
		if (computerMove) {
			selection.SetActive (false);
			twoSelection.SetActive (false);
			return;
		}
		if (threeD) {
			twoSelection.SetActive (false);
			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 50.0f, LayerMask.GetMask ("ChessPlane"))) {
				selectionX = (int)(hit.point.x + 0.5);
				selectionY = (int)(hit.point.z + 0.5);
				selection.SetActive (true);
			} else {
				selection.SetActive (false);
				selectionX = -1;
				selectionY = -1;
			}
			selection.transform.position = new Vector3 (selectionX, 0.01f, selectionY);
		} else {

			/////// edited
			Vector3 offset = Camera.main.WorldToScreenPoint(bottomLeft.position);
			Vector3 maxim =  Camera.main.WorldToScreenPoint(topRight.position);
	
			/////
			selection.SetActive (false);
			float x = Input.mousePosition.x;
			float y = Input.mousePosition.y;
			if (x>offset.x && x<maxim.x && y>offset.y && y<maxim.y){
				selectionX = (int)((x-offset.x)/((maxim.x - offset.x)/8));
				selectionY = (int)((y-offset.y)/((maxim.y - offset.y)/8));
				twoSelection.SetActive (true);
			} else {
				twoSelection.SetActive (false);
				selectionX = -1;
				selectionY = -1;
			}
			twoSelection.transform.position = new Vector3 (selectionX, selectionY, 0.02f);
		}
	}

	void DecreaseTime(){
		if (bitboardArray[12]%2 == 0) {
			if (whiteTime > 0) {
				whiteTime -= 1;
				whiteTimeText.text = "White: " + whiteTime / 60 + ":" + new System.String('0', ((whiteTime % 60).ToString ().Length) % 2) + (whiteTime % 60).ToString ();
				if (whiteTime == 0) {
					gameDone = true;
					gameOverButton.SetActive (true);
					Invoke ("Ending", 4.0f);
				}
			}
		} else if (blackTime > 0) {
			blackTime -= 1;
			blackTimeText.text = "Black: " + blackTime / 60 + ":" + new System.String('0', ((blackTime % 60).ToString ().Length) % 2) + (blackTime % 60).ToString ();
			if (blackTime == 0) {
				gameDone = true;
				gameOverButton.SetActive (true);
				Invoke ("Ending", 4.0f);
			}
		}
	}


	void Start (){


		moveLogText.text = "";

		maxDepth = PlayerPrefs.GetInt ("Depth");
		blackTurn.SetActive (false);
		whiteTurn.SetActive (true);
		players = new int[] { 0, 0 };
		whiteTime =  PlayerPrefs.GetInt("WhiteTime") * 60;
		blackTime = PlayerPrefs.GetInt("BlackTime") * 60;
		InvokeRepeating("DecreaseTime", 1.0f, 1.0f);
		if (whiteTime == 0) {
			whiteTimeText.text = "";
		} else {
			whiteTimeText.text = "White: " + whiteTime / 60 + ":" + new System.String('0', ((whiteTime % 60).ToString ().Length) % 2) + (whiteTime % 60).ToString ();
		}
		if (blackTime == 0) {
			blackTimeText.text = "";
		} else {
			blackTimeText.text = "Black: " + blackTime / 60 + ":" + new System.String('0', ((blackTime % 60).ToString ().Length) % 2) + (blackTime % 60).ToString ();
		}

		string white = PlayerPrefs.GetString("White");
		if (white != "Player") {
			players [0] = 1;
			computerMove = true;
		}
		string black = PlayerPrefs.GetString("Black");
		if (black != "Player") {
			players [1] = 1;
		}

		// Generate starting board
		// Pawns
		for (int i = 0; i<8; i++){
			GeneratePiece(i,1,0);
			GeneratePiece(i,6,6);
			}
		// Rooks
		for (int i = 0; i < 2; i++) {
			GeneratePiece (0, 7 * i,  3 + 6 * i);
			GeneratePiece (7, 7 * i,  3 + 6 * i);
		}
		// Knights
		for (int i = 0; i < 2; i++) {
			GeneratePiece (1, 7 * i,  1 + 6 * i);
			GeneratePiece (6, 7 * i,  1 + 6 * i);
		}
		// Bishops
		for (int i = 0; i < 2; i++) {
			GeneratePiece (2, 7 * i,  2 + 6 * i);
			GeneratePiece (5, 7 * i,  2 + 6 * i);
		}
		// Royalty
		for (int i = 0; i < 2; i++) {
			GeneratePiece (3, 7 * i, 4 + 6 * i);
			GeneratePiece (4, 7 * i, 5 + 6 * i);
		}

	}
		
	private void GeneratePiece(int x, int y, int pieceIndex){
		GameObject go = Instantiate (GetPiece(pieceIndex));
		go.transform.position = (Vector3.right * x) + (Vector3.forward * y);
		go.transform.parent = threePieces.transform;
		GameObject twoDimPiece = Instantiate (GetPieceTwo (pieceIndex));
		twoDimPiece.transform.position = (Vector3.right * x) + (Vector3.up * y);
		twoDimPiece.transform.parent = twoPieces.transform;
	}

	private GameObject GetPieceTwo(int pieceIndex){
		return twoDimensionPieces [pieceIndex];
	}

	private GameObject GetPiece(int pieceIndex){
		return piecePrefabs [pieceIndex];
	}
}
