
public class Token
{
    public string Value { get; private set; }
    public TokenType Type { get; private set; }
    public CodeLocation Location { get; private set; }
    public Token(TokenType type, string value, CodeLocation location)
    {
        this.Type = type;
        this.Value = value;
        this.Location = location;
    }

    public override string ToString()
    {
        return string.Format(Type.ToString(), " ", Value.ToString());
    }
}

public class CodeLocation
{
    public string File{get;set;}
    public int Line{get;set;}
    public int Column{get;set;}

    public CodeLocation()
    {
        
    }
}


public enum TokenType
{
    Unknwon,
    Number,
    Text,
    Keyword,
    Identifier,
    Symbol,
    BooleanComparative,
    BooleanSymbol,
    Bool,
    Dot
}

public class  TokenValues
{
    protected TokenValues() { }

    public const string Add = "Addition"; // +
    public const string Sub = "Subtract"; // -
    public const string Mul = "Multiplication"; // *
    public const string Div = "Division"; // /


    public const string Assign = "Assign"; // =
    public const string ValueSeparator = "ValueSeparator"; // ,
    public const string StatementSeparator = "StatementSeparator"; // ;
    public const string SpecialAssigner = "SpecialAssigner"; // :

    public const string OpenBracket = "OpenBracket"; // (
    public const string ClosedBracket = "ClosedBracket"; // )
    public const string OpenCurlyBraces = "OpenCurlyBraces"; // {
    public const string ClosedCurlyBraces = "ClosedCurlyBraces"; // }
    public const string OpenSquareBracket = "OpenSquareBracket"; // [
    public const string ClosedSquareBracket = "ClosedSquareBracket"; // ]
    public const string DotOperator = "DotOperator"; // .
    
    // Boolean Operators
    public const string BooleanEqual = "BooleanEqual"; // ==
    public const string BooleanAnd = "BooleanAnd";  // &&
    public const string BooleanOr = "BooleanOr";   // ||
    public const string BooleanNon = "BooleanNon";   // !
    public const string BooleanSmaller = "BooleanSmaller"; // <
    public const string BooleanSmallerEqual = "BooleanSmallerEqual"; // <=
    public const string BooleanGreather = "BooleanGreather"; // >
    public const string BooleanGreatherEqual = "BooleanGreatherEqual"; // >=
    public const string BooleanValueTrue = "true"; // true
    public const string BooleanValueFalse = "false"; // false

    public const string import = "import"; // using
    public const string Decks = "Decks";
    public const string AllCards = "AllCards";
    public const string print = "print"; // Console.WriteLine()

    public const string Card = "Card"; // Card
    public const string id = "id"; // id
    public const string CardType = "CardType"; //  Tipo de carta
    public const string Rareness = "Rareness"; // Rareza
    public const string Lore = "Lore";  // Lore
    public const string Attack = "Attack"; // Attack
    public const string Health = "Health"; // Health
    public const string political_current = "political_current"; // political_current for cards
    public const string PathToPhoto = "PathToPhoto"; // PathToPhoto
    // CardType, Rareness, political_currents
    public const string PoliticalCurrent = "PoliticalCurrent"; // Political_Current

    public const string EffectText = "EffectText"; // Text explaining the effect
    public const string Effect = "Effect"; // Effect
    public const string Unit = "Unit"; // Unit
    public const string Politic = "Politic"; // Politic
    public const string Legendary = "Legendary"; // Legendary
    public const string Rare = "Rare"; // Rare
    public const string Common = "Common"; // Common
    public const string If = "if"; // if
    public const string Else = "else"; // else

    // Effects -------------------------------
    public const string DrawCards = "DrawCards";
    public const string DestroyCard = "DestroyCard";
    public const string DecreaseHealth = "DecreaseHealth";
    public const string IncreaseHealth = "IncreaseHealth";
    public const string DecreaseAttack = "DecreaseAttack";
    public const string IncreaseAttack = "IncreaseAttack";
    public const string ReboundAttack = "ReboundAttack";
    public const string AddCardToDeck = "AddCardToDeck";
    public const string AddCardToBoard = "AddCardToBoard";

    // Effects Conditionals--------------------------------------
    public const string minHealth = "minHealth";
    public const string minAttack = "minAttack";
    public const string maxHealth = "maxHealth";
    public const string maxAttack = "maxAttack";
}
