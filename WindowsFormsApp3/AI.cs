using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{

    public class AI
    {
        
        public bool             Color;
        public bool             Enemy;
        public String           time_needed;
        public List<Object[]>   cache;
        public String           time_needed2;
        public int              evaluatedPositions;
        public int              movesCount;
        public int              depth = 4;
        public float            percentFinished;
        public int              firstLayer; 


        protected Dictionary<string, int> dictionary =
            new Dictionary<string, int>();
        protected Dictionary<string, int[,]> Worths =
            new Dictionary<string, int[,]>();
        protected Dictionary<string, int> directWorths =
            new Dictionary<string, int>();
        protected Dictionary<string, int[]> Colorchanger =
           new Dictionary<string, int[]>();
        protected Dictionary<string, int> mult =
          new Dictionary<string, int>();
        protected Form1 myForm;

        //everything for multithreading
        List<Thread> Threads= new List<Thread>();
        object Threadslocker = new object();
        int[]  moveworths;
        object moveworthslocker = new object();

        Dictionary<String, List<UInt64[]>> transpTable = new Dictionary<String, List<UInt64[]>>();

        public AI(bool Color, Form1 form)//die zeilenreihenfolge muss einmal getauscht werden.
        {
            Worths.Add("king", new int[,]   {   {-30,-40,-40,-50,-50,-40,-40,-30},
                                                {-30,-40,-40,-50,-50,-40,-40,-30},
                                                {-30,-40,-40,-50,-50,-40,-40,-30},
                                                {-30,-40,-40,-50,-50,-40,-40,-30},
                                                {-20,-30,-30,-40,-40,-30,-30,-20},
                                                {-10,-20,-20,-20,-20,-20,-20,-10},
                                                { 20, 20,  0,  0,  0,  0, 20, 20},
                                                { 20, 30, 10,  0,  0, 10, 30, 20 }
                                        });
            Worths.Add("rook", new int[,]   {

                                             { 0,  0,  0,  0,  0,  0,  0,  0},
                                             { 5, 10, 10, 10, 10, 10, 10,  5},
                                             {-5,  0,  0,  0,  0,  0,  0, -5},
                                             {-5,  0,  0,  0,  0,  0,  0, -5},
                                             {-5,  0,  0,  0,  0,  0,  0, -5},
                                             {-5,  0,  0,  0,  0,  0,  0, -5},
                                             {-5,  0,  0,  0,  0,  0,  0, -5},
                                             { 0,  0,  0,  5,  5,  0,  0,  0}
                                        });
            Worths.Add("knight", new int[,]{ {-50,-40,-30,-30,-30,-30,-40,-50},
                                            {-40,-20,  0,  0,  0,  0,-20,-40},
                                            {-30,  0, 10, 15, 15, 10,  0,-30},
                                            {-30,  5, 15, 20, 20, 15,  5,-30},
                                            {-30,  0, 15, 20, 20, 15,  0,-30},
                                            {-30,  5, 10, 15, 15, 10,  5,-30},
                                            {-40,-20,  0,  5,  5,  0,-20,-40},
                                            {-50,-40,-30,-30,-30,-30,-40,-50},
                                        });
            Worths.Add("bishop", new int[,]{ {-20,-10,-10,-10,-10,-10,-10,-20},
                                            {-10,  0,  0,  0,  0,  0,  0,-10},
                                            {-10,  0,  5, 10, 10,  5,  0,-10},
                                            {-10,  5,  5, 10, 10,  5,  5,-10},
                                            {-10,  0, 10, 10, 10, 10,  0,-10},
                                            {-10, 10, 10, 10, 10, 10, 10,-10},
                                            {-10,  5,  0,  0,  0,  0,  5,-10},
                                            {-20,-10,-10,-10,-10,-10,-10,-20}
                                        });
            Worths.Add("queen", new int[,]{  {-20,-10,-10,- 5,- 5,-10,-10,-20,},
                                            {-10,  0,  0,  0,  0,  0,  0,-10,},
                                            {-10,  0,  5,  5,  5,  5,  0,-10,},
                                            {- 5,  0,  5,  5,  5,  5,  0,- 5,},
                                            {  0,  0,  5,  5,  5,  5,  0,  0,},
                                            {-10,  5,  5,  5,  5,  5,  5,-10,},
                                            {-10,  0,  0,  0,  0,  0,  0,-10,},
                                            {-20,-10,-10,- 5,- 5,-10,-10,-20 }
                                        });
            Worths.Add("pawn", new int[,]{ {  0,  0,  0,  0,  0,  0,  0,  0 },
                                           {50, 50, 50, 50, 50, 50, 50, 50  },
                                           {10, 10, 20, 30, 30, 20, 10, 10  },
                                           { 5,  5, 10, 25, 25, 10,  5,  5  },
                                           { 0,  0,  0, 20, 20,  0,  0,  0  },
                                           { 5, -5,-10,  0,  0,-10, -5,  5  },
                                           { 5, 10, 10,-20,-20, 10, 10,  5  },
                                           { 0,  0,  0,  0,  0,  0,  0,  0 }
                                        });
            directWorths.Add("King", 999999);
            directWorths.Add("Queen", 1000);
            directWorths.Add("Rook", 525);
            directWorths.Add("Knight", 350);
            directWorths.Add("Bishop", 350);
            directWorths.Add("Pawn", 100);
            Colorchanger.Add("True", new int[] { 7, 6, 5, 4, 3, 2, 1, 0 });
            Colorchanger.Add("False", new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            mult.Add("True", 1);
            mult.Add("False", -1);

            this.Color = Color;
            if (Color)
            {
                this.Enemy = false;
            }
            else
            {
                this.Enemy = true;
            }

            this.myForm = form;
        }

        public int evaluateBoard(Board brd)
        {
            int value = 0;
            UInt64 val = 0;
            UInt64 LSB = 0;


            foreach (KeyValuePair<bool, Dictionary<String,UInt64>> entry in brd.AllBitboards)
            {
                // do something with entry.Value or entry.Key
                //entrykey is either true or false
                //value is a dictionary Dictionary<String,UInt64>

                foreach (KeyValuePair<String, UInt64> entry2 in entry.Value)
                {
                    if (entry2.Key != "All")
                    {
                        LSB = LSB = BitHelper.LSB(entry2.Value);
                        val = entry2.Value;
                        while (LSB > 0)
                        {
                            value = value + (directWorths[entry2.Key] * mult[entry.Key.ToString()]);
                            val = val ^ LSB;
                            LSB = LSB = BitHelper.LSB(val);
                        }
                    }
                    
                }


            }
            return value;

        }


        public int evaluateBoardPlus(Move mv)
        {
            
            return 10 ;

        }
        public void clearTranspTable()
        {
            
        }
        public void call()
        {

            time_needed = null;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            evaluatedPositions = 0;
            UInt64[] bestMove = giveBestMove(depth+ increaseDepth(myForm.main_game.currentMove), myForm.main_game.currentMove);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            time_needed = String.Format("{0:00}:{1:00}.{2:00}",
             ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine(bestMove[0] + "|" + bestMove[1] + "||" +"Color:"+Color);Console.Read();
            if (bestMove[0] == 0 && bestMove[0] == bestMove[1])
            {
                throw new System.ArgumentException("got a null move", "original");
            }
            else
            {
                //Thread.Sleep(1000);
                Console.WriteLine("Doing move " + bestMove[0]+ bestMove[1]); Console.Read();
                this.myForm.main_game.make_move(bestMove[0], bestMove[1] );
                if (myForm.main_game.currentMove.promotion)
                {
                    Console.WriteLine("trying to promote. Color: "+Color);Console.Read();
                    myForm.main_game.make_promotion("queen");
                    
                }

            }
            myForm.Invoke(myForm.DelegateBoard);
            myForm.Invoke(myForm.DelegateTurn);
            clearTranspTable();

        }
        
        public virtual UInt64[] giveBestMove(int layers, Move move)
          {
           
            List<UInt64[]> possibleMove;
            if (transpTable.TryGetValue(move.toString(), out possibleMove))
            {
                // Key was in dictionary; 
            }
            else
            {
                possibleMove = Calculator.AllPossible(move,this.Color);
                transpTable.Add(move.toString(), possibleMove);
            }

            
          int i = 0;
            int best;
            if (Color)
            {
                best = -999999999;
            }
            else
            {
                best = 999999999;
            }
          
          Move newerMove;
          int besti = 0;
          movesCount = possibleMove.Count;
          firstLayer = movesCount;
          int ret;
            while (i < movesCount)
          {
              newerMove = move.MakeAMove( possibleMove[i][0]
                                        , possibleMove[i][1]
                                        , false);
            if(move.num >= 9
            && move.toString() == move.previousMove.previousMove.previousMove.previousMove.toString()
            && move.toString() == move.previousMove.previousMove.previousMove.previousMove.previousMove.previousMove.previousMove.previousMove.toString()
            )//der dritte move in folge ist gleich
                {
                ret = 0;//patt
                    Console.WriteLine("instant patt");Console.Read();
            }
            else
            {
                if (move.promotion)
                {
                    move.promote("queen");
                }

                ret = findBestMove(layers - 1, newerMove, -999999999, 999999999);
            }
            if (!Color)
            {
                if (ret < best)
                {
                    besti = i;
                    if (ret == -999999999)
                    {
                        return possibleMove[i];
                    }
                    best = ret;

                }
            }
            else
            {
                if (ret > best)
                {
                    besti = i;
                    if (ret == 999999999)
                    {
                        return possibleMove[i];
                    }
                    best = ret;
                }
            }
            move.reset();
            this.percentFinished = (float)i / movesCount *100;
            myForm.Invoke(myForm.DelegateCount);
          i++;
          }
            this.percentFinished = 100;
            myForm.Invoke(myForm.DelegateCount);
          if (i != 0)
          {
              return possibleMove[besti];
          }
          else
          {
              return new UInt64[] { 0, 0};
          }
        }

        protected int findBestMove(int layer, Move move, int alpha, int beta)
        {
            
            if (layer == 0)
            {
                evaluatedPositions++;
                return evaluateBoard(move.BoardAfter);
                //return evaluateBoardPlus(move);
            }
            else
            {
                Move newerMove;
                List<UInt64[]> possibleMove;
                int movecount;
                if (transpTable.TryGetValue(move.toString(), out possibleMove))
                {
                    // Key was in dictionary; "value" contains corresponding value
                }
                else
                {
                    possibleMove = Calculator.AllPossible(move,move.turn);
                    transpTable.Add(move.toString(), possibleMove);
                }
                movecount = possibleMove.Count;
                if (move.turn)//weiss
                {

                    for (var i = 0; i < movecount; i++)
                    {
                        newerMove = move.MakeAMove(possibleMove[i][0]
                                                  , possibleMove[i][1]
                                                  , false);
                        if (move.num >= 9
                         && move.toString() == move.previousMove.previousMove.previousMove.previousMove.toString()
                         && move.toString() == move.previousMove.previousMove.previousMove.previousMove.previousMove.previousMove.previousMove.previousMove.toString()
                         )//der dritte move in folge ist gleich
                        {
                            return 0;//patt
                        }
                        if (newerMove.promotion)
                        {
                            newerMove.promote("queen");
                        }

                        alpha = Math.Max(alpha, findBestMove(layer - 1, newerMove, alpha, beta));
                        move.reset();
                        if (beta < alpha)
                        {

                            break;
                        }
                    }
                    if (movecount == 0 )
                    {
                        if (move.AmIInCheck())
                        {
                            return -999999999;
                        }
                        else
                        {
                            return 0;
                        }
                        
                    }
                    return alpha;
                }
                else
                {
                    for (var i = 0; i < movecount; i++)
                    {
                        newerMove = move.MakeAMove(possibleMove[i][0]
                                                  , possibleMove[i][1]
                                                  , false);
                        beta = Math.Min(beta, findBestMove(layer - 1, newerMove, alpha, beta));
                        move.reset();
                        if (beta < alpha)
                        {
                            break;
                        }
                    }
                    if (movecount == 0)
                    {
                        if (move.AmIInCheck())
                        {
                            return 999999999;
                        }
                        else
                        {
                            return 0;
                        }

                    } 
                    return beta;
                }
            }
            
        }

        //giving the best move, but while mutlithreading
        private int[] giveBestMoveMult(int layers, Move move)
        {
            int[] scores = new int[] { 97, 92, 81, 60 };
            return scores;
            /*
            List<int[]> possibleMove = possibleMoves(move);
            int i = 0;
            int smallest = 999999999;
            Move newerMove;
            int besti = 0;
            movesCount = possibleMove.Count;
            moveworths = new int[possibleMove.Count];
            int ret;
            while (i < movesCount)
            {
                while (Threads.Count <= 4)
                {
                    newerMove = move.MakeAMove(possibleMove[i][0]
                                                  , possibleMove[i][1]
                                                  , possibleMove[i][2]
                                                  , possibleMove[i][3]
                                                  , false);
                    if (move.promotion)
                    {
                        move.promote("queen");
                    }
                    lock (Threadslocker)
                    {
                        var t = new Thread(() => threadHandler(layers - 1, newerMove, -999999999, 999999999, i));
                        t.Start();
                        Threads.Add(t);
                    }


                    move.reset();
                    i++;
                }


                //Console.WriteLine("alpha beta"+ret);


                myForm.Invoke(myForm.DelegateCount);
            }

            if (i != 0)
            {
                while (Threads.Count > 0)
                {

                }
                int bestk = 0;
                for (int k = 0; k < moveworths.Count(); k++)
                {
                    if (moveworths[k] < bestk)
                    {
                        bestk = moveworths[k];
                    }
                }
                return possibleMove[bestk]; ;

            }
            else
            {
                return new int[] { 0, 0, 0, 0 };
            }

        */
        }
        private void threadHandler(int layer, Move move, int alpha, int beta, int i)
        {
            int ret = findBestMoveMult(layer, move, alpha, beta);
            lock (moveworthslocker)
            {
                moveworths[i] = ret;
            }
            lock (Threadslocker)
            {
                Threads.Remove(Thread.CurrentThread);
            }
        }
        private int findBestMoveMult(int layer, Move move, int alpha, int beta)
        {
            return 1;/*
            if (layer == 0)
            {

                //evaluatedPositions++;
                return evaluateBoard(move.BoardAfter);
            }
            else
            {
                Move newerMove;
                List<int[]> possibleMove;
                int movecount;
                possibleMove = possibleMoves(move);
                movecount = possibleMove.Count;
                if (move.turn )//weiss
                {

                    for (var i = 0; i < movecount; i++)
                    {
                        newerMove = move.MakeAMove(possibleMove[i][0]
                                                  , possibleMove[i][1]
                                                  , possibleMove[i][2]
                                                  , possibleMove[i][3]
                                                  , false);
                        if (newerMove.promotion)
                        {
                            newerMove.promote("queen");
                        }

                        alpha = Math.Max(alpha, findBestMoveMult(layer - 1, newerMove, alpha, beta));
                        move.reset();
                        if (beta < alpha)
                        {

                            break;
                        }
                    }
                    if (movecount == 0)
                    {
                        if (move.AmIInCheck())
                        {
                            return -999999999;
                        }
                        else
                        {
                            return 0;
                        }

                    }
                    return alpha;
                }
                else
                {
                    for (var i = 0; i < movecount; i++)
                    {
                        newerMove = move.MakeAMove(possibleMove[i][0]
                                                  , possibleMove[i][1]
                                                  , possibleMove[i][2]
                                                  , possibleMove[i][3]
                                                  , false);
                        beta = Math.Min(beta, findBestMove(layer - 1, newerMove, alpha, beta));
                        move.reset();
                        if (beta < alpha)
                        {
                            break;
                        }
                    }
                    if (movecount == 0)
                    {
                        if (move.AmIInCheck())
                        {
                            return 999999999;
                        }
                        else
                        {
                            return 0;
                        }

                    }
                    return beta;
                }




            }
            */
        }

        
        public virtual void cleanup()
        {

        }

        public int increaseDepth(Move move)//the depth is increased, if the calculationtime is low
        {
            float ret  = 0;
            return 0;
            

        }

        
    }
}


