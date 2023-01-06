/* Here we parse the token's list and return the corresponding AST nodes.
The Stream is useful to iterate over the token's list */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
public class Parser
{
    public TokenStream Stream { get; protected set; }
    public Parser(TokenStream stream)
    {
        Stream = stream;
    }

    public virtual ColdWarProgram ParseProgram(List<CompilingError> errors)
    {
        ColdWarProgram program = new ColdWarProgram(new CodeLocation());

        while (Stream.InRange)
        {
            switch (Stream.LookAhead().Value)
            {
                case TokenValues.import:
                    List<string> DecksImported = ParseImport(errors);

                    foreach (string deck in DecksImported)
                    {
                        program.political_currents.Add(deck, new PoliticalCurrent(deck, Stream.LookAhead().Location));
                    }

                    if (!Stream.Next(TokenValues.StatementSeparator))
                    {
                        errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
                        return program;
                    }

                    break;
                case TokenValues.PoliticalCurrent:
                    PoliticalCurrent political_current = ParsePoliticalCurrent(errors);
                    program.political_currents[political_current.Id] = political_current;
                    break;
                case TokenValues.Card:
                    Card card = ParseCard(errors);
                    program.Cards[card.Id] = card;
                    break;
                case TokenValues.If:
                    // Analizing the conditional in the if
                    List<string> polishNotationTokens = TransformToPolishNotation(errors);
                    if (polishNotationTokens == null)
                        return program;

                    List<string>.Enumerator enumerator = NewMethod(polishNotationTokens);
                    enumerator.MoveNext();
                    BoolExpr expr = Make(ref enumerator, errors);

                    expr.Evaluate();

                    // Analize the code in the if

                    TokenStream sub_if_stream = Sub_Stream(TokenValues.OpenCurlyBraces, TokenValues.ClosedCurlyBraces, errors);

                    if (sub_if_stream == null) break;

                    Parser sub_if_parser = new Parser(sub_if_stream);
                    ColdWarProgram sub_if_program = sub_if_parser.ParseProgram(errors);

                    TokenStream sub_else_stream = null;
                    Parser sub_else_parser = null;
                    ColdWarProgram sub_else_program = null;
                    if (Stream.Next(TokenValues.Else))
                    {
                        sub_else_stream = Sub_Stream(TokenValues.OpenCurlyBraces, TokenValues.ClosedCurlyBraces, errors);
                        if (sub_else_stream == null)
                        {
                            break;
                        }
                        sub_else_parser = new Parser(sub_else_stream);
                        sub_else_program = sub_else_parser.ParseProgram(errors);
                    }

                    if ((bool)expr.GetValue())
                    {
                        foreach(KeyValuePair<string,Card> if_cards in sub_if_program.Cards)                        
                            program.Cards.Add(if_cards.Key, if_cards.Value);
                        foreach(KeyValuePair<string,PoliticalCurrent> if_political_currents in sub_if_program.political_currents)                        
                            program.political_currents.Add(if_political_currents.Key, if_political_currents.Value);
                        foreach(Expression exp in sub_if_program.PrintingList)
                            program.PrintingList.Add(exp);
                    }
                    else
                    {
                        foreach(KeyValuePair<string,Card> else_cards in sub_else_program.Cards)                        
                            program.Cards.Add(else_cards.Key, else_cards.Value);
                        foreach(KeyValuePair<string,PoliticalCurrent> else_political_currents in sub_else_program.political_currents)                        
                            program.political_currents.Add(else_political_currents.Key, else_political_currents.Value);
                        foreach(Expression exp in sub_else_program.PrintingList)
                            program.PrintingList.Add(exp);
                    }

                    break;
                case TokenValues.print:
                    Expression to_print = ParsePrint(errors);
                    if (to_print != null)
                        program.PrintingList.Add(to_print);
                    break;
                // case TokenValues.id:
                //     ParseTheIdentifier();
                //     break;
                case TokenValues.StatementSeparator:
                    break;
                default:
                    errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad expression"));
                    return program;
            }

            Stream.MoveNext(1);
        }


        return program;
    }

    protected static List<string>.Enumerator NewMethod(List<string> polishNotationTokens)
    {
        return polishNotationTokens.GetEnumerator();
    }

