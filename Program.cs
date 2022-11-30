using CardManagement;

string path = Path.Join("..", "Decks");
var GM = new GameMaster(path);

while (true)
{
    GM.PrintMenu();
    string? Selection = Console.ReadLine().ToUpper();

    if (Selection == "D")
    {
        while (true)
        {
            DeckManager DM = new DeckManager(GM.path);

            DM.PrintMenu();
            Selection = Console.ReadLine().ToUpper();

            bool anything_done = false;

            int index = -1;

            switch (Selection)
            {
                case "A":
                    index = DM.DeckList.SelectOne();
                    break;
                case "N":
                    DM.AddDeck();
                    break;
                case "D":
                    DM.DeleteDeck();
                    break;
                default:
                    anything_done = true;
                    break;
            }

            if (anything_done) break;

            if (index != -1)
            {
                string PathToDeck = DM.directories[index];
                while (true)
                {
                    var CM = new CardManager(PathToDeck);

                    CM.PrintMenu();
                    Selection = Console.ReadLine().ToUpper();

                    anything_done = false;

                    switch (Selection)
                    {
                        case "C":
                            CM.GenerateCard();
                            break;
                        case "S":
                            CM.PrintCards();
                            var WaitForIt = Console.ReadKey();
                            break;
                        case "D":
                            CM.DeleteACard();
                            break;
                        case "A":
                            CM.DeleteAllCards();
                            break;
                        default:
                            anything_done = true;
                            break;
                    }

                    if (anything_done == true) break;
                }
            }
        }
    }
    else break;
}