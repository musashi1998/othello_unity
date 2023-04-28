using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class controll : MonoBehaviour
{
    public Transform[] wps;
    public TextMeshProUGUI score1, score2, winnerText;
    public GameObject turnImage;
    
    public GameObject myBlackPrefab;
    public GameObject myWhitePrefab;
    public int turn = 1;
    public bool modeAi = false;
    public int[,] gameBoardState = new int[8, 8] { {0,0,0,0,0,0,0,0},
                                            {0,0,0,0,0,0,0,0},
                                            {0,0,0,0,0,0,0,0},
                                            {0,0,0,-1,1,0,0,0},
                                            {0,0,0,1,-1,0,0,0},
                                            {0,0,0,0,0,0,0,0},
                                            {0,0,0,0,0,0,0,0},
                                            {0,0,0,0,0,0,0,0}};
    // Start is called before the first frame update
    public int[,] AiStaticScores = new int[8, 8] {{100,-40,13,6,6,13,-40,100},
                                                    {-30,-18,0,-2,-2,0,-18,-30},
                                                    {10,-1,5,0,0,5,-1,10},
                                                    {4,-3,1,0,0,1,-3,4},
                                                    {4,-3,1,0,0,1,-3,4},
                                                    {10,-1,5,0,0,5,-1,10},
                                                    {-30,-18,0,-2,-2,0,-18,-30},
                                                    {100,-40,13,6,6,13,-40,100}};
    GameObject[,] gameBoardObjects = new GameObject[8, 8];
    void Start()
    {
        // gameBoardState[3,3] = 1;
        // gameBoardState[3,2] = 1;
        winnerText.text = "";
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (gameBoardState[i, j] == 1)
                {
                    gameBoardObjects[i, j] = Instantiate(myBlackPrefab, new Vector3(wps[(j * 8) + i].position.x + 32.5f, wps[(j * 8) + i].position.y + 32.5f, wps[(j * 8) + i - 1].position.z), Quaternion.identity);
                }
                if (gameBoardState[i, j] == -1)
                {
                    gameBoardObjects[i, j] = Instantiate(myWhitePrefab, new Vector3(wps[(j * 8) + i].position.x + 32.5f, wps[(j * 8) + i].position.y + 32.5f, wps[(j * 8) + i - 1].position.z), Quaternion.identity);
                }
            }
        }
        calcPossibleMove();

    }

    // Update is called once per frame
    void Update()
    {
        updateBoard();
        detectClick();
        evalBoardScores();
        if (modeAi && turn == -1)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    if (Random.Range(-400, 500) == gameBoardState[i, j] && gameBoardState[i, j] == -2)
                    {
                        gameBoardState[i, j] = turn;
                        clear2s();
                        correctBoard(i, j);
                        checkEnd();
                        turn = turn * -1;
                        calcPossibleMove();
                    }
                }
            }
            //minimax(-1, 6, System.Int32.MinValue, System.Int32.MaxValue, gameBoardState.Clone() as int[,]);

        }
    }
    bool gameEND(int[,] board)
    {
        int blackScore = 0, whiteScore = 0;
        foreach (int a in board)
        {

            if (a == 1)
                blackScore++;
            if (a == -1)
                whiteScore++;

        }
        if (whiteScore + blackScore == 64)
            return true;
        else
            return false;
    }
    bool gameNOMOVES(int[,] board)
    {
        foreach (int a in board)
        {

            if (a == 2 || a == -2)
                return false;


        }
        return true;

    }

    public int minimax(int player, int depth, int alpha, int beta, int[,] board)
    {
        Debug.Log("player:" + player);
        if (depth == 0)
        {
            return evaluate(board, player);
        }
        board = calcAiPossible(board, player);
        if (gameNOMOVES(board) && depth != 6)
        {
            return evaluate(board, player);
            /*if (player == -1)
            {
                return 64;
            }
            else
            {
                return -64;
            }*/
        }


        if (player == -1)
        {
            int topi = 0;
            int topj = 0;


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    if (board[i, j] == -2)
                    {
                        // Debug.Log("here");
                        board[i, j] = -1;
                        board = correctBoardAI(i, j, board);
                        board = clear2sAi(board);
                        int score = minimax(player * -1, depth - 1, alpha, beta, board);
                        //score += AiStaticScores[i, j];
                        if (score > alpha)
                        {
                            alpha = score;
                            topi = i;
                            topj = j;
                        }

                        if (alpha >= beta)
                            break;
                    }
                }
            }
            if (depth == 6)
            {
                Debug.Log("XXXX" + topi + "YYYY" + topj);
                clear2s();
                calcPossibleMove();
                if (gameBoardState[topi, topj] == -2)
                    gameBoardState[topi, topj] = -1;
                clear2s();
                updateBoard();
                correctBoard(topi, topj);
                checkEnd();
                turn = turn * -1;
                calcPossibleMove();

            }
            return alpha;
        }
        else if (player == 1)
        {


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == 2)
                    {
                        board[i, j] = 1;
                        board = correctBoardAI(i, j, board);
                        board = clear2sAi(board);

                        int score = minimax(player * -1, depth - 1, alpha, beta, board);
                        //score -= AiStaticScores[i, j];
                        if (score < beta)
                            beta = score;

                        if (alpha >= beta)
                            break;
                    }
                }
            }
            return beta;
        }

        return 0;
    }

    int[,] correctBoardAI(int i, int j, int[,] board)
    {
        board = correctUpAI(i, j, board);
        board = correctRightAI(i, j, board);
        board = correctDownAI(i, j, board);
        board = correctLeftAI(i, j, board);
        board = correctUpRightAI(i, j, board);
        board = correctDownRrightAI(i, j, board);
        board = correctDownLeftAI(i, j, board);
        board = correctUpLeftAI(i, j, board);

        return board;
    }
    int[,] correctUpAI(int i, int j, int[,] board)
    {
        int self = board[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (j != 0)
        {
            //  
            --j;

            if (board[i, j] == 0)
            {
                return board;
            }
            if (board[i, j] == self && seenOpposite)
            {
                while (y - j != 0)
                {
                    board[i, j] = self;
                    j++;
                }
                return board;
            }
            if (board[i, j] == 0 && seenOpposite)
            {
                return board;
            }
            if (board[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;
    }
    int[,] correctRightAI(int i, int j, int[,] board)
    {
        int self = board[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (i != 7)
        {
            ++i;
            if (board[i, j] == 0)
            {
                return board;
            }

            if (board[i, j] == self && seenOpposite)
            {
                while (i - x != 0)
                {
                    board[i, j] = self;
                    i--;
                }
                return board;
            }
            if (board[i, j] == 0 && seenOpposite)
            {
                return board;
            }
            if (board[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;
    }
    int[,] correctDownAI(int i, int j, int[,] board)
    {
        int self = board[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (j != 7)
        {
            //  
            ++j;
            if (board[i, j] == 0)
            {
                return board;
            }
            if (board[i, j] == self && seenOpposite)
            {
                while (j - y != 0)
                {
                    board[i, j] = self;
                    j--;
                }
                return board;
            }
            if (board[i, j] == 0 && seenOpposite)
            {
                return board;
            }
            if (board[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }

        }
        return board;
    }
    int[,] correctLeftAI(int i, int j, int[,] board)
    {
        int self = board[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (i != 0)
        {
            //  
            --i;
            if (board[i, j] == 0)
            {
                return board;
            }
            if (board[i, j] == self && seenOpposite)
            {
                while (x - i != 0)
                {
                    //  

                    board[i, j] = self;
                    i++;
                }
                return board;
            }
            if (board[i, j] == 0 && seenOpposite)
            {
                return board;
            }
            if (board[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }

        }
        return board;
    }
    int[,] correctUpRightAI(int i, int j, int[,] board)
    {
        int self = board[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (j != 0 && i != 7)
        {

            --j;
            ++i;
            if (board[i, j] == 0)
            {
                return board;
            }

            if (board[i, j] == self && seenOpposite)
            {
                while (y - j != 0 && i - x != 0)
                {

                    board[i, j] = self;
                    j++;
                    i--;
                }
                return board;
            }
            if (board[i, j] == 0 && seenOpposite)
            {
                return board;
            }
            if (board[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;
    }
    int[,] correctDownRrightAI(int i, int j, int[,] board)
    {
        int self = board[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (j != 7 && i != 7)
        {

            ++j;
            ++i;

            if (board[i, j] == 0)
            {
                return board;
            }
            if (board[i, j] == self && seenOpposite)
            {
                while (j - y != 0 && i - x != 0)
                {

                    board[i, j] = self;
                    j--;
                    i--;
                }
                return board;
            }
            if (board[i, j] == 0 && seenOpposite)
            {
                return board;
            }
            if (board[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;
    }
    int[,] correctDownLeftAI(int i, int j, int[,] board)
    {
        int self = board[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (j != 7 && i != 0)
        {

            --i;
            ++j;

            if (board[i, j] == 0)
            {
                return board;
            }
            if (board[i, j] == self && seenOpposite)
            {
                while (j - y != 0 && x - i != 0)
                {

                    board[i, j] = self;
                    j--;
                    i++;
                }
                return board;
            }
            if (board[i, j] == 0 && seenOpposite)
            {
                return board;
            }
            if (board[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;
    }
    int[,] correctUpLeftAI(int i, int j, int[,] board)
    {
        int self = board[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (j != 0 && i != 0)
        {
            --j;
            --i;
            if (board[i, j] == 0)
            {
                return board;
            }

            if (board[i, j] == self && seenOpposite)
            {
                while (y - j != 0 && x - i != 0)
                {

                    board[i, j] = self;
                    j++;
                    i++;
                }
                return board;
            }
            if (board[i, j] == 0 && seenOpposite)
            {
                return board;
            }
            if (board[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;
    }

    int[,] calcAiPossible(int[,] board, int player)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == player)
                {
                    board = lookUpAI(i, j, board);
                    board = lookRightAI(i, j, board);
                    board = lookDownAI(i, j, board);
                    board = lookLeftAI(i, j, board);
                    board = lookUpRightAI(i, j, board);
                    board = lookDownRrightAI(i, j, board);
                    board = lookDownLeftAI(i, j, board);
                    board = lookUpLeftAI(i, j, board);
                }
            }
        }

        return board;
    }
    int[,] lookUpAI(int i, int j, int[,] board)
    {

        bool seenOpposite = false;
        while (j != 0)
        {
            --j;
            if (board[i, j] == turn)
            {
                return board;
            }

            if (board[i, j] == 0 && seenOpposite)
            {
                board[i, j] = 2 * turn;
                return board;
            }
            if (board[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;

    }
    int[,] lookRightAI(int i, int j, int[,] board)
    {

        bool seenOpposite = false;
        while (i != 7)
        {
            ++i;
            if (board[i, j] == turn)
            {
                return board;
            }

            if (board[i, j] == 0 && seenOpposite)
            {
                board[i, j] = 2 * turn;
                return board;
            }
            if (board[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;
    }
    int[,] lookDownAI(int i, int j, int[,] board)
    {

        bool seenOpposite = false;
        while (j != 7)
        {
            ++j;
            if (board[i, j] == turn)
            {
                return board;
            }

            if (board[i, j] == 0 && seenOpposite)
            {
                board[i, j] = 2 * turn;
                return board;
            }
            if (board[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;

    }
    int[,] lookLeftAI(int i, int j, int[,] board)
    {

        bool seenOpposite = false;
        while (i != 0)
        {
            --i;
            if (board[i, j] == turn)
            {
                return board;
            }

            if (board[i, j] == 0 && seenOpposite)
            {
                board[i, j] = 2 * turn;
                return board;
            }
            if (board[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;

    }
    int[,] lookUpRightAI(int i, int j, int[,] board)
    {

        bool seenOpposite = false;
        while (j != 0 && i != 7)
        {
            --j;
            ++i;
            if (board[i, j] == turn)
            {
                return board;
            }

            if (board[i, j] == 0 && seenOpposite)
            {
                board[i, j] = 2 * turn;
                return board;
            }
            if (board[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;
    }
    int[,] lookDownRrightAI(int i, int j, int[,] board)
    {

        bool seenOpposite = false;
        while (j != 7 && i != 7)
        {
            // Debug.Log(i+ " "+ j );
            ++j;
            ++i;
            if (board[i, j] == turn)
            {
                return board;
            }

            if (board[i, j] == 0 && seenOpposite)
            {
                board[i, j] = 2 * turn;
                return board;
            }
            if (board[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;

    }
    int[,] lookDownLeftAI(int i, int j, int[,] board)
    {

        bool seenOpposite = false;
        while (i != 0 && j != 7)
        {
            ++j;
            --i;
            if (board[i, j] == turn)
            {
                return board;
            }

            if (board[i, j] == 0 && seenOpposite)
            {
                board[i, j] = 2 * turn;
                return board;
            }
            if (board[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;

    }
    int[,] lookUpLeftAI(int i, int j, int[,] board)
    {

        bool seenOpposite = false;
        while (j != 0 && i != 0)
        {

            --j;
            --i;

            // Debug.Log(i + " " + j + " " + gameBoardState[2, 2] + " " + gameBoardState[1, 1] + " " + gameBoardState[0, 0]);
            if (board[i, j] == turn)
            {
                return board;
            }

            if (board[i, j] == 0 && seenOpposite)
            {
                board[i, j] = 2 * turn;
                return board;
            }
            if (board[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return board;

    }


    void movesSize(int t)
    {
        // foreach (){}
    }
    int evaluate(int[,] board, int player)
    {
        int blackScore = 0, whiteScore = 0, empty = 0, blackScoreAI = 0, whiteScoreAI = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i,j] == 0)
                empty++;
            if (board[i,j] == 1)
            {
                blackScore++;
                blackScoreAI+=AiStaticScores[i,j];
            }
            if (board[i,j] == -1)
            {
                whiteScore++;
                whiteScoreAI+=AiStaticScores[i,j];
            }

            }
            
        }
            

        
        if (empty > 0)
            return whiteScoreAI - blackScoreAI;
        else
             return whiteScore - blackScore;

    }
    void detectClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if (hit != null)
            {
                //gameBoardObjects[4,4].GetComponent<Animator>().SetInteger("AnimState",1);
                //Debug.Log("Hit Collider: " + hit.transform.name);

                if (hit.transform.name == "wp")
                {
                    if (gameBoardState[0, 0] == 2 && turn == 1)
                    {
                        gameBoardState[0, 0] = turn;
                        clear2s();
                        correctBoard(0, 0);
                        checkEnd();
                        turn = turn * -1;
                        calcPossibleMove();
                    }
                    else if (gameBoardState[0, 0] == -2 && turn == -1 && !modeAi)
                    {
                        gameBoardState[0, 0] = turn;
                        clear2s();
                        correctBoard(0, 0);
                        checkEnd();
                        turn = turn * -1;
                        calcPossibleMove();
                    }
                }
                for (int i = 1; i <= 63; i++)
                {
                    if (hit.transform.name == "wp (" + i.ToString() + ")")
                    {
                        if (gameBoardState[i % 8, i / 8] == 2 && turn == 1)
                        {
                            gameBoardState[i % 8, i / 8] = turn;
                            clear2s();
                            correctBoard(i % 8, i / 8);
                            turn = turn * -1;
                            calcPossibleMove();
                        }
                        else if (gameBoardState[i % 8, i / 8] == -2 && turn == -1 && !modeAi)
                        {
                            gameBoardState[i % 8, i / 8] = turn;
                            clear2s();
                            correctBoard(i % 8, i / 8);
                            turn = turn * -1;
                            calcPossibleMove();
                        }
                    }
                }

            }
            else
            {
                //Debug.Log("No colliders hit from mouse click");
            }
        }
    }


    void checkEnd()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (gameBoardState[i, j] == 2 || gameBoardState[i, j] == -2 || gameBoardState[i, j] == 0)
                    return;
                //declare winner
                int blackScore = 0, whiteScore = 0;
                foreach (int a in gameBoardState)
                {
                    if (a == 1)
                        blackScore++;
                    if (a == -1)
                        whiteScore++;
                }
                if (blackScore > whiteScore)
                {
                    winnerText.text = "BLACK WINS";
                }
                else if (blackScore < whiteScore)
                {
                    winnerText.text = "WHITE WINS";
                }
                else
                {
                    winnerText.text = "TIE";
                }
            }
        }
    }
    int[,] clear2sAi(int[,] board)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == 2 || board[i, j] == -2)
                    board[i, j] = 0;

            }
        }
        return board;
    }
    void clear2s()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (gameBoardState[i, j] == 2 || gameBoardState[i, j] == -2)
                    gameBoardState[i, j] = 0;

            }
        }
    }
    void correctBoard(int i, int j)
    {
        correctUp(i, j);
        correctRight(i, j);
        correctDown(i, j);
        correctLeft(i, j);
        correctUpRight(i, j);
        correctDownRright(i, j);
        correctDownLeft(i, j);
        correctUpLeft(i, j);
    }
    void correctUp(int i, int j)
    {
        int self = gameBoardState[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (j != 0)
        {
            //  
            --j;

            if (gameBoardState[i, j] == 0)
            {
                return;
            }
            if (gameBoardState[i, j] == self && seenOpposite)
            {
                while (y - j != 0)
                {
                    //  
                    if (gameBoardObjects[i, j] == null)
                        return;
                    gameBoardObjects[i, j].GetComponent<Animator>().SetInteger("AnimState", self);
                    gameBoardState[i, j] = self;
                    j++;
                }
                return;
            }
            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                return;
            }
            if (gameBoardState[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;
    }
    void correctRight(int i, int j)
    {
        int self = gameBoardState[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (i != 7)
        {
            ++i;
            if (gameBoardState[i, j] == 0)
            {
                return;
            }

            if (gameBoardState[i, j] == self && seenOpposite)
            {
                while (i - x != 0)
                {

                    if (gameBoardObjects[i, j] == null)
                        return;
                    gameBoardObjects[i, j].GetComponent<Animator>().SetInteger("AnimState", self);
                    gameBoardState[i, j] = self;
                    i--;
                }
                return;
            }
            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                return;
            }
            if (gameBoardState[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;
    }
    void correctDown(int i, int j)
    {
        int self = gameBoardState[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (j != 7)
        {
            //  
            ++j;
            if (gameBoardState[i, j] == 0)
            {
                return;
            }
            if (gameBoardState[i, j] == self && seenOpposite)
            {
                while (j - y != 0)
                {
                    //  
                    if (gameBoardObjects[i, j] == null)
                        return;
                    gameBoardObjects[i, j].GetComponent<Animator>().SetInteger("AnimState", self);
                    gameBoardState[i, j] = self;
                    j--;
                }
                return;
            }
            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                return;
            }
            if (gameBoardState[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }

        }
        return;
    }
    void correctLeft(int i, int j)
    {
        int self = gameBoardState[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (i != 0)
        {
            //  
            --i;
            if (gameBoardState[i, j] == 0)
            {
                return;
            }
            if (gameBoardState[i, j] == self && seenOpposite)
            {
                while (x - i != 0)
                {
                    //  
                    if (gameBoardObjects[i, j] == null)
                        return;
                    gameBoardObjects[i, j].GetComponent<Animator>().SetInteger("AnimState", self);
                    gameBoardState[i, j] = self;
                    i++;
                }
                return;
            }
            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                return;
            }
            if (gameBoardState[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }

        }
        return;
    }
    void correctUpRight(int i, int j)
    {
        int self = gameBoardState[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (j != 0 && i != 7)
        {

            --j;
            ++i;
            if (gameBoardState[i, j] == 0)
            {
                return;
            }

            if (gameBoardState[i, j] == self && seenOpposite)
            {
                while (y - j != 0 && i - x != 0)
                {
                    //  
                    if (gameBoardObjects[i, j] == null)
                        return;
                    gameBoardObjects[i, j].GetComponent<Animator>().SetInteger("AnimState", self);
                    gameBoardState[i, j] = self;
                    j++;
                    i--;
                }
                return;
            }
            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                return;
            }
            if (gameBoardState[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;
    }
    void correctDownRright(int i, int j)
    {
        int self = gameBoardState[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (j != 7 && i != 7)
        {

            ++j;
            ++i;

            if (gameBoardState[i, j] == 0)
            {
                return;
            }
            if (gameBoardState[i, j] == self && seenOpposite)
            {
                while (j - y != 0 && i - x != 0)
                {
                    if (gameBoardObjects[i, j] == null)
                        return;
                    gameBoardObjects[i, j].GetComponent<Animator>().SetInteger("AnimState", self);
                    gameBoardState[i, j] = self;
                    j--;
                    i--;
                }
                return;
            }
            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                return;
            }
            if (gameBoardState[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;
    }
    void correctDownLeft(int i, int j)
    {
        int self = gameBoardState[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (j != 7 && i != 0)
        {

            --i;
            ++j;

            if (gameBoardState[i, j] == 0)
            {
                return;
            }
            if (gameBoardState[i, j] == self && seenOpposite)
            {
                while (j - y != 0 && x - i != 0)
                {
                    if (gameBoardObjects[i, j] == null)
                        return;
                    gameBoardObjects[i, j].GetComponent<Animator>().SetInteger("AnimState", self);
                    gameBoardState[i, j] = self;
                    j--;
                    i++;
                }
                return;
            }
            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                return;
            }
            if (gameBoardState[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;
    }
    void correctUpLeft(int i, int j)
    {
        int self = gameBoardState[i, j];
        bool seenOpposite = false;
        int x = i;
        int y = j;
        while (j != 0 && i != 0)
        {
            --j;
            --i;
            if (gameBoardState[i, j] == 0)
            {
                return;
            }

            if (gameBoardState[i, j] == self && seenOpposite)
            {
                while (y - j != 0 && x - i != 0)
                {
                    if (gameBoardObjects[i, j] == null)
                        return;
                    gameBoardObjects[i, j].GetComponent<Animator>().SetInteger("AnimState", self);
                    gameBoardState[i, j] = self;
                    j++;
                    i++;
                }
                return;
            }
            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                return;
            }
            if (gameBoardState[i, j] == -1 * self && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;
    }
    void updateBoard()
    {
        if (turn == 1)
        {
            turnImage.SetActive(false);
        }
        else
        {
            turnImage.SetActive(true);
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (gameBoardState[i, j] == 1 && gameBoardObjects[i, j] == null)
                {
                    // Debug.Log(i+" "+j + " "+ (j * 8) + i);
                    gameBoardObjects[i, j] = Instantiate(myBlackPrefab, new Vector3(wps[(j * 8) + i].position.x + 32.5f, wps[(j * 8) + i].position.y + 32.5f, wps[(j * 8) + i].position.z), Quaternion.identity);

                }
                if (gameBoardState[i, j] == -1 && gameBoardObjects[i, j] == null)
                {
                    gameBoardObjects[i, j] = Instantiate(myWhitePrefab, new Vector3(wps[(j * 8) + i].position.x + 32.5f, wps[(j * 8) + i].position.y + 32.5f, wps[(j * 8) + i].position.z), Quaternion.identity);
                }
            }
        }
    }
    void calcPossibleMove()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (gameBoardState[i, j] == turn)
                {
                    lookUp(i, j);
                    lookRight(i, j);
                    lookDown(i, j);
                    lookLeft(i, j);
                    lookUpRight(i, j);
                    lookDownRright(i, j);
                    lookDownLeft(i, j);
                    lookUpLeft(i, j);
                }
            }
        }
    }
    void lookUp(int i, int j)
    {

        bool seenOpposite = false;
        while (j != 0)
        {
            --j;
            if (gameBoardState[i, j] == turn)
            {
                return;
            }

            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                gameBoardState[i, j] = 2 * turn;
                return;
            }
            if (gameBoardState[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;

    }
    void lookRight(int i, int j)
    {

        bool seenOpposite = false;
        while (i != 7)
        {
            ++i;
            if (gameBoardState[i, j] == turn)
            {
                return;
            }

            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                gameBoardState[i, j] = 2 * turn;
                return;
            }
            if (gameBoardState[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;

    }
    void lookDown(int i, int j)
    {

        bool seenOpposite = false;
        while (j != 7)
        {
            ++j;
            if (gameBoardState[i, j] == turn)
            {
                return;
            }

            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                gameBoardState[i, j] = 2 * turn;
                return;
            }
            if (gameBoardState[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;

    }
    void lookLeft(int i, int j)
    {

        bool seenOpposite = false;
        while (i != 0)
        {
            --i;
            if (gameBoardState[i, j] == turn)
            {
                return;
            }

            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                gameBoardState[i, j] = 2 * turn;
                return;
            }
            if (gameBoardState[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;

    }
    void lookUpRight(int i, int j)
    {

        bool seenOpposite = false;
        while (j != 0 && i != 7)
        {
            --j;
            ++i;
            if (gameBoardState[i, j] == turn)
            {
                return;
            }

            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                gameBoardState[i, j] = 2 * turn;
                return;
            }
            if (gameBoardState[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;
    }
    void lookDownRright(int i, int j)
    {

        bool seenOpposite = false;
        while (j != 7 && i != 7)
        {
            // Debug.Log(i+ " "+ j );
            ++j;
            ++i;
            if (gameBoardState[i, j] == turn)
            {
                return;
            }

            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                gameBoardState[i, j] = 2 * turn;
                return;
            }
            if (gameBoardState[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;
    }
    void lookDownLeft(int i, int j)
    {

        bool seenOpposite = false;
        while (i != 0 && j != 7)
        {
            ++j;
            --i;
            if (gameBoardState[i, j] == turn)
            {
                return;
            }

            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                gameBoardState[i, j] = 2 * turn;
                return;
            }
            if (gameBoardState[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;

    }
    void lookUpLeft(int i, int j)
    {

        bool seenOpposite = false;
        while (j != 0 && i != 0)
        {

            --j;
            --i;

            // Debug.Log(i + " " + j + " " + gameBoardState[2, 2] + " " + gameBoardState[1, 1] + " " + gameBoardState[0, 0]);
            if (gameBoardState[i, j] == turn)
            {
                return;
            }

            if (gameBoardState[i, j] == 0 && seenOpposite)
            {
                gameBoardState[i, j] = 2 * turn;
                return;
            }
            if (gameBoardState[i, j] == -1 * turn && !seenOpposite)
            {
                seenOpposite = true;
            }
        }
        return;

    }

    void evalBoardScores()
    {
        int blackScore = 0, whiteScore = 0;
        foreach (int a in gameBoardState)
        {
            if (a == 1)
                blackScore++;
            if (a == -1)
                whiteScore++;
        }
        // var[] txtz;
        // txtz.GetComponents<TextMeshPro>();
        score1.text = "WHITE:" + whiteScore.ToString();
        score2.text = "BLACK:" + blackScore.ToString();

    }
}