    protected List<string> TransformToPolishNotation(List<CompilingError> errors)
    {
        List<string> polishNotationTokens = new List<string>();

        Queue<string> outputQueue = new Queue<string>();
        Stack<string> stack = new Stack<string>();

        Stream.Next();

        while (Stream.InRange)
        {
            Token token = Stream.LookAhead();       // Value ~ TokenValues

            switch (token.Type)
            {
                case TokenType.Identifier:
                case TokenType.Number:
                case TokenType.Text:
                    Stream.MoveBack(1);
                    Expression exp = ParseComparativeBool(errors);
                    if(exp == null)
                    {
                        errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad expression. Some problems in the if"));
                        return null;
                    }

                    if((bool)exp.GetValue())
                    {
                        outputQueue.Enqueue(TokenValues.BooleanValueTrue);
                    }
                    else
                    {
                        outputQueue.Enqueue(TokenValues.BooleanValueFalse);
                    }

                    break;
                case TokenType.BooleanSymbol:
                    stack.Push(token.Value);
                    break;
                case TokenType.Bool:
                    outputQueue.Enqueue(token.Value);
                    break;
                case TokenType.Symbol:
                    if(token.Value == TokenValues.OpenBracket)
                        stack.Push(token.Value);
                    if(token.Value == TokenValues.ClosedBracket)
                    {
                        while (stack.Peek() != TokenValues.OpenBracket)
                        {
                            outputQueue.Enqueue(stack.Pop());
                        }
                        stack.Pop();
                        if (stack.Count > 0 && stack.Peek() == TokenValues.BooleanNon)
                        {
                            outputQueue.Enqueue(stack.Pop());
                        }
                    }
                    break;
                default:
                    errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad expression. Some problems in the if"));
                    return null;
            }

            if(stack.Count == 0) break;

            Stream.MoveNext(1);
        }

        while (stack.Count > 0)
        {
            outputQueue.Enqueue(stack.Pop());
        }

        return outputQueue.Reverse().ToList();
    }

    protected BoolExpr Make(ref List<string>.Enumerator polishNotationTokensEnumerator, List<CompilingError> errors)
    {
        if (polishNotationTokensEnumerator.Current == TokenValues.BooleanValueFalse || polishNotationTokensEnumerator.Current == TokenValues.BooleanValueTrue)
        {
            BoolExpr lit = CreateBoolVar(polishNotationTokensEnumerator.Current);
            if(lit == null)
            {
                errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad expression. Some problems in the if"));
            }
            polishNotationTokensEnumerator.MoveNext();
            return lit;
        }
        else
        {
            if (polishNotationTokensEnumerator.Current == TokenValues.BooleanNon)
            {
                polishNotationTokensEnumerator.MoveNext();
                BoolExpr operand = Make(ref polishNotationTokensEnumerator,errors);
                return CreateNot(operand);
            }
            else if (polishNotationTokensEnumerator.Current == TokenValues.BooleanAnd)
            {
                polishNotationTokensEnumerator.MoveNext();
                BoolExpr left = Make(ref polishNotationTokensEnumerator, errors);
                BoolExpr right = Make(ref polishNotationTokensEnumerator, errors);
                return CreateAnd(left, right);
            }
            else if (polishNotationTokensEnumerator.Current == TokenValues.BooleanOr)
            {
                polishNotationTokensEnumerator.MoveNext();
                BoolExpr left = Make(ref polishNotationTokensEnumerator, errors);
                BoolExpr right = Make(ref polishNotationTokensEnumerator, errors);
                return CreateOr(left, right);
            }
        }
        return null;
    }

    protected BoolExpr CreateBoolVar(string value)
    {
        if(value == TokenValues.BooleanValueTrue)
            return new BoolExpr(TokenValues.BooleanValueTrue, Stream.LookAhead().Location);
        else if(value == TokenValues.BooleanValueFalse)
            return new BoolExpr(TokenValues.BooleanValueFalse, Stream.LookAhead().Location);
        else 
            return null;
    }

    protected BoolExpr CreateNot(BoolExpr child)
    {
        return new BoolExpr(BOP.NOT, child, null, Stream.LookAhead().Location);
    }
    protected BoolExpr CreateAnd(BoolExpr left, BoolExpr right)
    {
        return new BoolExpr(BOP.AND, left, right, Stream.LookAhead().Location);
    }
    protected BoolExpr CreateOr(BoolExpr left, BoolExpr right)
    {
        return new BoolExpr(BOP.OR, left, right, Stream.LookAhead().Location);
    }


