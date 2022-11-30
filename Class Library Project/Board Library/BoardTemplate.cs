namespace Board_Library;
using CardLibrary;
using Player_Library;
using Selector;

// TODO ------------------------------------
// Repensar Board is Player?
// Arreglar Deck

public class Board
{
    public Player PlayerOfTheBoard { get; private set; }
    public List<Card> Cementery { get; private set; }
    public List<Card> HandCards { get; private set; }
    public Card[,] BoardStructure { get; private set; }
    public Board(Player PlayerOfTheBoard)
    {
        this.PlayerOfTheBoard = PlayerOfTheBoard;
        this.Cementery = new List<Card>();
        this.HandCards = new List<Card>();
        this.BoardStructure = new Card[2, 8];
    }
    public void DrawCards(int n)
    {
        for (int i = 0; i < n; i++)
        {
            if (PlayerOfTheBoard.PlayerDeck.Cards.Count == 0)
            {
                Console.WriteLine("No cards left");
                return;
            }
            HandCards.Add(PlayerOfTheBoard.PlayerDeck.Cards[0]);
            PlayerOfTheBoard.PlayerDeck.Cards.Remove(PlayerOfTheBoard.PlayerDeck.Cards[0]);
        }
    }
    public void Attack(Card AttackingCard)
    {
        List<(int, int)> AttackedCard = new List<(int, int)>();
        AttackedCard = SelectCardsFromtheBoard(1);
        BoardStructure[AttackedCard[0].Item1, AttackedCard[0].Item2].Life -= AttackingCard.Attack;
    }
    public void DestroyCards(List<(int, int)> CardsToDestroy)
    {

        for (int i = 0; i < CardsToDestroy.Count; i++)
        {
            Cementery.Add(BoardStructure[CardsToDestroy[i].Item1, CardsToDestroy[i].Item2]);
            BoardStructure[CardsToDestroy[i].Item1, CardsToDestroy[i].Item2] = null;
        }

    }
    public List<(int, int)> SelectCardsFromtheBoard(int n)
    {
        List<(int, int)> CardsCoordinates = new List<(int, int)>();
        List<string> BoardCardsNames = new List<string>();
        for (int i = 0; i < BoardStructure.GetLength(0); i++)
        {
            for (int j = 0; j < BoardStructure.GetLength(1); j++)
            {
                if (BoardStructure[i, j] != null)
                {
                    CardsCoordinates.Add((i, j));
                    BoardCardsNames.Add(BoardStructure[i, j].Name);
                }
            }
        }
        var a = new SelectionReferee(BoardCardsNames);
        a.PrintSelectionList();
        List<int> SelectedCardsIndex = new List<int>();
        SelectedCardsIndex = a.SelectMany(n);
        List<(int, int)> SelectedCards = new List<(int, int)>();
        for (int i = 0; i < SelectedCardsIndex.Count; i++)
        {
            SelectedCards.Add(CardsCoordinates[i]);
        }
        return SelectedCards;
    }

    public void SummonCard()
    {
        List<string> HandCardsNames = new List<string>();
        for (int i = 0; i < HandCards.Count; i++)
        {
            HandCardsNames.Add(HandCards[i].Name);
        }
        var a = new SelectionReferee(HandCardsNames);
        a.PrintSelectionList();
        int SelectedForSummon = a.SelectOne();
        List<(int, int)> EmptySpaces = new List<(int, int)>();
        List<string> Coordinates = new List<string>();
        for (int i = 0; i < BoardStructure.GetLength(0); i++)
        {
            for (int j = 0; j < BoardStructure.GetLength(1); j++)
            {
                if (BoardStructure[i, j] == null)
                {
                    EmptySpaces.Add((i, j));
                    Coordinates.Add($"[{i}, {j}]");
                }
            }
        }
        var b = new SelectionReferee(Coordinates);
        b.PrintSelectionList();
        int SelectedPosition = b.SelectOne();
        BoardStructure[EmptySpaces[SelectedPosition].Item1, EmptySpaces[SelectedPosition].Item2] = HandCards[SelectedForSummon];
        HandCards.Remove(HandCards[SelectedForSummon]);
    }
}
