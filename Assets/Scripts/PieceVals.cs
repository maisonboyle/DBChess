﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceVals : MonoBehaviour {

	int[] baseValues = {100, 320, 330, 500, 900, 20000, -100, -320, -330, -500, -900, -20000};
	int[][][] pieceSquareTable = new int[][][]{
		new int[][]
		{new int[]{0,  0,  0,  0,  0,  0,  0,  0},
			new int[]	{50, 50, 50, 50, 50, 50, 50, 50},
			new int[]	{10, 10, 20, 30, 30, 20, 10, 10},
			new int[]	{5,  5, 10, 25, 25, 10,  5,  5},
			new int[]	{0,  0,  0, 20, 20,  0,  0,  0},
			new int[]		{5, -5,-10,  0,  0,-10, -5,  5},
			new int[]		{5, 10, 10,-20,-30, 10, 10,  5},
			new int[]	{0,  0,  0,  0,  0,  0,  0,  0}} ,
		new int[][]
		{new int[]{-50,-40,-30,-30,-30,-30,-40,-50},
			new int[]	{-40,-20,  0,  0,  0,  0,-20,-40},
			new int[]	{-30,  0, 10, 15, 15, 10,  0,-30},
			new int[]	{-30,  5, 15, 20, 20, 15,  5,-30},
			new int[]	{-30,  0, 15, 20, 20, 15,  0,-30},
			new int[]	{-30,  5, 5, 15, 15, 5,  5,-30},
			new int[]	{-40,-20,  0,  5,  5,  0,-20,-40},
			new int[]	{-50,-35,-30,-30,-30,-30,-35,-50}},
		new int[][]
		{new int[]{-20,-10,-10,-10,-10,-10,-10,-20},
			new int[]	{-10,  0,  0,  0,  0,  0,  0,-10},
			new int[]	{-10,  0,  5, 10, 10,  5,  0,-10},
			new int[]	{-10,  5,  5, 10, 10,  5,  5,-10},
			new int[]	{-10,  0, 10, 10, 10, 10,  0,-10},
			new int[]	{-10, 10, 10, 10, 10, 10, 10,-10},
			new int[]	{-10,  5,  0,  0,  0,  0,  5,-10},
			new int[]	{-20,-10,-10,-10,-10,-10,-10,-20}},
		new int[][]
		{new int[]{0,  0,  0,  0,  0,  0,  0,  0},
			new int[]	{5, 10, 10, 10, 10, 10, 10,  5},
			new int[]	{-5,  0,  0,  0,  0,  0,  0, -5},
			new int[]	{-5,  0,  0,  0,  0,  0,  0, -5},
			new int[]	{-5,  0,  0,  0,  0,  0,  0, -5},
			new int[]	{-5,  0,  0,  0,  0,  0,  0, -5},
			new int[]	{-5,  0,  0,  0,  0,  0,  0, -5},
			new int[]	{0,  0,  0,  5,  5,  0,  0,  0}},
		new int[][]
		{new int[]{-20,-10,-10, -5, -5,-10,-10,-20},
			new int[]	{-10,  0,  0,  0,  0,  0,  0,-10},
			new int[]	{-10,  0,  5,  5,  5,  5,  0,-10},
			new int[]	{-5,  0,  5,  5,  5,  5,  0, -5},
			new int[]	{0,  0,  5,  5,  5,  5,  0, -5},
			new int[]	{-10,  5,  5,  5,  5,  5,  0,-10},
			new int[]	{-10,  0,  5,  0,  0,  0,  0,-10},
			new int[]	{-20,-10,-10, -5, -5,-10,-10,-20}},
		new int[][]
		{new int[]{-30,-40,-40,-50,-50,-40,-40,-30},
			new int[]	{-30,-40,-40,-50,-50,-40,-40,-30},
			new int[]	{-30,-40,-40,-50,-50,-40,-40,-30},
			new int[]	{-30,-40,-40,-50,-50,-40,-40,-30},
			new int[]	{-20,-30,-30,-40,-40,-30,-30,-20},
			new int[]	{-10,-20,-20,-20,-20,-20,-20,-10},
			new int[]	{20, 20,  0,  0,  0,  0, 20, 20},
			new int[]	{20, 30, 10,  0,  0, 10, 30, 20}},
		new int[][]
		{new int[]{0, 0, 0, 0, 0, 0, 0, 0},
			new int[]	{-5, -10, -10, 20, 30, -10, -10, -5},
			new int[]	{-5, 5, 10, 0, 0, 10, 5, -5},
			new int[]	{0, 0, 0, -20, -20, 0, 0, 0},
			new int[]	{-5, -5, -10, -25, -25, -10, -5, -5},
			new int[]		{-10, -10, -20, -30, -30, -20, -10, -10},
			new int[]		{-50, -50, -50, -50, -50, -50, -50, -50},
			new int[]		{0, 0, 0, 0, 0, 0, 0, 0}},
		new int[][]
		{new int[]{50, 35, 30, 30, 30, 30, 35, 50},
			new int[]	{40, 20, 0, -5, -5, 0, 20, 40},
			new int[]	{30, -5, -5, -15, -15, -5, -5, 30},
			new int[]	{30, 0, -15, -20, -20, -15, 0, 30},
			new int[]	{30, -5, -15, -20, -20, -15, -5, 30},
			new int[]	{30, 0, -10, -15, -15, -10, 0, 30},
			new int[]	{40, 20, 0, 0, 0, 0, 20, 40},
			new int[]	{50, 40, 30, 30, 30, 30, 40, 50}},
		new int[][]
		{new int[]{20, 10, 10, 10, 10, 10, 10, 20},
			new int[]	{10, -5, 0, 0, 0, 0, -5, 10},
			new int[]	{10, -10, -10, -10, -10, -10, -10, 10},
			new int[]	{10, 0, -10, -10, -10, -10, 0, 10},
			new int[]	{10, -5, -5, -10, -10, -5, -5, 10},
			new int[]	{10, 0, -5, -10, -10, -5, 0, 10},
			new int[]	{10, 0, 0, 0, 0, 0, 0, 10},
			new int[]	{20, 10, 10, 10, 10, 10, 10, 20}},
		new int[][]
		{new int[]{0, 0, 0, -5, -5, 0, 0, 0},
			new int[]	{5, 0, 0, 0, 0, 0, 0, 5},
			new int[]	{5, 0, 0, 0, 0, 0, 0, 5},
			new int[]	{5, 0, 0, 0, 0, 0, 0, 5},
			new int[]	{5, 0, 0, 0, 0, 0, 0, 5},
			new int[]	{5, 0, 0, 0, 0, 0, 0, 5},
			new int[]	{-5, -10, -10, -10, -10, -10, -10, -5},
			new int[]	{0, 0, 0, 0, 0, 0, 0, 0}},
		new int[][]
		{new int[]{20, 10, 10, 5, 5, 10, 10, 20},
			new int[]	{10, 0, -5, 0, 0, 0, 0, 10},
			new int[]	{10, -5, -5, -5, -5, -5, 0, 10},
			new int[]	{0, 0, -5, -5, -5, -5, 0, 5},
			new int[]	{5, 0, -5, -5, -5, -5, 0, 5},
			new int[]	{10, 0, -5, -5, -5, -5, 0, 10},
			new int[]	{10, 0, 0, 0, 0, 0, 0, 10},
			new int[]	{20, 10, 10, 5, 5, 10, 10, 20}},
		new int[][]
		{new int[]{-20, -30, -10, 0, 0, -10, -30, -20},
			new int[]	{-20, -20, 0, 0, 0, 0, -20, -20},
			new int[]	{-10, -20, -20, -20, 20, 20, 20, 10},
			new int[]	{20, 30, 30, 40, 40, 30, 30, 20},
			new int[]	{30, 40, 40, 50, 50, 40, 40, 30},
			new int[]	{30, 40, 40, 50, 50, 40, 40, 30},
			new int[]	{30, 40, 40, 50, 50, 40, 40, 30},
			new int[]	{30, 40, 40, 50, 50, 40, 40, 30}}
	};

	private int pieceIndex;
	private int total;
	private int value;
	private int fromPieceIndex;
	private int toPieceIndex;
	private int[][][] endPieceSquares = new int[][][] {
		// white pawns, white king, black pawns, black king
		new int[][]
		{new int[] {0,  0,  0,  0,  0,  0,  0,  0},
			new int[] 	{65, 65, 65, 75, 75, 65, 65, 65},
			new int[] 	{45, 45, 45, 55, 55, 45, 45, 45},
			new int[] 	{25,  25, 25, 45, 45, 25,  25,  25},
			new int[] 	{10,  10,  10, 30, 30,  10,  10,  10},
			new int[] 	{5, -5,-10,  0,  0,-10, -5,  5},
			new int[] 	{5, 10, 10,-20,-30, 10, 10,  5},
			new int[] 	{0,  0,  0,  0,  0,  0,  0,  0}} ,

		new int[][]
		{new int[] {-50,-40,-30,-20,-20,-30,-40,-50},
			new int[] 	{-30,-20,-10,0,0,-10,-20,-30},
			new int[] 	{-30,-10,20,30,30,20,-10,-30},
			new int[] 	{-30,-10,30,35,35,30,-10,-30},
			new int[] 	{-30,-10,30,35,35,30,-10,-30},
			new int[] 	{-30,-10,20,30,30,20,-10,-30},
			new int[] 	{-30,-30,0,0,0,0,-30,-30},
			new int[] 	{-50,-30,-30,-30,-30,-30,-30,-50}},

		new int[][]
		{new int[] {0,  0,  0,  0,  0,  0,  0,  0},
			new int[] {-5, -10, -10,20,30, -10, -10,  -5},
			new int[] 	{-5, 5,10,  0,  0, 10,  5,  -5},
			new int[] {-10,  -10,  -10, -30, -30,  -10,  -10,  -10},
			new int[] {-25,  -25, -25, -45, -45, -25,  -25,  -25},
			new int[] 	{-45, -45, -45, -55, -55, -45, -45, -45},
			new int[] 	{-65, -65, -65, -75, -75, -65, -65, -65},
			new int[] 	{0,  0,  0,  0,  0,  0,  0,  0}},

		new int[][]
		{new int[] {50,30,30,30,30,30,30,50},
			new int[] 	{30,30,0,0,0,0,30,30},
			new int[] 	{30,10,-20,-30,-30,-20,10,30},
			new int[] 	{30,10,-30,-35,-35,-30,10,30},
			new int[] 	{30,10,-30,-35,-35,-30,10,30},
			new int[] 	{30,10,-20,-30,-30,-20,10,30},
			new int[] {30,20,10,0,0,10,20,30},
			new int[] 	{50,40,30,20,20,30,40,50}}
		};


	public void EnterEndGame(){
		pieceSquareTable [0] = endPieceSquares [0];
		pieceSquareTable [5] = endPieceSquares [1];
		pieceSquareTable [6] = endPieceSquares [2];
		pieceSquareTable [11] = endPieceSquares [3];
	}

	public int FullEvaluate(int[,] board){
		total = 0;
		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				pieceIndex = board [x,y];
				if (pieceIndex != -1) {
					total += baseValues [pieceIndex] + pieceSquareTable [pieceIndex][y][x];
				}
			}
		}
		return total;
	}

	public int AdjustScore (int[,] board, int[] fromTile, int[] toTile){
		value = 0;
		fromPieceIndex = board [fromTile [0], fromTile [1]];
		toPieceIndex = board [toTile [0], toTile [1]];
		// castling
		if ((fromPieceIndex == 5 || fromPieceIndex == 11) && (fromTile [0] - toTile [0]) * (fromTile [0] - toTile [0]) == 4) {
			if (toTile [0] - fromTile [0] == 2) {
				return 80 - 10 * fromPieceIndex;
			}
			return 40 - 5 * fromPieceIndex;
		}
		// promotion
		if (fromPieceIndex % 6 == 0 && toTile [1] % 7 == 0) {
			return 870 - 290 * fromPieceIndex;
		}

		// en passant
		if (fromPieceIndex % 6 == 0 && fromTile [0] != toTile [0] && toPieceIndex == -1) {
			value = pieceSquareTable [fromPieceIndex][toTile [1]][toTile [0]] - pieceSquareTable [fromPieceIndex][fromTile [1]][fromTile [0]];
			value -= baseValues [6-fromPieceIndex] + pieceSquareTable [6 - fromPieceIndex][fromTile [1]][toTile [0]];
			return value;
		}


		// standard case
		value = pieceSquareTable [fromPieceIndex][toTile [1]][toTile [0]] - pieceSquareTable [fromPieceIndex][fromTile [1]][fromTile [0]];
		if (toPieceIndex != -1) {
			value -= baseValues [toPieceIndex] + pieceSquareTable [toPieceIndex][toTile [1]][toTile [0]];
		}
		return value;
	}

}
