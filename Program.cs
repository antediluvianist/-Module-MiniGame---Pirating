using System;
using System.Threading;

class Program
{
    static void Main()
    {
        LockGame game = new LockGame();
        game.Start();
    }
}

class LockGame
{
    const int NumberOfWheels = 5; // Nombre de rouages
    const int WheelSize = 6;      // Taille de chaque rouage
    const int Duration = 60;      // Durée du jeu en secondes (1 minute)
    char[,] wheels = new char[NumberOfWheels, WheelSize]; // Tableau pour les rouages
    int[] wheelPositions = new int[NumberOfWheels];       // Positions des rouages
    char[] solution = new char[NumberOfWheels];           // Solution des rouages
    bool isPlaying = true;
    DateTime startTime;
    int selectedWheel = 0;

    public void Start()
    {
        Console.CursorVisible = false;
        InitializeWheels();
        startTime = DateTime.Now;

        while (isPlaying && (DateTime.Now - startTime).TotalSeconds < Duration)
        {
            DrawWheels();
            HandleInput();
            CheckWinCondition();
            Thread.Sleep(50); // Réduire le délai de rafraîchissement pour diminuer le clignotement
        }

        Console.Clear();
        if (isPlaying)
            Console.WriteLine($"Temps écoulé ! Vous n'avez pas réussi à déverrouiller le mécanisme à temps.");
        else
            Console.WriteLine($"Félicitations ! Vous avez déverrouillé le mécanisme.");

        Console.WriteLine("\nAppuyez sur n'importe quelle touche pour quitter...");
        Console.ReadKey();
    }

    void InitializeWheels()
    {
        char[] symbols = { '♥', '♦', '♣', '♠', 'X', '♫' }; // Symboles possibles
        var random = new Random();

        for (int i = 0; i < NumberOfWheels; i++)
        {
            for (int j = 0; j < WheelSize; j++)
            {
                wheels[i, j] = symbols[j];
            }
            // Mélanger le rouage
            for (int j = WheelSize - 1; j > 0; j--)
            {
                int k = random.Next(j + 1);
                char temp = wheels[i, j];
                wheels[i, j] = wheels[i, k];
                wheels[i, k] = temp;
            }
            // Définir une solution aléatoire
            solution[i] = wheels[i, random.Next(WheelSize)];
        }
    }

    void DrawWheels()
    {
        Console.SetCursorPosition(0, 0); // Positionner le curseur en haut de la console
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Jeu de Serrure - Déverrouillez en alignant les symboles !");
        Console.WriteLine();

        // Dessiner chaque rouage avec mise à jour sélective
        for (int i = 0; i < NumberOfWheels; i++)
        {
            DrawWheel(i);
            Console.WriteLine(); // Saut de ligne entre chaque rouage
        }

        // Afficher les instructions et le temps restant
        Console.WriteLine("\nUtilisez les flèches Haut et Bas pour déplacer le rouage sélectionné.");
        Console.WriteLine("Utilisez les flèches Gauche et Droite pour changer le rouage sélectionné.");
        Console.WriteLine($"Temps restant : {Duration - (int)(DateTime.Now - startTime).TotalSeconds} secondes");
    }

    void DrawWheel(int wheelIndex)
    {
        int left = Console.CursorLeft;
        int top = Console.CursorTop;

        for (int j = 0; j < WheelSize; j++)
        {
            Console.SetCursorPosition(left + j * 4, top);

            if (j == wheelPositions[wheelIndex])
            {
                if (wheelIndex == selectedWheel)
                {
                    if (j < wheelPositions[wheelIndex] || wheels[wheelIndex, j] != solution[wheelIndex])
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.Write($"[{wheels[wheelIndex, j]}]");
        }

        Console.SetCursorPosition(left + WheelSize * 4, top);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write((wheelIndex == selectedWheel) ? " <" : "  ");
    }

    void HandleInput()
    {
        if (!Console.KeyAvailable)
            return;

        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.UpArrow:
                MoveWheel(selectedWheel, -1);
                break;
            case ConsoleKey.DownArrow:
                MoveWheel(selectedWheel, 1);
                break;
            case ConsoleKey.LeftArrow:
                selectedWheel = (selectedWheel - 1 + NumberOfWheels) % NumberOfWheels;
                break;
            case ConsoleKey.RightArrow:
                selectedWheel = (selectedWheel + 1) % NumberOfWheels;
                break;
        }
    }

    void MoveWheel(int wheelIndex, int direction)
    {
        wheelPositions[wheelIndex] = (wheelPositions[wheelIndex] + direction + WheelSize) % WheelSize;
    }

    void CheckWinCondition()
    {
        for (int i = 0; i < NumberOfWheels; i++)
        {
            if (wheels[i, wheelPositions[i]] != solution[i])
                return;
        }
        isPlaying = false;
    }
}
