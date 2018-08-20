using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    class Calculator
    {

        public static UInt64 AllPossibleForPiece(Move currentMove, ulong Piece, Boolean pieceColor)
        {
            UInt64 PseudoMoves = AllPseudoPossibleForPiece(currentMove, Piece,pieceColor);
            UInt64 Moves=0;


            UInt64 LSB = BitHelper.LSB(PseudoMoves);
            while (LSB > 0)
            {
                
                if (!currentMove.MakeAMove(Piece, LSB, false).EnemyInCheck())
                {
                    Moves = Moves | LSB;
                }
                currentMove.reset();
                PseudoMoves = PseudoMoves ^ LSB;
                LSB = LSB = BitHelper.LSB(PseudoMoves);
            }
            

            return Moves;
        }
        public static UInt64 AllPseudoPossible(Move currentMove)
        {
            UInt64 ret = 0;
            UInt64 LSB = 0;
            UInt64 Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Rook"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                ret = ret | AllPseudoPossibleRook(currentMove, LSB, currentMove.turn);
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Bishop"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                ret = ret | AllPseudoPossibleBishop(currentMove, LSB, currentMove.turn);
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Queen"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                ret = ret | AllPseudoPossibleQueen(currentMove, LSB, currentMove.turn);
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["King"];
            ret = ret | AllPseudoPossibleKing(currentMove, Pieces, currentMove.turn);
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Pawn"];
            ret = ret | AllPseudoPossiblePawn(currentMove, Pieces, currentMove.turn);
            Pieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["Knight"];
            ret = ret | AllPseudoPossibleKnight(currentMove, Pieces, currentMove.turn);





            return ret;
        }
        public static UInt64 AllPseudoPossibleEnemy(Move currentMove)
        {
            UInt64 ret = 0;
            UInt64 LSB = 0;
            UInt64 Pieces = currentMove.BoardBefore.AllBitboards[!currentMove.turn]["Rook"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                ret = ret | AllPseudoPossibleRook(currentMove, LSB, !currentMove.turn);
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[!currentMove.turn]["Bishop"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                ret = ret | AllPseudoPossibleBishop(currentMove, LSB, !currentMove.turn);
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[!currentMove.turn]["Queen"];
            LSB = LSB = BitHelper.LSB(Pieces);
            while (LSB > 0)
            {
                ret = ret | AllPseudoPossibleQueen(currentMove, LSB, !currentMove.turn);
                Pieces = Pieces ^ LSB;
                LSB = LSB = BitHelper.LSB(Pieces);
            }
            Pieces = currentMove.BoardBefore.AllBitboards[!currentMove.turn]["King"];
            ret = ret | AllPseudoPossibleKing(currentMove, Pieces, !currentMove.turn);
            Pieces = currentMove.BoardBefore.AllBitboards[!currentMove.turn]["Pawn"];
            ret = ret | AllPseudoPossiblePawn(currentMove, Pieces, currentMove.turn);
            Pieces = currentMove.BoardBefore.AllBitboards[!currentMove.turn]["Knight"];
            ret = ret | AllPseudoPossibleKnight(currentMove, Pieces, !currentMove.turn);
            return ret;
        }
        public static UInt64 AllPseudoPossibleForPiece(Move currentMove, ulong Piece, Boolean pieceColor)
        {
            
            Board currentBoard = currentMove.BoardAfter;
            UInt64 returnBoard=0;
           
            
            if ((currentBoard.AllBitboards[currentMove.turn]["Bishop"] & Piece) != 0)
            {
                returnBoard = AllPseudoPossibleBishop(currentMove, Piece, pieceColor);
            }
            else if ((currentBoard.AllBitboards[currentMove.turn]["King"] & Piece) != 0) 
            {
                returnBoard= AllPseudoPossibleKing(currentMove, Piece, pieceColor);
            }
            else if ((currentBoard.AllBitboards[currentMove.turn]["Knight"] & Piece) != 0)
            {
                returnBoard = AllPseudoPossibleKnight(currentMove, Piece, pieceColor);
            }
            else if ((currentBoard.AllBitboards[currentMove.turn]["Pawn"] & Piece) != 0)
            {
                returnBoard=AllPseudoPossiblePawn(currentMove, Piece, pieceColor);
            }
            else if ((currentBoard.AllBitboards[currentMove.turn]["Queen"] & Piece) != 0)
            {
                returnBoard = AllPseudoPossibleQueen(currentMove, Piece, pieceColor);
            }
            else if ((currentBoard.AllBitboards[currentMove.turn]["Rook"] & Piece) != 0)
            {
                returnBoard = AllPseudoPossibleRook(currentMove, Piece, pieceColor);
            }
            return returnBoard;
        }
        public static UInt64 AllPseudoPossibleBishop(Move currentMove, UInt64 Piece, Boolean pieceColor)
        {
            Board currentBoard = currentMove.BoardBefore;
            UInt64 OCCUPIED = currentMove.BoardBefore.AllBitboards[!pieceColor]["All"] | currentMove.BoardBefore.AllBitboards[pieceColor]["All"];
            int setBit = BitHelper.setBit(Piece);
            UInt64 Diagonal = BitHelper.Diagonal[(setBit % 8) + (setBit / 8)];
            UInt64 AntiDiagonal = BitHelper.Antidiagonal[(7-(setBit / 8))+ (setBit % 8)];
            UInt64 OCCUPIEDonDiag = OCCUPIED & Diagonal;
            UInt64 LeftUp = (OCCUPIEDonDiag ^ (OCCUPIEDonDiag - 2 * Piece)) & Diagonal;//korrekt
            UInt64 RightDown = (Piece - 1 & ~(BitHelper.MSB(((OCCUPIEDonDiag | BitHelper.LSB(Diagonal)) & (Piece - 1))) - 1)) & Diagonal;//korrekt
            OCCUPIEDonDiag = OCCUPIED & AntiDiagonal;
            UInt64 RightUp = (OCCUPIEDonDiag ^ (OCCUPIEDonDiag - 2 * Piece)) & AntiDiagonal;//korrekt
            UInt64 LeftDown = (Piece - 1 & ~(BitHelper.MSB(((OCCUPIEDonDiag | BitHelper.LSB(AntiDiagonal)) & (Piece - 1))) - 1)) & AntiDiagonal;
            return (LeftUp | RightUp | LeftDown | RightDown) & ~currentMove.BoardBefore.AllBitboards[pieceColor]["All"]; ;
        }
        public static UInt64 AllPseudoPossibleRook(Move currentMove, UInt64 Piece, Boolean pieceColor)
        {
            Board currentBoard = currentMove.BoardBefore;
            UInt64 OCCUPIED = currentMove.BoardBefore.AllBitboards[!pieceColor]["All"] | currentMove.BoardBefore.AllBitboards[pieceColor]["All"];
            int setBit = BitHelper.setBit(Piece);
            UInt64 File = BitHelper.File[(setBit % 8)];
            UInt64 Rank = BitHelper.Rank[(setBit / 8)];

            //Console.WriteLine("OCCUPIED: " + Convert.ToString((long)OCCUPIED, 2));
            //Console.WriteLine("binaryS: " + Convert.ToString((long)Piece, 2));
            UInt64 possibilitiesVertical = (((OCCUPIED & File) - (2 * Piece)) ^ BitHelper.reverse(BitHelper.reverse(OCCUPIED & File) - (2 * BitHelper.reverse(Piece)))) ;
            possibilitiesVertical = possibilitiesVertical & File;
            UInt64 toTheRight= (OCCUPIED ^ (OCCUPIED - 2 * Piece)) & Rank;
            UInt64 toTheLeft = Piece - 1 & ~(BitHelper.MSB(((OCCUPIED | (Rank >> 7)) & (Piece - 1))) - 1);
            UInt64 possibilitiesHorizontal = toTheLeft | toTheRight;
            //chessprogramming wiki (OCCUPIED - 2 * binaryS) ^ BitHelper.reverse(BitHelper.reverse(OCCUPIED) - 2 * BitHelper.reverse(binaryS));
            //BitHelper.MSB(((OCCUPIED | (Rank >> 7)) & (binaryS - 1)));//der am weitesten rechts stehende blocker
            return (possibilitiesHorizontal | possibilitiesVertical) & ~currentMove.BoardBefore.AllBitboards[pieceColor]["All"];  
        }
        public static UInt64 AllPseudoPossiblePawn(Move currentMove, UInt64 Piece, Boolean pieceColor)
        {
            Board currentBoard = currentMove.BoardBefore;
            UInt64 returnBoard = 0;
            if (pieceColor)//weiss
            {
                returnBoard = (((Piece << 8)) & (~currentBoard.AllPieces))                  //eine zeile nach unten, wenn da keine figur steht
                            | (((Piece & BitHelper.Rank[1])  << 16) & (~currentBoard.AllPieces) & (~currentBoard.AllPieces<<8))                            //zwei zeilen nach unten, wenn das piece im siebten rank liegt, und da keinpiece ist
                            | (((Piece & (~BitHelper.File[7])) << 9) & (currentBoard.AllBitboards[!pieceColor]["All"] | currentMove.enPassentSquare[pieceColor]))  //nach links oben, wenn da ein feind steht und ich nicht in der linksten zeile stehe
                            | (((Piece & (~BitHelper.File[0])) << 7) & (currentBoard.AllBitboards[!pieceColor]["All"] | currentMove.enPassentSquare[pieceColor]))  //nach reechts oben, wenn da ein feind steht und ich nicht in der linksten zeile stehe
                    ;
            }
            else
            {
                returnBoard = ((Piece >> 8) & (~currentBoard.AllPieces))                                                   //eine zeile nach unten
                            | (((Piece & BitHelper.Rank[6]) >> 16) & (~currentBoard.AllPieces) & (~currentBoard.AllPieces>>8))  //zwei zeilen nach unten, wenn das piece im siebten rank liegt, und da keine eigenen pieces sind
                            | (((Piece & (~BitHelper.File[7])) >> 7) & (currentBoard.AllBitboards[!pieceColor]["All"] | currentMove.enPassentSquare[pieceColor]))  //nach links unten, wenn da ein feind steht und ich nicht in der linksten zeile stehe
                            | (((Piece & (~BitHelper.File[0])) >> 9) & (currentBoard.AllBitboards[!pieceColor]["All"] | currentMove.enPassentSquare[pieceColor]))  //nach reechts unten, wenn da ein feind steht und ich nicht in der linksten zeile stehe
                    ;
            }
            

            return returnBoard;
        }
        public static UInt64 AllPseudoPossibleKnight(Move currentMove, UInt64 Piece, Boolean pieceColor)
        {
            Board currentBoard = currentMove.BoardAfter;
            UInt64 returnBoard = 0;
            returnBoard =  ( 
                                ( Piece & ~(BitHelper.Rank[6] | BitHelper.Rank[7] | BitHelper.File[7])) <<17    //zwei nach oben, einer nach rechts, wenn du nicht in rank 7/8 stehst, und nicht in der a file
                               | (Piece & ~(BitHelper.Rank[6] | BitHelper.Rank[7] | BitHelper.File[0])) << 15   //zwei nach oben, einer nach links, wenn du nicht in rank 7/8 stehst, und nicht in der h file
                               | (Piece & ~(BitHelper.Rank[7] | BitHelper.File[0] | BitHelper.File[1])) << 6    //einer nach oben zwei nach links, wenn du nicht in rank 8 stehst und nicht in der a oger b spalte
                               | (Piece & ~(BitHelper.Rank[7] | BitHelper.File[6] | BitHelper.File[7])) << 10    //einer nach oben zwei nach rechts, wenn du nicht in rank 8 stehst und nicht in der H oger g spalte
                               | (Piece & ~(BitHelper.Rank[0] | BitHelper.File[0] | BitHelper.File[1])) >> 10       //einer nach unten zwei nach links
                               | (Piece & ~(BitHelper.Rank[0] | BitHelper.File[6] | BitHelper.File[7])) >> 6       //einer nach unten zwei nach rechts
                               | (Piece & ~(BitHelper.Rank[0] | BitHelper.Rank[1] | BitHelper.File[0])) >> 17    //einer nach links, zwei nach unten
                               | (Piece & ~(BitHelper.Rank[0] | BitHelper.Rank[1] | BitHelper.File[7])) >> 15   //einer nach rechts, zwei nach unten

                               )
                            & (~currentBoard.AllBitboards[pieceColor]["All"])//beim ziel darf niemand stehen von der eignen farbe
                            ;





            return returnBoard;
        }
        public static UInt64 AllPseudoPossibleKing(Move currentMove, UInt64 Piece, Boolean pieceColor)
        {
            Board currentBoard = currentMove.BoardAfter;
            UInt64 returnBoard = 0;
            bool leftCastleAllowed=true;
            bool rightCastleAllowed = true;

            returnBoard = (
                                    (Piece & ~(BitHelper.Rank[0])) >> 8 //unten
                                  | (Piece & ~(BitHelper.Rank[0] | BitHelper.File[0])) >> 9//unten links
                                   | (Piece & ~(BitHelper.Rank[0] | BitHelper.File[7])) >> 7//unten rechts
                                  | (Piece & ~( BitHelper.File[0])) >> 1//links
                                  | (Piece & ~(BitHelper.File[7])) << 1//rechts
                                  | (Piece & ~(BitHelper.File[7] | BitHelper.Rank[7])) << 9//oben rechts
                                   | (Piece & ~(BitHelper.Rank[7])) << 8//oben
                                    | (Piece & ~(BitHelper.File[0] | BitHelper.Rank[7])) << 7//oben links
                               ) & (~currentBoard.AllBitboards[pieceColor]["All"])
                            ;
            if (leftCastleAllowed)
            {
                UInt64 OCCUPIED = currentMove.BoardBefore.AllBitboards[!pieceColor]["All"] | currentMove.BoardBefore.AllBitboards[pieceColor]["All"];
                UInt64 Rank = BitHelper.Rank[(BitHelper.setBit(Piece) / 8)];
                if (((Piece - 1 & ~(BitHelper.MSB(((OCCUPIED | (Rank >> 7)) & (Piece - 1))) - 1)) & currentBoard.AllBitboards[pieceColor]["Rook"]) > 0)
                {
                    returnBoard = returnBoard | Piece >> 2;
                }
            }
            if (rightCastleAllowed)
            {
                UInt64 OCCUPIED = currentMove.BoardBefore.AllBitboards[!pieceColor]["All"] | currentMove.BoardBefore.AllBitboards[pieceColor]["All"];
                UInt64 Rank = BitHelper.Rank[(BitHelper.setBit(Piece) / 8)];
                if (((OCCUPIED ^ (OCCUPIED - 2 * Piece)) & Rank & currentBoard.AllBitboards[pieceColor]["Rook"]) > 0)
                {
                    returnBoard = returnBoard | Piece << 2;
                }
            }


            return returnBoard ;
        }
        public static UInt64 AllPseudoPossibleQueen(Move currentMove, UInt64 Piece, Boolean pieceColor)
        {
            return AllPseudoPossibleRook(currentMove, Piece, pieceColor) | AllPseudoPossibleBishop(currentMove, Piece, pieceColor);
        }
        public static List<UInt64[]> AllPossible(Move currentMove, Boolean pieceColor)
        {
            List<UInt64[]> result = new List<UInt64[]>();
            UInt64[] Move = { 0, 0 };
            UInt64 PseudoMoves = 0;
            UInt64 AllPieces = currentMove.BoardBefore.AllBitboards[currentMove.turn]["All"];
            UInt64 Piece = BitHelper.LSB(AllPieces);
            while (Piece > 0)
            {
                PseudoMoves = AllPseudoPossibleForPiece(currentMove, Piece, pieceColor);
               
                UInt64 LSB = BitHelper.LSB(PseudoMoves);
                while (LSB > 0)
                {

                    if (!currentMove.MakeAMove(Piece, LSB, false).EnemyInCheck())
                    {
                        result.Add(new UInt64[] { Piece, LSB });
                    }
                    currentMove.reset();
                    PseudoMoves = PseudoMoves ^ LSB;
                    LSB = LSB = BitHelper.LSB(PseudoMoves);
                }

                AllPieces = AllPieces ^ Piece;
                Piece = BitHelper.LSB(AllPieces);
            }
            return result;

        }




    }
}
