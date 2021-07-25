using System;
using System.Security.Cryptography;
using System.Linq;

namespace ConsoleApp1
{


    
    class GameLogic
    {

        public void DefineWinner(int PCMove,int PlayerMove,int length)
        {
            int[,] RuleGraph = RulesOfDefineWinner(length);
            if (RuleGraph[PCMove, PlayerMove] == 1)
                Console.WriteLine("You win!");
            else if (RuleGraph[PCMove, PlayerMove] == -1)
                Console.WriteLine("Computer wins!");
            else
                Console.WriteLine("dead heat!");

        }
        public int[,] RulesOfDefineWinner(int len)
        {
            int[,]WinMatr = new int[len, len];

            //filling in the matrix of determining the winning and losing combinations with the indices of the array of arguments
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {

                    if (i > j && i-j<=len/2 || j-i>=len/2)
                        WinMatr[i, j] = -1;
                    else if (i == j)
                        WinMatr[i, j] = 0;
                    else
                        WinMatr[i, j] = 1;

                }
            }
;            return WinMatr;
        }
        //random move generate
        public (int, string) GenerateMove(string[] moves)
        {
            int CompMoveID = new Random().Next(0, moves.Length);
            (int, string) PCmove = (CompMoveID, moves[CompMoveID]);
            return PCmove;
        }

    }

    class UserInterface
    {

       
        public void  PrintMenu(string[] moves)
        {
            for (int i = 0; i < moves.Length; i++)
            {
                Console.WriteLine(i + 1 + " - " + moves[i]);            
            }
            Console.WriteLine("0 - " + "exit");
        }
        public int GetPlayerMove(string[] moves)
        {
            Console.Write("Enter your move: ");
            string PlayerMove = Console.ReadLine();
            int moveIndex;
            //Is user move correct?
            if (int.TryParse(PlayerMove,out moveIndex) && moveIndex > 0 && moveIndex < moves.Length + 1)
            {
                moveIndex--;
                Console.Write("Your move: " + moves[moveIndex]);
                Console.WriteLine();
                return moveIndex;
            }
            else if (int.TryParse(PlayerMove, out moveIndex) && moveIndex == 0)
            { 
                Environment.Exit(0);
                return 0;

            }
            else
            {
                PrintMenu(moves);
                GetPlayerMove(moves);

                return moveIndex--;
            }
        }
    }
    class Cryptographer
    {

        
        //creates the key that is used in HMAC sign using strong random
        private static string KeyGeneration()  
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] secretkey = new byte[16];
                rng.GetBytes(secretkey);
                return BitConverter.ToString(secretkey).Replace("-", string.Empty).ToLower();
            }

        }

        //sign computer move by key
        public (string, string) SignData(string data) 
        {
            string key = KeyGeneration();
            byte[] bdata = System.Text.Encoding.Default.GetBytes(data);
            byte[] bkey = System.Text.Encoding.Default.GetBytes(key);
            using (HMACSHA256 hmac = new HMACSHA256(bkey))
            {
                byte[] hashValue = hmac.ComputeHash(bdata);
                (string, string)sign = (key, BitConverter.ToString(hashValue).Replace("-", string.Empty).ToLower());
                return sign;
            }

        }
       
    }
    class Program
    {
        static void Main(string[] args)
        {
            //Is args correct? 
            if (args.Length % 2 == 0 || !(args.Distinct().Count() == args.Length) || args.Length < 3)
            {
                Console.WriteLine("There is an error in the entered moves, please enter an odd number of moves more than three, without repetitions, for example:");
                Console.WriteLine("rock paper scissors lizard Spock");
                Environment.Exit(0);
            }
            //in tuple (id,move) randomly generates move of PC
            GameLogic pc = new GameLogic();
            (int,string)PCmove = pc.GenerateMove(args);
            
            Cryptographer crypto = new Cryptographer();
            //tuple (HMAC,key)
            (string, string) sign = crypto.SignData(PCmove.Item2);
            
            Console.WriteLine("HMAC:");
            Console.WriteLine(sign.Item2);

            UserInterface UI = new UserInterface();
            UI.PrintMenu(args);
            int playerMove = UI.GetPlayerMove(args);
            

            Console.Write("Computer move: ");
            Console.WriteLine(PCmove.Item2);

            pc.DefineWinner(PCmove.Item1, playerMove,args.Length);
            

            Console.WriteLine("key:");
            Console.WriteLine(sign.Item1);
            
        }
    
    }

}
