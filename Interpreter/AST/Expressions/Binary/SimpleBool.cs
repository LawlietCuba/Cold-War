using System;
using System.Collections.Generic;
class BoolExpr : BinaryExpression
{

    private BOP op_bool{ get; set;}

    private object value;

    public override object GetValue()
    {
        return value;
    }

    public override void SetValue(object value)
    {
        this.value = value;
    }

    public override ExpressionType Type { get; set; }

    public BoolExpr(string value , CodeLocation location) : base(location)
    {
        this.op_bool = BOP.LEAF;
        if(value == TokenValues.BooleanValueTrue)
            this.SetValue(true);
        else
            this.SetValue(false);
    }

    public BoolExpr(BOP op, BoolExpr left, BoolExpr right, CodeLocation location) : base(location)
    {
        this.op_bool = op;
        this.Left = left;
        this.Right = right;
    }

    public BoolExpr(BoolExpr other, CodeLocation location) : base (location)
    {
        // No share any object on purpose
        op_bool = other.op_bool;
        Left  = other.Left  == null ? null : new BoolExpr(other.Left.ToString(), this.Location);
        Right = other.Right == null ? null : new BoolExpr(other.Right.ToString(), this.Location);
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool check_left = Left.CheckSemantic(context, scope, errors);
        bool check_right = Right.CheckSemantic(context, scope, errors);
        if(Left.Type != ExpressionType.Boolean || Right.Type != ExpressionType.Boolean)
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "We don't do that here... "));
            Type = ExpressionType.ErrorType;
            return false;
        }

        Type = ExpressionType.Boolean;
        return check_left && check_right;
    }

    public override void Evaluate()
    {
        if(IsLeaf()){
            return;
        }
        if(op_bool == BOP.NOT){
            Left.Evaluate();
            SetValue(!(bool)Left.GetValue());
            return;
        }
        if(op_bool == BOP.AND){
            Left.Evaluate();
            Right.Evaluate();
            SetValue((bool)Left.GetValue() && (bool)Right.GetValue());
            return;
        }
        if(op_bool == BOP.OR)
        {
            Left.Evaluate();
            Right.Evaluate();
            SetValue((bool)Left.GetValue() || (bool)Right.GetValue());
            return;
        }
    }

    public bool IsLeaf()
    {
        return (op_bool == BOP.LEAF);
    }

    // public bool IsAtomic()
    // {
    //     return (this.IsLeaf() || (op_bool == BOP.NOT && this.Left.IsLeaf()));
    // }
}
public enum BOP { LEAF, AND, OR, NOT };