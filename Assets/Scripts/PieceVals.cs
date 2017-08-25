﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceVals : MonoBehaviour {


	int[] baseValues = {100, 320, 330, 500, 900, 20000, 0, 0};
	int[][] pieceSquareTable = new int[][]{
		// wp
		new int[]
		{0, 0, 0, 0, 0, 0, 0, 0,
			0, 10, 10, -20, -25, 10, 10, 0,
			5,-5, -10, -5, -5,-10, -5, 5,
			5, 0, 0, 20, 20, 0, 0, 0, 
			5, 5, 10, 25, 25, 10, 5, 5, 10, 10, 20, 30, 30, 20, 10, 10,
			50, 50, 50, 50, 50, 50, 50, 50, 0, 0, 0, 0, 0, 0, 0, 0} , 

		// wn
		new int []
		{-50, -35, -30, -30, -30, -30, -35, -50, -40, -20, 0, 5, 5,
			0, -20, -40, -30, 5, 5, 15, 15, 5, 5, -30, -30, 0, 15, 20,
			20, 15, 0, -30, -30, 5, 15, 20, 20, 15, 5, -30, -30, 0, 10,
			15, 15, 10, 0, -30, -40, -20, 0, 0, 0, 0, -20, -40, -50, -40,
			-30, -30, -30, -30, -40, -50},

		// wb
		new int []
		{-20, -10, -10, -10, -10, -10, -10, -20, -10, 5, 0, 0, 0, 0, 5, -10, 
			-10, 10, 10, 10, 10, 10, 10, -10, -10, 0, 10, 10, 10, 10, 0, -10,
			-10, 5, 5, 10, 10, 5, 5, -10, -10, 0, 5, 10, 10, 5, 0, -10, -10,
			0, 0, 0, 0, 0, 0, -10, -20, -10, -10, -10, -10, -10, -10, -20},

		// wr
		new int []
		{0, 0, 0, 5, 5, 0, 0, 0, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0,
			0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5,
			-5, 0, 0, 0, 0, 0, 0, -5, 5, 10, 10, 10, 10, 10, 10, 5, 0, 0,
			0, 0, 0, 0, 0, 0},

		// wq
		new int []
		{-20, -10, -10, -5, -5, -10, -10, -20, -10, 0, 5, 0, 0, 0, 0, -10,
			-10, 5, 5, 5, 5, 5, 0, -10, 0, 0, 5, 5, 5, 5, 0, -5, -5, 0, 5,
			5, 5, 5, 0, -5, -10, 0, 5, 5, 5, 5, 0, -10, -10, 0, 0, 0, 0, 0,
			0, -10, -20, -10, -10, -5, -5, -10, -10, -20},
	
		// wk
		new int []
		{20, 30, 10, 0, 0, 10, 30, 20, 20, 20, 0, 0, 0, 0, 20, 20, -10, -20, -20, 
			-20, -20, -20, -20, -10, -20, -30, -30, -40, -40, -30, -30, -20, -30,
			-40, -40, -50, -50, -40, -40, -30, 
			-30, -40, -40, -50, -50, -40, -40,-30,
			-30, -40, -40, -50, -50, -40, -40, -30, 
			-30, -40, -40, -50, -50, -40, -40, -30},
	
		// black

		// bp
		new int []
		{0, 0, 0, 0, 0, 0, 0, 0, 50, 50, 50, 50, 50, 50, 50, 50, 10, 10, 20, 30, 30, 20, 10, 10, 5, 5, 10, 25, 25, 10, 5, 5, 5, 0, 0, 20, 20, 0, 0, 0, 5, -5, -10, -5, -5, -10, -5, 5, 0, 10, 10, -20, -25, 10, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0},
	
		// bn
		new int []
		{-50, -40, -30, -30, -30, -30, -40, -50, -40, -20, 0, 0, 0, 0, -20, -40, -30, 0, 10, 15, 15, 10, 0, -30, -30, 5, 15, 20, 20, 15, 5, -30, -30, 0, 15, 20, 20, 15, 0, -30, -30, 5, 5, 15, 15, 5, 5, -30, -40, -20, 0, 5, 5, 0, -20, -40, -50, -35, -30, -30, -30, -30, -35, -50},
	
		// bb
		new int[]
		{-20, -10, -10, -10, -10, -10, -10, -20, -10, 0, 0, 0, 0, 0, 0, -10, -10, 0, 5, 10, 10, 5, 0, -10, -10, 5, 5, 10, 10, 5, 5, -10, -10, 0, 10, 10, 10, 10, 0, -10, -10, 10, 10, 10, 10, 10, 10, -10, -10, 5, 0, 0, 0, 0, 5, -10, -20, -10, -10, -10, -10, -10, -10, -20},

		// br
		new int[]
		{0, 0, 0, 0, 0, 0, 0, 0, 5, 10, 10, 10, 10, 10, 10, 5, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5, 0, 0, 0, 5, 5, 0, 0, 0},
	
		// bq
		new int[]
		{-20, -10, -10, -5, -5, -10, -10, -20, -10, 0, 0, 0, 0, 0, 0, -10, -10, 0, 5, 5, 5, 5, 0, -10, -5, 0, 5, 5, 5, 5, 0, -5, 0, 0, 5, 5, 5, 5, 0, -5, -10, 5, 5, 5, 5, 5, 0, -10, -10, 0, 5, 0, 0, 0, 0, -10, -20, -10, -10, -5, -5, -10, -10, -20},
	
		// bk
		new int[]
		{-30, -40, -40, -50, -50, -40, -40, -30, -30, -40, -40, -50, -50, -40, -40, -30, -30, -40, -40, -50, -50, -40, -40, -30, -30, -40, -40, -50, -50, -40, -40, -30, -20, -30, -30, -40, -40, -30, -30, -20, -10, -20, -20, -20, -20, -20, -20, -10, 20, 20, 0, 0, 0, 0, 20, 20, 20, 30, 10, 0, 0, 10, 30, 20}
	};

	private int pieceIndex;
	private int total;
	private int value;
	private int fromPieceIndex;
	private int toPieceIndex;
	private int[][] endPieceSquares = new int[][] {
		// white pawns, white king, black pawns, black king
		new int[]
		{0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 0, 0, 5, 5, 5, 10, 10, 10, 10, 10, 10, 10,
			10, 20, 20, 20, 20, 20, 20, 20, 20, 30, 30, 30, 30, 30, 30, 30, 30, 45, 45,
			45, 50, 50, 45, 45, 45, 65, 65, 65, 70, 70, 65, 65, 65, 80, 80, 80, 80, 80, 80, 80, 80} ,

new int[]
		{-50, -30, -30, -30, -30, -30, -30, -50, -30, -30, 0, 0, 0, 0, -30, -30, -30, -10, 20, 
			30, 30, 20, -10, -30, -30, -10, 30, 35, 35, 30, -10, -30, -30, -10, 30, 35, 35, 30,
			-10, -30, -30, -10, 20, 30, 30, 20, -10, -30, -30, -20, -10, 0, 0, -10, -20, -30,
			-50, -40, -30, -20, -20, -30, -40, -50},

new int[]
		{80, 80, 80, 80, 80, 80, 80, 80, 65, 65, 65, 70, 70, 65, 65, 65, 45, 45, 45, 50, 50, 45,
			45, 45, 30, 30, 30, 30, 30, 30, 30, 30, 20, 20, 20, 20, 20, 20, 20, 20, 10, 10, 10,
			10, 10, 10, 10, 10, 5, 5, 5, 0, 0, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0},

new int[]
		{-50, -40, -30, -20, -20, -30, -40, -50, -30, -20, -10, 0, 0, -10, -20, -30, -30, -10, 20, 
			30, 30, 20, -10, -30, -30, -10, 30, 35, 35, 30, -10, -30, -30, -10, 30, 35, 35, 30, -10, 
			-30, -30, -10, 20, 30, 30, 20, -10, -30, -30, -30, 0, 0, 0, 0, -30, -30, -50, -30, -30, 
			-30, -30, -30, -30, -50}
		};


	public void EnterEndGame(){
		pieceSquareTable [0] = endPieceSquares [0];
		pieceSquareTable [5] = endPieceSquares [1];
		pieceSquareTable [6] = endPieceSquares [2];
		pieceSquareTable [11] = endPieceSquares [3];
	}

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

//	public int FullEvaluate(ulong[] bitboards){
//		value = 0;
//		uint lsbIndex;
//		for (int a = 0; a < 6; a++) {
//			ulong pieceBoard = bitboards [a];
//			while (pieceBoard != 0) {
//				lsbIndex = leastSigLookup [(pieceBoard * debruijnleast) >> 58];
//				value += pieceSquareTable [a] [lsbIndex] + baseValues [a];
//				pieceBoard &= pieceBoard - 1;
//			}
//		}
//		for (int a = 6; a < 12; a++) {
//			ulong pieceBoard = bitboards [a];
//			while (pieceBoard != 0) {
//				lsbIndex = leastSigLookup [(pieceBoard * debruijnleast) >> 58];
//				value -= pieceSquareTable [a] [lsbIndex] + baseValues [a - 6];
//				pieceBoard &= pieceBoard - 1;
//			}
//		}
//
//		return value;
//	}

	public int FullEvaluate(ulong[] bitboards){
		value = 0;
		uint lsbIndex;
		for (int a = 0; a < 6; a++) {
			ulong pieceBoard = bitboards [a];
			while (pieceBoard != 0) {
				lsbIndex = leastSigLookup [((pieceBoard & (~pieceBoard+1)) * debruijnleast) >> 58];
//				Debug.Log (pieceSquareTable [a] [lsbIndex] + baseValues [a]);

				value += pieceSquareTable [a] [lsbIndex] + baseValues [a];
				pieceBoard &= pieceBoard - 1;
			}
		}
		for (int a = 6; a < 12; a++) {
			ulong pieceBoard = bitboards [a];
			while (pieceBoard != 0) {
				lsbIndex = leastSigLookup [((pieceBoard & (~pieceBoard+1)) * debruijnleast) >> 58];

//				Debug.Log (pieceSquareTable [a] [lsbIndex] + baseValues [a - 6]);

				value -= pieceSquareTable [a] [lsbIndex] + baseValues [a - 6];
				pieceBoard &= pieceBoard - 1;
			}
		}

		return value;
	}



	public int AdjustScore(uint move){
		value = 0;
		// castling
		if ((move >> 2) % 2 == 1) {
			return 30;
		} else if ((move >> 3) % 2 == 1) {
			return 15;
		}

		uint toIndex = ((move >> 11) & 0x3f); 
		uint fromIndex = move >> 17; 
		uint pieceIndex = ((move >> 7) & 0x7);
		uint capturedIndex = ((move >> 4) & 0x7);
		uint turn = (move >> 10) % 2;

		// promotion
		if ((toIndex > 55 | toIndex < 8) && pieceIndex == 0) {
			if (move % 2 == 0) {
				return 750;
			} else {
				return 180;
			}
		}

		// standard
		value += pieceSquareTable[pieceIndex+6*turn][toIndex] - pieceSquareTable[pieceIndex+6*turn][fromIndex];
	
		if (capturedIndex != 7) {
			if (capturedIndex == 6) {
				// changed
				value += 100 + pieceSquareTable [6 - 6 * turn][(toIndex+fromIndex)/2];
			} else {
				value += pieceSquareTable [capturedIndex + 6 - 6 * turn] [toIndex] + baseValues[capturedIndex];
			}
		}
		return value;
	}

}
