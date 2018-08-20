using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.InteropServices;

namespace WindowsFormsApp3
{
    public class Game
    {
        //public String[,] chessboard;
        public Board chessboard;
        public String[,] chessboard_human= {{ "A1", "white_pawn", "", "", "", "", "black_pawn", "black_rook" },    //x=1
                             { "B1","white_pawn","","","","","black_pawn","black_knight"},  //x=2
                             { "C1","white_pawn","","","","","black_pawn","black_bishop"},  //x=3
                             { "D1" ,"white_pawn","","","","","black_pawn","black_queen"},   //x=4
                             { "E1"  ,"white_pawn","","","","","black_pawn","black_king"},    //x=5
                             { "F1","white_pawn","","","","","black_pawn","black_bishop"},  //x=6
                             { "G1","white_pawn","","","","","black_pawn","black_knight"},  //x=7
                             { "H1"  ,"white_pawn","","","","","black_pawn","black_rook"},    //x=8
                                };
        
        public String state;
        public int time_left_for_turn = 1000;
        public String moveState;
        public String movedenial;
        public Move currentMove;
        public string winner;
        public String type;

        public Game()
        {
            init();
        }
        void init()
        {
            state = "running";
            this.chessboard=new Board();
            chessboard.init();
            currentMove = new Move(this.chessboard);
        }
        
        
        public void make_move(UInt64 from, UInt64 to)
        {
            Console.WriteLine("game.make_move");
            currentMove = currentMove.MakeAMove(from, to, true);
            if (currentMove.num >= 9
            && currentMove.toString() == currentMove.previousMove.previousMove.previousMove.previousMove.toString()
            && currentMove.toString() == currentMove.previousMove.previousMove.previousMove.previousMove.previousMove.previousMove.previousMove.previousMove.toString()
            )
            {
                winner = "no";
            }
            
            if (!currentMove.promotion)
            {
                afterMove();
            }
            
            

        }
        public void make_promotion(String wish)
        {
            currentMove = currentMove.promote(wish);
            afterMove();
        }
        public void afterMove()
        {

            if (currentMove.Checkmate())
            {
                state = "over";
                if (currentMove.turn)
                {
                    winner = "black";
                }
                else
                {
                    winner = "white";
                }
            }
            
        }

    }
}

