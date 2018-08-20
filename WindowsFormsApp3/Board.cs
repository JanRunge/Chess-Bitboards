using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{

    public struct Board
    {
         
        public ulong  WhiteKing;
        public ulong  WhiteQueens;
        public ulong  WhiteRooks;
        public ulong  WhiteBishops;
        public ulong  WhiteKnights;
        public ulong  WhitePawns;
        public ulong  WhitePieces;

        public ulong BlackKing;
        public ulong BlackQueens;
        public ulong BlackRooks;
        public ulong BlackBishops;
        public ulong BlackKnights;
        public ulong BlackPawns;
        public ulong BlackPieces;

        public ulong AllPieces;
        public Dictionary<Boolean, Dictionary<String, ulong>> AllBitboards; //nicht fertig
        
        



        public void init()
        {

            //präfix 0b oder 0B zum Kennzeichnen eines binären Literals.
            /*
             NORMAL BOARD CONSTELLATION
             
             */
            //---------------1       1       1       1       1       1       1       1
            //---------------1234567812345678123456781234567812345678123456781234567812345678 
            WhiteRooks   =  0B0000000000000000000000000000000000000000000000000000000010000001;
            WhiteKnights = 0B0000000000000000000000000000000000000000000000000000000001000010;
            WhiteBishops = 0B0000000000000000000000000000000000000000000000000000000000100100;
            WhiteKing    = 0B0000000000000000000000000000000000000000000000000000000000010000;
            WhiteQueens  = 0B0000000000000000000000000000000000000000000000000000000000001000;
            WhitePawns   = 0B0000000000000000000000000000000000000000000000001111111100000000;
            //---------------1       1       1       1       1       1       1       1
            //---------------1234567812345678123456781234567812345678123456781234567812345678   
            BlackRooks   = 0B1000000100000000000000000000000000000000000000000000000000000000;
            BlackKnights =  0B0100001000000000000000000000000000000000000000000000000000000000;
            BlackBishops =  0B0010010000000000000000000000000000000000000000000000000000000000;
            BlackKing    =  0B0001000000000000000000000000000000000000000000000000000000000000;
            BlackQueens  =  0B0000100000000000000000000000000000000000000000000000000000000000;
            BlackPawns   = 0B0000000011111111000000000000000000000000000000000000000000000000;
            //---------------1       1       1       1       1       1       1       1
            //---------------1234567812345678123456781234567812345678123456781234567812345678   
            

            refactor();


            
        }
        private void RookTest()
        {
            //sample for  tests
            //---------------1       1       1       1       1       1       1       1
            //---------------1234567812345678123456781234567812345678123456781234567812345678   
            BlackRooks =   0;
            BlackKnights = 0;
            BlackBishops = 0;
            BlackKing = 0;
            BlackQueens = 0;
            BlackPawns = 0;
            //---------------1       1       1       1       1       1       1       1
            //---------------1234567812345678123456781234567812345678123456781234567812345678   
            WhiteRooks =   0;
            WhiteKnights = 0;
            WhiteBishops = 0B0000100100001010000001000000010000000100000110000100001000100000;
            WhiteKing = 0;
            WhiteQueens = 0;
            WhitePawns = 0;
        }

        public void refactor()
        {
            WhitePieces = WhiteRooks | WhiteKnights |
                          WhiteBishops | WhiteKing |
                          WhiteQueens | WhitePawns;
            BlackPieces = BlackRooks | BlackKnights |
                          BlackBishops | BlackKing |
                          BlackQueens | BlackPawns;
            AllPieces = WhitePieces | BlackPieces;
            Dictionary<String, UInt64> WBoards = new Dictionary<String, UInt64>() ;
            Dictionary<String, UInt64> BBoards = new Dictionary<String, UInt64>();
            AllBitboards = new Dictionary<Boolean, Dictionary<String, UInt64>>();
            WBoards.Add("Pawn", WhitePawns);
            WBoards.Add("Rook",WhiteRooks   );
            WBoards.Add("Knight",WhiteKnights );
            WBoards.Add("Bishop",WhiteBishops );
            WBoards.Add("King",WhiteKing    );
            WBoards.Add("Queen",WhiteQueens  );
            WBoards.Add("All", WhitePieces);
            AllBitboards.Add(true, WBoards);

            BBoards.Clear();
            BBoards.Add("Pawn",   BlackPawns);
            BBoards.Add("Rook",   BlackRooks);
            BBoards.Add("Knight", BlackKnights);
            BBoards.Add("Bishop", BlackBishops);
            BBoards.Add("King",   BlackKing);
            BBoards.Add("Queen",  BlackQueens);
            BBoards.Add("All", BlackPieces);
            AllBitboards.Add(false, BBoards);



        }
        public Board copy()
        {
            Board ret = new Board();
            ret.WhiteRooks = this.WhiteRooks;
            ret.WhiteKnights = this.WhiteKnights;
            ret.WhiteBishops = this.WhiteBishops;
            ret.WhiteKing = this.WhiteKing;
            ret.WhiteQueens = this.WhiteQueens;
            ret.WhitePawns = this.WhitePawns;
            ret.BlackRooks = this.BlackRooks;
            ret.BlackKnights = this.BlackKnights;
            ret.BlackBishops = this.BlackBishops;
            ret.BlackKing = this.BlackKing;
            ret.BlackQueens = this.BlackQueens;
            ret.BlackPawns = this.BlackPawns;
            ret.WhitePieces = this.WhitePieces;
            ret.BlackPieces = this.BlackPieces;
            ret.AllPieces = this.AllPieces;
            ret.AllBitboards = AllBitboards;
            return ret;
        }
        public void repositionPiece(UInt64 from, UInt64 to)
        {
            if ((from & this.WhitePieces) != 0)//weisses piece wir bewegt
            {
                if ((WhiteRooks & from) != 0)
                {
                    WhiteRooks = WhiteRooks & ~from;
                    WhiteRooks = WhiteRooks | to;
                }
                else if((WhiteKnights & from) != 0)
                {
                    WhiteKnights = WhiteKnights & ~from;
                    WhiteKnights = WhiteKnights | to;
                }
                else if((WhiteBishops & from) != 0)
                {
                    WhiteBishops = WhiteBishops & ~from;
                    WhiteBishops = WhiteBishops | to;
                } 
                else if((WhiteKing & from) != 0)
                {
                    WhiteKing = WhiteKing & ~from;
                    WhiteKing = WhiteKing | to;
                }
                else if((WhiteQueens & from) != 0)
                {
                    WhiteQueens = WhiteQueens & ~from;
                    WhiteQueens = WhiteQueens | to;
                }
                else if((WhitePawns & from) != 0)
                {
                    WhitePawns = WhitePawns & ~from;
                    WhitePawns = WhitePawns | to;
                }
                if ((this.BlackPieces&to)!=0)
                {
                    BlackRooks   = BlackRooks &   ~to;
                    BlackKnights = BlackKnights & ~to;
                    BlackBishops = BlackBishops & ~to;
                    BlackKing    = BlackKing &    ~to;
                    BlackQueens  = BlackQueens &  ~to;
                    BlackPawns   = BlackPawns &   ~to;
                }
            }
            else if((from & this.BlackPieces) != 0)//scxhwarzes piece wird bewegt
            {
                if ((BlackRooks & from) != 0)
                {
                    BlackRooks = BlackRooks & ~from;
                    BlackRooks = BlackRooks | to;
                }
                else if ((BlackKnights & from) != 0)
                {
                    BlackKnights = BlackKnights & ~from;
                    BlackKnights = BlackKnights | to;
                }
                else if ((BlackBishops & from) != 0)
                {
                    BlackBishops = BlackBishops & ~from;
                    BlackBishops = BlackBishops | to;
                }
                else if ((BlackKing & from) != 0)
                {
                    BlackKing = BlackKing & ~from;
                    BlackKing = BlackKing | to;
                }
                else if ((BlackQueens & from) != 0)
                {
                    BlackQueens = BlackQueens & ~from;
                    BlackQueens = BlackQueens | to;
                }
                else if ((BlackPawns & from) != 0)
                {
                    BlackPawns = BlackPawns & ~from;
                    BlackPawns = BlackPawns | to;
                }
                if ((this.WhitePieces & to) != 0)
                {
                    WhiteRooks =   WhiteRooks & ~to;
                    WhiteKnights = WhiteKnights & ~to;
                    WhiteBishops = WhiteBishops & ~to;
                    WhiteKing =    WhiteKing & ~to;
                    WhiteQueens =  WhiteQueens & ~to;
                    WhitePawns = WhitePawns & ~to;
                }
            }
            refactor();
        }
        public String toString()
        {

            return
                WhiteRooks.ToString() +
                WhiteKnights.ToString() +
                WhiteBishops.ToString() +
                WhiteKing.ToString() +
                WhiteQueens.ToString() +
                WhitePawns.ToString() +
                BlackRooks.ToString() +
                BlackKnights.ToString() +
                BlackBishops.ToString() +
                BlackKing.ToString() +
                BlackQueens.ToString() +
                BlackPawns.ToString();
        }

    }

}
