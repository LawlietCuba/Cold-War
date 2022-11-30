namespace Selector;

public class SelectionReferee
{
    public List<string>? ListOfSelection { get; private set; }

    public SelectionReferee(List<string> ListOfSelection)
    {
        if (List<string>.Equals(ListOfSelection, null))
        {
            System.Console.WriteLine("Can't work with an empty input");
        }
        this.ListOfSelection = ListOfSelection;
    }

    public SelectionReferee(string[] StringList)
    {
        int n = StringList.Length;
        this.ListOfSelection = new List<string>();

        for (int i = 0; i < n; i++)
        {
            this.ListOfSelection.Add(StringList[i]);
        }

        if (List<string>.Equals(ListOfSelection, null))
        {
            System.Console.WriteLine("Can't work with an empty input");
        }
    }

    public SelectionReferee(List<string> ListOfSelection, bool IsPaths)
    {
        if (IsPaths)
        {
            for (int i = 0; i < ListOfSelection.Count(); i++)
            {
                ListOfSelection[i] = Path.GetFileName(ListOfSelection[i]);
            }

            this.ListOfSelection = ListOfSelection;
        }
        else
        {
            if (List<string>.Equals(ListOfSelection, null))
            {
                System.Console.WriteLine("Can't work with an empty input");
            }
            this.ListOfSelection = ListOfSelection;
        }
    }

    public SelectionReferee(string[] StringList, bool IsPaths)
    {
        int n = StringList.Length;
        this.ListOfSelection = new List<string>(n);

        if (IsPaths)
        {
            for (int i = 0; i < n; i++)
            {
                this.ListOfSelection[i] = Path.GetFileName(StringList[i]);
            }
        }
        else
        {
            for (int i = 0; i < n; i++)
            {
                this.ListOfSelection[i] = StringList[i];
            }

        }
    }

    public int SelectOne()
    {
        System.Console.Clear();
        PrintSelectionList();
        System.Console.WriteLine("Choose one of the options between 1 and " + ListOfSelection.Count());
        string? input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            return SelectOne();
        }
        else
        {
            int index = int.Parse(input);

            if (index < 1 || index > ListOfSelection.Count())
            {
                System.Console.WriteLine("Wrong. You must choose a number between 1 and " + ListOfSelection.Count());
                System.Console.WriteLine("Press any key to try again");
                var UserInput = Console.ReadKey();
                System.Console.Clear();
                return SelectOne();
            }
            else
            {
                System.Console.Clear();
                return index - 1;
            }
        }
    }

    public List<int> SelectMany(int SelectionTimes)
    {
        var SelectionList = new List<int>();
        while (SelectionTimes > 0)
        {
            System.Console.WriteLine("You have " + SelectionTimes + " selections left");
            SelectionList.Add(SelectOne());
            SelectionTimes--;
        }

        return SelectionList;
    }

    public void PrintSelectionList()
    {
        for (int i = 0; i < ListOfSelection.Count(); i++)
        {
            System.Console.WriteLine($"{i + 1}. {ListOfSelection[i]}");
        }
    }
}