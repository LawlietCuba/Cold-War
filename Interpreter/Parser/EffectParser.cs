using System;
using System.Collections.Generic;
using Godot;

public class EffectParser : Parser
{
    public EffectParser(TokenStream stream) : base(stream)
    {

    }

    public override ColdWarProgram ParseProgram(List<CompilingError> errors)
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

                    List<string>.Enumerator enumerator = polishNotationTokens.GetEnumerator();
                    enumerator.MoveNext();
                    BoolExpr expr = Make(ref enumerator, errors);

                    expr.Evaluate();

                    // Analize the code in the if

                    TokenStream sub_if_stream = Sub_Stream(TokenValues.OpenCurlyBraces, TokenValues.ClosedCurlyBraces, errors);

                    if (sub_if_stream == null) break;

                    EffectParser sub_if_parser = new EffectParser(sub_if_stream);
                    ColdWarProgram sub_if_program = sub_if_parser.ParseProgram(errors);

                    TokenStream sub_else_stream = null;
                    EffectParser sub_else_parser = new EffectParser(sub_else_stream);
                    ColdWarProgram sub_else_program = new ColdWarProgram(Stream.LookAhead().Location);
                    if (Stream.Next(TokenValues.Else))
                    {
                        sub_else_stream = Sub_Stream(TokenValues.OpenCurlyBraces, TokenValues.ClosedCurlyBraces, errors);
                        if (sub_else_stream == null)
                        {
                            break;
                        }
                        sub_else_parser = new EffectParser(sub_else_stream);
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
                        foreach(EffectExpression eff in sub_if_program.Effects)
                            program.Effects.Add(eff);
                    }
                    else
                    {
                        foreach(KeyValuePair<string,Card> else_cards in sub_else_program.Cards)                        
                            program.Cards.Add(else_cards.Key, else_cards.Value);
                        foreach(KeyValuePair<string,PoliticalCurrent> else_political_currents in sub_else_program.political_currents)                        
                            program.political_currents.Add(else_political_currents.Key, else_political_currents.Value);
                        foreach(Expression exp in sub_else_program.PrintingList)
                            program.PrintingList.Add(exp);
                        foreach(EffectExpression eff in sub_else_program.Effects)
                            program.Effects.Add(eff);
                    }

                    break;
                case TokenValues.print:
                    Expression to_print = ParsePrint(errors);
                    if (to_print != null)
                        program.PrintingList.Add(to_print);
                    break;
                case TokenValues.StatementSeparator:
                    break;
                case TokenValues.DrawCards:
                case TokenValues.DecreaseHealth:
                case TokenValues.DecreaseAttack:
                case TokenValues.IncreaseHealth:
                case TokenValues.IncreaseAttack:
                    ParseEffectWithAmount(program, errors);
                    break;
                case TokenValues.DestroyCard:
                    ParseDestroyCard(program, errors);
                    break;
                case TokenValues.AddCardToDeck:
                case TokenValues.AddCardToBoard:
                    ParseEffectWithCard(program, errors);
                    break;
                default:
                    errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad expression"));
                    return program;
            }

            Stream.MoveNext(1);
        }

        return program;
    }

    protected void ParseEffectWithCard(ColdWarProgram program, List<CompilingError> errors)
    {
        EffectExpression effexp = new EffectExpression(Stream.LookAhead().Location);
        effexp.SetValue(Stream.LookAhead().Value.ToString());

        if (!Stream.Next(TokenValues.OpenBracket))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "( expected"));
            return;
        }

        if(!Stream.Next(TokenType.Identifier))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "A card expected"));
            return;
        }

        effexp.CardToHandle = program.Cards[Stream.LookAhead().Value];
        // GD.Print(effexp.CardToHandle.Id);

        if (!Stream.Next(TokenValues.ClosedBracket))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "( expected"));
            return;
        }

        if (!Stream.Next(TokenValues.StatementSeparator))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
            return;
        }

        program.Effects.Add(effexp);
    }

    public void ParseEffectWithAmount(ColdWarProgram program, List<CompilingError> errors)
    {
        EffectExpression effexp = new EffectExpression(Stream.LookAhead().Location);
        effexp.SetValue(Stream.LookAhead().Value.ToString());

        if (!Stream.Next(TokenValues.OpenBracket))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "( expected"));
            return;
        }

        Expression exp = ParseExpression();
        if(exp == null)
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad Expression"));
            return;
        }

        effexp.Amount = exp;
        exp.Evaluate();

        if(Stream.Next(TokenValues.ValueSeparator))
        {
            switch(Stream.LookAhead(1).Value)
            {
                case TokenValues.minHealth:
                case TokenValues.minAttack:
                case TokenValues.maxHealth:
                case TokenValues.maxAttack:
                    effexp.EffectConditional = Stream.LookAhead(1).Value;
                    Stream.MoveNext(1);
                    break;
                default:
                    errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Invalid, "Bad expression. Expected an effect conditional"));
                    return;
            }
        }
        else
        {
            effexp.EffectConditional = null;
        }

        if (!Stream.Next(TokenValues.ClosedBracket))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, ") expected"));
            return;
        }

        if (!Stream.Next(TokenValues.StatementSeparator))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
            return;
        }

        program.Effects.Add(effexp);    
    }

    public void ParseDestroyCard(ColdWarProgram program, List<CompilingError> errors)
    {
        EffectExpression effexp = new EffectExpression(Stream.LookAhead().Location);
        effexp.SetValue(Stream.LookAhead().Value.ToString());

        if (!Stream.Next(TokenValues.OpenBracket))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "( expected"));
            return;
        }

        if (!Stream.Next(TokenValues.ClosedBracket))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, ") expected"));
            return;
        }

        if (!Stream.Next(TokenValues.StatementSeparator))
        {
            errors.Add(new CompilingError(Stream.LookAhead().Location, ErrorCode.Expected, "; expected"));
            return;
        }

        program.Effects.Add(effexp);    
    }
    
}