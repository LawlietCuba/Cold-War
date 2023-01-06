/* Lexical analysis. Allows to split a raw Text representing the program into 
 the first abstract elements (tokens). */
public class Compiling
{
    private static LexicalAnalyzer __LexicalProcess;
    public static LexicalAnalyzer Lexical
    {
        get
        {
            if (__LexicalProcess == null)
            {
                __LexicalProcess = new LexicalAnalyzer();


                __LexicalProcess.RegisterOperator("+", TokenValues.Add);
                __LexicalProcess.RegisterOperator("*", TokenValues.Mul);
                __LexicalProcess.RegisterOperator("-", TokenValues.Sub);
                __LexicalProcess.RegisterOperator("/", TokenValues.Div);
                __LexicalProcess.RegisterOperator("=", TokenValues.Assign);

                __LexicalProcess.RegisterOperator(",", TokenValues.ValueSeparator);
                __LexicalProcess.RegisterOperator(";", TokenValues.StatementSeparator);
                __LexicalProcess.RegisterOperator(":", TokenValues.SpecialAssigner);
                __LexicalProcess.RegisterOperator("(", TokenValues.OpenBracket);
                __LexicalProcess.RegisterOperator(")", TokenValues.ClosedBracket);
                __LexicalProcess.RegisterOperator("{", TokenValues.OpenCurlyBraces);
                __LexicalProcess.RegisterOperator("}", TokenValues.ClosedCurlyBraces);
                __LexicalProcess.RegisterOperator("[", TokenValues.OpenSquareBracket);
                __LexicalProcess.RegisterOperator("]", TokenValues.ClosedSquareBracket);

                __LexicalProcess.RegisterOperator("==", TokenValues.BooleanEqual);
                __LexicalProcess.RegisterOperator("&&", TokenValues.BooleanAnd);
                __LexicalProcess.RegisterOperator("||", TokenValues.BooleanOr);
                __LexicalProcess.RegisterOperator("!", TokenValues.BooleanNon);
                __LexicalProcess.RegisterOperator("<", TokenValues.BooleanSmaller);
                __LexicalProcess.RegisterOperator("<=", TokenValues.BooleanSmallerEqual);
                __LexicalProcess.RegisterOperator(">", TokenValues.BooleanGreather);
                __LexicalProcess.RegisterOperator(">=", TokenValues.BooleanGreatherEqual);
                __LexicalProcess.RegisterOperator(".", TokenValues.DotOperator);

                __LexicalProcess.RegisterKeyword("true", TokenValues.BooleanValueTrue);
                __LexicalProcess.RegisterKeyword("false", TokenValues.BooleanValueFalse);   

                __LexicalProcess.RegisterKeyword("print", TokenValues.print);
                __LexicalProcess.RegisterKeyword("import", TokenValues.import);
               
                __LexicalProcess.RegisterKeyword("Card", TokenValues.Card);
                __LexicalProcess.RegisterKeyword("CardType", TokenValues.CardType);
                // Here the card types
                __LexicalProcess.RegisterKeyword("Unit", TokenValues.Unit);
                __LexicalProcess.RegisterKeyword("Event", TokenValues.Event);
                __LexicalProcess.RegisterKeyword("Politic", TokenValues.Politic);

                __LexicalProcess.RegisterKeyword("Rareness", TokenValues.Rareness);
                // Here the card rarenesses
                __LexicalProcess.RegisterKeyword("Legendary", TokenValues.Legendary);
                __LexicalProcess.RegisterKeyword("Rare", TokenValues.Rare);
                __LexicalProcess.RegisterKeyword("Common", TokenValues.Common);

                __LexicalProcess.RegisterKeyword("Lore", TokenValues.Lore);
                __LexicalProcess.RegisterKeyword("Attack", TokenValues.Attack);
                __LexicalProcess.RegisterKeyword("Health", TokenValues.Health);

                __LexicalProcess.RegisterKeyword("PoliticalCurrent", TokenValues.PoliticalCurrent);
                __LexicalProcess.RegisterKeyword("PathToPhoto", TokenValues.PathToPhoto);
                __LexicalProcess.RegisterKeyword("EffectText", TokenValues.EffectText);
                __LexicalProcess.RegisterKeyword("Effect", TokenValues.Effect);

                __LexicalProcess.RegisterText("\"", "\"");

                __LexicalProcess.RegisterKeyword("if", TokenValues.If);
                __LexicalProcess.RegisterKeyword("else", TokenValues.Else);

                // Effects ------------------------------------------------
                __LexicalProcess.RegisterKeyword("DrawCards", TokenValues.DrawCards);
                __LexicalProcess.RegisterKeyword("DestroyCard", TokenValues.DestroyCard);
                __LexicalProcess.RegisterKeyword("DecreaseHealth", TokenValues.DecreaseHealth);
                __LexicalProcess.RegisterKeyword("IncreaseHealth", TokenValues.IncreaseHealth);
                __LexicalProcess.RegisterKeyword("DecreaseAttack", TokenValues.DecreaseAttack);
                __LexicalProcess.RegisterKeyword("IncreaseAttack", TokenValues.IncreaseAttack);

                // Effects Conditionals ----------------------------------------
                __LexicalProcess.RegisterKeyword("minHealth", TokenValues.minHealth);
                __LexicalProcess.RegisterKeyword("minAttack", TokenValues.minAttack);
                __LexicalProcess.RegisterKeyword("maxHealth", TokenValues.maxHealth);
                __LexicalProcess.RegisterKeyword("maxAttack", TokenValues.maxAttack);
            }

            return __LexicalProcess;
        }
    }
}