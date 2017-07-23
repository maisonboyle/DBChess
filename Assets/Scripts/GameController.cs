using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour{

	// [WP,WKn,WB,WR,WQ,WK, bp,bkn,bb,br,bq,bk]
	public List<GameObject> piecePrefabs;
	public List<GameObject> twoDimensionPieces;
	private int[,] boardData = new int[8, 8];
	private int gameTurn = 0;
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

	// human (0) or comp (1) for white 1st and black 2nd
	public int[] players;

	private int[] staleList = new int[] {-1,-2,-3,-4,-5,-6,-7,-8,-9,-1,-25,-3,-4,-5,-6,-7,-8,-9,-10,-2,-3,-4,-5,-6,-7,-8,-9,-1,-2,-31,-4,-5,-6,-7,-8,-9,
		-1,-2,-3,-4,-5,-6,-7,-8,-9, 17, -5, -2};
	private int selectionX;
	private int selectionY;
	private int[] startTile = new int[] {-1,-1};
	private int[] endTile = new int[] {-1,-1};
	private int startPieceIndex;
	private int endPieceIndex;
	private int passantTurn = 0;
	private int[] passantTile = new int[] { 0, 0 };
	private bool computerMove = false;
	private bool goToComputer = false;
	private bool threeD = true;
	private int whiteTime;
	private int blackTime;
	private bool gameDone = false;
	// experimental to try to make better opening
	private int numberOfMoves = 0;
	private int maxDepth = 4;

	private string[] across = new string[] {"a","b","c","d","e","f","g","h"};
	private string[] up = new string[] {"1","2","3","4","5","6","7","8"};
	private List<string> moveLog = new List<string>();


	// castling represented by moving king across two
	private bool castleWhiteLeft = true, castleWhiteRight = true, castleBlackLeft = true, castleBlackRight = true;

	private void UpdateMoveLog(int[] move){
		string moveText;
		if (move [0] - move [2] == 2 && boardData [move [0], move [1]] % 6 == 5) {
			 moveText = "O-O-O";
		} else if (move [0] - move [2] == -2 && boardData [move [0], move [1]] % 6 == 5) {
			 moveText = "O-O";
		} else {
			 moveText = across [move [0]] + up [move [1]] + across [move [2]] + up [move [3]];
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
		if (goToComputer) {
			goToComputer = false;
			CompTurn ();
		}
		if (Input.GetMouseButtonUp(0) && !gameDone) {
			MouseUp ();
		}
		UpdateSelection ();
		if (computerMove) {
			goToComputer = true;
		}
	}

	private int[] updateStale(int[] stale, int[] newMove){
		return new int[] {
			stale [4],
			stale [5],
			stale [6],
			stale [7],
			stale [8],
			stale [9],
			stale [10],
			stale [11],
			stale [12],
			stale [13],
			stale [14],
			stale [15],
			stale [16],
			stale [17],
			stale [18],
			stale [19],
			stale [20],
			stale [21],
			stale [22],
			stale [23],
			stale [24],
			stale [25],
			stale [26],
			stale [27],
			stale [28],
			stale [29],
			stale [30],
			stale [31],
			stale [32],
			stale [33],
			stale [34],
			stale [35],
			stale [36],
			stale [37],
			stale[38], 
			stale [39],
			stale [40],
			stale [41],
			stale [42],
			stale [43],
			stale [44],
			stale [45],
			stale [46],
			stale [47],
			newMove [0],
			newMove [1],
			newMove [2],
			newMove [3]
		};
	}

	private bool isStale(int[] stale){
		for (int i = 0; i < 16; i++){
			if (!(stale[i] == stale[16+i] && stale[16+i] == stale[32+i])){
				return false;
			}
		}
		return true;
	}

	private void MouseUp(){

		// player's turn
		if (selectionX != -1 && players [gameTurn] == 0) {
			
			int pieceIndex = boardData [selectionX, selectionY]; 

			// own piece selected
			if (pieceIndex > -1 + 6 * gameTurn && pieceIndex < 6 + 6 * gameTurn) {
				chosen.transform.position = new Vector3 (selectionX, 0.01f, selectionY);
				chosen.SetActive (true);
				twoChosen.transform.position = new Vector3 (selectionX, selectionY, 0.01f);
				twoChosen.SetActive (true);
				startTile = new int[] { selectionX, selectionY };
				startPieceIndex = pieceIndex;
			} else if (startTile[0] != -1){
				endPieceIndex = pieceIndex;
				endTile = new int[] { selectionX, selectionY };
				if (ValidMove (boardData, startTile, endTile, startPieceIndex, endPieceIndex, gameTurn)) {
					int[,] boardClone = (int[,])boardData.Clone ();
					boardClone = MakeMove (boardClone, startTile, endTile, startPieceIndex, endPieceIndex);
					int[] kingPos = FindKing (boardClone, gameTurn);
					if (!InCheck (boardClone, gameTurn, kingPos)) {
//						boardData = MakeMove (boardData, startTile, endTile, startPieceIndex, endPieceIndex);
						CastleBools (startTile, endTile);
						gameTurn = 1 - gameTurn;
						UpdateMoveLog (new int[] { startTile [0], startTile [1], endTile [0], endTile [1] });
						if (whiteTurn.activeSelf) {
							whiteTurn.SetActive (false);
							blackTurn.SetActive (true);
						} else {
							whiteTurn.SetActive (true);
							blackTurn.SetActive (false);
						}
						passantTurn -= 1;
						chosen.SetActive (false);
						twoChosen.SetActive (false);
						// drawing castling
						if ((startPieceIndex == 5 || startPieceIndex == 11) && (startTile [0] - endTile [0]) * (startTile [0] - endTile [0]) == 4) {
							if (endTile [0] - startTile [0] == -2) {
								VisualUpdate (new int[] { 0, startTile [1] }, new int[] { 3, startTile [1] });
							} else {
								VisualUpdate (new int[] { 7, startTile [1] }, new int[] { 5, startTile [1] });
							}
							// drawing en passant
						} else if ((startPieceIndex == 0 || startPieceIndex == 6) && startTile [0] != endTile [0] && endPieceIndex == -1) {
							VisualUpdate (new int[] { -1, -1 }, new int[] { endTile [0], startTile [1] });
						}
						VisualUpdate (startTile, endTile);
						// moved boarddata
						boardData = MakeMove (boardData, startTile, endTile, startPieceIndex, endPieceIndex);
						startTile = new int[] { -1, -1 };



						numberOfMoves += 1;
						staleList = updateStale (staleList, new int[] { startTile [0], startTile [1], endTile [0], endTile [1] });
						if (isStale (staleList)) {
							gameDone = true;
							gameOverButton.SetActive (true);
							Invoke ("Ending", 4.0f);
						}

						if (InCheckmate (boardData, gameTurn)) {
							gameDone = true;
							gameOverButton.SetActive (true);
							Invoke ("Ending", 4.0f);
					
						} else if (players [gameTurn] == 1) {
							computerMove = true;
						}
					}
				}
			}
		}
	}

	private void Ending(){
		gameOverButton.SetActive (false);
	}


	private int HowManyPieces(int[,] board){
		int total = 0;
		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				int startPieceIndex = board [x, y];
				if (startPieceIndex != -1) {
					total += 1;
				}
			}
		}
		return total;
	}

	private void CompTurn(){
		
		int[] moveData;
		computerMove = false;
		if (numberOfMoves < 2) {
			moveData = new int[] { 4, 1 + 5 * gameTurn, 4, 3 + gameTurn, 0 };
			int[,] boardClone = (int[,])boardData.Clone ();
			boardClone = MakeMove (boardClone, new int[] {4, 1 + 5 * gameTurn}, new int[] {4, 3+gameTurn}, 6*gameTurn, -1);
			for (int xStart = 0; xStart < 8; xStart++) {
				for (int yStart = 0; yStart < 8; yStart++) {
					int startPieceIndex = boardClone [xStart, yStart];
					if (startPieceIndex > -1 + 6 * (1-gameTurn) && startPieceIndex < 6 + 6 * (1-gameTurn)) {
						if (ValidMove(boardClone, new int[] {xStart,yStart}, new int[] {4, 3+gameTurn},startPieceIndex, 6*gameTurn, (1-gameTurn))){
							moveData = new int[] { 3, 1 + 5 * gameTurn, 3, 3 + gameTurn, 0 };
							break;
						}
					}
				}
			}
		
		} else {

		// thishasbeenchanged
			moveData = CompSearch (boardData, gameTurn, maxDepth, 1, 0, staleList);
		}

//		moveData = CompSearch (boardData, gameTurn, 4, 1, 0);
		if (moveData [0] == -1) {
			gameDone = true;
			gameOverButton.SetActive (true);
			Invoke ("Ending", 4.0f);
			return;
		}

		startTile = new int[] { moveData [0], moveData [1] };
		endTile = new int[] { moveData [2], moveData [3] };
		startPieceIndex = boardData [startTile [0], startTile [1]];
		endPieceIndex = boardData [endTile [0], endTile [1]];
		CastleBools (startTile, endTile);
		gameTurn = 1 - gameTurn;
		UpdateMoveLog(new int[] {startTile[0],startTile[1], endTile[0],endTile[1]});
		if (whiteTurn.activeSelf) {
			whiteTurn.SetActive (false);
			blackTurn.SetActive (true);
		} else {
			whiteTurn.SetActive (true);
			blackTurn.SetActive (false);
		}
		passantTurn -= 1;
		numberOfMoves += 1;

		if ((startPieceIndex == 5 || startPieceIndex == 11) && (startTile [0] - endTile [0]) * (startTile [0] - endTile [0]) == 4) {
			if (endTile [0] - startTile [0] == -2) {
				VisualUpdate (new int[] { 0, startTile [1] }, new int[] { 3, startTile [1] });
			} else {
				VisualUpdate (new int[] { 7, startTile [1] }, new int[] { 5, startTile [1] });
			}
			// drawing en passant
		} else if ((startPieceIndex == 0 || startPieceIndex == 6) && startTile [0] != endTile [0] && endPieceIndex == -1) {
			VisualUpdate (new int[] { -1, -1 }, new int[] { endTile [0], startTile [1] });
		}
		VisualUpdate (startTile, endTile);
		// moved boarddata
		staleList = updateStale (staleList, new int[] { startTile [0], startTile [1], endTile [0], endTile [1] });
		if (isStale (staleList)) {
			gameDone = true;
			gameOverButton.SetActive (true);
			Invoke ("Ending", 4.0f);
		}
		boardData = MakeMove (boardData, startTile, endTile, startPieceIndex, endPieceIndex);
		if (InCheckmate (boardData, gameTurn)) {
			gameDone = true;
			gameOverButton.SetActive (true);
			Invoke ("Ending", 4.0f);
		}else if (players [gameTurn] == 1) {
			computerMove = true;
		}
	}
	//thishasbeenchanged
	private int[] CompSearch(int[,] board, int turn, int maxDepth, int currentDepth, int value, int[] staleloc){
		if (currentDepth == 1) {
			value = pieceVals.FullEvaluate (board);
		}
		// bestmove format xstart, ystart, xend, yend, value
		List<int[]> bestMove = new List<int[]> {new int[] {-1,-1,-1,-1,-100000+200000*turn}};
		for (int xStart = 0; xStart < 8; xStart++) {
			for (int yStart = 0; yStart < 8; yStart++) {
				int startPieceIndex = board [xStart, yStart];
				if (startPieceIndex > -1 + 6 * turn && startPieceIndex < 6 + 6 * turn) {
					for (int xEnd = 0; xEnd < 8; xEnd++) {
						for (int yEnd = 0; yEnd < 8; yEnd++) {
							int endPieceIndex = board [xEnd, yEnd];
							if (endPieceIndex < 6 * turn || endPieceIndex > 5 + 6 * turn) {
								if (ValidMove (board, new int[] { xStart, yStart }, new int[] { xEnd, yEnd }, startPieceIndex, endPieceIndex, turn)) {
									int[,] boardClone = (int[,])board.Clone ();
									boardClone = MakeMove (boardClone, new int[] { xStart, yStart }, new int[] { xEnd, yEnd }, startPieceIndex, endPieceIndex);
									int[] kingPos = FindKing (boardClone, turn);
									if (!InCheck (boardClone, turn, kingPos)) {
										int testValue = value + pieceVals.AdjustScore (board, new int[] { xStart, yStart }, new int[] { xEnd, yEnd });

										bool cont = true;
										if (((turn == 0 && testValue < bestMove [0][4]) || (turn == 1 && testValue > bestMove [0][4])) && maxDepth == 4) {
											cont = false;
										}
										// thishasbeenchanged
										if (isStale (staleloc)) {
											testValue = 0;
										} else if (currentDepth == maxDepth) {

											kingPos = FindKing (boardClone, 1 - turn);
											if (InCheck (boardClone, 1 - turn, kingPos)) {
												testValue += 5 - 10 * turn;
												if (InCheckmate (boardClone, 1 - turn)) {
													testValue += 2000 - 4000 * turn;
												}
											}
											// adjust score if check found here
										}

										if (currentDepth != maxDepth && cont) {
											// thishasbeenchanged
											testValue = CompSearch (boardClone,1-turn,maxDepth, currentDepth+1,testValue, updateStale(staleloc, new int[] {xStart,yStart,xEnd,yEnd}))[4];
										}
										
										if (turn == 1) {
											if (testValue < bestMove [0] [4] || bestMove[0][4] == 100000) {
												bestMove = new List<int[]> { new int[] { xStart, yStart, xEnd, yEnd, testValue } };
											} else if (testValue == bestMove [0] [4] && currentDepth == 1) {
												bestMove.Add (new int[] { xStart, yStart, xEnd, yEnd, testValue });
											}
										} else if (testValue > bestMove [0] [4] || bestMove[0][4] == -100000) {
											bestMove = new List<int[]> { new int[] { xStart, yStart, xEnd, yEnd, testValue } };
										} else if (testValue == bestMove [0] [4] && currentDepth == 1) {
											bestMove.Add (new int[] { xStart, yStart, xEnd, yEnd, testValue });
										}
									
									}
								}
							}
						}
					}
				}
			}
		}
		return bestMove[Random.Range (0, bestMove.Count)];
	}




	private void CastleBools(int[] fromTile, int[] toTile){
		for (int i = 0; i < 2; i++){
			int[] checkTile = new int[][] { fromTile, toTile } [i];

			if (checkTile[0] == 0 && checkTile[1] == 0) {
				castleWhiteLeft = false;
			} else if (checkTile[0] == 7 && checkTile[1] == 0 ) {
				castleWhiteRight = false;
			} else if (checkTile[0] == 0 && checkTile[1] == 7) {
				castleBlackLeft = false;
			} else if (checkTile[0] == 7 && checkTile[1] == 7) {
				castleBlackRight = false;
			} else if (checkTile[0] == 4 && checkTile[1] == 0) {
				castleWhiteRight = false;
				castleWhiteLeft = false;
			} else if (checkTile[0] == 4 && checkTile[1] == 7) {
				castleBlackLeft = false;
				castleBlackRight = false;
			}
		}
	}

	private int[] FindKing(int[,] board, int turnToTest){
		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				if (board [x, y] == 5 + 6 * turnToTest) {
					return new int[] {x, y};
				}
			}
		}
		return new int[] { -1, -1 };
	}

	private bool InCheck(int[,] board, int turnToTest, int[] kingPos){
		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				if (board[x,y] > 5-6*turnToTest && board[x,y] < 12-6*turnToTest){
					if (ValidMove(board, new int[] {x,y}, kingPos, board[x,y], 5+6*turnToTest, 1-turnToTest)){
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool InCheckmate(int[,] board, int turnToTest){
		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				if (board [x, y] > -1 + 6 * turnToTest && board [x, y] < 6 + 6 * turnToTest) {
					for (int w = 0; w < 8; w++) {
						for (int z = 0; z < 8; z++) {
							if (board [w, z] < 0 + 6 * turnToTest || board [w, z] > 5 + 6 * turnToTest) {
								if (ValidMove(board, new int[] {x,y}, new int[] {w,z}, board[x,y] ,board[w,z] , turnToTest)){
									int[,] boardClone = (int[,])board.Clone ();
									boardClone = MakeMove (boardClone, new int[] {x,y}, new int[] {w,z},  board[x,y] ,board[w,z]);
									int[] kingPos = FindKing (boardClone, turnToTest);
									if (!InCheck (boardClone, turnToTest, kingPos)) {
										return false;
									}
								}
							}
						}
					}
				}
			}
		}
		return true;
	}

	private int[,] MakeMove(int[,] board, int[] fromTile, int[] toTile, int fromPieceIndex, int toPieceIndex){

		// casting
		if ((fromPieceIndex == 5 || fromPieceIndex == 11) && (fromTile [0] - toTile [0]) * (fromTile [0] - toTile [0]) == 4) {
			board [toTile [0], toTile [1]] = fromPieceIndex;
			board [fromTile [0], fromTile [1]] = -1;
			if (toTile [0] - fromTile [0] == -2) {
				board [3, toTile [1]] = board [0, toTile [1]];
				board [0, fromTile [1]] = -1;
			} else { 
				board [5, toTile [1]] = board [7, toTile [1]];
				board [7, fromTile [1]] = -1;
			}
			return board;
		}
		// en passant
		if ((fromPieceIndex == 0 || fromPieceIndex == 6) && fromTile[0] != toTile[0] && toPieceIndex == -1){
			board [toTile [0], fromTile [1]] = -1;
		}
		board [toTile [0], toTile [1]] = fromPieceIndex;
		board [fromTile [0], fromTile [1]] = -1;

		// promotions
		if ((fromPieceIndex == 0 || fromPieceIndex == 6) && (toTile [1] == 7 || toTile [1] == 0)) {
			board [toTile [0], toTile [1]] = fromPieceIndex + 4;
		}
		return board;
	}

	private void VisualUpdate(int[] fromTile, int[] toTile){
		if (boardData [toTile [0], toTile [1]] != -1) {
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
		}

		foreach (Transform child in threePieces.transform) {
			if (child.position.x == fromTile [0] && child.position.z == fromTile [1] && child.gameObject.CompareTag ("Piece")) {
				if (boardData [fromTile [0], fromTile [1]] % 6 == 0 && toTile [1] % 7 == 0) {
					Destroy (child.gameObject);
//					GeneratePiece (toTile [0], toTile [1], boardData [fromTile [0], fromTile [1]] + 4);
				}
				child.position = new Vector3 (toTile [0], 0.0f, toTile [1]);

			} 
		}
		
		foreach (Transform child in twoPieces.transform){
			if (child.position.x == fromTile [0] && child.position.y == fromTile [1] && child.gameObject.CompareTag ("twoDimensionPiece")) {

				if (boardData [fromTile [0], fromTile [1]] % 6 == 0 && toTile [1] % 7 == 0) {
					Destroy (child.gameObject);
					GeneratePiece (toTile [0], toTile [1], boardData [fromTile [0], fromTile [1]] + 4);
				}
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
		//	Debug.Log("target is " + screenPos.x + " pixels from the left");
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
		if (gameTurn == 0) {
			if (whiteTime > 0) {
				whiteTime -= 1;
				whiteTimeText.text = "White: " + whiteTime / 60 + ":" + new System.String('0', ((whiteTime % 60).ToString ().Length) % 2) + (whiteTime % 60).ToString ();
				if (whiteTime == 0) {
					GameOver ();
				}
			}
		} else if (blackTime > 0) {
			blackTime -= 1;
			blackTimeText.text = "Black: " + blackTime / 60 + ":" + new System.String('0', ((blackTime % 60).ToString ().Length) % 2) + (blackTime % 60).ToString ();
			if (blackTime == 0) {
				GameOver ();
			}
		}
	}


	void Start (){
		moveLogText.text = "";
		PlayerPrefs.SetInt ("Depth", 4);
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
		// sets starting board data to all -1 to represent empty
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				boardData [i, j] = -1;
			}
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

		boardData [x, y] = pieceIndex;
	}

	private GameObject GetPieceTwo(int pieceIndex){
		return twoDimensionPieces [pieceIndex];
	}

	private GameObject GetPiece(int pieceIndex){
		return piecePrefabs [pieceIndex];
	}

	private bool ValidMove (int[,] board, int[] fromTile, int[] toTile,
		int fromPieceIndex, int toPieceIndex, int currentTurn){
		// need to add stepping through possible interuptions
		int dx = toTile[0] - fromTile[0];
		int dy = toTile[1] - fromTile[1];
		if (fromTile [0] + dx > 7 || fromTile [1] + dy > 7 || fromTile[0] - dx < 0 || fromTile[1] - dy < 0) {
		}
		// knights
		if (fromPieceIndex == 1 + 6 * currentTurn) {
			if ((dx * dx + dy * dy) == 5) {
				return true;
			}
			// bishops / queens
		} else if (fromPieceIndex == 2 + 6 * currentTurn ) {
			if (dx * dx == dy * dy) {
				return StepMove (board, fromTile, dx, dy);
			}
			// rooks / queens
		} else if (fromPieceIndex == 3 + 6 * currentTurn ) {
			if (dx == 0 || dy == 0) {
				return StepMove (board, fromTile, dx, dy);
			}
			//Queens
		} else if (fromPieceIndex == 4 + 6 * currentTurn){
			if (dx * dx == dy * dy || dx == 0 || dy == 0) {
				return StepMove (board, fromTile, dx, dy);
			}
			// kings
		} else if (fromPieceIndex == 5 + 6 * currentTurn) {
			if (dx * dx + dy * dy < 3) {
				return true;
				// castling
			} else if (dx * dx == 4 && dy * dy == 0 && fromTile[0] == 4 && fromTile[1]%7 ==0) {
				// wl, wr, bl, br
				bool[] castleOptions = new bool[] {castleWhiteLeft, castleWhiteRight, castleBlackLeft, castleBlackRight};
				if (castleOptions [2 * currentTurn + (dx + 2) / 4]) {

					if (StepMove (board, fromTile, ((dx + 2) / 4) * 7 - 4, 0)) {
						int[] prekingPos = FindKing (board, currentTurn);
						if (InCheck (board, currentTurn, prekingPos)) {
							return false;
						}
						int[,] boardClone = (int[,])board.Clone ();
						boardClone [fromTile [0] + dx / 2, fromTile [1]] = fromPieceIndex;
						boardClone [fromTile [0], fromTile [1]] = -1;
						int[] kingPos = FindKing (boardClone, currentTurn);
						return (!InCheck (boardClone, currentTurn, kingPos));

					}
				}
			}


		} else if (fromPieceIndex == 6 * currentTurn) {
			if (dx == 0 && board [toTile [0], toTile [1]] == -1) {
				if (dy == 1 - 2 * currentTurn || (dy == 2 - 4 * currentTurn && board [toTile [0], toTile [1] - dy / 2] == -1  && fromTile[1] == 1+5*currentTurn)) {
					if (dy * dy == 4) {
						passantTile = new int[] { fromTile [0], (fromTile [1] + toTile [1]) / 2 };
						passantTurn = 2;
					}
					return true;
				}
				// taking a piece
			} else if (dx * dx == 1 && dy == 1 - 2 * currentTurn && (board [toTile [0], toTile [1]] != -1 ||
				(toTile[0] == passantTile [0] && toTile[1] == passantTile[1] && passantTurn ==1 ))) {
				return true;
			}
		} 
		return false;

	}

	private bool StepMove(int[,] board, int[] fromTile, int dx, int dy){
		int steps;
		if (dx != 0) {
			steps = System.Math.Abs (dx); 
		} else { 
			steps = System.Math.Abs (dy);
		}

		for (int i = 1; i < steps; i++) {
			if (board [fromTile [0] + dx / steps * i , fromTile [1] + dy / steps * i] != -1) {
				return false;
			}
		}
		return true;


	}
}