    public PoliticalCurrent ParsePoliticalCurrent(List<CompilingError> errors)
    {

        PoliticalCurrent political_current = new PoliticalCurrent("null", Stream.LookAhead().Location);

        if (!Stream.Next(TokenType.Identifier))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "id expected"));
            return political_current;
        }
        political_current.Id = Stream.LookAhead().Value;

        if (!Stream.Next(TokenValues.OpenCurlyBraces))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "{ expected"));
            return political_current;
        }

        if (!Stream.Next(TokenValues.ClosedCurlyBraces))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "} expected"));
            return political_current;
        }
        return political_current;
    }

    public Card ParseCard(List<CompilingError> errors)
    {
        Card card = new Card("null", Stream.LookAhead().Location);

        if (!Stream.Next(TokenType.Identifier))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "id expected"));
            return card;
        }
        else
        {
            card.Id = Stream.LookAhead().Value;
        }


        if (!Stream.Next(TokenValues.OpenCurlyBraces))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "{ expected"));
            return card;
        }

        // CARD TYPE--------------------------------

        if (!SimpleParse(TokenValues.CardType, errors)) return card;

        /* Here we parse the expression. If null is returned, we send an error */
        if (!(Stream.Next(TokenValues.Unit)) && !(Stream.Next(TokenValues.Event)) && !(Stream.Next(TokenValues.Politic)))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad expression. Expected Unit, Event or Politic"));
            return card;
        }
        card.cardtype = new Text(Stream.LookAhead().Value, Stream.LookAhead().Location);

        if (!Stream.Next(TokenValues.StatementSeparator))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
            return card;
        }

        // RARENESS---------------------------------------

        if (!SimpleParse(TokenValues.Rareness, errors)) return card;

        /* Here we parse the expression. If null is returned, we send an error */
        if (!(Stream.Next(TokenValues.Legendary) || Stream.LookAhead().Value == TokenValues.Rare || Stream.LookAhead().Value == TokenValues.Common))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad expression. Expected Legendary, Rare or Common"));
            return card;
        }
        card.Rareness = new Text(Stream.LookAhead().Value, Stream.LookAhead().Location);


        if (!Stream.Next(TokenValues.StatementSeparator))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
            return card;
        }

        // Lore---------------------------------------

        if (!SimpleParse(TokenValues.Lore, errors)) return card;

        Expression exp = ParseExpression();
        if (exp == null) card.Lore = new Text("Insert epic lore", Stream.LookAhead().Location);
        else card.Lore = exp;

        if (!Stream.Next(TokenValues.StatementSeparator))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
            return card;
        }

        if(card.cardtype.GetValue().ToString() == TokenValues.Unit)
        {
            // Health---------------------------------------

            if (!SimpleParse(TokenValues.Health, errors)) return card;

            exp = ParseExpression();
            if (exp == null)
            {
                errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad expression"));
                return card;
            }
            card.Health = exp;

            if (!Stream.Next(TokenValues.StatementSeparator))
            {
                errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
                return card;
            }

            // ATTACK-------------------------------------

            if (!SimpleParse(TokenValues.Attack, errors)) return card;

            exp = ParseExpression();
            if (exp == null)
            {
                errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad expression"));
                return card;
            }
            card.Attack = exp;

            if (!Stream.Next(TokenValues.StatementSeparator))
            {
                errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
                return card;
            }
        }
        else
        {
            card.Health = new Number(0, Stream.LookAhead().Location);
            card.Attack = new Number(0, Stream.LookAhead().Location);
        }


        // POLITICAL_CURRENT

        if (!SimpleParse(TokenValues.political_current, errors)) return card;

        if (Stream.Next(TokenType.Identifier))
        {
            card.political_current = new Text(Stream.LookAhead().Value, Stream.LookAhead().Location);
        }
        else
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Doesn't exist in this context"));
            return card;
        }

        if (!Stream.Next(TokenValues.StatementSeparator))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
            return card;
        }

        // PathToPhoto-----------------------------------------s

        if(!SimpleParse(TokenValues.PathToPhoto, errors)) return card;

        if(Stream.Next(TokenType.Text))
        {
            card.PathToPhoto = new Text(Stream.LookAhead().Value, Stream.LookAhead().Location);
        }
        else
        {
            if(Stream.Next(TokenValues.StatementSeparator))
            {
                card.PathToPhoto = new Text(null, Stream.LookAhead().Location);
                Stream.MoveBack(1);
            }
            else
            {
                errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad expression. If there is a photo, the argument must be a string"));
                return card;
            }
        }


        if (!Stream.Next(TokenValues.StatementSeparator))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
            return card;
        }

        // Effect Text ----------------------------------------------------

        if(Stream.Next(TokenValues.EffectText))
        {
            Stream.MoveBack(1);
            if(!SimpleParse(TokenValues.EffectText, errors)) return card;
            exp = ParseExpression();
            if (exp != null)
            {
                card.EffectText = exp;  
            }
            else
            {
                card.EffectText = new Text("", Stream.LookAhead().Location);
            }

            if (!Stream.Next(TokenValues.StatementSeparator))
            {
                errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
                return card;
            }  
        }
        else
        {
            card.EffectText = new Text("", Stream.LookAhead().Location);
        }

        // Effect ------------------------------------------------

        if(Stream.Next(TokenValues.Effect))
        {
            if(!Stream.Next(TokenValues.Assign))
            {
                errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "= expected"));
                return card;
            }

            List<Token> new_effect = ParseEffect(errors);
            if(exp == null)
            {
                card.Effect = null;
            }
            else
            {
                card.Effect = new_effect;
            }
        }
        else
        {
            card.Effect = null;
        }

        if (!Stream.Next(TokenValues.ClosedCurlyBraces))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "} expected"));
            return card;
        }

        return card;
    }

    protected Expression ParsePrint(List<CompilingError> errors)
    {
        if (!Stream.Next(TokenValues.OpenBracket))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "( expected"));
            return null;
        }

        Expression exp = ParseExpression();
        if (object.Equals(exp, null))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad Expression"));
            return null;
        }

        if (!Stream.Next(TokenValues.ClosedBracket))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, ") expected"));
            return null;
        }

        if (!Stream.Next(TokenValues.StatementSeparator))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
            return null;
        }

        exp.Evaluate();

        return exp;
    }

    protected bool SimpleParse(string token_type, List<CompilingError> errors)
    {
        if (!Stream.Next(token_type))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, token_type + " expected"));
            return false;
        }
        if (!Stream.Next(TokenValues.Assign))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "= expected"));
            return false;
        }

        return true;
    }

    public List<string> ParseImport(List<CompilingError> errors)
    {
        List<string> DecksImported = new List<string>();

        if (Stream.Next(TokenValues.Decks))
        {
            string path = Stream.tokens[Stream.Position].Value;
            string[] decks = System.IO.Directory.GetDirectories(path);

            foreach (string deck_path in decks)
            {
                DecksImported.Add(System.IO.Path.GetFileName(deck_path));
            }
        }
        else
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Invalid expression"));
        }

        return DecksImported;
    }

    protected List<Token> ParseEffect(List<CompilingError> errors)
    {
        if(!Stream.Next(TokenValues.OpenSquareBracket))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "[ expected. The effects expression must be between []"));
            return null;
        }

        List<Token> code_card_effect = new List<Token>();
        while(Stream.LookAhead(1).Value != TokenValues.ClosedSquareBracket && Stream.CanLookAhead(1))
        {
            Stream.MoveNext(1);
            code_card_effect.Add(Stream.LookAhead());
        }

        if(!Stream.Next(TokenValues.ClosedSquareBracket))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "] expected"));
            return null;
        }

        return code_card_effect;
    }


    /* Turbidity. You should have been in the conference to understand this part. 
    If you still lost, contact to @Rodrigo43 via Telegram and i will try to explain it again */
    protected Expression ParseExpression()
    {
        return ParseExpressionLv1(null);
    }

    protected Expression ParseExpressionLv1(Expression left)
    {
        Expression newLeft = ParseExpressionLv2(left);
        Expression exp = ParseExpressionLv1_(newLeft);
        return exp;
    }

    protected Expression ParseExpressionLv1_(Expression left)
    {
        Expression exp = ParseAdd(left);
        if (exp != null)
        {
            return exp;
        }
        exp = ParseSub(left);
        if (exp != null)
        {
            return exp;
        }

        return left;
    }

    protected POObject ParseExpressionLv1__(POObject left)
    {
        if(Stream.Next(TokenType.Identifier))
        {
            POObject poo = ParsePOOExpression();
            if(poo != null)
            {
                return poo;
            }
        }

        return left;
    }

    protected Expression ParseExpressionLv2(Expression left)
    {
        Expression newLeft = ParseExpressionLv3(left);
        return ParseExpressionLv2_(newLeft);
    }
    protected Expression ParseExpressionLv2_(Expression left)
    {
        Expression exp = ParseMul(left);
        if (exp != null)
        {
            return exp;
        }
        exp = ParseDiv(left);
        if (exp != null)
        {
            return exp;
        }
        return left;
    }

    protected Expression ParseExpressionLv3(Expression left)
    {
        Expression exp = ParseNumber();
        if (exp != null)
        {
            return exp;
        }
        exp = ParseText();
        if (exp != null)
        {
            return exp;
        }
        if(Stream.Next(TokenType.Identifier))
        {
            POObject poo = ParsePOOExpression();
            if(poo != null)
            {
                return poo;
            }
        }

        return null;
    }


    protected Expression ParseAdd(Expression left)
    {
        Add sum = new Add(Stream.LookAhead().Location);

        if (left == null || !Stream.Next(TokenValues.Add))
            return null;

        sum.Left = left;

        Expression right = ParseExpressionLv2(null);
        if (right == null)
        {
            Stream.MoveBack(2);
            return null;
        }
        sum.Right = right;

        return ParseExpressionLv1_(sum);
    }

    protected Expression ParseSub(Expression left)
    {
        Sub sub = new Sub(Stream.LookAhead().Location);

        if (left == null || !Stream.Next(TokenValues.Sub))
            return null;

        sub.Left = left;

        Expression right = ParseExpressionLv2(null);
        if (right == null)
        {
            Stream.MoveBack(2);
            return null;
        }
        sub.Right = right;

        return ParseExpressionLv1_(sub);
    }

    protected Expression ParseMul(Expression left)
    {
        Mul mul = new Mul(Stream.LookAhead().Location);

        if (left == null || !Stream.Next(TokenValues.Mul))
            return null;

        mul.Left = left;

        Expression right = ParseExpressionLv3(null);
        if (right == null)
        {
            Stream.MoveBack(2);
            return null;
        }
        mul.Right = right;

        return ParseExpressionLv2_(mul);
    }

    protected Expression ParseDiv(Expression left)
    {
        Div div = new Div(Stream.LookAhead().Location);

        if (left == null || !Stream.Next(TokenValues.Div))
            return null;

        div.Left = left;

        Expression right = ParseExpressionLv3(null);
        if (right == null)
        {
            Stream.MoveBack(2);
            return null;
        }
        div.Right = right;

        return ParseExpressionLv2_(div);
    }

    protected Expression ParseNumber()
    {
        if (!Stream.Next(TokenType.Number))
            return null;
        return new Number(double.Parse(Stream.LookAhead().Value), Stream.LookAhead().Location);
    }

    protected Expression ParseText()
    {
        if (!Stream.Next(TokenType.Text))
            return null;
        return new Text(Stream.LookAhead().Value, Stream.LookAhead().Location);
    }
    protected Expression ParseId()
    {
        if(!Stream.Next(TokenType.Identifier))
            return null;
        return new Text(Stream.LookAhead().Value, Stream.LookAhead().Location);
    }
    protected Expression ParseKeyword()
    {
        if(!Stream.Next(TokenType.Keyword))
            return null;
        return new Text(Stream.LookAhead().Value, Stream.LookAhead().Location);
    }

    protected POObject ParsePOOExpression()
    {
        POObject poo = new POObject(Stream.LookAhead().Location);

        Stream.MoveBack(1);
        
        Expression exp = ParseId();
        if(exp == null)
            return null;
        poo.Left = exp;

        if(!Stream.Next(TokenValues.DotOperator))
            return null;

        Expression right = ParseKeyword();
        if (right == null)
        {
            return null;
        }

        poo.Right = right;

        poo.Evaluate(true);

        return ParseExpressionLv1__(poo);
    }


    protected Expression ParseComparativeBool(List<CompilingError> errors)
    {
        BoolComparer nblc = new BoolComparer(Stream.LookAhead().Location);

        Expression exp = ParseExpression();
        exp.Evaluate();

        if (exp == null)
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Invalid expression"));
            return null;
        }

        nblc.Left = exp;

        if (!Stream.Next(TokenType.BooleanComparative))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Invalid expression. Expected boolean"));
            return null;
        }

        nblc.op_comparer = Stream.LookAhead().Value;

        exp = ParseExpression();
        exp.Evaluate();

        if (exp == null)
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Invalid expression. Must be numerical"));
            return null;
        }

        nblc.Right = exp;
        nblc.Evaluate();

        bool result = (bool)nblc.GetValue();

        return new Boolean(result, Stream.LookAhead().Location);
    }

    protected TokenStream Sub_Stream(string OpenSymbol, string CloseSymbol, List<CompilingError> errors)
    {
        TokenStream sub_stream = new TokenStream();

        int BalancedSymbols = 0;

        if (!Stream.Next(OpenSymbol))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, OpenSymbol + " expected"));
            return null;
        }

        BalancedSymbols++;

        while (BalancedSymbols > 0)
        {
            Stream.Next();
            if (Stream.LookAhead().Value == OpenSymbol)
                BalancedSymbols++;
            if (Stream.LookAhead().Value == CloseSymbol)
                BalancedSymbols--;
            sub_stream.tokens.Add(Stream.LookAhead());
        }

        sub_stream.tokens.RemoveAt(sub_stream.tokens.Count - 1);

        return sub_stream;
    }
}

public enum CardAttributes
{
    CardType,
    Rareness,
    Health,
    Attack,
    political_current
}