// namespace Effects;
// using CardLibrary;

// // How do I link a card with its class effect and the interface that must be
// // implemented to such card to resolve the effects required

// public class Effect
// {
//     public string? explanation { get; private set; }
//     public int ActivationFrequency { get; private set; }
//     // 0 Once every Duel
//     // 1 Once every Round
//     // 2 Once every Turn
//     public int TimesThatCanBeActivatedInATurn { get; private set; }
//     public bool CanBeNegated = true;

//     public Effect(string explanation, int ActivationFrequency, int TimesThatCanBeActivatedInATurn)
//     {
//         this.explanation = explanation;
//         this.ActivationFrequency = ActivationFrequency;
//         this.TimesThatCanBeActivatedInATurn = TimesThatCanBeActivatedInATurn;
//     }
// }

// public class IncreaseAttack : Effect
// {
//     public int amount { get; private set; }
//     public IncreaseAttack(int amount, string explanation, int AF, int TTCBAIAT) : base(explanation, AF, TTCBAIAT)
//     {
//         this.amount = amount;
//     }
// }

// abstract class UnitEffect
// {
//     int TimesThatCanBeActivatedInATurn;
//     bool ActivatedOnceADuel;        // False -> More than once
//     bool ActivatedOnceARound;       // False -> More than once
// }

// abstract class PoliticEffect
// {
//     bool ActivatedInPlayerTurn = true;
// }

// abstract class CotinuePolitic : PoliticEffect
// {
//     string TypeOfPolitic = "Continue";
//     int TurnsTheCardLast = int.MaxValue;
// }

// abstract class NormalPolitic
// {
//     string TypeOfPolitic = "Normal";
//     int TurnsTheCardLast;
// }

// abstract class QuickPolitic
// {
//     string TypeOfPolitic = "Quick Play";
//     int TurnsTheCardLast = 0;
// }

// abstract class EventEffect
// {
//     bool ActivatedInPlayerTurn = false;
// }