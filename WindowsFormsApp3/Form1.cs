using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public delegate void refreshBoard();
        public refreshBoard DelegateBoard;
        public delegate void refreshTurnd();
        public refreshTurnd DelegateTurn;
        public refreshTurnd DelegateCount;
        bool keep_learning=true;

        Thread myThread;
        int testvar;

        public Game main_game;
        public ArrayList coloredButtons;
        int clicks = 0;
        int fromCol;
        int fromRow;
        AI ki;
        AI ki2;
        AI kiDeaf;
        UInt64 FromPosition;
        UInt64 ToPosition;
        private ulong clicked;
        System.Windows.Forms.Button[,] buttons;
        public Form1()
        {
            //Console.WriteLine(null==null + "");Console.Read();
            InitializeComponent();
            coloredButtons= new ArrayList();
            buttons = new System.Windows.Forms.Button[,] {  { this.button57,this.button49,this.button41,this.button33,this.button25,this.button17,this.button9 ,this.button1},  //x=1
                                                            { this.button58,this.button50,this.button42,this.button34,this.button26,this.button18,this.button10,this.button2},  //x=2
                                                            { this.button59,this.button51,this.button43,this.button35,this.button27,this.button19,this.button11,this.button3},  //x=3
                                                            { this.button60,this.button52,this.button44,this.button36,this.button28,this.button20,this.button12,this.button4},  //x=4
                                                            { this.button61,this.button53,this.button45,this.button37,this.button29,this.button21,this.button13,this.button5},  //x=5
                                                            { this.button62,this.button54,this.button46,this.button38,this.button30,this.button22,this.button14,this.button6},  //x=6
                                                            { this.button63,this.button55,this.button47,this.button39,this.button31,this.button23,this.button15,this.button7},  //x=7
                                                            { this.button64,this.button56,this.button48,this.button40,this.button32,this.button24,this.button16,this.button8},  //x=8
                                                         };
            Text = "JAR CHESS AI";
            DelegateBoard = new refreshBoard(afterAi);
            DelegateTurn = new refreshTurnd(refreshTurn);
            DelegateCount = new refreshTurnd(AiThoughts);
            kiDeaf = new AI(false, this);
        }
        private void startGame(String type)
        {
            groupPlay.Visible = false;
            main_game=new Game();
            redraw_board();
            main_game.type = type;
            if (type == "ai")
            {
                Console.WriteLine(" creating an AI");
                ki = new AI(false, this);
                kiPanel.Visible = true;
                textBox7.Text = ki.depth + "";
            }
            kiPanel.Visible = true;



        }
      
        public void click(int column, int row)
        {
            uncolorButtons();
            clicked = 0B000000000000000000000000000000000000000000000000000000000000001;
            clicked = clicked << (((row * 8) + (column)));
            
            //colorButtons(BitHelper.File[(BitHelper.setBit(clicked) % 8)], Color.Blue);
            //colorButtons(BitHelper.Rank[(BitHelper.setBit(clicked) / 8)], Color.Blue);



            Console.WriteLine("clicked column " + column + ",  row " + row);
            if (main_game != null && main_game.type != "aiai")//interaction with the board should be prevented when two Ais are battling
            {
                if (!(main_game.type == "ai" && ki.Color == main_game.currentMove.turn))////interaction with the board should be prevented when the ai is thinking
                {
                    Console.WriteLine("player is allowed to click");
                    

                    //Console.WriteLine("Bitboard: "+Convert.ToString((long)clicked, 2));
                    //Console.WriteLine("All Pieces of turn " + Convert.ToString((long)main_game.currentMove.BoardAfter.AllBitboards[main_game.currentMove.turn]["All"], 2));

                    if ((clicked & main_game.currentMove.BoardAfter.AllBitboards[main_game.currentMove.turn]["All"]) != 0)//eigene Figur angecklickt
                    {
                        Console.WriteLine("player clicked one of his figures");
                        FromPosition = clicked;
                        

                        UInt64 PlaceHeCanGo = Calculator.AllPossibleForPiece(main_game.currentMove, clicked,main_game.currentMove.turn);

                        colorButtons(PlaceHeCanGo, Color.Green);
                    }
                    else
                    {
                        Console.WriteLine("Form triggers make_move");
                        ToPosition = clicked;
                        main_game.make_move(FromPosition, ToPosition);

                        aftermove();
                        if (ki != null)
                        {
                            Console.WriteLine("KI exists");
                            if (ki.Color == main_game.currentMove.turn && main_game.winner == null)
                            {
                                Console.WriteLine("AI gets called from Form1");
                                myThread = new Thread(new ThreadStart(ki.call));
                                myThread.Start();
                            }

                        }
                    }
                }
            }
        }
        public void colorButton(int x, int y, Color newColor)
        {
             buttons[x, y].BackColor = newColor;
        }
        public void colorButtons(UInt64 BitBoard, Color newColor)
        {
            byte[] bytes = BitConverter.GetBytes(BitBoard);
            int bitPos = 0;
            while (bitPos < 8 * bytes.Length)//durch die bits des boards loopen
            {
                int byteIndex = bitPos / 8;
                int offset = bitPos % 8;
                bool isSet = (bytes[byteIndex] & (1 << offset)) != 0;

                // isSet = [True] if the bit at bitPos is set, false otherwise
                if (isSet)
                {
                    colorButton(offset, byteIndex, newColor);
                }


                bitPos++;
            }
        }
        public void uncolorButtons()
        {
            for (int i = 0; i < 8; i++){
                for (int k = 0; k < 8; k++)
                {
                    if ((k + i) % 2 == 1)//ungerade zahl
                    {
                        buttons[i, k].BackColor = Color.Khaki;
                    }
                    else
                    {
                        buttons[i, k].BackColor = Color.Sienna;
                    }
                }
            }
            
            
            if (main_game.currentMove.previousMove.from !=0)
            {
                colorButtons(main_game.currentMove.previousMove.from| main_game.currentMove.previousMove.to, Color.LightGreen);

            }
            UInt64 checkers = main_game.currentMove.checkers();
            if (checkers!=0)
            {
                colorButtons(main_game.currentMove.BoardAfter.AllBitboards[main_game.currentMove.turn]["King"]| checkers, Color.Red);
            }
            if (main_game.currentMove.AmIInCheck())
            {
                colorButtons(main_game.currentMove.BoardAfter.AllBitboards[main_game.currentMove.turn]["King"], Color.Red);
            }
        }


        public void DisplayDebugInfo()
        {
            this.textBox9.Text = kiDeaf.evaluateBoardPlus(main_game.currentMove) + "";
            this.textBox8.Text = kiDeaf.evaluateBoard(main_game.currentMove.BoardBefore) + "";
            label8.Text = "estimated: " + kiDeaf.increaseDepth(main_game.currentMove);
            if (ki != null)
            {
                label14.Text = ki.firstLayer + "";
            }
            
        }
        public void aftermove()
        {
            redraw_board();
            uncolorButtons();
            DisplayDebugInfo();
            
            textBox6.Text = main_game.currentMove.num+"";

            
            if (main_game.winner != null)
            {
                textBox7.Text = main_game.winner + " side wins!";
                
                
            }
            else if (ki2 != null &&!main_game.currentMove.promotion) {
                if (main_game.currentMove.turn)
                {
                    myThread = new Thread(new ThreadStart(ki.call));
                    myThread.Start();
                }
                else
                {
                    myThread = new Thread(new ThreadStart(ki2.call));
                    myThread.Start();
                }
            }
            if (main_game.currentMove.promotion)
            {
                //highlight promoter
                colorButtons(main_game.currentMove.promotionBoard, Color.Chartreuse);
                groupPromotion.Visible = true;
            }
            refreshTurn();

        }
        public void afterAi()
        {
            aftermove();
            label10.Text = "evaluated " + ki.evaluatedPositions + " positions in " + ki.time_needed;
        }
        public void AiThoughts()
        {
            if (main_game.currentMove.turn)
            {
                label10.Text = "evaluated " + ki2.evaluatedPositions + " positions in " + ki2.time_needed;
                label11.Text = ki2.percentFinished + "%";
            }
            else
            {
                label10.Text = "evaluated " + ki.evaluatedPositions + " positions in " + ki.time_needed;
                label11.Text = (int)ki.percentFinished + "%";
            }
            
        }
        public void redraw_board ()//das schachfeld aus main_game.currentMove malen
        {
            Button but;
            String col;
            String name;
            foreach(Button btn in buttons)//erst alle bilder rausclearen
            {
                btn.Image = null;
            }
            UInt64[] WBitboards ={main_game.currentMove.BoardAfter.WhiteKing,
                                  main_game.currentMove.BoardAfter.WhiteKnights,
                                  main_game.currentMove.BoardAfter.WhitePawns,
                                  main_game.currentMove.BoardAfter.WhiteQueens,
                                  main_game.currentMove.BoardAfter.WhiteRooks,
                                  main_game.currentMove.BoardAfter.WhiteBishops
                                };
            UInt64[] BBitboards ={main_game.currentMove.BoardAfter.BlackKing,
                                  main_game.currentMove.BoardAfter.BlackKnights,
                                  main_game.currentMove.BoardAfter.BlackPawns,
                                  main_game.currentMove.BoardAfter.BlackQueens,
                                  main_game.currentMove.BoardAfter.BlackRooks,
                                  main_game.currentMove.BoardAfter.BlackBishops
                                };
            UInt64[][] Bitboards = { WBitboards, BBitboards };
            for (int k = 0; k < Bitboards.Length; k++)//durch schwarz/weiss loopen
            {
                if (k == 0)
                {
                    col = "white";
                }
                else
                {
                    col = "black";
                }
                for (int i = 0; i < Bitboards[k].Length; i++)//durch die boards der farbe loopen
                {
                    switch (i)
                    {
                        case 0:
                            name = "king";
                            break;
                        case 1:
                            name = "knight";
                            break;
                        case 2:
                            name = "pawn";
                            break;
                        case 3:
                            name = "queen";
                            break;
                        case 4:
                            name = "rook";
                            break;
                        case 5:
                            name = "bishop";
                            break;
                        default:
                            name = "could not determine piece";
                            break;
                    }

                    byte[] bytes = BitConverter.GetBytes(Bitboards[k][i]);

                    int bitPos = 0;
                    while (bitPos < 8 * bytes.Length)//durch die bits des boards loopen
                    {
                        int byteIndex = bitPos / 8;
                        int offset = bitPos % 8;
                        bool isSet = (bytes[byteIndex] & (1 << offset)) != 0;

                        // isSet = [True] if the bit at bitPos is set, false otherwise
                        if (isSet)
                        {
                            but = buttons[(offset), (byteIndex)];

                            but.Image = ResizeImage(
                                                      Image.FromFile(col
                                                     + "_" + name + "_white.png"
                                                     , true)
                                                     , but.Width
                                                     , but.Height);
                        }
                        

                        bitPos++;
                    }

                }
            }
        }
        public void refreshTurn()//anzeigen, welcher Spieler dran ist
        {
            if (this.main_game.currentMove.turn) {
                this.label4.Text = "white";
            }
            else
            {
                this.label4.Text = "black";
            }
            
        }
        
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width-2, height-2);
            var destImage = new Bitmap(width-2, height-2);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            click(0, 7);
            
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            click(1, 7);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            click(2, 7);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            click(3, 7);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            click(4, 7);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            click(5, 7);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            click(6, 7);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            click(7, 7);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            click(0, 6);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            click(1, 6);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            click(2, 6);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            click(3, 6);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            click(4, 6);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            click(5, 6);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            click(6, 6);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            click(7, 6);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            click(0, 5);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            click(1, 5);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            click(2, 5);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            click(3, 5);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            click(4, 5);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            click(5, 5);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            click(6, 5);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            click(7, 5);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            click(0, 4);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            click(1, 4);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            click(2, 4);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            click(3, 4);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            click(4, 4);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            click(5, 4);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            click(6, 4);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            click(7, 4);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            click(0, 3);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            click(1, 3);
        }

        private void button35_Click(object sender, EventArgs e)
        {
            click(2, 3);
        }

        private void button36_Click(object sender, EventArgs e)
        {
            click(3, 3);
        }

        private void button37_Click(object sender, EventArgs e)
        {
            click(4, 3);
        }

        private void button38_Click(object sender, EventArgs e)
        {
            click(5, 3);
        }

        private void button39_Click(object sender, EventArgs e)
        {
            click(6, 3);
        }

        private void button40_Click(object sender, EventArgs e)
        {
            click(7, 3);
        }

        private void button41_Click(object sender, EventArgs e)
        {
            click(0, 2);
        }

        private void button42_Click(object sender, EventArgs e)
        {
            click(1, 2);

        }

        private void button43_Click(object sender, EventArgs e)
        {
            click(2, 2);
        }

        private void button44_Click(object sender, EventArgs e)
        {
            click(3, 2);
        }

        private void button45_Click(object sender, EventArgs e)
        {
            click(4, 2);
        }

        private void button46_Click(object sender, EventArgs e)
        {
            click(5, 2);
        }

        private void button47_Click(object sender, EventArgs e)
        {
            click(6, 2);
        }

        private void button48_Click(object sender, EventArgs e)
        {
            click(7, 2);
        }

        private void button49_Click(object sender, EventArgs e)
        {
            click(0, 1);
        }

        private void button50_Click(object sender, EventArgs e)
        {
            click(1, 1);
        }

        private void button51_Click(object sender, EventArgs e)
        {
            click(2, 1);
        }

        private void button52_Click(object sender, EventArgs e)
        {
            click(3, 1);
        }

        private void button53_Click(object sender, EventArgs e)
        {
            click(4, 1);
        }

        private void button54_Click(object sender, EventArgs e)
        {
            click(5, 1);
        }

        private void button55_Click(object sender, EventArgs e)
        {
            click(6, 1);
        }

        private void button56_Click(object sender, EventArgs e)
        {
            click(7, 1);
        }

        private void button57_Click(object sender, EventArgs e)
        {
            click(0, 0);
        }

        private void button58_Click(object sender, EventArgs e)
        {
            click(1, 0);
        }

        private void button59_Click(object sender, EventArgs e)
        {
            click(2, 0);
        }

        private void button60_Click(object sender, EventArgs e)
        {
            click(3, 0);
        }

        private void button61_Click(object sender, EventArgs e)
        {
            click(4, 0);
        }

        private void button62_Click(object sender, EventArgs e)
        {
            click(5, 0);
        }

        private void button63_Click(object sender, EventArgs e)
        {
            click(6, 0);
        }

        private void button64_Click(object sender, EventArgs e)
        {
            click(7, 0);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button65_Click(object sender, EventArgs e)
        {

            unvisiblepromotion();
            main_game.make_promotion("queen");
            redraw_board();
            aftermove();
        }

        private void button66_Click(object sender, EventArgs e)
        {
            unvisiblepromotion();
            main_game.make_promotion("rook");
            redraw_board();
            aftermove();
        }

        private void button67_Click(object sender, EventArgs e)
        {
            unvisiblepromotion();
            main_game.make_promotion("knight");
            redraw_board();
            aftermove();
        }

        private void button68_Click(object sender, EventArgs e)
        {
            unvisiblepromotion();
            main_game.make_promotion("bishop");
            redraw_board();
            aftermove();
        }
        private void unvisiblepromotion()
        {
            groupPromotion.Visible = false;
            uncolorButtons();
        }

        private void button69_Click(object sender, EventArgs e)
        {
            //play against ai
            startGame("ai");
        }

        private void button70_Click(object sender, EventArgs e)
        {
            //play against human
            startGame("human");
        }
        private void button73_Click_1(object sender, EventArgs e)
        {
            //ai vs AI
            startGame("aiai");
        }
        private void button71_Click(object sender, EventArgs e)
        {
            this.ki.call();
            this.redraw_board();
        }

        private void button72_Click(object sender, EventArgs e)
        {

            
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button73_Click(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            int x;
            if(Int32.TryParse(textBox7.Text, out x))
            {
                ki.depth = x;
            }
            
           
        }

        private void button74_Click(object sender, EventArgs e)
        {
            keep_learning = !keep_learning;
            button74.Text = "learning " + keep_learning;
        }

        private void groupPlay_Enter(object sender, EventArgs e)
        {

        }
    }
}
