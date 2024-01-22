using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExercicioAvaliacaoBatalhaNaval
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int opt; //Variável para escolher opções
            int xDim = 0; //Variáveis para controlo da dimensão do tabuleiro
            string[,] TabJ1 = new string[xDim, xDim]; //Tabuleiro do jogador 1
            string[,] TabJ2 = new string[xDim, xDim]; //Tabuleiro do jogador 2
            string[,] TabC = new string[xDim, xDim]; //Tabuleiro do computador
            bool vitoria = false; //Variável de controlo de vitória
            string Jogador1 = "", Jogador2 = ""; //Variável para nome dos jogadores
            List<Winner> Winners = new List<Winner>(); //Instancia uma lista da class winner
            HighScores(ref Winners); //Carrega a lista de highscores do file
            //Pede ao user para escolher o tabuleiro (idem dificuldade em que quer jogar. Sem isso não é possível jogar.
            Console.WriteLine("*******************************Bem-vindo à Batalha Naval*******************************\nVamos jogar?");
            Console.WriteLine("Primeiro, tens que definir a dimensão do tabuleiro.");
        Playagain:
            EscolherTabuleiro(ref xDim);
            Console.Clear();
            do
            {
                do
                {
                    Console.WriteLine("*******************************Bem-vindo à Batalha Naval*******************************" +
                        "\n1. Jogador vs Jogador\n2. Jogador vs Computador\n3. Mudar de Tabuleiro\n4. Highscores\n5. Regras\n0. Sair");
                    Console.WriteLine("Escolha a opção pretendida:");
                }
                while (!int.TryParse(Console.ReadLine(), out opt) && opt != ' ');
                Console.Clear();
                switch (opt)
                {
                    case 0:
                        Console.WriteLine("Espero que tenhas gostado!");
                        Console.ReadKey();
                        break;
                    case 1:
                        Console.WriteLine("*******************************Player vs Player On*******************************");
                        EscolherPosicoes(ref TabJ1, xDim, ref Jogador1);
                        Console.ReadKey(); Console.Clear();
                        Console.WriteLine("Player 2: É a tua vez!");
                        EscolherPosicoes(ref TabJ2, xDim, ref Jogador2);
                        Console.ReadKey(); Console.Clear();
                        vitoria = false;
                        while (vitoria == false)
                        {
                            Batalha(ref TabJ1, ref TabJ2, ref Jogador1, ref Jogador2, xDim, ref vitoria, ref Winners);
                        }
                        break;
                    case 2:
                        Console.WriteLine("*******************************Player vs Computer On*******************************");
                        EscolherPosicoes(ref TabJ1, xDim, ref Jogador1);
                        Console.Clear();
                        Computer(ref TabC, xDim, ref Jogador2);
                        Console.Clear();
                        vitoria = false;
                        while (vitoria == false)
                        {
                            Batalha(ref TabJ1, ref TabC, ref Jogador1, ref Jogador2, xDim, ref vitoria, ref Winners);
                        }
                        break;
                    case 3:
                        goto Playagain;
                    case 4:
                        ReadingList(ref Winners);
                        Console.ReadKey();
                        break;
                    case 5:
                        Console.WriteLine("Regras:\nTipos de embarcações:\nSubmarinos - 1 posição\nCorvetas - 2 posições\nFragatas - 3 posições\nPorta-aviões - 4 posições" +
                            "\nPodes escolher 2 peças de cada e onde as vais colocar.\nPodes jogar contra outro player ou contra o PC\nPercebido? Estás preparado? Let's go!\nLegenda:");
                        Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("[S] - Submarino\n[C] - Corveta\n[F] - Fragata\n[P] - Porta-Aviões"); Console.ForegroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("[X] - Embarcação Destruída"); Console.ForegroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Blue; Console.WriteLine("[~] - Água"); Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Escolha uma das opções disponíveis. Obrigada.");
                        Console.ReadKey();
                        break;
                }
                Console.Clear();

            } while (opt != 0);

            Console.ReadKey();
        }
        public static void EscolherTabuleiro(ref int xDim)
        {
            int tabuleiro;
            do
            {
                do
                {
                    Console.WriteLine("Escolha do Tabuleiro:\n1. Fácil-10x10\n2. Intermédio-15x15\n3. Difícil-20x20");
                    Console.WriteLine("Escolha a opção pretendida:");
                }
                while (!int.TryParse(Console.ReadLine(), out tabuleiro));

                switch (tabuleiro)
                {
                    case 1:
                        xDim = 10;
                        break;
                    case 2:
                        xDim = 15;
                        break;
                    case 3:
                        xDim = 20;
                        break;
                    default:
                        Console.WriteLine("Escolha uma das opções disponíveis. Obrigada.");
                        Console.ReadKey();
                        break;
                };
            } while (tabuleiro != 1 && tabuleiro != 2 && tabuleiro != 3);
        }
        public static void EscolherPosicoes(ref string[,] tab, int xDim, ref string jogador)
        {
            int turn = 0, line, column, ContSub = 0, ContFrag = 0, ContCorv = 0, ContPort = 0; //Contadores
            tab = new string[xDim, xDim]; //Tabuleiro do jogador
            string direccao = " ", peca; //Variáveis para a direção da peca e para a peca em si
            char[] letras = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T' }; //Array de letras para determinar qual a letra que o user insere
            do
            {
                Console.Write("Nome do jogador: ");
                jogador = Console.ReadLine();
                if (jogador == "Computer")
                    Console.WriteLine("Palavra reservada. Escolhe outro nome!");
            } while (string.IsNullOrEmpty(jogador) || jogador == "Computer"); //Não deixa o jogador escolher o nome Computer; está reservado para o computador

            do
            {
                Console.WriteLine($"Tabuleiro de {jogador}");
                LerTabuleiroPreenchido(ref tab, ref xDim);
            PecaMais:
                Console.WriteLine($"Insere a coordenada inicial:");
                do
                {//Do..While para o user escolher a embarcação
                    Console.Write("Pretende jogar com S-Submarinos, C-Corvetas, F-Fragatas, ou P-Porta-Aviões? ");
                    peca = Console.ReadLine().ToUpper();
                } while (peca != "S" && peca != "F" && peca != "P" && peca != "C");
                if (peca == "C" || peca == "F" || peca == "P")
                {//Caso a peça escolhida for uma corveta, uma fragata ou um porta-aviões, o user escolhe se pretende que a embarcação esteja na vertical ou na horizontal
                    do
                    {
                        Console.Write("Escolhe:\nV - Vertical\nH - Horizontal\n");
                        direccao = Console.ReadLine().ToUpper();
                    } while (direccao != "V" && direccao != "H");
                }

            Loop: //Enquanto as linhas/colunas estiverem vazias, não forem chars reconhecidos/inteiros, forem maiores que a dimensão da matriz ou menores ou iguais a 0, pede linha/coluna
                do
                {  
                    line = 0; //Variável para passar a letra para a linha correspondente
                    char letra; //variável que recebe a letra introduzida pelo user
                    do
                    {
                        Console.Write($"Linha: ");
                    }
                    while (!char.TryParse(Console.ReadLine().ToUpper(), out letra));

                    for (int i = 0; i < xDim; i++)
                    {
                        if (letras[i] == letra) //Caso a letra inserida faça parte do array retorna verdadeiro
                            line = i + 1; //Indice da letra + 1
                    }

                    do
                    {
                        Console.Write($"Coluna: ");
                    }
                    while (!int.TryParse(Console.ReadLine(), out column) && column != ' ');
                    //Atenção: Solicitado ao user que insira a coluna "real" - e.g., A,1 para o user, 0,0 para o pc

                    //Controla o nr de embarcações de cada, só permitindo inserir duas de cada
                    if (peca == "S" && ContSub == 2)
                    {
                        Console.WriteLine("Só podes inserir dois submarinos! Escolhe outra embarcação!");
                        goto PecaMais;
                    }
                    if (peca == "C" && ContCorv == 2)
                    {
                        Console.WriteLine("Só podes inserir duas corvetas! Escolhe outra embarcação!");
                        goto PecaMais;
                    }
                    if (peca == "F" && ContFrag == 2)
                    {
                        Console.WriteLine("Só podes inserir duas fragatas! Escolhe outra embarcação!");
                        goto PecaMais;
                    }
                    if (peca == "P" && ContPort == 2)
                    {
                        Console.WriteLine("Só podes inserir dois porta-aviões! Escolhe outra embarcação!");
                        goto PecaMais;
                    }
                    if ((peca == "C" && (line > xDim - 1)) && direccao == "V")
                    {
                        //Caso se a linha introduzida for maior que a dimensão - 2 (!!! -1 da introdução + -1 do if), é impossível colocar a corveta no tabuleiro
                        //Controlo se a corveta saí fora do tabuleiro
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("A tua corveta vai sair fora do tabuleiro!\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        goto Loop;
                    }
                    if ((peca == "C" && (column > xDim - 1)) && direccao == "H")
                    {
                        //Caso se a coluna introduzida for maior que a dimensão - 2 (!!! -1 da introdução + -1 do if), é impossível colocar a corveta no tabuleiro
                        //Controlo se a corveta saí fora do tabuleiro
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("A tua corveta vai sair fora do tabuleiro!\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        goto Loop;
                    }
                    if ((peca == "F" && (line > xDim - 2)) && direccao == "V")
                    {
                        //Controlo se a fragata saí fora do tabuleiro
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("A tua fragata vai sair fora do tabuleiro!\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        goto Loop;
                    }
                    if ((peca == "F" && (column > xDim - 2)) && direccao == "H")
                    {
                        //Controlo se a fragata saí fora do tabuleiro
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("A tua fragata vai sair fora do tabuleiro!\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        goto Loop;
                    }
                    if ((peca == "P" && (line > xDim - 3)) && direccao == "V")
                    {
                        //Controlo se o porta-aviões saí fora do tabuleiro 
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write("O teu porta-aviões vai sair fora do tabuleiro!\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        goto Loop;
                    }
                    if ((peca == "P" && (column > xDim - 3)) && direccao == "H")
                    { //Controlo se o porta-aviões saí fora do tabuleiro 
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write("O teu porta-aviões vai sair fora do tabuleiro!\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        goto Loop;
                    }
                } while (line > (xDim) || column > (xDim) || (line <= 0) || (column <= 0));
                if (peca == "S") //Caso a peça seja um submarino, pesquisa se a posição pretendida se encontra ocupada por outra embarcação
                {
                    if (tab[(line - 1), (column - 1)] == "[S]" || tab[(line - 1), (column - 1)] == "[C]"
                        || tab[(line - 1), (column - 1)] == "[F]" || tab[(line - 1), (column - 1)] == "[P]")
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("Já existe um barco a ocupar essa posição!\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        goto Loop;
                    }
                }
                if (peca == "C") //Caso a peça seja uma corveta, pesquisa se a posição pretendida se encontra ocupada por outra embarcação, assim como as posições seguintes na coluna ou linha
                {
                    //Procura nas colunas
                    if (direccao == "H")
                    {
                        if (tab[line - 1, column - 1] == "[C]" || tab[line - 1, column - 1] == "[S]" || tab[line - 1, column - 1] == "[F]" || tab[line - 1, column - 1] == "[P]"
                           || tab[line - 1, column] == "[C]" || tab[line - 1, column] == "[S]" || tab[line - 1, column] == "[F]" || tab[line - 1, column] == "[P]")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("Já existe um barco a ocupar umas das duas posições pretendidas!\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            goto Loop;
                        }
                    }//Procura linhas
                    if (direccao == "V")
                    {
                        if (tab[line - 1, column - 1] == "[C]" || tab[line - 1, column - 1] == "[S]" || tab[line - 1, column - 1] == "[F]" || tab[line - 1, column - 1] == "[P]"
                           || tab[line, column - 1] == "[C]" || tab[line, column - 1] == "[S]" || tab[line, column - 1] == "[F]" || tab[line, column - 1] == "[P]")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("Já existe um barco a ocupar umas das duas posições pretendidas!\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            goto Loop;
                        }
                    }
                }
                if (peca == "F") //Caso a peça seja uma fragata, pesquisa se a posição pretendida se encontra ocupada por outra embarcação, assim como as duas posições seguintes na coluna ou linha
                {
                    //Procura colunas
                    if (direccao == "H")
                    {
                        if (tab[line - 1, column - 1] == "[C]" || tab[line - 1, column - 1] == "[S]" || tab[line - 1, column - 1] == "[F]" || tab[line - 1, column - 1] == "[P]"
                            || tab[line - 1, column] == "[C]" || tab[line - 1, column] == "[S]" || tab[line - 1, column] == "[F]" || tab[line - 1, column] == "[P]"
                            || tab[line - 1, column + 1] == "[C]" || tab[line - 1, column + 1] == "[S]" || tab[line - 1, column + 1] == "[F]" || tab[line - 1, column + 1] == "[P]")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.Write("Já existe um barco a ocupar umas das três posições pretendidas!\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            goto Loop;
                        }
                    } //Procura linhas
                    if (direccao == "V")
                    {
                        if (
                           tab[line - 1, column - 1] == "[C]" || tab[line - 1, column - 1] == "[S]" || tab[line - 1, column - 1] == "[F]" || tab[line - 1, column - 1] == "[P]"
                           || tab[line, column - 1] == "[C]" || tab[line, column - 1] == "[S]" || tab[line, column - 1] == "[F]" || tab[line, column - 1] == "[P]"
                           || tab[line + 1, column - 1] == "[C]" || tab[line + 1, column - 1] == "[S]" || tab[line + 1, column - 1] == "[F]" || tab[line + 1, column - 1] == "[P]")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.Write("Já existe um barco a ocupar umas das duas posições pretendidas!\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            goto Loop;
                        }
                    }
                }
                if (peca == "P") //Caso a peça seja um porta-aviões, pesquisa se a posição pretendida se encontra ocupada por outra embarcação, assim como as três posições seguintes na coluna ou linha
                {
                    //Procura colunas
                    if (direccao == "H")
                    {
                        if (tab[line - 1, column - 1] == "[C]" || tab[line - 1, column - 1] == "[S]" || tab[line - 1, column - 1] == "[F]" || tab[line - 1, column - 1] == "[P]"
                         || tab[line - 1, column] == "[C]" || tab[line - 1, column] == "[S]" || tab[line - 1, column] == "[F]" || tab[line - 1, column] == "[P]"
                         || tab[line - 1, column + 1] == "[C]" || tab[line - 1, column + 1] == "[S]" || tab[line - 1, column + 1] == "[F]" || tab[line - 1, column + 1] == "[P]"
                         || tab[line - 1, column + 2] == "[C]" || tab[line - 1, column + 2] == "[S]" || tab[line - 1, column + 2] == "[F]" || tab[line - 1, column + 2] == "[P]")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.Write("Já existe um barco a ocupar umas das quatro posições pretendidas!\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            goto Loop;
                        }
                    }//Procura linhas
                    if (direccao == "V")
                    {
                        if (tab[line - 1, column - 1] == "[C]" || tab[line - 1, column - 1] == "[S]" || tab[line - 1, column - 1] == "[F]" || tab[line - 1, column - 1] == "[P]"
                         || tab[line, column - 1] == "[C]" || tab[line, column - 1] == "[S]" || tab[line, column - 1] == "[F]" || tab[line, column - 1] == "[P]"
                         || tab[line + 1, column - 1] == "[C]" || tab[line + 1, column - 1] == "[S]" || tab[line + 1, column - 1] == "[F]" || tab[line + 1, column - 1] == "[P]"
                         || tab[line + 2, column - 1] == "[C]" || tab[line + 2, column - 1] == "[S]" || tab[line + 2, column - 1] == "[F]" || tab[line + 2, column - 1] == "[P]")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.Write("Já existe um barco a ocupar umas das duas posições pretendidas!\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            goto Loop;
                        }
                    }
                }
                Console.ReadKey(); Console.Clear();
                //Depois de feitos os controlos, preenche a matriz com as pecas em cada posição
                if (peca == "S")//Caso a peça seja submarino, preenche a posição linha-1,coluna-1 da matriz
                {
                    tab[(line - 1), (column - 1)] = "[S]";
                    ContSub++;

                }
                if (direccao == "V")//Caso a direcção seja vertical
                {
                    if (peca == "C")//Caso a peça seja corveta, preenche as posições linha-1,coluna-1 e a linha,coluna-1 da matriz
                    {
                        tab[line - 1, column - 1] = "[C]";
                        tab[line, column - 1] = "[C]";
                        ContCorv++;
                    }
                    if (peca == "F")//Caso a peça seja fragata, preenche as posições linha-1,coluna-1, a linha,coluna-1 e a linha+1,coluna-1 da matriz
                    {
                        tab[line - 1, column - 1] = "[F]";
                        tab[line, column - 1] = "[F]";
                        tab[line + 1, column - 1] = "[F]";
                        ContFrag++;
                    }
                    if (peca == "P")//Caso a peça seja porta-aviões, preenche as posições linha-1,coluna-1, a linha,coluna-1, a linha+1,coluna-1 e a linha+2,coluna-1 da matriz
                    {
                        tab[line - 1, column - 1] = "[P]";
                        tab[line, column - 1] = "[P]";
                        tab[line + 1, column - 1] = "[P]";
                        tab[line + 2, column - 1] = "[P]";
                        ContPort++;
                    }
                }
                if (direccao == "H")
                {
                    if (peca == "C")
                    {
                        tab[line - 1, column - 1] = "[C]";
                        tab[line - 1, column] = "[C]";
                        ContCorv++;
                    }
                    if (peca == "F")
                    {
                        tab[line - 1, column - 1] = "[F]";
                        tab[line - 1, column] = "[F]";
                        tab[line - 1, column + 1] = "[F]";
                        ContFrag++;
                    }
                    if (peca == "P")
                    {
                        tab[line - 1, column - 1] = "[P]";
                        tab[line - 1, column] = "[P]";
                        tab[line - 1, column + 1] = "[P]";
                        tab[line - 1, column + 2] = "[P]";
                        ContPort++;
                    }
                }
                turn++;
            } while (turn < 8); //Enquanto não estiverem registados 8 barcos; Turn: 0-7 = 8
            Console.ReadKey();
        }
        public static void Computer(ref string[,] tab, int xDim, ref string jogador)
        {
            tab = new string[xDim, xDim]; //Matriz do tabuleiro do PC
            string[] peca = new string[8]; //Array para armazenar a letra de cada embarcação
            int[] pecas = { 1, 1, 2, 2, 3, 3, 4, 4 }; //Array para armazenar as embarcações sendo 1-Submarino, 2-Corveta, 3-Fragata, 4-Porta-Aviões
            int[] direction = new int[8]; //Array para armazenar as direcções de cada embarcação
            Random npecas = new Random(); //Objeto random para gerar números aleatórios que vão determinar as posições a jogar
            jogador = "Computer"; //Variavél com o nome do jogador

            for (int i = 0; i < 8; i++)
            {
                direction[i] = npecas.Next(1, 3); //Gera as direções das embarcações entre 1 e 2, 1 - Vertical, 2 - Horizontal
                switch (pecas[i]) //Para cada caso, atribui a letra devida a cada embarcação
                {
                    case 1:
                        peca[i] = "[S]";
                        break;
                    case 2:
                        peca[i] = "[C]";
                        break;
                    case 3:
                        peca[i] = "[F]";
                        break;
                    case 4:
                        peca[i] = "[P]";
                        break;
                }
            }

            int contpecas = 0; //Contador para o nr de barcos
            while (contpecas < 8)
            {
            Loop:
                int x = npecas.Next(0, xDim), y = npecas.Next(0, xDim); //Gera posições aleatórias entre 0 e a dimensão do tabuleiro - 1 (!!)

                for (int i = 0; i < xDim; i++)
                {
                    if (peca[contpecas] == "[C]" && (x > xDim - 2) && direction[contpecas] == 1)
                        //Controlo de se a corveta saí fora do tabuleiro na vertical
                        goto Loop;
                    if (peca[contpecas] == "[C]" && (y > xDim - 2) && direction[contpecas] == 2)
                        //Controlo de se a corveta saí fora do tabuleiro na horizontal
                        goto Loop;
                    if (peca[contpecas] == "[F]" && (x > xDim - 3) && direction[contpecas] == 1)
                        //Controlo de se a fragata saí fora do tabuleiro na vertical
                        goto Loop;
                    if (peca[contpecas] == "[F]" && (y > xDim - 3) && direction[contpecas] == 2)
                        //Controlo de se a fragata saí fora do tabuleiro na horizontal
                        goto Loop;
                    if (peca[contpecas] == "[P]" && (x > xDim - 4) && direction[contpecas] == 1)
                        //Controlo de se o porta-aviões saí fora do tabuleiro na vertical
                        goto Loop;
                    if (peca[contpecas] == "[P]" && (y > xDim - 4) && direction[contpecas] == 2)
                        //Controlo de se o porta-aviões saí fora do tabuleiro na horizontal
                        goto Loop;

                    if (peca[contpecas] == "[S]") //Controlo se a posição pretendida já está preenchida
                    {
                        if (tab[x, y] == "[S]" || tab[x, y] == "[C]" || tab[x, y] == "[F]" || tab[x, y] == "[P]")
                        {
                            goto Loop;
                        }
                    }
                    if (peca[contpecas] == "[C]")
                    {
                        //Procura colunas
                        if (direction[contpecas] == 1)
                        {
                            if (tab[x, y] == "[C]" || tab[x, y] == "[S]" || tab[x, y] == "[F]" || tab[x, y] == "[P]"
                               || tab[x + 1, y] == "[C]" || tab[x + 1, y] == "[S]" || tab[x + 1, y] == "[F]" || tab[x + 1, y] == "[P]")
                            {
                                goto Loop;
                            }
                        }
                        //Procura linhas 
                        if (direction[contpecas] == 2)
                        {
                            if (tab[x, y] == "[C]" || tab[x, y] == "[S]" || tab[x, y] == "[F]" || tab[x, y] == "[P]"
                               || tab[x, y + 1] == "[C]" || tab[x, y + 1] == "[S]" || tab[x, y + 1] == "[F]" || tab[x, y + 1] == "[P]")
                            {
                                goto Loop;
                            }
                        }
                    }
                    if (peca[contpecas] == "[F]")
                    {
                        //Procura colunas
                        if (direction[contpecas] == 1)
                            if (tab[x, y] == "[C]" || tab[x, y] == "[S]" || tab[x, y] == "[F]" || tab[x, y] == "[P]"
                            || tab[x + 1, y] == "[C]" || tab[x + 1, y] == "[S]" || tab[x + 1, y] == "[F]" || tab[x + 1, y] == "[P]"
                            || tab[x + 2, y] == "[C]" || tab[x + 2, y] == "[S]" || tab[x + 2, y] == "[F]" || tab[x + 2, y] == "[P]")
                            {
                                goto Loop;
                            }
                        //Procura linhas
                        if (direction[contpecas] == 2)
                        {
                            if (tab[x, y] == "[C]" || tab[x, y] == "[S]" || tab[x, y] == "[F]" || tab[x, y] == "[P]"
                            || tab[x, y + 1] == "[C]" || tab[x, y + 1] == "[S]" || tab[x, y + 1] == "[F]" || tab[x, y + 1] == "[P]"
                            || tab[x, y + 2] == "[C]" || tab[x, y + 2] == "[S]" || tab[x, y + 2] == "[F]" || tab[x, y + 2] == "[P]")
                            {
                                goto Loop;
                            }
                        }
                    }
                    if (peca[contpecas] == "[P]")
                    {
                        if (direction[contpecas] == 1)
                            //Procura colunas
                            if (tab[x, y] == "[C]" || tab[x, y] == "[S]" || tab[x, y] == "[F]" || tab[x, y] == "[P]"
                               || tab[x + 1, y] == "[C]" || tab[x + 1, y] == "[S]" || tab[x + 1, y] == "[F]" || tab[x + 1, y] == "[P]"
                               || tab[x + 2, y] == "[C]" || tab[x + 2, y] == "[S]" || tab[x + 2, y] == "[F]" || tab[x + 2, y] == "[P]"
                               || tab[x + 3, y] == "[C]" || tab[x + 3, y] == "[S]" || tab[x + 3, y] == "[F]" || tab[x + 3, y] == "[P]")
                            {
                                goto Loop;
                            }
                        //Procura linhas
                        if (direction[contpecas] == 2)
                        {
                            if (tab[x, y] == "[C]" || tab[x, y] == "[S]" || tab[x, y] == "[F]" || tab[x, y] == "[P]"
                            || tab[x, y + 1] == "[C]" || tab[x, y + 1] == "[S]" || tab[x, y + 1] == "[F]" || tab[x, y + 1] == "[P]"
                            || tab[x, y + 2] == "[C]" || tab[x, y + 2] == "[S]" || tab[x, y + 2] == "[F]" || tab[x, y + 2] == "[P]"
                            || tab[x, y + 3] == "[C]" || tab[x, y + 3] == "[S]" || tab[x, y + 3] == "[F]" || tab[x, y + 3] == "[P]")
                            {
                                goto Loop;
                            }
                        }
                    }

                    if (peca[contpecas] == "[S]")
                    {
                        tab[x, y] = "[S]";
                    }
                    if (direction[contpecas] == 1)
                    {
                        if (peca[contpecas] == "[C]")
                        {
                            tab[x, y] = "[C]";
                            tab[x + 1, y] = "[C]";
                        }
                        if (peca[contpecas] == "[F]")
                        {
                            tab[x, y] = "[F]";
                            tab[x + 1, y] = "[F]";
                            tab[x + 2, y] = "[F]";
                        }
                        if (peca[contpecas] == "[P]")
                        {
                            tab[x, y] = "[P]";
                            tab[x + 1, y] = "[P]";
                            tab[x + 2, y] = "[P]";
                            tab[x + 3, y] = "[P]";
                        }
                    }
                    if (direction[contpecas] == 2)
                    {
                        if (peca[contpecas] == "[C]")
                        {
                            tab[x, y] = "[C]";
                            tab[x, y + 1] = "[C]";
                        }
                        if (peca[contpecas] == "[F]")
                        {
                            tab[x, y] = "[F]";
                            tab[x, y + 1] = "[F]";
                            tab[x, y + 2] = "[F]";
                        }
                        if (peca[contpecas] == "[P]")
                        {
                            tab[x, y] = "[P]";
                            tab[x, y + 1] = "[P]";
                            tab[x, y + 2] = "[P]";
                            tab[x, y + 3] = "[P]";
                        }
                    }
                    contpecas++;
                    if (contpecas == 8)
                    { break;}
                }
            }
            //Linhas comentadas usadas para controlo da versão do PC
            //Console.WriteLine($"Tabuleiro de {jogador}");
            //LerTabuleiroPreenchido(ref tab, ref xDim);
            //Console.ReadKey();
        }
        public static void LerTabuleiroPreenchido(ref string[,] tab, ref int xDim)
        {
            char letra = 'A';
            int x = 1;
            Console.Write("  ");
            do
            {
                Console.ForegroundColor= ConsoleColor.DarkCyan;
                Console.Write($"{x}".PadLeft(3));
                x++;
                Console.ForegroundColor = ConsoleColor.White;
            } while (x <= xDim);
            Console.WriteLine();

            for (int i = 0; i < xDim; i++)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"{letra} ");
                Console.ForegroundColor = ConsoleColor.White;
                for (int j = 0; j < xDim; j++)
                {
                    if (tab[i, j] == "[S]" || tab[i, j] == "[C]" || tab[i, j] == "[F]" || tab[i, j] == "[P]")
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"{tab[i, j]}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (tab[i, j] == "[X]")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"{tab[i, j]}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (tab[i, j] == "[~]")
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write($"{tab[i, j]}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                        Console.Write($"[-]");
                }
                letra++;
                Console.WriteLine();
            }

        }
        public static void LerTabuleiroPreenchidoAdv(ref string[,] tab, ref int xDim)
        {
            char letra = 'A';
            int x = 1;
            Console.Write("  ");
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"{x}".PadLeft(3));
                x++;
                Console.ForegroundColor = ConsoleColor.White;
            } while (x <= xDim);
            Console.WriteLine();

            for (int i = 0; i < xDim; i++)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"{letra} ");
                Console.ForegroundColor = ConsoleColor.White;
                for (int j = 0; j < xDim; j++)
                {
                    if (tab[i, j] == "[X]")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"{tab[i, j]}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (tab[i, j] == "[~]")
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write($"[~]");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                        Console.Write($"[-]");
                }
                letra++;
                Console.WriteLine();
            }
        }
        public static void Batalha(ref string[,] tab, ref string[,] tab2, ref string jogador, ref string jogador2, int xDim, ref bool winner, ref List<Winner> Winners)
        {
            int line, column, tries = 0, tries2 = 0, wintries = 0, wintries2 = 0, player; //Variáveis para a linha, coluna, tentativas do jogador1, tentativas do jogador2, tentativas vencedoras do jogador1, tentativas vencedores do jogador2 e nr do jogador 1/2
            bool jogada = true; //Variável que controla quem joga: jogador 1 ou jogador 2
            winner = false; //Variável que controla quem ganha - passagem por parâmetro à main
            Random npecas = new Random(); //Instanciação de um objeto random para gerar posição a atacar quando J2 é o Computer
            Winner win = new Winner(); //Instanciação de um objeto da class Winner
            char[] letras = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T' }; //Array de letras para conversões
            char letra; //Variável de letra a receber dos users

            FileInfo fich = new FileInfo(@"C:\Ficheiros\ListWinnerUnordered.txt"); //Ficheiro da Lista de Vencedores - Not Sorted
            StreamWriter writer = fich.AppendText();
            if (File.Exists(@"C:\Ficheiros\ListWinnerUnordered.txt")) { } //Se o ficheiro já existir, não cria novamente
            else
            {//Caso contrário, cria o file
                FileStream fstr = fich.Create();
                fstr.Close();
            }
            player = 1; //Começa o jogador 1 a jogar

            while (jogada == true && winner == false) //Enquanto for a sua vez e não houver um vencedor
            {
                while (winner == false) 
                {
                    if (player == 1) //Caso o player seja o jogador 1
                    {
                        Console.WriteLine($"Tabuleiro de {jogador}");
                        LerTabuleiroPreenchido(ref tab, ref xDim); //Mostra o tabuleiro do jogador 1
                        Console.WriteLine($"Tabuleiro de {jogador2}");
                        LerTabuleiroPreenchidoAdv(ref tab2, ref xDim);//Mostra o tabuleiro do jogador 2 - user or pc
                        Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("\nLegenda:"); Console.ForegroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("[S] - Submarino\n[C] - Corveta\n[F] - Fragata\n[P] - Porta-Aviões"); Console.ForegroundColor=ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("[X] - Embarcação Destruída"); Console.ForegroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Blue; Console.WriteLine("[~] - Água"); Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"{jogador}");
                        Console.WriteLine($"Escolha as suas posições pelas coordenadas:");
                        line = 0;
                        do
                        {
                            Console.Write($"Linha: ");
                        }
                        while (!char.TryParse(Console.ReadLine().ToUpper(), out letra));
                        for (int i = 0; i < xDim; i++)
                        {
                            if (letras[i] == letra)
                                line = i + 1;
                        }
                        do
                        {
                            Console.Write($"Coluna: ");
                        }
                        while (!int.TryParse(Console.ReadLine(), out column) && column != ' ');
                        //Controlo se o jogador joga dentro do tabuleiro
                        if (line > (xDim) || column > (xDim) || line <= 0 || column <= 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write("Jogada fora do tabuleiro. Escolhe uma nova linha ou coluna:\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (tab2[line - 1, column - 1] == "[S]") //Caso acerte num submarino
                        {
                            Console.Write("Afundou um submarino!");
                            tab2[line - 1, column - 1] = "[X]";
                            tries++;
                            wintries++;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (tab2[line - 1, column - 1] == "[C]")//Caso acerte numa corveta
                        {
                            Console.Write("Afundou parte de uma corveta!");
                            tab2[line - 1, column - 1] = "[X]";
                            tries++;
                            wintries++;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (tab2[line - 1, column - 1] == "[F]")//Caso acerte numa fragata
                        {
                            Console.Write("Afundou parte de uma fragata!");
                            tab2[line - 1, column - 1] = "[X]";
                            tries++;
                            wintries++;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (tab2[line - 1, column - 1] == "[P]")//Caso acerte num porta-aviões
                        {
                            Console.Write("Afundou parte de um porta-aviões!");
                            tab2[line - 1, column - 1] = "[X]";
                            tries++;
                            wintries++;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (tab2[line - 1, column - 1] == "[X]")//Caso tente abater um navio já destruído
                        {
                            Console.Write("Já afundou este navio!");
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (tab2[line - 1, column - 1] == "[~]")//Caso caía em água que já caiu
                        {
                            Console.Write("Está a nadar!");
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            Console.WriteLine("Foi à àgua!");//Caso vá à água
                            tab2[line - 1, column - 1] = "[~]";
                            tries++;
                            player = 2; //Troca o nr do jogador
                            jogada = false;//Jogada passa a falsa e troca de jogador
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                    else if (player == 2 && jogador2 != "Computer")
                    {
                        Console.WriteLine($"Tabuleiro de {jogador2}");
                        LerTabuleiroPreenchido(ref tab2, ref xDim);
                        Console.WriteLine($"Tabuleiro de {jogador}");
                        LerTabuleiroPreenchidoAdv(ref tab, ref xDim);
                        Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("\nLegenda:"); Console.ForegroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("[S] - Submarino\n[C] - Corveta\n[F] - Fragata\n[P] - Porta-Aviões"); Console.ForegroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("[X] - Embarcação Destruída"); Console.ForegroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Blue; Console.WriteLine("[~] - Água"); Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"{jogador2}");
                        Console.WriteLine($"Escolha as suas posições pelas coordenadas:");
                        line = 0;
                        do
                        {
                            Console.Write($"Linha: ");
                        }
                        while (!char.TryParse(Console.ReadLine().ToUpper(), out letra));
                        for (int i = 0; i < xDim; i++)
                        {
                            if (letras[i] == letra)
                                line = i + 1;
                        }
                        do
                        {
                            Console.Write($"Coluna: ");
                        }
                        while (!int.TryParse(Console.ReadLine(), out column) && column != ' ');

                        if (line > xDim || column > xDim || line <= 0 || column <= 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("Jogada fora do tabuleiro. Escolhe uma nova linha ou coluna:");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (tab[line - 1, column - 1] == "[S]")
                        {
                            Console.Write("Afundou um submarino!");
                            tab[line - 1, column - 1] = "[X]";
                            tries2++;
                            wintries2++;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (tab[(line - 1), (column - 1)] == "[C]")
                        {
                            Console.Write("Afundou parte de uma corveta!");
                            tab[line - 1, column - 1] = "[X]";
                            tries2++;
                            wintries2++;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (tab[(line - 1), (column - 1)] == "[F]")
                        {
                            Console.Write("Afundou parte de uma fragata!");
                            tab[line - 1, column - 1] = "[X]";
                            tries2++;
                            wintries2++;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (tab[line - 1, column - 1] == "[P]")
                        {
                            Console.Write("Afundou parte de um porta-aviões!");
                            tab[line - 1, column - 1] = "[X]";
                            tries2++;
                            wintries2++;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else if (tab[line - 1, column - 1] == "[X]")
                        {
                            Console.Write("Já afundou este navio!");
                            Console.Clear();
                        }
                        else if (tab[line - 1, column - 1] == "[~]")
                        {
                            Console.Write("Está a nadar!"); ;
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            Console.WriteLine("Foi à àgua!");
                            tab[line - 1, column - 1] = "[~]";
                            tries2++;
                            player = 1;
                            jogada = false;
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                    else if (player == 2 && jogador2 == "Computer")
                    {
                        //Linhas comentadas usadas para controlo da versão de PC 
                        //Console.WriteLine($"Tabuleiro de {jogador2}");
                        //LerTabuleiroPreenchido(ref tab2, ref xDim);
                        //Console.WriteLine($"Tabuleiro de {jogador}");
                        //LerTabuleiroPreenchidoAdv(ref tab, ref xDim);
                        //Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("\nLegenda:"); Console.ForegroundColor = ConsoleColor.White;
                        //Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("[S] - Submarino\n[C] - Corveta\n[F] - Fragata\n[P] - Porta-Aviões"); Console.ForegroundColor = ConsoleColor.White;
                        //Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("[X] - Embarcação Destruída"); Console.ForegroundColor = ConsoleColor.White;
                        //Console.ForegroundColor = ConsoleColor.Blue; Console.WriteLine("[~] - Água"); Console.ForegroundColor = ConsoleColor.White;

                        line = npecas.Next(0, xDim);
                        column = npecas.Next(0, xDim);
                        //letra = ' ';
                        //for (int i = 0; i <= xDim; i++)
                        //{
                        //    if (i == line)
                        //        letra = letras[i];
                        //}
                        //Console.WriteLine($"Linha: " + letra);
                        //Console.WriteLine($"Coluna: " + (column + 1));

                        if (tab[line, column] == "[S]")
                        {
                            //Console.Write("O Computer afundou um submarino!");
                            tab[line, column] = "[X]";
                            tries2++;
                            wintries2++;
                            //Console.ReadKey();
                            //Console.Clear();
                        }
                        else if (tab[line, column] == "[C]")
                        {
                            //Console.Write("O Computer afundou parte de uma corveta!");
                            tab[line, column] = "[X]";
                            tries2++;
                            wintries2++;
                            //Console.ReadKey();
                            //Console.Clear();
                        }
                        else if (tab[line, column] == "[F]")
                        {
                            //Console.Write("O Computer afundou parte de uma fragata!");
                            tab[line, column] = "[X]";
                            tries2++;
                            wintries2++;
                            //Console.ReadKey();
                            //Console.Clear();
                        }
                        else if (tab[line, column] == "[P]")
                        {
                            //Console.Write("O computer afundou parte de um porta-aviões!");
                            tab[line, column] = "[X]";
                            tries2++;
                            wintries2++;
                            //Console.ReadKey();
                            //Console.Clear();
                        }
                        else if (tab[line, column] == "[X]")
                        {
                            //Console.Write("O computer já afundou este navio!");
                            tab[line, column] = "[X]";
                        }
                        else if (tab[line, column] == "[~]")
                        {
                            //Console.Write("O Computer está a nadar!");
                        }
                        else
                        {
                            //Console.WriteLine("Foi à àgua!");
                            tab[line, column] = "[~]";
                            tries2++;
                            player = 1;
                            jogada = false;
                            //Console.ReadKey();
                            //Console.Clear();
                        }
                    }
                    if (wintries == 20)
                    {
                        win.Winners = jogador;
                        win.Tries = tries;
                        win.Difficulty = xDim * xDim;
                        jogada = false;
                        winner = true;
                        Winners.Add(win);
                    }
                    else if (wintries2 == 20)
                    {
                        win.Winners = jogador2;
                        win.Tries = tries2;
                        win.Difficulty = xDim * xDim;
                        jogada = false;
                        winner = true;
                        Winners.Add(win);
                    }
                }
            }

            writer.WriteLine($"{win.Winners} - {win.Tries} | {win.Difficulty}"); //Escreve as informações do Winner no file
            writer.Close();//Fecha o StreamWriter para que possa voltar a ser utilizado
            //Anuncia o vencedor
            Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"O jogador {win.Winners} venceu com {win.Tries}/{win.Difficulty}."); Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }
        public static void HighScores(ref List<Winner> Winners)
        {
            //Função que lê a lista de vencedores se existir
            if (File.Exists(@"C:\Ficheiros\ListWinnerUnordered.txt"))
            {
                StreamReader reader = new StreamReader(@"C:\Ficheiros\ListWinnerUnordered.txt");
                string linha = null;

                while ((linha = reader.ReadLine()) != null)
                {
                    Winner win = new Winner();
                    win.AddWinner(linha.Substring(0, linha.IndexOf("-")),
                        int.Parse(linha.Substring(linha.IndexOf("-") + 1, (linha.IndexOf("|") - linha.IndexOf("-")) - 1)),
                        int.Parse(linha.Substring(linha.IndexOf("|") + 1)));
                    Winners.Add(win);
                }
                reader.Close();
            }
        }
        public static void ReadingList(ref List<Winner> Winners)
        {
            //Função que ordena a lista de vencedores e mostra os 25 primeiros resultados
            Console.WriteLine("*******************************Lista de HighScores*******************************");
            int podium = 1;
            foreach (Winner W in Winners.OrderBy(x => x.Tries).OrderByDescending(x => x.Difficulty).Take(25))
            {
                string level = "";
                if (W.Difficulty == 100)
                    level = "Fácil";
                if (W.Difficulty == 225)
                    level = "Intermédio";
                if (W.Difficulty == 400)
                    level = "Difícil";
                Console.WriteLine($"{podium}º : {W.Winners} - {W.Tries} | {level}");
                podium++;
            }
        }
        public class Winner
        {
            public string Winners { get; set; }
            public int Tries { get; set; }
            public int Difficulty { get; set; }

            public void AddWinner(string winners, int tries, int difficulty)
            {
                Winners = winners;
                Tries = tries;
                Difficulty = difficulty;
            }
        }
    }
}