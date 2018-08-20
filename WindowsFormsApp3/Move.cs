using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace WindowsFormsApp3
{
    public class Move
    {
        public Move             previousMove;
        public Move             nextMove;
        public int              num;
        public Board            BoardAfter;

        /**Alle Parameter, die für diesen Zug zählen**/
        public Board BoardBefore;
        

        public bool WKingSideCastleAvailable;
        public bool WQueenSideCastleAvailable;
        public bool BKingSideCastleAvailable;
        public bool BQueenSideCastleAvailable;

        public bool promotion;
        public UInt64 promotionBoard;

        public UInt64 from;
        public UInt64 to;
        public Dictionary <bool,UInt64> enPassentSquare= new Dictionary<bool, UInt64> { {true,0 },{false,0 } };

        public bool turn;
        Dictionary<bool, char> bools = new Dictionary<bool, char>()
            {
            { false, '0'},
            { true , '1'},
            };


        public Move(Move lastMove)
        {

            //do the move
            init(lastMove);



        }
        public void init(Move lastMove)
        {
            this.previousMove = lastMove;
            BoardBefore = previousMove.BoardAfter.copy();
            BoardAfter = BoardBefore.copy();
            
            turn = !previousMove.turn;
           
           
            /*performance  die ermittlung der könig kann man eig mit bezug auf vorigen zug machen. dann kann man sich eine iteration sparen*/

            
            nextMove = null;
            num = previousMove.num + 1;
            if ((previousMove.from & (previousMove.BoardBefore.AllBitboards[!turn]["Pawn"])) > 0 && (previousMove.from << 16 == previousMove.to || previousMove.from >> 16 == previousMove.to))
            {
                if (previousMove.turn)
                {
                    this.enPassentSquare[turn] = previousMove.from << 8;
                }
                else
                {
                    this.enPassentSquare[turn] = previousMove.from >> 8;
                }
            }
            WKingSideCastleAvailable    = previousMove.WKingSideCastleAvailable;
            WQueenSideCastleAvailable   = previousMove.WQueenSideCastleAvailable;
            BKingSideCastleAvailable    = previousMove.BKingSideCastleAvailable;
            BQueenSideCastleAvailable   = previousMove.BQueenSideCastleAvailable;
            promotion = false;
            /*promotionX = -1;
            promotionY = -1;
            fromx = -1;
            fromy = -1;
            tox = -1;
            toy = -1;*/

        }
        public Move(Board chessboard)
        { 
            BoardBefore = chessboard.copy();
            BoardAfter = chessboard.copy();

            previousMove = new Move(this);
            previousMove.WKingSideCastleAvailable    =true;
            previousMove.WQueenSideCastleAvailable   =true;
            previousMove.BKingSideCastleAvailable    =true;
            previousMove.BQueenSideCastleAvailable   =true;
            previousMove.turn = false;
            previousMove.num = 0;
            previousMove.BoardBefore = chessboard.copy();
            previousMove.BoardAfter = chessboard.copy();
            previousMove.nextMove = this;

            


            init(previousMove);
            
            //do the move




        }

        public Move MakeAMove(UInt64 from, UInt64 to,Boolean CheckIfLegal)
        {
            bool moveSucceeded=false;
            if (CheckIfLegal)
            {
                if( ((this.BoardAfter.AllBitboards[this.turn]["All"]&from)!=0)&( Calculator.AllPossibleForPiece(this, from, this.turn) & to) != 0)
                {
                   
                    moveSucceeded = true;
                }
            }
            else//wenn nicht überprüft werden soll ob der move legal ist
            {
                //einfach bewegen
                
                moveSucceeded = true;
            }

            
            if (moveSucceeded)
            {
                if ((from & this.BoardBefore.AllBitboards[turn]["King"]) > 0 )
                {
                    if (turn)
                    {
                        if ((to << 2 == from))
                        {
                            this.BoardAfter.repositionPiece(0B0000000000000000000000000000000000000000000000000000000000000001, 0B0000000000000000000000000000000000000000000000000000000000001000); 
                        }
                        else if (to >> 2 == from)
                        {
                            this.BoardAfter.repositionPiece(0B0000000000000000000000000000000000000000000000000000000010000001, 0B0000000000000000000000000000000000000000000000000000000000100000);
                        }
                    }
                    else
                    {
                        if ((to << 2 == from))
                        {
                            this.BoardAfter.repositionPiece(0B0000000100000000000000000000000000000000000000000000000000000000, 0B0000010000000000000000000000000000000000000000000000000000000000);
                        }
                        else if (to >> 2 == from)
                        {
                            
                            this.BoardAfter.repositionPiece(0B1000000000000000000000000000000000000000000000000000000000000000, 0B00010000000000000000000000000000000000000000000000000000000000000);
                        }
                    }
                    
                }

                this.BoardAfter.repositionPiece(from, to);


                this.from = from;
                this.to = to;
                if (this.promotion)
                {
                    Console.WriteLine("promoter!");
                    return this;
                }
                else
                {
                    nextMove = new Move(this);
                    nextMove.WKingSideCastleAvailable   = this.WKingSideCastleAvailable;
                    nextMove.WQueenSideCastleAvailable  = this.WQueenSideCastleAvailable;
                    nextMove.BKingSideCastleAvailable   = this.BKingSideCastleAvailable;
                    nextMove.BQueenSideCastleAvailable  = this.BQueenSideCastleAvailable;
                    return nextMove;
                }

            }
            else
            {
                init(this.previousMove);
                //wenn move nicht gültig war
                Console.WriteLine("unallowed move stopped");Console.Read();
                return this;
            }
            
        }
        public void reset()
        {
            init(this.previousMove);
        }
        public Move promote(String wishedPiece)
        {
            
            if (wishedPiece == "queen")
            {
                
            }
            else if (wishedPiece == "knight")
            {
                
            }
            else if (wishedPiece == "rook")
            {
                
            }
            else if (wishedPiece == "bishop")
            {
               
            }


            nextMove = new Move(this);
            nextMove.WKingSideCastleAvailable   = this.WKingSideCastleAvailable;
            nextMove.WQueenSideCastleAvailable  = this.WQueenSideCastleAvailable;
            nextMove.BKingSideCastleAvailable   = this.BKingSideCastleAvailable;
            nextMove.BQueenSideCastleAvailable  = this.BQueenSideCastleAvailable;
            return nextMove;
        }
        public bool Checkmate()
        {
            //am i in checkmate
           
            
            return false;
        }
        public bool AmIInCheck()
        {
            if ((Calculator.AllPseudoPossibleEnemy(this) & this.BoardBefore.AllBitboards[this.turn]["King"]) != 0)
            {
                return true;
            }
            return false;
        }

        public bool EnemyInCheck()
        {
            UInt64 ret = 0;
            UInt64 LSB = 0;
            Move currentMove = this;
            UInt64 Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["King"];
            if ((Calculator.AllPseudoPossibleKing(currentMove, LSB, currentMove.turn) & this.BoardBefore.AllBitboards[!this.turn]["King"]) > 0)
            {
                return true;
            }
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Pawn"];
            if ((Calculator.AllPseudoPossiblePawn(currentMove, LSB, currentMove.turn) & this.BoardBefore.AllBitboards[!this.turn]["King"]) > 0)
            {
                return true;
            }
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Knight"];
            if ((Calculator.AllPseudoPossibleKnight(currentMove, LSB, currentMove.turn) & this.BoardBefore.AllBitboards[!this.turn]["King"]) > 0)
            {
                return true;
            }
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Rook"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if ((Calculator.AllPseudoPossibleRook(currentMove, LSB, currentMove.turn) & this.BoardBefore.AllBitboards[!this.turn]["King"]) > 0)
                {
                    return true;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Bishop"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if ((Calculator.AllPseudoPossibleBishop(currentMove, LSB, currentMove.turn) & this.BoardBefore.AllBitboards[!this.turn]["King"]) > 0)
                {
                    return true;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Queen"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if ((Calculator.AllPseudoPossibleQueen(currentMove, LSB, currentMove.turn) & this.BoardBefore.AllBitboards[!this.turn]["King"]) > 0)
                {
                    return true;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            return false;
        }
        public string toString()
        {
            return ""    + bools[this.WKingSideCastleAvailable]
                         + bools[this.WQueenSideCastleAvailable]
                         + bools[this.BKingSideCastleAvailable]
                         + bools[this.BQueenSideCastleAvailable]
                         + this.BoardBefore.toString()
                         + bools[this.turn];
        }

        public UInt64 checkers()
        {
            //allppl that check the king
            Move currentMove = this;
            UInt64 ret = 0; 
            UInt64 LSB = 0;
            UInt64 Pieces = currentMove.BoardBefore.AllBitboards[!currentMove.turn]["Rook"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if( (Calculator.AllPseudoPossibleRook(currentMove, LSB, !currentMove.turn) & currentMove.BoardBefore.AllBitboards[currentMove.turn]["King"]) > 0)
                {
                    ret = ret | LSB;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[!currentMove.turn]["Bishop"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if ((Calculator.AllPseudoPossibleBishop(currentMove, LSB, !currentMove.turn) & currentMove.BoardBefore.AllBitboards[currentMove.turn]["King"]) > 0)
                {
                    ret = ret | LSB;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[!currentMove.turn]["Queen"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if ((Calculator.AllPseudoPossibleQueen(currentMove, LSB, !currentMove.turn) & currentMove.BoardBefore.AllBitboards[currentMove.turn]["King"]) > 0)
                {
                    ret = ret | LSB;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[!currentMove.turn]["King"];
            LSB = Pieces;
            if ((Calculator.AllPseudoPossibleKing(currentMove, LSB, !currentMove.turn) & currentMove.BoardBefore.AllBitboards[currentMove.turn]["King"]) > 0)
            {
                ret = ret | LSB;
            }
            Pieces = Pieces ^ LSB;
            LSB = LSB = BitHelper.LSB(Pieces);
            
            Pieces = currentMove.BoardBefore.AllBitboards[!currentMove.turn]["Pawn"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if ((Calculator.AllPseudoPossiblePawn(currentMove, LSB, !currentMove.turn) & currentMove.BoardBefore.AllBitboards[currentMove.turn]["King"]) > 0)
                {
                    ret = ret | LSB;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[!currentMove.turn]["Knight"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if ((Calculator.AllPseudoPossibleKnight(currentMove, LSB, !currentMove.turn) & currentMove.BoardBefore.AllBitboards[currentMove.turn]["King"]) > 0)
                {
                    ret = ret | LSB;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            return ret;
        }

        public UInt64 Enemycheckers()
        {
            Move currentMove = this;
            UInt64 ret = 0;
            UInt64 LSB = 0;
            UInt64 Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Rook"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if ((Calculator.AllPseudoPossibleRook(currentMove, LSB, currentMove.turn) & currentMove.BoardBefore.AllBitboards[!currentMove.turn]["King"]) > 0)
                {
                    ret = ret | LSB;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Bishop"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if ((Calculator.AllPseudoPossibleBishop(currentMove, LSB, currentMove.turn) & currentMove.BoardBefore.AllBitboards[!currentMove.turn]["King"]) > 0)
                {
                    ret = ret | LSB;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Queen"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if ((Calculator.AllPseudoPossibleQueen(currentMove, LSB, currentMove.turn) & currentMove.BoardBefore.AllBitboards[!currentMove.turn]["King"]) > 0)
                {
                    ret = ret | LSB;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["King"];
            LSB = Pieces;
            if ((Calculator.AllPseudoPossibleKing(currentMove, LSB, currentMove.turn) & currentMove.BoardBefore.AllBitboards[!currentMove.turn]["King"]) > 0)
            {
                ret = ret | LSB;
            }
            Pieces = Pieces ^ LSB;
            LSB = LSB = BitHelper.LSB(Pieces);

            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Pawn"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if ((Calculator.AllPseudoPossiblePawn(currentMove, LSB, currentMove.turn) & currentMove.BoardBefore.AllBitboards[!currentMove.turn]["King"]) > 0)
                {
                    ret = ret | LSB;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Knight"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                if ((Calculator.AllPseudoPossibleKnight(currentMove, LSB, currentMove.turn) & currentMove.BoardBefore.AllBitboards[!currentMove.turn]["King"]) > 0)
                {
                    ret = ret | LSB;
                }
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            return ret;
        }
    }
}
