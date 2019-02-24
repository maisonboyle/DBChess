using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	// [WP,WKn,WB,WR,WQ,WK, bp,bkn,bb,br,bq,bk]
	public AudioSource moveSound;
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
	public GameObject threePieces;
	public GameObject twoPieces;
	public Text moveLogText;
	public GameObject whiteTurn;
	public GameObject blackTurn;

	private static ulong[] rookMagics = new ulong[64];
	private static ulong[] bishopMagics = new ulong[64];

	private static int[] rookShifts = new int[] {52, 53, 53, 53, 53, 53, 53, 52, 53, 54, 54, 54, 54, 54, 54, 53, 53, 54, 54, 54, 54, 54, 54, 53, 53, 54, 54, 54, 54, 54, 54, 53,
		53, 54, 54, 54, 54, 54, 54, 53, 53, 54, 54, 54, 54, 54, 54, 53, 53, 54, 54, 54, 54, 54, 54, 53, 52, 53, 53, 53, 53, 53, 53, 52
	};
	private static int[] bishopShifts = new int[] {58, 59, 59, 59, 59, 59, 59, 58, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 57, 57, 57, 57, 59, 59, 59, 59, 57, 55, 55, 57, 59, 59, 59, 59, 57, 55,
		55, 57, 59, 59, 59, 59, 57, 57, 57, 57, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 58, 59, 59, 59, 59, 59, 59, 58
	};

	private static ulong[] rookPremasks = new ulong[64];
	private static ulong[] bishopPremasks = new ulong[64];

	// provides quick lookup for positions under attack given position and occupancy of board
	private static ulong[][] magicRookAttacks = new ulong[64][];
	private static ulong[][] magicBishopAttacks = new ulong[64][];

	private static List<uint[]> openingLines = null; 

	private static ulong[] knightMoves = new ulong[64];
	private static ulong[] kingMoves = new ulong[64];

	// board evaluation scores
	static int[] baseValues = {100, 320, 330, 500, 900, 20000, 0, 0};
	static int[][] pieceSquareTable = null;
	// modified scores towards endgame
	private static int[][] endPieceSquares = null;

	private static void EnterEndGame(){
		pieceSquareTable [0] = endPieceSquares [0];
		pieceSquareTable [5] = endPieceSquares [1];
		pieceSquareTable [6] = endPieceSquares [2];
		pieceSquareTable [11] = endPieceSquares [3];
	}

	private static int FullEvaluate(ulong[] bitboards){
		int value = 0;
		uint lsbIndex;
		// white pieces
		for (int a = 0; a < 6; a++) {
			ulong pieceBoard = bitboards [a];
			while (pieceBoard != 0) {
				lsbIndex = leastSigLookup [((pieceBoard & (~pieceBoard+1)) * debruijnleast) >> 58];
				value += pieceSquareTable [a] [lsbIndex] + baseValues [a];
				pieceBoard &= pieceBoard - 1;
			}
		}
		// black pieces
		for (int a = 6; a < 12; a++) {
			ulong pieceBoard = bitboards [a];
			while (pieceBoard != 0) {
				lsbIndex = leastSigLookup [((pieceBoard & (~pieceBoard+1)) * debruijnleast) >> 58];
				value -= pieceSquareTable [a] [lsbIndex] + baseValues [a - 6];
				pieceBoard &= pieceBoard - 1;
			}
		}
		return value;
	}

	private static int AdjustScore(uint move){
		int value = 0;
		// castling
		if (((move >> 2) & 1) == 1) {
			return 30;
		} else if (((move >> 3) & 1) == 1) {
			return 15;
		}
		uint toIndex = ((move >> 11) & 0x3f); 
		uint fromIndex = move >> 17; 
		uint pieceIndex = ((move >> 7) & 0x7);
		uint capturedIndex = ((move >> 4) & 0x7);
		uint turn = (move >> 10) & 1;
		// promotion
		if ((toIndex > 55 | toIndex < 8) && pieceIndex == 0) {
			if ((move & 1) == 0) {
				return 750;
			} else {
				return 180;
			}
		}
		// standard
		value += pieceSquareTable[pieceIndex+6*turn][toIndex] - pieceSquareTable[pieceIndex+6*turn][fromIndex];

		if (capturedIndex != 7) {
			if (capturedIndex == 6) {
				value += 100 + pieceSquareTable [6 - 6 * turn][(toIndex+fromIndex)/2];
			} else {
				value += pieceSquareTable [capturedIndex + 6 - 6 * turn] [toIndex] + baseValues[capturedIndex];
			}
		}
		return value;
	}

	// initial game conditions
	// { (white) pawn, knight, bishop, rook, queen, king, (black) pawn, knight ...} Gamestate added on as 12th item (0x1e), hashvalue as 13th item

	private ulong[] bitboardArray = new ulong[] {0xff00, 0x42, 0x24, 0x81, 0x8, 0x10,
		0xff000000000000, 0x4200000000000000, 0x2400000000000000, 0x8100000000000000, 0x800000000000000, 0x1000000000000000, 0x1e, 0};

	private static ulong[][] hashValues = new ulong[12][];
	private List<ulong> visitedHashes = new List<ulong> ();
	private List<ulong> cantRepeat = new List<ulong> ();
	// tabulates recent positions and scores for quick value lookup of repeated positions
	private static uint[] TTHashes = new uint[262144];
	private static int[] TTValues = new int[262144];
	private static int[] TTDepths = new int[262144];
	private static uint[] TTMoves = new uint[262144];

	// human (0) or comp (1) for white 1st and black 2nd
	public int[] players;

	// click selection highlighted on board
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
	private bool gameDone = false;
	
	private int numberOfMoves = 0;
	private int maxDepth, extensionDepth;
	List<uint> bestMoves = new List<uint>();

	// setup to display moves in log on screen
	private string[] across = new string[] {"a","b","c","d","e","f","g","h"};
	private string[] up = new string[] {"1","2","3","4","5","6","7","8"};
	private List<string> moveLog = new List<string>();

	// separate thread for searching, keeps game interactive
	private bool threadComplete = false;
	static System.Random rnd = new System.Random();

	// castling represented by moving king across two
	private int[] basicValues = new int[] {1,3,3,5,9,0,1,3,3,5,9,0};
	bool theEndGame = false;

	// quick method for LSB lookup
	private static uint[] leastSigLookup = {
		0,  1, 48,  2, 57, 49, 28,  3,
		61, 58, 50, 42, 38, 29, 17,  4,
		62, 55, 59, 36, 53, 51, 43, 22,
		45, 39, 33, 30, 24, 18, 12,  5,
		63, 47, 56, 27, 60, 41, 37, 16,
		54, 35, 52, 21, 44, 32, 23, 11,
		46, 26, 40, 15, 34, 20, 31, 10,
		25, 14, 19,  9, 13,  8,  7,  6
	}; 
	private static ulong debruijnleast = 0x03f79d71b4cb0a89;

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

	// for finding valid moves/positions
	private static int numberChecks;
	private static ulong bishopCheckers;
	private static ulong rookCheckers;
	private static ulong captureMask;
	private static ulong kingDangers;
	private static uint kingPos;

	// identify if king in check / where from
	private static int FindChecks(ulong[] bitboards, ulong occupancy){
		uint lsbIndex;
		numberChecks = 0;
		captureMask = 0;
		kingDangers = 0;
		rookCheckers = 0;
		bishopCheckers = 0;
		int turn = (int)bitboards [12] & 1;
		ulong kingBoard = bitboards [5 + 6 * turn];
		kingPos = leastSigLookup [((kingBoard & (~kingBoard + 1)) * debruijnleast) >> 58];
		occupancy ^= kingBoard;

		// pawn captures
		if (turn == 0) {
			// white
			kingDangers |= ((bitboards [6] >> 9)&0x7f7f7f7f7f7f7f7f | (bitboards [6] >> 7)&0xfefefefefefefefe);
			if ((kingDangers & kingBoard) != 0) {
				captureMask = ((kingBoard << 9) & 0xfefefefefefefefe | (kingBoard >> 7) & 0x7f7f7f7f7f7f7f7f) & bitboards [6];
				numberChecks++;
			}
		} else {
			// black
			kingDangers |= ((bitboards [0] << 9)&0xfefefefefefefefe | (bitboards [0] << 7)&0x7f7f7f7f7f7f7f7f);
			if ((kingDangers & kingBoard) != 0) {
				captureMask = ((kingBoard >> 9) & 0x7f7f7f7f7f7f7f7f | (kingBoard >> 7) & 0xfefefefefefefefe) & bitboards [0];
				numberChecks++;
			}
		}
		// knights
		ulong pieceBitBoard = bitboards[7-6*turn];
		ulong pieceAttacks = 0;
		while (pieceBitBoard != 0) {
			lsbIndex = leastSigLookup [((pieceBitBoard & (~pieceBitBoard + 1)) * debruijnleast) >> 58];
			pieceAttacks |= knightMoves [lsbIndex];
			pieceBitBoard &= pieceBitBoard - 1;
		}
		kingDangers |= pieceAttacks;
		if ((pieceAttacks & kingBoard) != 0) {
			captureMask |= knightMoves[kingPos] & bitboards [7-6*turn];
			numberChecks++;
		}
		// opponent king
		pieceBitBoard = bitboards [11-6*turn];
		kingDangers |= kingMoves [leastSigLookup [((pieceBitBoard & (~pieceBitBoard + 1)) * debruijnleast) >> 58]];

		// sliding pieces
		// bishops
		pieceBitBoard = bitboards [8-6*turn];
		while (pieceBitBoard != 0) {
			lsbIndex = leastSigLookup [((pieceBitBoard & (~pieceBitBoard + 1)) * debruijnleast) >> 58];
			pieceAttacks = magicBishopAttacks [lsbIndex] [((bishopPremasks [lsbIndex] & occupancy) * bishopMagics [lsbIndex]) >> bishopShifts [lsbIndex]];
			if ((pieceAttacks & kingBoard) != 0) {
				captureMask |= magicBishopAttacks [kingPos] [((bishopPremasks [kingPos] & occupancy) * bishopMagics [kingPos]) >> bishopShifts [kingPos]] & bitboards [8 - 6 * turn];
				numberChecks++;
				bishopCheckers |= pieceBitBoard & (~pieceBitBoard + 1);
			}
			kingDangers |= pieceAttacks;
			pieceBitBoard &= pieceBitBoard - 1;
		}

		// rooks
		pieceBitBoard = bitboards [9-6*turn];
		while (pieceBitBoard != 0) {
			lsbIndex = leastSigLookup [((pieceBitBoard & (~pieceBitBoard + 1)) * debruijnleast) >> 58];
			pieceAttacks = magicRookAttacks [lsbIndex] [((rookPremasks [lsbIndex] & occupancy) * rookMagics [lsbIndex]) >> rookShifts [lsbIndex]];
			if ((pieceAttacks & kingBoard) != 0) {
				captureMask |= magicRookAttacks [kingPos] [((rookPremasks [kingPos] & occupancy) * rookMagics [kingPos]) >> rookShifts [kingPos]] & bitboards [9 - 6 * turn];
				numberChecks++;
				rookCheckers |= pieceBitBoard & (~pieceBitBoard + 1);
			}
			kingDangers |= pieceAttacks;
			pieceBitBoard &= pieceBitBoard - 1;
		}

		// queens
		pieceBitBoard = bitboards [10-6*turn];
		while (pieceBitBoard != 0) {
			lsbIndex = leastSigLookup [((pieceBitBoard & (~pieceBitBoard + 1)) * debruijnleast) >> 58];
			pieceAttacks = magicBishopAttacks [lsbIndex] [((bishopPremasks [lsbIndex] & occupancy) * bishopMagics [lsbIndex]) >> bishopShifts [lsbIndex]];
			if ((pieceAttacks & kingBoard) != 0) {
				captureMask |= magicBishopAttacks [kingPos] [((bishopPremasks [kingPos] & occupancy) * bishopMagics [kingPos]) >> bishopShifts [kingPos]] & bitboards [10 - 6 * turn];

				numberChecks++;
				bishopCheckers |= pieceBitBoard & (~pieceBitBoard + 1);
			}
			kingDangers |= pieceAttacks;
			pieceAttacks = magicRookAttacks [lsbIndex] [((rookPremasks [lsbIndex] & occupancy) * rookMagics [lsbIndex]) >> rookShifts [lsbIndex]];
			if ((pieceAttacks & kingBoard) != 0) {
				captureMask |= magicRookAttacks [kingPos] [((rookPremasks [kingPos] & occupancy) * rookMagics [kingPos]) >> rookShifts [kingPos]] & bitboards [10 - 6 * turn];
				numberChecks++;
				rookCheckers |= pieceBitBoard & (~pieceBitBoard + 1);
			}
			kingDangers |= pieceAttacks;
			pieceBitBoard &= pieceBitBoard - 1;
		}
		return numberChecks;
	}

	static ulong sliderMask = 0;

	// find all moves that could be made
	private static List<uint> TrueLegalMoves(ulong[] bitboards){
		List<uint> validMoves = new List<uint>(); 
		List<uint> captureMoves = new List<uint>();
		uint turn = (uint)bitboards [12] & 1;
		ulong destinations = 0;
		ulong pinBoard = 0xffffffffffffffff;
		ulong destinationBit, pieceBoard;
		sliderMask = 0;
		uint capturedPiece, destinationIndex;
		uint lsb;
		ulong[] sidePieces = new ulong[] {bitboards [0] | bitboards [1] | bitboards [2] | bitboards [3] | bitboards [4] | bitboards [5], 
			bitboards [6] | bitboards [7] | bitboards [8] | bitboards [9] | bitboards [10] | bitboards [11]
		};
		ulong occupancy = sidePieces [0] | sidePieces [1];

		if (FindChecks (bitboards, occupancy) > 1) {
			// must move king only, under attack by 2 pieces
			captureMask = 0;
			sliderMask = 0;

		} else if (numberChecks == 1) {
			// must move king, block attack or capture
			if (rookCheckers != 0) {
				// rook slide check to block
				sliderMask = magicRookAttacks [kingPos] [((rookPremasks [kingPos] & occupancy) * rookMagics [kingPos]) >> rookShifts [kingPos]];
				lsb = leastSigLookup [(rookCheckers * debruijnleast) >> 58];
				sliderMask &= magicRookAttacks [lsb] [((rookPremasks [lsb] & occupancy) * rookMagics [lsb]) >> rookShifts [lsb]];
			} else if (bishopCheckers != 0) {
				// bishop style check to block
				sliderMask = magicBishopAttacks [kingPos] [((bishopPremasks [kingPos] & occupancy) * bishopMagics [kingPos]) >> bishopShifts [kingPos]];
				lsb = leastSigLookup [(bishopCheckers * debruijnleast) >> 58];
				sliderMask &= magicBishopAttacks [lsb] [((bishopPremasks [lsb] & occupancy) * bishopMagics [lsb]) >> bishopShifts [lsb]];

			} else {
				// not a sliding piece causing check
				sliderMask = 0;
			}

		} else {
			// can move normally
			captureMask = 0xffffffffffffffff;
			sliderMask = 0xffffffffffffffff;
			if (turn == 0) {
				// castle left
				if ((bitboards [3] & 0x1) != 0 && (occupancy & 0xe) == 0 && (bitboards [12] & 8) == 8 && (kingDangers & 12) == 0) {
					captureMoves.Add (4 << 17 | 2 << 11 | 5 << 7 | 7 << 4 | 8); 
				}
				// castle right
				if ((bitboards [3] & 0x80) != 0 && (occupancy & 0x60) == 0 && (bitboards [12] & 16) == 16 && (kingDangers & 96) == 0) {
					captureMoves.Add (4 << 17 | 6 << 11 | 5 << 7 | 7 << 4 | 4); 
				}
			} else {
				// castle left
				if ((bitboards [9] & 0x100000000000000) != 0 && (occupancy & 0xe00000000000000) == 0 && (bitboards [12] & 0x2) == 0x2 && (kingDangers & 0xc00000000000000) == 0) {
					captureMoves.Add (60 << 17 | 58 << 11 | 13 << 7 | 7 << 4 | 8); 
				}
				// castle right
				if ((bitboards [9] & 0x8000000000000000) != 0 && (occupancy & 0x6000000000000000) == 0 && (bitboards [12] & 0x4) == 0x4 && (kingDangers & 0x6000000000000000) == 0) {
					captureMoves.Add (60 << 17 | 62 << 11 | 13 << 7 | 7 << 4 | 4); 
				}
			}
		}

		if (numberChecks < 2) {
			// Pinned pieces
			// enemy bishops causing pins
			ulong enemyBishops = bitboards[8-6*turn] | bitboards[10-6*turn];
			ulong attacks;
			ulong overlap;
			ulong trapped;
			while (enemyBishops != 0) {
				lsb = leastSigLookup [((enemyBishops & (~enemyBishops + 1)) * debruijnleast) >> 58];
				attacks = magicBishopAttacks [lsb] [((bishopPremasks [lsb] & (sidePieces[1-turn]|bitboards[5+6*turn])) * bishopMagics [lsb]) >> bishopShifts [lsb]];
				if ((attacks & bitboards [5 + 6 * turn]) != 0) {
					// piece attacks king
					overlap = (attacks & magicBishopAttacks [kingPos] [((bishopPremasks [kingPos] & sidePieces[1-turn]) * bishopMagics [kingPos]) >> bishopShifts [kingPos]]);

					// only one piece between king and pinner
					if ((overlap & sidePieces[1-turn]) == 0 && (overlap & sidePieces[turn]) != 0) {
						trapped = overlap & sidePieces[turn];
						if ((trapped & (trapped - 1)) == 0) {
							// THIS IS A PIN, overlap is all valid positions to move to.
							lsb = leastSigLookup [(trapped * debruijnleast) >> 58];
							for (uint a = 6 * turn; a < 5 + 6 * turn; a++) {
								if ((bitboards [a] & overlap) != 0) {
									// piece removed from regular generation that occurs later
									pinBoard ^= trapped;
									if (numberChecks == 0) {
										// generate move for piece normally
										//pawn
										if (a - 6 * turn == 0) {
											if (turn == 0) {
												// regular capture
												destinations = (enemyBishops & (~enemyBishops + 1)) & captureMask & (((trapped << 7) & 0x7f7f7f7f7f7f7f7f) | ((trapped << 9) & 0xfefefefefefefefe));
												if (destinations != 0) {
													destinationIndex = leastSigLookup [(destinations * debruijnleast) >> 58];
													if ((destinations & bitboards [8]) != 0) {
														captureMoves.Add (lsb << 17 | destinationIndex << 11 | 32);
														if ((trapped & 0xff000000000000) != 0) {
															captureMoves.Add (lsb << 17 | destinationIndex << 11 | 32 | 1);
														}
													} else {
														captureMoves.Add (lsb << 17 | destinationIndex << 11 | 64);
														if ((trapped & 0xff000000000000) != 0) {
															captureMoves.Add (lsb << 17 | destinationIndex << 11 | 64 | 1);
														}
													}
												}
												// en passant

												ulong EPTile = (ulong)1 << (int)((bitboards [12] & 0x7e0) >> 5);
												if (((((trapped << 7) & 0x7f7f7f7f7f7f7f7f) | ((trapped << 9) & 0xfefefefefefefefe)) & EPTile & overlap & sliderMask) != 0 && (bitboards [12] & 0x800) != 0) {
													captureMoves.Add (lsb << 17 | (uint)(bitboards [12] & 0x7e0) << 6 | 96);
												}

											} else {
												destinations = (enemyBishops & (~enemyBishops + 1)) & captureMask & (((trapped >> 9) & 0x7f7f7f7f7f7f7f7f) | ((trapped >> 7) & 0xfefefefefefefefe));
												if (destinations != 0) {
													destinationIndex = leastSigLookup [(destinations * debruijnleast) >> 58];
													if ((destinations & bitboards [2]) != 0) {
														captureMoves.Add (lsb << 17 | destinationIndex << 11 | 32 | 1024);
														if ((trapped & 0xff00) != 0) {
															captureMoves.Add (lsb << 17 | destinationIndex << 11 | 32 | 1025);
														}
													} else {
														captureMoves.Add (lsb << 17 | destinationIndex << 11 | 64 | 1024);
														if ((trapped & 0xff00) != 0) {
															captureMoves.Add (lsb << 17 | destinationIndex << 11 | 64 | 1025);
														}
													}
												}
												// en passant
												ulong EPTile = (ulong)1 << (int)((bitboards [12] & 0x7e0) >> 5);
												if (((((trapped >> 9) & 0x7f7f7f7f7f7f7f7f) | ((trapped >> 7) & 0xfefefefefefefefe)) & EPTile & overlap & sliderMask) != 0 && (bitboards [12] & 0x800) != 0) {
													captureMoves.Add (lsb << 17 | (uint)(bitboards [12] & 0x7e0) << 6 | 1120);
												}
											}
										}
									// bishop
									else if (a - 6 * turn == 2) {
											destinations = overlap & (sliderMask | captureMask) & (~sidePieces [turn]);
											while (destinations != 0) {
												destinationIndex = leastSigLookup [((destinations & (~destinations + 1)) * debruijnleast) >> 58];
												validMoves.Add (lsb << 17 | destinationIndex << 11 | 2 << 7 | 112 | 1024 * turn);
												destinations &= destinations - 1;
											}
											destinations = (enemyBishops & (~enemyBishops + 1)) & captureMask;
											if (destinations != 0) {
												destinationIndex = leastSigLookup [(destinations * debruijnleast) >> 58];
												if ((destinations & bitboards [8 - 6 * turn]) != 0) {
													captureMoves.Add (lsb << 17 | destinationIndex << 11 | 2 << 7 | 32 | 1024 * turn);
												} else {
													captureMoves.Add (lsb << 17 | destinationIndex << 11 | 2 << 7 | 64 | 1024 * turn);
												}
											}
											// Queen
										} else if (a - 6 * turn == 4) {
											destinations = overlap & (sliderMask | captureMask) & (~sidePieces [turn]);
											while (destinations != 0) {
												destinationIndex = leastSigLookup [((destinations & (~destinations + 1)) * debruijnleast) >> 58];
												validMoves.Add (lsb << 17 | destinationIndex << 11 | 4 << 7 | 112 | 1024 * turn);
												destinations &= destinations - 1;
											}
											destinations = (enemyBishops & (~enemyBishops + 1)) & captureMask;
											if (destinations != 0) {
												destinationIndex = leastSigLookup [(destinations * debruijnleast) >> 58];
												if ((destinations & bitboards [8 - 6 * turn]) != 0) {
													captureMoves.Add (lsb << 17 | destinationIndex << 11 | 4 << 7 | 32 | 1024 * turn);
												} else {
													captureMoves.Add (lsb << 17 | destinationIndex << 11 | 4 << 7 | 64 | 1024 * turn);
												}
											}
										}
									}
									break;
								}
							}
						}
					}
				}
				enemyBishops &= enemyBishops - 1;
			}

			// rook style pins
			ulong enemyRooks = bitboards[9-6*turn] | bitboards[10-6*turn];
			while (enemyRooks != 0) {
				lsb = leastSigLookup [((enemyRooks & (~enemyRooks + 1)) * debruijnleast) >> 58];
				attacks = magicRookAttacks [lsb] [((rookPremasks [lsb] & (sidePieces[1-turn]|bitboards[5+6*turn])) * rookMagics [lsb]) >> rookShifts [lsb]];
				if ((attacks & bitboards [5 + 6 * turn]) != 0) {
					// piece attacks king
					overlap = (attacks & magicRookAttacks [kingPos] [((rookPremasks [kingPos] & sidePieces[1-turn]) * rookMagics [kingPos]) >> rookShifts [kingPos]]);
					// only one piece between king and pinner
					if ((overlap & sidePieces[1-turn]) == 0 && (overlap & sidePieces[turn]) != 0) {
						trapped = overlap & sidePieces[turn];
						if ((trapped & (trapped - 1)) == 0) {
							lsb = leastSigLookup [(trapped * debruijnleast) >> 58];
							for (uint a = 6 * turn; a < 6 + 6 * turn; a++) {
								if ((bitboards [a] & overlap) != 0) {
									// piece removed from regular generation that occurs later
									pinBoard ^= trapped;
									//pawn
									if (a - 6 * turn == 0) {
										if (turn == 0 && ((trapped<<8)&occupancy) == 0 && ((trapped<<8)&overlap&sliderMask) != 0) {
											if (((trapped<<16)&occupancy)==0 && (trapped & 0xff00) != 0 && ((trapped<<16)&sliderMask)!=0){
												validMoves.Add (lsb << 17 | (lsb + 16) << 11 | 7 << 4);
											}
											if ((trapped & 0xff000000000000) != 0) {
												captureMoves.Add (lsb << 17 | (lsb + 8) << 11 | 7 << 4 | 1);
												captureMoves.Add (lsb << 17 | (lsb + 8) << 11 | 7 << 4);
											}else{
												validMoves.Add (lsb << 17 | (lsb + 8) << 11 | 7 << 4);
											}
										}else if (((trapped>>8)&occupancy) == 0 && ((trapped>>8)&overlap&sliderMask) != 0) {
											if (((trapped>>16)&occupancy)==0 && (trapped & 0xff000000000000) != 0 && ((trapped>>16)&sliderMask)!=0){
												validMoves.Add (lsb << 17 | (lsb - 16) << 11 |1024| 7 << 4);
											}
											if ((trapped & 0xff00) != 0) {
												captureMoves.Add (lsb << 17 | (lsb - 8) << 11 |1024| 7 << 4 | 1);
												captureMoves.Add (lsb << 17 | (lsb - 8) << 11 |1024| 7 << 4);
											}else{
												validMoves.Add (lsb << 17 | (lsb - 8) << 11 |1024| 7 << 4);
											}
										} 
									}
									// rook
									else if (a-6*turn == 3){
										destinations = overlap & (sliderMask|captureMask) & (~sidePieces[turn]);
										while (destinations != 0) {
											destinationIndex = leastSigLookup [((destinations & (~destinations + 1)) * debruijnleast) >> 58];
											validMoves.Add (lsb << 17 | destinationIndex << 11 | 3 << 7 | 112 | 1024 * turn);
											destinations &= destinations - 1;
										}
										destinations = (enemyRooks & (~enemyRooks + 1))&captureMask;
										if (destinations != 0) {
											destinationIndex = leastSigLookup [(destinations * debruijnleast) >> 58];
											if ((destinations & bitboards [9 - 6 * turn]) != 0) {
												captureMoves.Add (lsb << 17 | destinationIndex << 11 | 3 << 7 | 48 | 1024 * turn);
											} else {
												captureMoves.Add (lsb << 17 | destinationIndex << 11 | 3 << 7 | 64 | 1024 * turn);
											}
										}
										// Queen
									}else if (a-6*turn == 4){
										destinations = overlap & (sliderMask|captureMask) & (~sidePieces[turn]);
										while (destinations != 0) {
											destinationIndex = leastSigLookup [((destinations & (~destinations + 1)) * debruijnleast) >> 58];
											validMoves.Add (lsb << 17 | destinationIndex << 11 | 4 << 7 | 112 | 1024 * turn);
											destinations &= destinations - 1;
										}
										destinations = (enemyRooks & (~enemyRooks + 1))&captureMask;
										if (destinations != 0) {
											destinationIndex = leastSigLookup [(destinations * debruijnleast) >> 58];
											if ((destinations & bitboards [9 - 6 * turn]) != 0) {
												captureMoves.Add (lsb << 17 | destinationIndex << 11 | 4 << 7 | 48 | 1024 * turn);
											} else {
												captureMoves.Add (lsb << 17 | destinationIndex << 11 | 4 << 7 | 64 | 1024 * turn);
											}
										}
									}
									break;
								}
							}
						}
					}
				}
				enemyRooks &= enemyRooks - 1;
			}
			// all pins now removed

			// knights
			pieceBoard= bitboards [1+6*turn] & pinBoard;
			while (pieceBoard != 0) {

				lsb = leastSigLookup [((pieceBoard & (~pieceBoard+1)) * debruijnleast) >> 58];
				// lsbIndex = index of knight on board
				destinations = knightMoves [lsb] & (~sidePieces[turn]) & (captureMask|sliderMask);
				while (destinations  != 0) {
					destinationBit = destinations & (~destinations+1);
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					if ((destinationBit & sidePieces[1-turn]) != 0) {
						capturedPiece = pieceCaptureVal(bitboards, turn, destinationBit);
						captureMoves.Add ( lsb << 17 | destinationIndex << 11 | 1 << 7 | capturedPiece | 1024*turn);
					} else {
						validMoves.Add (lsb << 17 | destinationIndex << 11 | 1 << 7 | 112 | 1024*turn);
					}
					destinations &= destinations - 1;
				}
				pieceBoard &= pieceBoard - 1;
			}

			// bishops
			pieceBoard= bitboards [2+6*turn]& pinBoard;
			while (pieceBoard != 0) {
				lsb = leastSigLookup [((pieceBoard & (~pieceBoard+1)) * debruijnleast) >> 58];
				// lsbIndex = index of piece on board
				destinations = magicBishopAttacks[lsb][((bishopPremasks [lsb] & occupancy)*bishopMagics[lsb])>>bishopShifts[lsb]] & (~sidePieces[turn]) & (captureMask|sliderMask);
				while (destinations  != 0) {
					destinationBit = destinations & (~destinations+1);
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					if ((destinationBit & sidePieces[1-turn]) != 0) {
						capturedPiece = pieceCaptureVal(bitboards, turn, destinationBit);
						captureMoves.Add ( lsb << 17 | destinationIndex << 11 | 2<< 7 | capturedPiece | 1024*turn);
					} else {
						validMoves.Add (lsb << 17 | destinationIndex << 11 | 2 << 7 | 112 | 1024*turn);
					}
					destinations &= destinations - 1;
				}
				pieceBoard &= pieceBoard - 1;
			}

			// rooks
			pieceBoard= bitboards [3+6*turn]& pinBoard;
			while (pieceBoard != 0) {

				lsb = leastSigLookup [((pieceBoard & (~pieceBoard+1)) * debruijnleast) >> 58];
				// lsbIndex = index of piece on board
				destinations = magicRookAttacks[lsb][((rookPremasks [lsb] & occupancy)*rookMagics[lsb])>>rookShifts[lsb]] & (~sidePieces[turn]) & (captureMask|sliderMask);
				while (destinations  != 0) {
					destinationBit = destinations & (~destinations+1);
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					if ((destinationBit & sidePieces[1-turn]) != 0) {
						capturedPiece = pieceCaptureVal(bitboards, turn, destinationBit);
						captureMoves.Add ( lsb << 17 | destinationIndex << 11 | 3 << 7 | capturedPiece | 1024*turn);
					} else {
						validMoves.Add (lsb << 17 | destinationIndex << 11 | 3 << 7 | 112 | 1024*turn);
					}
					destinations &= destinations - 1;
				}
				pieceBoard &= pieceBoard - 1;
			}

			// queens
			pieceBoard= bitboards [4+6*turn]& pinBoard;
			while (pieceBoard != 0) {

				lsb = leastSigLookup [((pieceBoard & (~pieceBoard+1)) * debruijnleast) >> 58];
				// lsbIndex = index of piece on board
				destinations = magicBishopAttacks[lsb][((bishopPremasks [lsb] & occupancy)*bishopMagics[lsb])>>bishopShifts[lsb]];
				destinations |= magicRookAttacks [lsb] [((rookPremasks [lsb] & occupancy) * rookMagics [lsb]) >> rookShifts [lsb]];
				destinations &= (~sidePieces[turn]) & (captureMask|sliderMask);
				while (destinations  != 0) {
					destinationBit = destinations & (~destinations+1);
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					if ((destinationBit & sidePieces[1-turn]) != 0) {
						capturedPiece = pieceCaptureVal(bitboards, turn, destinationBit);
						captureMoves.Add ( lsb << 17 | destinationIndex << 11 | 4 << 7 | capturedPiece | 1024*turn);
					} else {
						validMoves.Add (lsb << 17 | destinationIndex << 11 | 4 << 7 | 112 | 1024*turn);
					}
					destinations &= destinations - 1;
				}
				pieceBoard &= pieceBoard - 1;
			}

			// pawns
			// TODO: REMOVE CASE
			if (turn == 0) {
				ulong pawnBoard = (bitboards [0] & pinBoard);
				pieceBoard = (pawnBoard << 8) & ~occupancy;
				ulong furtherBoard = (pieceBoard << 8) & ~occupancy & 0xff000000 & sliderMask;
				pieceBoard &= sliderMask;
				while (pieceBoard != 0) {
					destinationIndex = leastSigLookup [((pieceBoard & (~pieceBoard + 1)) * debruijnleast) >> 58];

					if (destinationIndex > 55) {
						captureMoves.Add ((destinationIndex - 8) << 17 | (destinationIndex) << 11 | 7 << 4 | 1);
						captureMoves.Add ((destinationIndex - 8) << 17 | (destinationIndex) << 11 | 7 << 4);
					} else {
						validMoves.Add ((destinationIndex - 8) << 17 | (destinationIndex) << 11 | 7 << 4);
					}
					pieceBoard &= pieceBoard - 1;
				}

				while (furtherBoard != 0) {
					destinationIndex = leastSigLookup [((furtherBoard & (~furtherBoard + 1)) * debruijnleast) >> 58];
					validMoves.Add ((destinationIndex - 16) << 17 | (destinationIndex) << 11 | 7 << 4);
					furtherBoard &= furtherBoard - 1;
				}

				// capture
				pieceBoard = (pawnBoard << 7) & 0x7f7f7f7f7f7f7f7f & sidePieces[1] & captureMask;
				while (pieceBoard != 0) {
					destinationBit = (pieceBoard & (~pieceBoard + 1));
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					capturedPiece = pieceCaptureVal(bitboards, turn, destinationBit);
					if (destinationIndex > 55) {
						captureMoves.Add ((destinationIndex - 7) << 17 | (destinationIndex) << 11 | capturedPiece | 1);
					}
					captureMoves.Add ((destinationIndex - 7) << 17 | (destinationIndex) << 11 | capturedPiece);
					pieceBoard &= pieceBoard - 1;
				}

				// capture
				pieceBoard = (pawnBoard << 9) & 0xfefefefefefefefe & sidePieces[1] & captureMask;
				while (pieceBoard != 0) {
					destinationBit = (pieceBoard & (~pieceBoard + 1));
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					capturedPiece = pieceCaptureVal(bitboards, turn, destinationBit);
					if (destinationIndex > 55) {
						captureMoves.Add ((destinationIndex - 9) << 17 | (destinationIndex) << 11 |capturedPiece | 1);
					}
					captureMoves.Add ((destinationIndex - 9) << 17 | (destinationIndex) << 11 |capturedPiece);
					pieceBoard &= pieceBoard - 1;
				}

				// en passant
				ulong epBoard = ((ulong)1 << (int)((bitboards [12] & 0x7e0) >> 5));
					if ((bitboards [12] & 0x800) != 0 && ((epBoard >> 9) & 0x7f7f7f7f7f7f7f7f & pawnBoard) != 0 && (((epBoard >> 8) & captureMask) != 0 || (epBoard & sliderMask)!=0)) {
					occupancy ^= epBoard | (epBoard >> 8) | (epBoard >> 9);
					if ((magicBishopAttacks [kingPos] [((bishopPremasks [kingPos] & occupancy) * bishopMagics [kingPos]) >> bishopShifts [kingPos]] & (bitboards [8] | bitboards [10])) == 0
					    && (magicRookAttacks [kingPos] [((rookPremasks [kingPos] & occupancy) * rookMagics [kingPos]) >> rookShifts [kingPos]] & (bitboards [9] | bitboards [10])) == 0) {
						captureMoves.Add (((uint)((bitboards [12] & 0x7e0) >> 5) - 9) << 17 | (uint)(bitboards [12] & 0x7e0) << 6 | 96);
					}
					occupancy ^= epBoard | (epBoard >> 8) | (epBoard >> 9);
				}
				if ((bitboards [12] & 0x800) != 0 && ((epBoard >> 7) & 0xfefefefefefefefe & pawnBoard) != 0 && (((epBoard >> 8) & captureMask) != 0 || (epBoard & sliderMask)!=0)) {
					occupancy ^= epBoard | (epBoard >> 8) | (epBoard >> 7);
					if ((magicBishopAttacks [kingPos] [((bishopPremasks [kingPos] & occupancy) * bishopMagics [kingPos]) >> bishopShifts [kingPos]] & (bitboards [8] | bitboards [10])) == 0
					    && (magicRookAttacks [kingPos] [((rookPremasks [kingPos] & occupancy) * rookMagics [kingPos]) >> rookShifts [kingPos]] & (bitboards [9] | bitboards [10])) == 0) {
						captureMoves.Add (((uint)((bitboards [12] & 0x7e0) >> 5) - 7) << 17 | (uint)(bitboards [12] & 0x7e0) << 6 | 96);
					}
					occupancy ^= epBoard | (epBoard >> 8) | (epBoard >> 7);
				}
			} else {
				ulong pawnBoard = (bitboards [6] & pinBoard);
				pieceBoard = (pawnBoard >> 8)&~occupancy;
				ulong furtherBoard = (pieceBoard >> 8) & ~occupancy & 0xff00000000 & sliderMask;
				pieceBoard &= sliderMask;

				while (pieceBoard != 0) {
					destinationIndex = leastSigLookup [((pieceBoard & (~pieceBoard+1)) * debruijnleast) >> 58];
					if (destinationIndex < 8) {
						captureMoves.Add ((destinationIndex + 8) << 17 | (destinationIndex) << 11 | 7 << 4 | 1025);
						captureMoves.Add ((destinationIndex + 8) << 17 | (destinationIndex) << 11 | 7 << 4|1024);
					} else {
						validMoves.Add ((destinationIndex + 8) << 17 | (destinationIndex) << 11 | 7 << 4|1024);
					}
					pieceBoard &= pieceBoard - 1;
				}

				while (furtherBoard != 0) {
					destinationIndex = leastSigLookup [((furtherBoard & (~furtherBoard+1)) * debruijnleast) >> 58];
					validMoves.Add ((destinationIndex + 16) << 17 | (destinationIndex) << 11 | 7 << 4|1024);
					furtherBoard &= furtherBoard - 1;
				}

				// capture
				pieceBoard = (pawnBoard >>9)& 0x7f7f7f7f7f7f7f7f & sidePieces[0]&captureMask;
				while (pieceBoard != 0) {
					destinationBit = (pieceBoard & (~pieceBoard + 1));
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					capturedPiece = pieceCaptureVal(bitboards, turn, destinationBit);
					if (destinationIndex <0) {
						captureMoves.Add ((destinationIndex +9) << 17 | (destinationIndex) << 11 | capturedPiece | 1025);
					}
					captureMoves.Add ((destinationIndex +9) << 17 | (destinationIndex) << 11 | capturedPiece|1024);
					pieceBoard &= pieceBoard - 1;
				}

				// capture
				pieceBoard = (pawnBoard >>7)& 0xfefefefefefefefe & sidePieces[0]&captureMask;
				while (pieceBoard != 0) {
					destinationBit = (pieceBoard & (~pieceBoard + 1));
					destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
					capturedPiece = pieceCaptureVal(bitboards, turn, destinationBit);
					if (destinationIndex < 8) {
						captureMoves.Add ((destinationIndex + 7) << 17 | (destinationIndex) << 11 | capturedPiece | 1025);
					}
					captureMoves.Add ((destinationIndex +7) << 17 | (destinationIndex) << 11 |capturedPiece|1024);
					pieceBoard &= pieceBoard - 1;
				}

				ulong epBoard = ((ulong)1<<(int)((bitboards [12] & 0x7e0)>>5));
				if ((bitboards [12] & 0x800) != 0 && ((epBoard <<7) & 0x7f7f7f7f7f7f7f7f & pawnBoard) != 0 && (((epBoard << 8) & captureMask) != 0 || (epBoard & sliderMask)!=0)) {
					occupancy ^= epBoard | (epBoard << 8) | (epBoard <<7);
					if ((magicBishopAttacks[kingPos][((bishopPremasks [kingPos] & occupancy)*bishopMagics[kingPos])>>bishopShifts[kingPos]] & (bitboards[8]|bitboards[10])) == 0
						&& (magicRookAttacks[kingPos][((rookPremasks [kingPos] & occupancy)*rookMagics[kingPos])>>rookShifts[kingPos]] & (bitboards[9]|bitboards[10])) == 0){
						captureMoves.Add (((uint)((bitboards [12] & 0x7e0)>>5)+7) << 17 | (uint)(bitboards [12] & 0x7e0) << 6 | 96|1024);
					}
					occupancy ^= epBoard | (epBoard << 8) | (epBoard <<7);
				}
				if ((bitboards [12] & 0x800) != 0 && ((epBoard << 9) & 0xfefefefefefefefe & pawnBoard) != 0 && (((epBoard << 8) & captureMask) != 0 || (epBoard & sliderMask)!=0)) {
					occupancy ^= epBoard | (epBoard << 8) | (epBoard << 9);
					if ((magicBishopAttacks [kingPos] [((bishopPremasks [kingPos] & occupancy) * bishopMagics [kingPos]) >> bishopShifts [kingPos]] & (bitboards [8] | bitboards [10])) == 0
					    && (magicRookAttacks [kingPos] [((rookPremasks [kingPos] & occupancy) * rookMagics [kingPos]) >> rookShifts [kingPos]] & (bitboards [9] | bitboards [10])) == 0) {
						captureMoves.Add (((uint)((bitboards [12] & 0x7e0) >> 5) + 9) << 17 | (uint)(bitboards [12] & 0x7e0) << 6 | 96|1024);
					}
					occupancy ^= epBoard | (epBoard << 8) | (epBoard << 9);
				}
			}
		}

		// Always valid king moves
		destinations = kingMoves[leastSigLookup [(bitboards[5+6*turn] * debruijnleast) >> 58]] & ~kingDangers & ~sidePieces[turn];
		destinationIndex = 0;
		lsb = leastSigLookup [(bitboards[5+6*turn] * debruijnleast) >> 58];
		while (destinations  != 0) {
			destinationBit = destinations & (~destinations+1);
			destinationIndex = leastSigLookup [(destinationBit * debruijnleast) >> 58];
			if ((destinationBit & sidePieces[1-turn]) != 0) {
				capturedPiece = pieceCaptureVal(bitboards, turn, destinationBit);
				captureMoves.Add ( lsb << 17 | destinationIndex << 11 | 5 << 7 | capturedPiece | 1024*turn);
			} else {
				validMoves.Add (lsb << 17 | destinationIndex << 11 | 5 << 7 | 112 | 1024*turn);
			}
			destinations &= destinations - 1;
		}
		// want to consider captures before other moves
		captureMoves.AddRange (validMoves);

		return captureMoves;
	}
	
	private static uint pieceCaptureVal(ulong[] bitboards, uint turn, ulong destinationBit){
		if ((destinationBit & bitboards [6 - 6 * turn]) != 0) {
			return 0;
		} else if ((destinationBit & bitboards [7 - 6 * turn]) != 0) {
			return 16;
		} else if ((destinationBit & bitboards [8 - 6 * turn]) != 0) {
			return 32;
		} else if ((destinationBit & bitboards [9 - 6 * turn]) != 0) {
			return 48;
		} else {
			return 64;
		}
	}

	private bool staleMate = false;

	// displaying move made in standard format
	private void UpdateMoveLog(uint move, ulong gameState){
		moveSound.Play ();
		// move made on bitboards, then this is called
		// 50 move rule
		if (bitboardArray [12] >= 0x64000) {
			staleMate = true;
		}
		// hash data for repeat positions
		if (visitedHashes.Contains (bitboardArray [13])) {
			if (cantRepeat.Contains (bitboardArray [13])) {
				staleMate = true;
			} else {
				cantRepeat.Add (bitboardArray [13]);
			}
		} else {
			visitedHashes.Add (bitboardArray [13]);
		}

		ulong[] boardClone;
		string moveText;
		if (((move>>2)&1) == 1){
			moveText = "O-O";
		}else if (((move>>3)&1) == 1){
			moveText = "O-O-O";

		} else {
			boardClone = (ulong[])bitboardArray.Clone ();
			boardClone [12] = gameState;
			UnMakeMove (boardClone, move);
			List<uint> allMoves = TrueLegalMoves (boardClone);

			uint fromTile = move >> 17;
			uint toTile = (move >> 11) & 0x3f;
			uint pieceIndex = (move >> 7) & 0x7;

			moveText = new string[] { "", "N", "B", "R", "Q", "K" } [pieceIndex];
			if (pieceIndex != 0) {
				foreach (uint otherMove in allMoves) {
					// same piece move to
					if ((otherMove & 0x1ffff) == (move & 0x1ffff) && move != otherMove) {
						// same row
						if (((otherMove >> 17) &7) != (fromTile &7)) {
							moveText += across[fromTile &7];
							break;
						} else {
							moveText += up[fromTile / 8];
							break;
						}
					}
				}
			}

			if (((move >> 4) & 0x7) != 7) {
				if (pieceIndex == 0) {
					moveText = across[fromTile % 8];
				}
				moveText += "x";
			}
			moveText += across[toTile % 8];
			moveText += up[toTile / 8];
			if (pieceIndex == 0 && (toTile > 55 || toTile < 8)) {
				if (move % 2 == 0) {
					moveText += "=Q";
				} else {
					moveText += "=N";
				}
			}
		}

		boardClone = (ulong[])bitboardArray.Clone ();
		boardClone [12] ^= 1;
		// detect check/mate
		if (CanTakeKing (boardClone)) {
			if (InCheckmate (bitboardArray)) {
				moveText = moveText + "#";
			} else {
				moveText = moveText + "+";
			}
			if (players [bitboardArray [12] & 1] == 0) {
				Debug.Log ("vib");
				Handheld.Vibrate ();
			}
		}
		if (numberOfMoves % 2 == 0) {
			moveText = ((numberOfMoves + 2) / 2).ToString() + ": " + moveText;
		}
		moveLog.Add (moveText);
		if (moveLog.Count >= 9) {
			moveLog.RemoveAt (0);
			moveLog.RemoveAt (0);
		}
		Debug.Log (moveText);
		moveLogText.text = "";

		if (moveLog.Count % 2 == 1) {
			moveLogText.text = moveLog [moveLog.Count - 1] + "\n";
		}
		for (int i = moveLog.Count / 2; i > 0; i--) {
			moveLogText.text += moveLog [2 * i - 2] + "  " + moveLog [2 * i - 1] + "\n";
		}
		// decide if in endgame yet
		int whiteSum = 0;
		int blackSum = 0;
		for (int a = 0; a < 5; a++) {
			ulong pieceBoard = bitboardArray [a];
			while (pieceBoard != 0) {
				whiteSum += basicValues [a];
				pieceBoard &= pieceBoard - 1;
			}
		}
		for (int a = 6; a < 11; a++) {
			ulong pieceBoard = bitboardArray [a];
			while (pieceBoard != 0) {
				blackSum += basicValues [a];
				pieceBoard &= pieceBoard - 1;
			}
		}
		if (!theEndGame && blackSum < 14 && whiteSum < 14) {
			EnterEndGame ();
			theEndGame = true;
		}
		if (blackSum == 0 && whiteSum == 0) {
			staleMate = true;
		}
	}

	// switch view 2D-3D
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
		// statics preserved, reset so reloaded on start
		openingLines = null;
		SceneManager.LoadScene("NewMenu");
	}

	void Update(){
		if (threadComplete) {
			threadComplete = false;
			CompleteComp ();
		}
		if (goToComputer) {
			goToComputer = false;
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

	// identify and respond to selection
	private void MouseUp(){
		int gameTurn = (int)(bitboardArray [12] % 2);
		List<uint> allMoves = TrueLegalMoves (bitboardArray);
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
							drawTurn(startPieceIndex, startTile, endTile, gameTurn, true);
							ulong gameState = bitboardArray [12];
							MakeMove (bitboardArray, move);
							startTile = new int[] { -1, -1 };
							UpdateMoveLog (move, gameState);
							endTurn(move, gameTurn);
							break;
						}
					}
				}
			}
		}
	}
	bool inOpening = true;
	
	private void drawTurn(int startPieceIndex, int[] startTile, int[] endTile, int gameTurn, bool queenKnight){
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
			if (queenKnight){
				GeneratePiece (endTile [0], endTile [1], 4 + 6 * gameTurn);
			} else {
				GeneratePiece (endTile [0], endTile [1], 1 + 6 * gameTurn);
			}
		}
	}
	
	private void endTurn(uint move, int gameTurn){
		numberOfMoves += 1;
		UpdateOpenings (move);
		// end of game
		if (InCheckmate (bitboardArray) || staleMate) {
			gameDone = true;
			gameOverButton.SetActive (true);
			Invoke ("Ending", 4.0f);
		// computer to move
		} else if (players [1-gameTurn] == 1) { 
			computerMove = true; 
		}
	}

	// filter openings which can still be followed
	private void UpdateOpenings (uint move){
		List <uint[]> newOpenings = new List<uint[]> ();
		for (int i = 0; i < openingLines.Count; i++) {
			if (openingLines[i].Length > numberOfMoves && openingLines[i][numberOfMoves-1] == move>>11){
				newOpenings.Add (openingLines [i]);
			}
		}
		if (newOpenings.Count == 0) {
			inOpening = false;
		} else {
			openingLines = newOpenings;
		}
	}
		
	private void Ending(){
		gameOverButton.SetActive (false);
	}

	Thread worker;

	private int maxTime;

	private void CompTurn(){
		stopThread = false;
		CancelInvoke("EndThread");
		new Thread(() => CompStart()) { IsBackground = true }.Start();
		Invoke ("EndThread", maxTime);
	}


	private void EndThread(){
		if (!threadComplete) {
			stopThread = true;
		}
	}

	private bool stopThread;
	private uint[] PV;
	private uint[] oldPV;
	private int searchDepth;

	private void CompStart(){
		searchDepth = 2;
		bestMoves = new List<uint> ();
		uint move;
		int turn = (int)bitboardArray[12]&1;
		// select from opening lines unless both players are CPU
		if (inOpening && players[1-turn] != 1){
			int bestDepth = 0;
			for (int i = 0; i < openingLines.Count; i++) {
				if (openingLines [i].Length-numberOfMoves > bestDepth) {
					bestDepth = openingLines [i].Length;
				}
			}
			while (true) {
				int index = rnd.Next (openingLines.Count);
				if (openingLines [index].Length > (bestDepth-numberOfMoves) / 2) {
					move = openingLines [index][numberOfMoves];
					break;
				}
			}
		
			List <uint> allMoves = TrueLegalMoves (bitboardArray);
			foreach(uint choice in allMoves){
				if ((choice >> 11) == move) {
					move = choice;
					break;
				}
			}
			bestMoves = new List<uint> { move };
			Debug.Log ("opening");
		} else {
				PVS (bitboardArray, searchDepth, -10000000, 10000000, turn, FullEvaluate (bitboardArray) * (1 - 2 * turn), false);
				int lookup = (int)(bitboardArray [13] & 0x3ffff);
				// iterative deepening until time out or search complete
				while (searchDepth < maxDepth && TTMoves [lookup] != 0 && !stopThread) {
					bestMoves = new List<uint> { TTMoves [lookup] };
					searchDepth += 2;
					PVS (bitboardArray, searchDepth, -10000000, 10000000, turn, FullEvaluate (bitboardArray) * (1 - 2 * turn), false);
				}
				if (TTMoves [lookup] != 0 && !stopThread) {
					bestMoves = new List<uint> { TTMoves [lookup] };
				}

		}
		threadComplete = true; 
		return;
	}

	private void CompleteComp(){
		// can't move
		if (bestMoves.Count == 0) {
			Debug.Log ("NOMOVE");
			gameDone = true;
			gameOverButton.SetActive (true);
			Invoke ("Ending", 4.0f);
			return;
		}else{
			int index = rnd.Next (bestMoves.Count);
			uint move = bestMoves[index];
			ulong gameState = bitboardArray [12];
			MakeMove (bitboardArray, move);
			UpdateMoveLog (move, gameState);
			

			int startPieceIndex = (int)(((move >> 7) & 7)+6*((move>>10)&1));
			int endPieceIndex = (int)((move >> 4) & 7);
			if (endPieceIndex > 5){
				endPieceIndex = -1;
			}
			int[] startTile = new int[] { (int)((move >> 17) % 8), (int)((move >> 17) / 8 )};
			int[] endTile = new int[] { (int)(((move >> 11)&0x3f) % 8), (int)(((move >> 11)&0x3f) / 8 )};
			int gameTurn = 1-(int)(bitboardArray [12] % 2);

			drawTurn(startPieceIndex, startTile, endTile, gameTurn, move % 2 == 0);
			endTurn(move, gameTurn);
		}
	}

	// Transposition search, finds and tracks best moves
	private int PVS(ulong[] mainboard, int depthLeft, int alpha, int beta, int turn, int baseValue, bool extend){
		bool canMove = false;
		bool complete = true;
		int value;
		uint idealMove = 0;
		uint bestMove = 0;
		// stop searching
		if (depthLeft < 1 || stopThread) {
			if (!extend || depthLeft < extensionDepth || stopThread) {
				return baseValue;
			}
		}
		int lookup = (int)(mainboard [13] & 0x3ffff);
		if (TTHashes [lookup] == (mainboard [13] >> 32)) {
			if (TTDepths [lookup] >= depthLeft) {
				return TTValues [lookup] * (1 - 2 * turn);
			} else {
				idealMove = TTMoves [lookup];
			}
		}
		// all moves to search
		List<uint> childNodes = TrueLegalMoves(mainboard);
		ulong gameState = mainboard [12];
		int bestValue = -1000000;
		if (depthLeft == maxDepth) {
			bestValue = -1000005;
		}
		// best move from previous depth searches
		if (idealMove != 0 && childNodes.Contains(idealMove)) {
				childNodes.Remove (idealMove);
				MakeMove (mainboard, idealMove);
				canMove = true;
				if (cantRepeat.Contains (mainboard [13]) || mainboard [12] >= 0x64000) {
					value = 0;
				} else {
					value = -PVS (mainboard, depthLeft - 1, -beta, -alpha, 1 - turn, -baseValue - AdjustScore (idealMove), (idealMove&0x70)==0x70);
				}
				mainboard [12] = gameState;
				UnMakeMove (mainboard, idealMove);

				if (value > bestValue) {
					bestValue = value;
				bestMove = idealMove;
					// if broken, set PV here
				}
				if (value > alpha && !stopThread) {
					// set PV on alpha change
					alpha = value;
				} 
			if (alpha >= beta) {
				complete = false;
				childNodes = new List<uint> ();
			}
		}
		// consider all moves, alpha beta pruning where possible
		foreach (uint childMove in childNodes) {
			MakeMove (mainboard, childMove);
			canMove = true;
			if (cantRepeat.Contains (mainboard [13]) || mainboard [12] >= 0x64000) {
				value = 0;
			} else {
				value = -PVS (mainboard, depthLeft - 1, -beta, -alpha, 1 - turn, -baseValue - AdjustScore (childMove), (childMove&0x70)==0x70);
			}
			mainboard [12] = gameState;
			UnMakeMove (mainboard, childMove);

			if (value > bestValue) {
				bestValue = value;
				bestMove = childMove;
			}
			if (value > alpha && !stopThread) {
				alpha = value;
			}
			if (alpha >= beta) {
				complete = false;
				break;
			}
		}
		// check/stalemate
		if (!canMove) {
			mainboard [12] ^= 1;
			if (CanTakeKing (mainboard)) {
				mainboard [12] ^= 1;
				// add to TT
				if (searchDepth - depthLeft < 5) {
					TTHashes [lookup] = (uint)(mainboard [13] >> 32);
					TTDepths [lookup] = depthLeft;
					TTValues [lookup] = (-100000-depthLeft)*(1-2*turn);
					TTMoves [lookup] = 0;
				}
				return -100000-depthLeft;
			} else {
				mainboard [12] ^= 1;
				// add to TT
				if (searchDepth - depthLeft < 5) {
					TTHashes [lookup] =(uint)(mainboard [13] >> 32);
					TTDepths [lookup] = depthLeft;
					TTValues [lookup] = 0;
					TTMoves [lookup] = 0;
				}
				return 0;
			}
		}
		// add to Transpostion table
		if (searchDepth - depthLeft < 5) {
			TTHashes [lookup] = (uint)(mainboard [13] >> 32);
			if (!complete){
				depthLeft = 0;
			}
			TTDepths [lookup] = depthLeft;
			TTValues [lookup] = bestValue*(1-2*turn);
			TTMoves [lookup] = bestMove;
		}
		return bestValue;
	}

	private static bool CanTakeKing(ulong[] bitboards){
		ulong whitePieces = bitboards [0] | bitboards [1] | bitboards [2] | bitboards [3] | bitboards [4] | bitboards [5];
		ulong blackPieces = bitboards [6] | bitboards [7] | bitboards [8] | bitboards [9] | bitboards [10] | bitboards [11];
		ulong occupied = whitePieces | blackPieces;
		ulong destinations;
		ulong gameState = bitboards [12];

		// white turn
		if ((gameState & 1) == 0) {

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
				destinationBit = (lsb&0x7f7f7f7f7f7f7f7f) << 9;
				if ((destinationBit & bitboards [11]) != 0) {
					return true;
				}
				pawnBitBoard &= pawnBitBoard - 1;
			}


			// bishops
			ulong bishopBitBoard = bitboards [2];
			while (bishopBitBoard != 0) {
//				destinations = 0;
				lsbIndex = leastSigLookup [((bishopBitBoard & (~bishopBitBoard+1)) * debruijnleast) >> 58];

				// ITS MAGIC!
				destinations = magicBishopAttacks[lsbIndex][((bishopPremasks [lsbIndex] & occupied)*bishopMagics[lsbIndex])>>bishopShifts[lsbIndex]];



				if ((destinations & bitboards [11]) != 0) {
					return true;
				}

				bishopBitBoard &= bishopBitBoard - 1;
			}

			// rooks
			ulong rookBitBoard = bitboards [3];
			while (rookBitBoard != 0) {
//				destinations = 0;
				lsbIndex = leastSigLookup [((rookBitBoard & (~rookBitBoard+1)) * debruijnleast) >> 58];

				// magic move lookup
				destinations = magicRookAttacks[lsbIndex][((rookPremasks [lsbIndex] & occupied)*rookMagics[lsbIndex])>>rookShifts[lsbIndex]];



				if ((destinations & bitboards [11]) != 0) {
					return true;
				}
				rookBitBoard &= rookBitBoard - 1;
			}
			// queen
			ulong queenBitBoard = bitboards [4];

			while (queenBitBoard != 0) {
				lsbIndex = leastSigLookup [((queenBitBoard & (~queenBitBoard+1)) * debruijnleast) >> 58];
	
				// magic numbers for attack lookup
				destinations = magicBishopAttacks[lsbIndex][((bishopPremasks [lsbIndex] & occupied)*bishopMagics[lsbIndex])>>bishopShifts[lsbIndex]];
				destinations |= magicRookAttacks[lsbIndex][((rookPremasks [lsbIndex] & occupied)*rookMagics[lsbIndex])>>rookShifts[lsbIndex]];


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
//				destinations = 0;
				lsbIndex = leastSigLookup [((bishopBitBoard & (~bishopBitBoard+1)) * debruijnleast) >> 58];

				// magic numbers for attack lookup
				destinations = magicBishopAttacks[lsbIndex][((bishopPremasks [lsbIndex] & occupied)*bishopMagics[lsbIndex])>>bishopShifts[lsbIndex]];


				if ((destinations & bitboards [5]) != 0) {
					return true;
				}

				bishopBitBoard &= bishopBitBoard - 1;
			}
			// rooks
			ulong rookBitBoard = bitboards [9];
			while (rookBitBoard != 0) {
				lsbIndex = leastSigLookup [((rookBitBoard & (~rookBitBoard+1)) * debruijnleast) >> 58];

				// magic numbers for attack lookup
				destinations = magicRookAttacks[lsbIndex][((rookPremasks [lsbIndex] & occupied)*rookMagics[lsbIndex])>>rookShifts[lsbIndex]];


				if ((destinations & bitboards [5]) != 0) {
					return true;
				}

				rookBitBoard &= rookBitBoard - 1;
			}
			// queen
			ulong queenBitBoard = bitboards [10];

			while (queenBitBoard != 0) {
				lsbIndex = leastSigLookup [((queenBitBoard & (~queenBitBoard+1)) * debruijnleast) >> 58];

				// magic numbers for attack lookup
				destinations = magicBishopAttacks[lsbIndex][((bishopPremasks [lsbIndex] & occupied)*bishopMagics[lsbIndex])>>bishopShifts[lsbIndex]];
				destinations |= magicRookAttacks[lsbIndex][((rookPremasks [lsbIndex] & occupied)*rookMagics[lsbIndex])>>rookShifts[lsbIndex]];


				if ((destinations & bitboards [5]) != 0) {
					return true;
				}
				queenBitBoard &= queenBitBoard - 1;
			}
		}
		return false;
	}
	
	// check and can't move without losing king
	private bool InCheckmate(ulong[] bitboards){
		List<uint> possibleMoves = TrueLegalMoves(bitboards);
		ulong gameState = bitboards [12];
		foreach (uint move in possibleMoves) {
			MakeMove (bitboards, move);
			if (!CanTakeKing (bitboards)) {
				bitboards [12] = gameState;
				UnMakeMove (bitboards, move);
				return false;
			}
			bitboards [12] = gameState;
			UnMakeMove (bitboards, move);
		}
		return true;
	}

	private static void UnMakeMove(ulong[] bitboards, uint move){
		
		uint turn = ((move & 0x400) >> 10);
		uint movingPieceIndex = ((move & 0x380) >> 7) + 6 * turn;

		// castling
		if ((move & 0xc) == 4) {
			// king side
			bitboards [3 + 6 * turn] ^= (ulong)0xa0 << 56 * (int)turn;
			// hash
			bitboards[13] ^= hashValues[3 + 6 * turn][5+56*(int)turn];
			bitboards[13] ^= hashValues[3 + 6 * turn][7+56*(int)turn];
		} else if ((move & 0xc) == 8) {
			// queen side
			bitboards [3 + 6 * turn] ^= (ulong)0x9 << 56 * (int)turn;
			// hash
			bitboards[13] ^= hashValues[3 + 6 * turn][56*(int)turn];
			bitboards[13] ^= hashValues[3 + 6 * turn][3+56*(int)turn];
		}
		// possible captured piece
		uint capturedType = ((move & 0x70) >> 4);
		if (capturedType < 6) {
			bitboards [capturedType + 6 - 6 * turn] ^= ((ulong)1 << (int)((move & 0x1f800) >> 11));
			// hash 
			bitboards[13] ^= hashValues[capturedType + 6 - 6 * turn][(int)((move & 0x1f800) >> 11)];
		} else if (capturedType == 6) {
			// en passant, correct pawn bitboard based on en passant tile
			bitboards[6-6*turn] ^= (ulong)1<<(((int)(bitboards[12] & 0x7e0) >> 5)-8+16*(int)turn);
			// hash 

			bitboards [13] ^= hashValues [6 - 6 * turn] [(((int)(bitboards[12] & 0x7e0) >> 5)-8+16*(int)turn)];
		}
		//swap end line pawns for promoted pieces
		if (movingPieceIndex - 6*turn == 0) {
			if ((move & 0x3) == 0) {
				// promote to queen
				bitboards [4 + 6 * turn] ^= (((ulong)1 << (int)((move & 0x1f800)>>11))& 0xff000000000000ff);
			} else {
				bitboards [1 + 6 * turn] ^= (((ulong)1 << (int)((move & 0x1f800)>>11))& 0xff000000000000ff);
			}
			bitboards [6 * turn] ^= (((ulong)1 << (int)((move & 0x1f800)>>11))& 0xff000000000000ff);
		}

		bitboards[movingPieceIndex] ^= ((ulong)1 << (int)((move & 0x7e0000)>>17)) + ((ulong)1 << (int)((move & 0x1f800)>>11));
		// hash
		bitboards[13] ^= hashValues[movingPieceIndex][(int)((move & 0x7e0000)>>17)];
		bitboards[13] ^= hashValues[movingPieceIndex][(int)((move & 0x1f800)>>11)];
	}


	private static void MakeMove(ulong[] bitboards, uint move){

		bitboards [12] += 0x1000;

		uint turn = ((move & 0x400) >> 10);
		// move piece on its bitboard
		uint movingPieceIndex = ((move & 0x380) >> 7) + 6 * turn;
		bitboards[movingPieceIndex] ^= ((ulong)1 << (int)((move & 0x7e0000)>>17)) + ((ulong)1 << (int)((move & 0x1f800)>>11));

		// hash
		bitboards[13] ^= hashValues[movingPieceIndex][(int)((move & 0x7e0000)>>17)];
		bitboards[13] ^= hashValues[movingPieceIndex][(int)((move & 0x1f800)>>11)];

		//swap end line pawns for promoted pieces
		if (movingPieceIndex - 6 * turn == 0) {
			bitboards [12] &= 0xfff;
			if ((move & 0x3) == 0) {
				// promote to queen
				bitboards [4 + 6 * turn] ^= (bitboards [turn * 6] & 0xff000000000000ff);
			} else {
				bitboards [1 + 6 * turn] ^= (bitboards [turn * 6] & 0xff000000000000ff);
			}
			bitboards [6 * turn] &= 0xffffffffffff00;
		}


		// possible captured piece
		uint capturedType = ((move & 0x70) >> 4);
		if (capturedType < 6) {
			bitboards [12] &= 0xfff;
			bitboards [capturedType + 6 - 6 * turn] ^= ((ulong)1 << (int)((move & 0x1f800) >> 11));
			// hash
			bitboards[13] ^= hashValues[capturedType + 6 - 6 * turn][(int)((move & 0x1f800) >> 11)];

		} else if (capturedType == 6) {
			bitboards [12] &= 0xfff;
			// en passant, correct pawn bitboard based on en passant tile
			bitboards[6-6*turn] ^= (ulong)1<<(((int)(bitboards[12] & 0x7e0) >> 5)-8+16*(int)turn);
			// hash
			bitboards[13] ^= hashValues[6-6*turn][(((int)(bitboards[12] & 0x7e0) >> 5)-8+16*(int)turn)];

		}
		// castling
		if ((move & 0xc) == 4) {
			// king side
			bitboards [3 + 6 * turn] ^= (ulong)0xa0 << 56 * (int)turn;
			// hash
			bitboards[13] ^= hashValues[3 + 6 * turn][5+56*(int)turn];
			bitboards[13] ^= hashValues[3 + 6 * turn][7+56*(int)turn];

		} else if ((move & 0xc) == 8) {
			// queen side
			bitboards [3 + 6 * turn] ^= (ulong)0x9 << 56 * (int)turn;
			// hash
			bitboards[13] ^= hashValues[3 + 6 * turn][56*(int)turn];
			bitboards[13] ^= hashValues[3 + 6 * turn][3+56*(int)turn];

		}

		// set en passant tile, en passant allowed, castling variables
		// castling
		//white
		if ((bitboards [12] & 24) != 0) {
			// king
			if (bitboards [5] != 16) {
				bitboards [12] &= 0xfffe7;
			} else { 
				// rooks
				if ((bitboards [3] & 1) == 0) {
					bitboards [12] &= 0xffff7;
				}
				if ((bitboards [3] & 0x80) == 0) {
					bitboards [12] &= 0xfffef;
				}
			}
		}
		// black
		if ((bitboards [12] & 6) != 0) {
			// king
			if (bitboards [11] != 0x1000000000000000) {
				bitboards [12] &= 0xffff9;
			} else { 
				// rooks
				if ((bitboards [9] & 0x100000000000000) == 0) {
					bitboards [12] &= 0xffffd;
				}
				if ((bitboards [9] & 0x8000000000000000) == 0) {
					bitboards [12] &= 0xffffb;
				}
			}
		}
		// reset en passant
		bitboards [12] &= 0xff01f;
		if ((move & 0x380) == 0 && (((move & 0x7e0000) >> 17) + 16 - 32 * turn) == ((move & 0x1f800) >> 11)) {
			// en passant
			uint epTile = ((move & 0x7e0000) >> 17) + 8 - 16 * turn;
			bitboards [12] |= 0x800 + (epTile << 5);
		} 
			
		// change turn
		bitboards [12] ^= 1;
	}

	// perform moves on board
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
			Vector3 offset = Camera.main.WorldToScreenPoint(bottomLeft.position);
			Vector3 maxim =  Camera.main.WorldToScreenPoint(topRight.position);
	
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
	

	void Start (){
		// load data such as magic numbers/positions and evaluation tables
		// fields static so only loaded first time game played, not on repeats
		string[] sources64 = new string[] {"bishopMagic.txt", "rookMagic.txt", "bishopPremasks.txt",
									"rookPremasks.txt", "knightMoves.txt", "kingMoves.txt"};
		ulong[][] arrays64 = new ulong[][] {bishopMagics, rookMagics, bishopPremasks,
									rookPremasks, knightMoves, kingMoves};
		string text;
		for (int i = 0; i < arrays64.Length; i++){
			if (arrays64[i][0] == 0){
				string path = "Assets/resources/"+sources64[i];
				using (StreamReader reader = new StreamReader(path)){
					for (int j = 0; j < 64; j++){
						text = reader.ReadLine();
						arrays64[i][j] = System.Convert.ToUInt64(text,16);
					}
				}
			}
		}
		string[] sources64x = new string[] {"magicRookAttacks.txt","magicBishopAttacks.txt"};
		ulong[][][] arrays64x = new ulong[][][] {magicRookAttacks, magicBishopAttacks};
		for (int i = 0; i < arrays64x.Length; i++){
			if (arrays64x[i][0] == null){
				string path = "Assets/resources/"+sources64x[i];
				using (StreamReader reader = new StreamReader(path)){
					for (int j = 0; j < 64; j++){
						text = reader.ReadLine();
						string[] split = text.Split(' ');
						ulong[] line = new ulong[split.Length];
						for (int k = 0; k < line.Length; k++){
							line[k] = System.Convert.ToUInt64(split[k],16);
						}
						arrays64x[i][j] = line;
					}
				}
			}
		}
		if (openingLines == null){
			openingLines = new List<uint[]> ();
			string path = "Assets/resources/openingLines.txt";
			using (StreamReader reader = new StreamReader(path)){
				text = reader.ReadLine();
				while (text != null) {
					string[] split = text.Split(',');
					uint[] line = new uint[split.Length];
					for (int j = 0; j < line.Length; j++){
						line[j] = System.Convert.ToUInt32(split[j],16);
					}
					openingLines.Add(line);
					text = reader.ReadLine();
				}
			}
		}
		if (pieceSquareTable == null){
			pieceSquareTable = new int[12][];
			string path = "Assets/resources/evaluationTables.txt";
			using (StreamReader reader = new StreamReader(path)){
				for (int i = 0; i < 12; i++){
					text = reader.ReadLine();
					string[] split = text.Split(',');
					int[] line = new int[64];
					for (int j = 0; j<64; j++){
						line[j] = System.Convert.ToInt32(split[j]);
					}
					pieceSquareTable[i] = line;
				}
			}
		}
		if (endPieceSquares == null){
			endPieceSquares = new int[4][];
			string path = "Assets/resources/endgameTables.txt";
			using (StreamReader reader = new StreamReader(path)){
				for (int i = 0; i < 4; i++){
					text = reader.ReadLine();
					string[] split = text.Split(',');
					int[] line = new int[64];
					for (int j = 0; j<64; j++){
						line[j] = System.Convert.ToInt32(split[j]);
					}
					endPieceSquares[i] = line;
				}
			}
		}
		
		ulong pt1, pt2;
		// zobrist hash values
		for (int a = 0; a < 12; a++) {
			hashValues [a] = new ulong[64];
			for (int b = 0; b < 64; b++) {
				pt1 = (ulong)rnd.Next ();
				pt2 = ((ulong)rnd.Next ())<<32;
				hashValues [a][b] = pt2 + pt1;
			}
		}
		for (int a = 0; a < 12; a++) {
			ulong pieceBoard = bitboardArray [a];
			while (pieceBoard != 0) {
				uint lsbIndex = leastSigLookup [((pieceBoard & (~pieceBoard + 1)) * debruijnleast) >> 58];
				bitboardArray [13] ^= hashValues [a] [lsbIndex];
				pieceBoard &= pieceBoard - 1;
			}
		}
		visitedHashes.Add (bitboardArray [13]);

		moveLogText.text = "";

		maxDepth = PlayerPrefs.GetInt ("Depth");
		maxTime = PlayerPrefs.GetInt ("Time");
		extensionDepth = 1 - PlayerPrefs.GetInt ("Extensions");
		blackTurn.SetActive (false);
		whiteTurn.SetActive (true);
		players = new int[] { 0, 0 };

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

		changeDimension ();
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
